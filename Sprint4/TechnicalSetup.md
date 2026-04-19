# Tehnička Dokumentacija (Setup)

---

## 1. Tehnološki Stack

### Backend
| Tehnologija | Verzija | Svrha | Napomene |
|---|---|---|---|
| **.NET** | 10.0 | Runtime i Framework | Višeplatformski, visoke performanse |
| **ASP.NET Core** | 10.0 | Web API Framework | RESTful API server |
| **Entity Framework Core** | 10.0.0 | ORM i Pristup Podacima | Sloj apstrakcije baze podataka |
| **SQL Server Express** | 2019+ | Relaciona Baza Podataka | Primarno skladište podataka (moguće prebaciti na PostgreSQL) |
| **SignalR** | 10.0.0 | Real-time Komunikacija | WebSocket podrška za notifikacije |
| **JWT (System.IdentityModel.Tokens.Jwt)** | 7.4.0 | Autentifikacija | Sigurna autentifikacija bazirana na tokenima |
| **BCrypt.Net-Next** | 4.0.3 | Hashiranje Lozinki | Sigurno čuvanje lozinki |
| **Serilog** | 4.0.0 | Logovanje | Strukturirano logovanje |
| **Swagger/OpenAPI** | 10.1.7 | API Dokumentacija | Auto-generisana API dokumentacija |

### Frontend
| Tehnologija | Verzija | Svrha | Napomene |
|---|---|---|---|
| **React** | 19.2.4 | UI Framework | Moderni komponentni UI |
| **Vite** | 8.0.4+ | Build Alat | Brzi development i produkcijski buildovi |
| **React Router** | 7.14.1 | Navigacija | Client-side routing |
| **Axios** | 1.6.0+ | HTTP Klijent | API komunikacija |
| **Socket.io Client** | 4.7.0+ | WebSocket Klijent | Real-time notifikacije |
| **TailwindCSS** | 4.0.0+ | Stilizacija | Utility-first CSS framework |
| **React Hook Form** | 7.50.0+ | Upravljanje Formama | Lagano upravljanje formama |
| **date-fns** | 3.0.0+ | Datum Utiliti | Obrada datuma uz podršku za vremenske zone |
| **Zustand** | 4.4.0+ | Upravljanje Stanjem | Jednostavno za korištenje |

### DevOps i Infrastruktura
| Tehnologija | Verzija | Svrha | Napomene |
|---|---|---|---|
| **Docker** | 18.0+ | Kontejnerizacija | Deployment baziran na kontejnerima |
| **Docker Compose** | 2.0+ | Orkestracija višestrukih kontejnera | Lokalno dev okruženje |
| **GitHub Actions** | Najnoviji | CI/CD Pipeline | Automatizovano testiranje i deployment |
| **GitHub Secrets** | N/A | Upravljanje Okruženjem | Sigurno čuvanje kredencijala |
| **Git** | 2.40+ | Verzionisanje Koda | Upravljanje izvornim kodom |

### Verzije Baze Podataka
- **SQL Server Express:** 2019 Community Edition (besplatna verzija za razvoj)
- **Alternativa:** PostgreSQL 15+ (za produkcijski deployment)
- **Lokalni Razvoj:** Koristi Docker kontejner sa SQL Server Express

---

## 2. Postavljanje Razvojnog Okruženja

### Preduvjeti
```bash
dotnet --version  # Treba biti 10.0.0+

node --version    # Treba biti 18.0+
npm --version     # Treba biti 9.0+

docker --version  # Treba biti 18.0+

git --version     # Treba biti 2.40+
```

### Inicijalno Postavljanje
```bash
git clone https://github.com/AjnurKusundzija/SI-TIM2.git
cd SI-TIM2/Project/TelecomCustomerSupportSystem

cd backend
dotnet restore

cd ../frontend
npm install

cd ../..
docker-compose up -d 

cd backend
dotnet ef database update
```

---

## 3. Strategija Grananja (GitFlow)

### Vrste Grana i Konvencija Imenovanja

#### Glavne Grane
- **`main`** (produkcija)
  - Sadrži samo kod spreman za release
  - Zaštićena: zahtijeva PR review + prolaz testova
  - Svaki commit označen verzijom (v1.0.0, v1.0.1, itd.)
  - Auto-deploy na produkciju
  - Direktni pushovi nisu dozvoljeni

- **`develop`** (integracija)
  - Paralelna kopija `main`-a koja služi kao integracijska grana
  - Sve feature grane se kreiraju iz `develop`-a i spajaju nazad u njega putem Pull Requesta.
  - Integracijska grana za završene funkcionalnosti
  - Zaštićena: zahtijeva PR review + prolaz testova
  - Osnova za sve feature/bugfix grane
  - Auto-deploy na staging

