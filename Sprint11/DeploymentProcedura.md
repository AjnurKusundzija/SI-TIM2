# Continuous Deployment — TelecomSupportSystem

## Pregled

Ovaj dokument opisuje automatizovani Continuous Deployment (CD) pipeline za projekat **TelecomSupportSystem** — sistema za podršku korisnicima telekomunikacijske kompanije.

### Arhitektura sistema koji se deploya

Sistem se sastoji od četiri Docker servisa koji se pokreću zajedno putem Docker Compose:

| Servis | Tehnologija | Interni port | Eksterni port |
|---|---|---|---|
| `sqlserver` | Microsoft SQL Server 2019 | 1433 | 1433 |
| `api` | ASP.NET Core 10.0 | 8080 | 5000 |
| `mcp-server` | Node.js 20 + TypeScript | 3001 | 3001 |
| `frontend` | React 19 + Nginx | 80 | 80 |

Frontend (Nginx) prima sve zahtjeve i prosljeđuje API pozive (`/api`, `/chathub`, `/notificationhub`) ka backendu. Baza podataka je dostupna isključivo backendu i MCP serveru unutar Docker mreže.

---

## Lokacija skripte / pipeline-a

CD pipeline je implementiran kao **GitHub Actions workflow**:

```
.github/workflows/deploy.yml
```

CI pipeline (build, test, lint) nalazi se na:

```
.github/workflows/ci.yml
```

Produkcijski Docker Compose fajl na serveru:

```
~/dzelo/docker-compose.yml  (na serveru 46.224.179.251)
```

---

## Kako se CD pipeline pokreće

Pipeline se **automatski aktivira** pri svakom `push` na granu `main`:

```yaml
on:
  push:
    branches:
      - main
```

Ručno pokretanje nije potrebno. Svaki merge u `main` → automatski deployment na produkciju.

---

## Preduvjeti

### Lokalni razvoj (Docker Compose)

- Docker Desktop ≥ 24.x
- Docker Compose v2 (uključen u Docker Desktop)
- Git

### GitHub Actions (automatski deployment)

Sljedeći **GitHub Secrets** moraju biti postavljeni u repozitoriju (`Settings → Secrets and variables → Actions`):

| Secret | Opis |
|---|---|
| `DOCKER_HUB_USERNAME` | Korisničko ime na Docker Hub (`adzelo2`) |
| `DOCKER_HUB_TOKEN` | Docker Hub access token (ne lozinka) |
| `SSH_PRIVATE_KEY` | Privatni SSH ključ za pristup produkcijskom serveru |
| `JWT_KEY` | JWT signing ključ (koristi se i za CI testove) |

### Produkcijski server (46.224.179.251)

- Docker i Docker Compose instalirani
- SSH pristup kao `root`
- Folder `~/dzelo/` sa produkcijskim `docker-compose.yml`
- `.env` fajl u `~/dzelo/` sa svim environment varijablama

---

## Environment varijable

Svi servisi koriste varijable definirane u `.env` fajlu. Predložak se nalazi na:

```
Project/.env
```

### Obavezne varijable

```env
JWT_KEY=
SA_PASSWORD=
ConnectionStrings__DefaultConnection=
GROQ_API_KEY=
GROQ_API_KEY_2=
```

> **Napomena:** `MCP_SERVER_URL=http://mcp-server:3001/mcp` je hardkodiran u `docker-compose.yml` i ne treba se dodavati u `.env`.

---

## Što se tačno deploya

Pipeline deploya sljedeće Docker image-e na Docker Hub pod tagom `adzelo2/si_deploy`:

| Image tag | Izvor Dockerfile-a | Opis |
|---|---|---|
| `adzelo2/si_deploy:api-latest` | `Project/TelecomSupportSystem/TelecomSupportSystem.API/Dockerfile` | ASP.NET Core 10.0 REST API + SignalR |
| `adzelo2/si_deploy:mcp-latest` | `Project/mcp-server/Dockerfile` | Node.js TypeScript MCP server |
| `adzelo2/si_deploy:frontend-latest` | `Project/frontend/Dockerfile` | React SPA serviran kroz Nginx |