#### Pomoćne Grane

**Feature grane** (iz `develop`)
 - Feature grane prate konvenciju `feature/naziv-funkcionalnosti` i uvijek se baziraju na `develop`

```
feature/auth-login                   # PB-19
feature/ticket-creation              # PB-22
feature/websocket-notifications      # Real-time poruke
feature/admin-dashboard              # PB-45
```

**Bugfix grane** (iz `develop`)
```
bugfix/authentication-token-renewal
bugfix/websocket-reconnection
```

**Release grane**
 - Kreiraju se iz `develop`-a i prate unaprijed definirani release plan
 - Nakon završetka, spajaju se nazad u `main` i tag-uju verzijom, a izmjene se propagiraju i u `develop` kako bi ostao sinhronizovan
```
release/v1.0.0
release/v1.0.1
```

**Hotfix grane** (iz `main`, spajaju se u `main` i nazad u `develop`)
 - Kreiraju se direktno iz `main`-a za hitne ispravke u produkciji. Nakon spajanja u `main`, obavezno se spajaju i u `develop`
```
hotfix/security-patch-jwt
hotfix/database-connection-pool
```

### Pravila Toka Rada sa Granama

1. **Razvoj Funkcionalnosti**
   ```bash
   git checkout develop
   git pull origin develop
   git checkout -b feature/naziv-funkcionalnosti
   # ... rad na funkcionalnosti
   git push origin feature/naziv-funkcionalnosti
   # Kreiraj Pull Request na GitHubu
   ```

2. **Proces Pull Requesta**
   - Svi testovi prolaze (lokalno + CI/CD)
   - Code review od strane 1+ člana tima
   - Nema konflikata sa develop granom
   - Prati Definition of Done
   - Komentari riješeni
   - Spajanje putem "Create a merge commit"

3. **Rješavanje Konflikata**
   ```bash
   git fetch origin
   git rebase origin/develop
   # Popravi konflikte u IDE-u
   git add .
   git rebase --continue
   git push origin feature/naziv-funkcionalnosti -f
   ```

4. **Proces Releasea**
   ```bash
   git checkout -b release/v1.0.0 develop
   # Ažuriraj brojeve verzija, CHANGELOG
   git commit -m "Verzija 1.0.0"
   git push origin release/v1.0.0
   # Kreiraj PR prema main
   # Spoji u main sa tagom: git tag v1.0.0
   # Spoji nazad u develop
   ```

### Pravila Zaštite Grana (GitHub Postavke)
- Zahtijevaj pull request review prije spajanja (minimum 1 reviewer)
- Zahtijevaj prolaz status provjera (CI/CD pipeline)
- Zahtijevaj da grane budu ažurne prije spajanja
- Zahtijevaj da code conversation budu riješeni prije spajanja
- Odbaci stare pull request approvals kada se pushaju novi commiti
- Ograniči ko može pushati na main/develop (samo administratori)

---

## 4. Arhitektura Baze Podataka

### SQL Server + Entity Framework Core

- Odlična integracija sa .NET/EF Core
- ACID usklađenost za konzistentnost tiketa
- Express Edition besplatna za razvoj
- Laka migracija na produkcijski SQL Server/Azure

#### Postavljanje Baze Podataka

**Lokalni Razvoj (Docker)**
```bash
# U korijenu projekta, docker-compose.yml:
docker-compose up -d

# Connection string u appsettings.Development.json:
"DefaultConnection": "Server=localhost,1433;Database=TelecomDB;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;Encrypt=false;"
```

**Produkcija (SQL Server na VM-u ili Azure SQL)**
```
"DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=TelecomDB;Persist Security Info=False;User ID=admin;Password=***;Encrypt=True;Connection Timeout=30;"
```

**Entity Framework Migracije**
```bash
dotnet ef migrations add InitialCreate

dotnet ef database update

# Za produkciju, koristi:
dotnet ef database update -- --environment Production
```

#### Strategija Perzistencije Podataka
- Svi korisnički podaci čuvaju se u relacionoj bazi podataka
- Dostupna anonimizacija podataka usklađena sa GDPR-om
- Audit logovi za sve operacije (posebna tabela za performanse)
- Automatske sigurnosne kopije konfigurisane (dnevno za produkciju)