SQL Server **nije buildovan** — koristi se gotov `mcr.microsoft.com/mssql/server:2019-latest` image.

---

## Detaljan opis CD pipeline-a

### Job 1: `build-and-push`

**Radi na:** `ubuntu-latest` GitHub Actions runneru

```
1. Checkout koda iz repozitorija (actions/checkout@v4)
2. Login na Docker Hub (docker/login-action@v3)
   → Koristi secrets: DOCKER_HUB_USERNAME, DOCKER_HUB_TOKEN
3. Build svih Docker image-a (docker compose build)
   → Radi iz direktorija ./Project
   → Builduje: api, mcp-server, frontend
4. Tagiranje i push na Docker Hub:
   → project-api:latest          → adzelo2/si_deploy:api-latest
   → project-mcp-server:latest   → adzelo2/si_deploy:mcp-latest
   → project-frontend:latest     → adzelo2/si_deploy:frontend-latest
```

### Job 2: `deploy`

**Čeka na:** uspješan završetak `build-and-push` joba (`needs: build-and-push`)

**Radi na:** `ubuntu-latest` GitHub Actions runneru

```
5. SSH konekcija na produkcijski server (appleboy/ssh-action@v1.0.3)
   → Host: 46.224.179.251
   → Korisnik: root
   → Port: 22
   → Ključ: secrets.SSH_PRIVATE_KEY
6. Na serveru:
   a. cd ~/dzelo
   b. docker compose down -v       ← gasi kontejnere i briše volumene
   c. docker compose pull          ← preuzima nove image-e s Docker Huba
   d. docker compose up -d         ← pokreće kontejnere u pozadini
```

> **Napomena o `down -v`:** Produkcijski deployment briše SQL Server volumen (`docker compose down -v`) što resetuje bazu pri svakom deploymentu. Ovo je trenutno poznato ograničenje — pogledati sekciju [Poznata ograničenja](#poznata-ograničenja).

---

## Build proces po servisu

### Backend (ASP.NET Core)

Multi-stage build u `TelecomSupportSystem.API/Dockerfile`:

```
Stage 1 (build): mcr.microsoft.com/dotnet/sdk:10.0
  - dotnet restore
  - dotnet build -c Release
  - dotnet publish -c Release → /app/publish

Stage 2 (final): mcr.microsoft.com/dotnet/aspnet:10.0
  - Kopira publish artefakte
  - ENTRYPOINT: dotnet TelecomSupportSystem.API.dll
  - Expose: 8080
```

### Frontend (React + Nginx)

Multi-stage build u `frontend/Dockerfile`:

```
Stage 1 (builder): node:22-alpine
  - npm install --legacy-peer-deps
  - npm run build → /app/dist (Vite build)

Stage 2 (final): nginx:alpine
  - Kopira dist/ u /usr/share/nginx/html
  - Kopira nginx.conf (SPA routing + API proxy + WebSocket proxy)
  - HEALTHCHECK: wget http://localhost:80/index.html
  - Expose: 80
```

Nginx konfiguracija:
- `/` → SPA routing (`try_files $uri /index.html`)
- `/api` → proxy na `api:8080`
- `/chathub` → WebSocket proxy za SignalR chat
- `/notificationhub` → WebSocket proxy za SignalR notifikacije
- Statički asseti → 1 godina cache (`Cache-Control: public, immutable`)

### MCP Server (Node.js TypeScript)

Multi-stage build u `mcp-server/Dockerfile`:

```
Stage 1 (build): node:20-alpine
  - npm install
  - npm run build (tsc) → /app/dist

Stage 2 (final): node:20-alpine
  - npm install --omit=dev
  - CMD: node dist/index.js
  - Expose: 3001
```

### Baza podataka (SQL Server)

Koristi `mcr.microsoft.com/mssql/server:2019-latest` bez prilagođenog build-a.

Migracije se **automatski primjenjuju pri pokretanju** backend servisa (`Program.cs`) uz mehanizam ponovnog pokušaja (10 pokušaja, 3 sekunde pauze):

```csharp
// Automatska migracija na startup sa retry logikom
// Čeka da SQL Server bude spreman
```

Seed podaci (testni korisnici, timovi, paketi, FAQ) se automatski upisuju ako baza ne sadrži podatke.

---

## Kako ručno pokrenuti lokalno (Docker Compose)

### Korak 1 — Preuzmi repozitorij

```bash
git clone <repo-url>
cd SI-TIM2/Project
```

### Korak 2 — Kreiraj `.env` fajl

```bash
cp .env.example .env
```

Uredi `.env` i popuni sve varijable:
- `JWT_KEY`
- `SA_PASSWORD`
- `ConnectionStrings__DefaultConnection`
- `GROQ_API_KEY`
- `GROQ_API_KEY_2`

### Korak 3 — Pokreni sve servise

```bash
docker compose up --build
```

### Korak 4 — Provjeri pokretanje

```bash
# Provjeri status kontejnera
docker compose ps

# Provjeri logove API-ja
docker compose logs api --follow

# Provjeri da li je baza dostupna
docker compose logs sqlserver
```

### Korak 5 — Pristup aplikaciji

| Servis | URL |
|---|---|
| Frontend | http://localhost:80 |
| Backend API | http://localhost:5000/api |
| Swagger dokumentacija | http://localhost:5000/swagger |
| MCP Server | http://localhost:3001 |

---

## Pokretanje testova

### Backend testovi

```bash
cd Project/TelecomSupportSystem
dotnet test TelecomSupportSystem.Tests
```

Sa filterima po domenu:

```bash
# Samo Auth testovi
dotnet test TelecomSupportSystem.Tests --filter "FullyQualifiedName~Auth"

# Bez performansnih testova (preporučeno za lokalni razvoj)
dotnet test TelecomSupportSystem.Tests --filter "Category!=Performance"
```

Sa code coverage izvještajem:

```bash
dotnet test TelecomSupportSystem.Tests --collect:"XPlat Code Coverage"
```

### Frontend testovi

```bash
cd Project/frontend
npx vitest run
```

Sa coverage izvještajem:

```bash
npx vitest run --coverage
```

### Napomena o performansnim testovima

`AuthPerformanceTests.Login_ShouldCompleteWithinTimeLimit_InTestEnvironment` je dokumentovan kao flaky u CI okruženju i ne blokira build. Preporučuje se pokretanje bez performansnih testova pri lokalnoj provjeri.

---

## Produkcijski URL

Nakon uspješnog CD pipeline-a, aplikacija je dostupna na:

- **Primarna adresa:** http://46.224.179.251/
- **Domena:** https://telecomsupport.hodzicmirza.com/

---

## Kako provjeriti da je deployment uspješan

### Automatska provjera (GitHub Actions)

GitHub Actions prikazuje status svakog koraka u `Actions` tabu repozitorija. Zelena kvačica na `deploy` jobu znači uspješan deployment.

### Ručna provjera

```bash
# 1. Provjeri da je frontend dostupan
curl -I http://46.224.179.251/

# 2. Provjeri da API odgovara
curl http://46.224.179.251/api/faq

# 3. Provjeri health endpoint (Nginx)
curl http://46.224.179.251/health

# 4. Na serveru — provjeri status kontejnera
ssh root@46.224.179.251 "docker ps"
```

Očekivani odgovor: HTTP 200 za frontend i API.

---

## CI pipeline (verifikacija prije deploy-a)

Svaki push koji prethodi mergu u `main` prolazi kroz CI pipeline (`.github/workflows/ci.yml`):

### Backend CI

- **Trigger:** push na `main`, `develop`, `feature/**`, `bugfix/**`, `hotfix/**`, `release/**`
- **.NET SDK:** 10.0
- Koraci: `dotnet restore` → `dotnet build -c Release` → `dotnet test`
- Na PR: pokreće testove sa code coverage (Cobertura XML + HTML izvještaj)
- Coverage artefakt: `coverage-backend-html`

### Frontend CI

- **Node.js:** 20 (sa npm cache-om)
- Koraci: `npm install --legacy-peer-deps` → `eslint src/` → `vitest run` → `vite build`
- Na PR: pokreće testove sa coverage (v8 provider)
- Coverage artefakt: `coverage-frontend-html`

CD pipeline (`deploy.yml`) se pokreće **samo na `main`** i pretpostavlja da je CI već prošao kroz branch protection ili prethodni PR.

---

## Veze između servisa

```
Browser
  │
  ▼ HTTP:80
Frontend (Nginx)
  │
  ├─► /api, /chathub, /notificationhub → API (api:8080)
  │
  └─► Statički fajlovi (React SPA)
  
API (api:8080)
  │
  ├─► SQL Server (sqlserver:1433)  ← Entity Framework Core
  └─► MCP Server (mcp-server:3001) ← AI Copilot funkcionalnosti

MCP Server (mcp-server:3001)
  └─► SQL Server (sqlserver:1433)  ← Read-only SQL pristup
```

Svi servisi komuniciraju unutar Docker Compose Docker mreže (automatski kreirana). SQL Server nije izložen vanjski u produkcijskom okruženju u idealnom slučaju.

---

## Poznata ograničenja

### Kritično: Brisanje baze pri deploymentu

Produkcijski deployment koristi `docker compose down -v` što **briše Docker volumen** i time sve podatke u bazi. Ovo je namijenjeno za razvojno okruženje, ali se prenijelo i na produkciju.

**Posljedica:** Svaki deployment na `main` resetuje produkcijsku bazu. Podaci preživljavaju isključivo zahvaljujući automatskom seed mehanizmu koji puni testne korisnike i pakete.

**Privremeno rješenje:** Backend automatski primjenjuje migracije i seed podatke pri startu, tako da aplikacija bude funkcionalna odmah nakon deploymenta sa test podacima.

### Ručni koraci na produkcijskom serveru

Produkcijski `~/dzelo/docker-compose.yml` i `~/dzelo/.env` nisu dio ovog repozitorija i moraju biti ručno konfigurirani na serveru. Ovo je jedini nedokumentovani ručni korak u procesu.

### SQL Server na Linux/Docker

`MSSQL_ALLOW_RUNNING_AS_ROOT: "1"` je postavljeno u Docker Compose zbog potrebe za pokretanjem SQL Server kontejnera kao root na nekim hosting platformama.

### Ovisnost o Docker Hub dostupnosti

Build-and-push job zahtijeva dostupnost Docker Hub servisa. Ako Docker Hub nije dostupan, deployment neće uspjeti.

### Nema health check-a u CD pipeline-u

Pipeline ne vrši eksplicitnu provjeru da li je aplikacija zdravo pokrenuta nakon `docker compose up -d`. Uspjeh deploymenta se mjeri uspjehom SSH komande, ne odgovorom aplikacije.

---

## Rješavanje čestih problema

### Kontejneri se ne pokreću

```bash
# Na serveru
docker compose logs api
docker compose logs sqlserver
```

Najčešći uzrok: SQL Server nije spreman kad API pokuša konekciju. Backend ima retry mehanizam (10 pokušaja × 3 sec), ali ako SQL Server traje duže, API se može resetovati.

### Frontend pokazuje stari sadržaj

```bash
# Na serveru
docker compose pull frontend
docker compose up -d --force-recreate frontend
```

### API vraća 500

Provjeri da li su environment varijable ispravno postavljene u `.env` na serveru:

```bash
cat ~/dzelo/.env
```

### GitHub Actions deployment job padne

Provjeri:
1. SSH ključ u GitHub Secrets odgovara ključu koji ima pristup serveru
2. Folder `~/dzelo` postoji na serveru
3. `docker compose` je dostupan kao komanda na serveru (`docker compose` v2, ne `docker-compose`)

### Build pada zbog `--legacy-peer-deps`

Frontend koristi `--legacy-peer-deps` zbog konflikta peer zavisnosti između React 19 i nekih paketa. Ovo je namjerna odluka dokumentovana u CI i Docker build konfiguraciji.