#### Skalabilnost i Performanse
- Connection pooling: Min 5, Max 100 konekcija
- Optimizacija upita: Indeksirano po stranim ključevima, statusu, datumima
- Caching sloj: Redis za često pristupane podatke (buduće poboljšanje)
- Database replike za read skaliranje (produkcija)

---

## 5. Strategija Deploymenta

### Pregled Arhitekture

```
┌─────────────────────────────────────────────────────────┐
│                    LOAD BALANCER                        │
│                  (Azure LB / nginx)                      │
└────────────────┬──────────────────┬────────────────────┘
                 │                  │
        ┌────────▼─────┐   ┌───────▼──────┐
        │  Web Server 1 │   │ Web Server 2  │
        │ (Docker: API) │   │ (Docker: API) │
        │  Port: 443    │   │  Port: 443    │
        └────────┬──────┘   └───────┬───────┘
                 │                  │
                 └────────┬─────────┘
                          │
            ┌─────────────▼──────────────┐
            │   Dijeljeni Server Baze    │
            │   (SQL Server / Azure SQL) │
            │      Port: 1433 (TLS)      │
            └────────────────────────────┘
```

### Opcije Deploymenta (odlučiti se na jednu)


#### Opcija 1: Docker + Virtuelna Mašina
- **Backend:** Docker kontejner na Linux VM-u
- **Frontend:** Statički hosting na CDN-u (Azure Blob Storage / AWS S3) ili isti server putem nginx-a
- **Baza Podataka:** SQL Server na zasebnom VM-u (razdvajanje odgovornosti)
- **Load Balancing:** Azure Load Balancer / nginx reverse proxy

#### Opcija 2: Azure App Service
- **Backend:** Azure App Service (upravljani PaaS)
- **Frontend:** Azure Static Web Apps ili App Service
- **Baza Podataka:** Azure SQL Database
- **SSL/TLS:** Auto-upravljano od strane Azurea

#### Opcija 3: Kubernetes
- **Backend:** Kubernetes deployment sa 3+ replika
- **Frontend:** Kubernetes servis sa ingressom
- **Baza Podataka:** Upravljani servis baze podataka (Azure Database for SQL Server)

### NFR-05: Arhitektura za 99.5% Dostupnosti

**Komponente za Visoku Dostupnost:**

1. **Aplikacijski Nivo (cilj 99.99%)**
   - 3+ Docker kontejner replike (load balanced)
   - Auto-restart pri grešci
   - Health checks svakih 30 sekundi
   - Auto-rollback pri neuspješnom deploymentu

2. **Nivo Baze Podataka (cilj 99.99%)**
   - Always-On Availability Group (SQL Server) ILI Failover replike
   - Automatske sigurnosne kopije (svaki sat, čuvanje 30 dana)
   - RTO: 5 minuta
   - RPO: 5 minuta

3. **Mrežni Nivo (cilj 99.99%)**
   - Load balancer sa health probama
   - CDN za statičke resurse (geo-distribuirano)
   - DDoS zaštita omogućena

4. **Monitoring i Alerting**
   - Application Insights za monitoring backenda
   - Monitoring baze podataka putem SQL Server agenta
   - Alert na: visok CPU, memoriju, veličinu baze, greške konekcije
   - Alert na svaki pad poda
   - Automatsko praćenje incidenata u GitHub Issues

**Budžet Nedostupnosti (99.5%):** 3.6 sati godišnje = ~18 minuta mjesečno

---

## 6. CI/CD Pipeline (GitHub Actions)

### Automatizovani Tokovi Rada

**Na Pull Requestu (prije spajanja)**
- Build backenda (dotnet build)
- Pokretanje unit testova (dotnet test)
- Build frontenda (npm build)
- Pokretanje ESLint-a
- SonarQube analiza koda
- Skeniranje ranjivosti zavisnosti
- Izvještaj o pokrivenosti koda

**Na Spajanju u develop**
- Build i testiranje
- Deploy na staging okruženje
- Pokretanje integracijskih testova
- Pokretanje E2E testova
- Stress testiranje

**Na Spajanju u main (Release)**
- Build i testiranje
- Kreiranje GitHub Releasea sa tagom
- Build Docker slika
- Push na Docker registry
- Deploy na produkciju
- Migracije baze podataka (ako postoje)
- Smoke testiranje

---

## 7. Sigurnosna Lista Provjere

### Autentifikacija i Autorizacija
- JWT tokeni sa istekom od 15 minuta
- Refresh tokeni sa istekom od 7 dana
- Hashiranje lozinki sa bcryptom
- Rate limiting na login endpointu
- RBAC (Role-Based Access Control) na svim endpointima
- CORS konfigurisan

### Zaštita Podataka
- Sva komunikacija putem HTTPS/TLS 1.2+
- Enkripcija baze podataka u mirovanju
- Osjetljiva polja enkriptovana u bazi
- Anonimizacija PII podataka pri brisanju
- Audit logovi nepromjenjivi 

### API Sigurnost
- Validacija unosa na svim endpointima
- Prevencija SQL injekcije
- Prevencija XSS-a (CSP headeri)
- CSRF zaštita na operacijama koje mijenjaju stanje
- Rate limiting
- API key autentifikacija za integracije trećih strana (buduće)

---

## 8. Upravljanje Konfiguracijom

### Postavke Specifične za Okruženje

**Razvoj (appsettings.Development.json)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TelecomDB_Dev;User Id=sa;Password=DevPassword123!;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "Jwt": {
    "SecretKey": "your-dev-secret-key-min-32-chars",
    "Issuer": "http://localhost:5000",
    "Audience": "http://localhost:3000",
    "ExpirationMinutes": 15
  }
}
```

**Produkcija (appsettings.Production.json)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "***enkriptovano u Azure Key Vault***"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "Jwt": {
    "SecretKey": "***iz Azure Key Vault***",
    "ExpirationMinutes": 15
  }
}
```

### Upravljanje Tajnama
- Razvoj: Lokalne user secrets (`dotnet user-secrets set`)
- Produkcija: Azure Key Vault
- CI/CD: GitHub Secrets

---

## 9. Monitoring i Logovanje

### Strategija Logovanja
- Strukturirano logovanje sa Serilogom
- Nivoi loga: Debug, Information, Warning, Error, Fatal
- Svi API zahtjevi logovani (metoda, putanja, statusni kod, trajanje)
- Sve operacije baze podataka logovane
- Audit trail za poslovne operacije (kreiranje tiketa, promjena statusa, dodjele)

### Monitoring Dashboardi
- Application Insights dashboard (prosječno vrijeme odgovora, stopa grešaka, trajanje zavisnosti)
- Monitor performansi baze podataka (spori upiti, status connection poola)
- Monitor infrastrukture (CPU, memorija, disk, mreža)
- Monitor dostupnosti (ping vanjskog health endpointa svakih 1 min)

### Pravila Alertinga
- CPU > 80% tokom 5 minuta → Alert
- Memorija > 90% → Hitni alert
- Odgovor baze podataka > 1s → Alert
- Stopa grešaka > 1% → Alert
- Health check pada → Hitni alert
- Neuspješan deployment → Hitni alert

---

## 10. Smjernice za Tim

### Standardi Koda
- **C#:** Prati Microsoftove konvencije kodiranja u C#
- **JavaScript/React:** Prati Airbnb JavaScript Style Guide
- **SQL:** Koristi snake_case za objekte baze podataka
- **API Rute:** RESTful konvencije imenovanja
- **Poruke Commita:** Format Conventional Commits (`feat:`, `fix:`, `docs:`, itd.)

### Zahtjevi Code Reviewa
- Minimum 1 odobrenje prije spajanja
- Zahtijevane izmjene moraju biti riješene
- CI/CD pipeline mora proći
- Konflikti nisu dozvoljeni
- Najmanje 80% pokrivenosti testovima za novi kod

---

## 11. Strategija Vraćanja na Prethodnu Verziju (Rollback)

U slučaju neuspješnog produkcijskog deploymenta:

1. **Automatski Rollback (u roku od 5 minuta)**
   - Kubernetes detektuje nezdrav pod
   - Automatski smanjuje neuspješnu verziju
   - Skalira prethodnu stabilnu verziju
   - Health checks potvrđuju uspjeh

2. **Ručni Rollback (ako je potrebno)**
   ```bash
   # Vrati se na prethodnu Docker sliku
   docker service update --image previous-image:version api_service
   
   # Vrati bazu podataka (ako su migracije potrebne)
   dotnet ef database update [PrethodnaM igracija] --environment Production
   ```

3. **Odgovor na Incident**
   - Objavi incident i dokumentuj uzrok u GitHub Issue-u
   - Planiraj mjere prevencije
   - Zakaži debriefing tima

---

## 13. Shema Verzionisanja

**Semantičko Verzionisanje: MAJOR.MINOR.PATCH**

- **MAJOR:** Promjene koje nisu kompatibilne (v2.0.0)
- **MINOR:** Nove funkcionalnosti, kompatibilne (v1.1.0)
- **PATCH:** Ispravke grešaka, kompatibilne (v1.0.1)

---