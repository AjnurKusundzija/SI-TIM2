# Tehnička Dokumentacija (Setup)

---

## 1. Tehnološki Stack

### Backend
| Tehnologija | Verzija | Svrha | Napomene |
|---|---|---|---|
| **.NET** | 10.0 | Runtime i Framework | Višeplatformski, visoke performanse |
| **ASP.NET Core** | 10.0 | Web API Framework | RESTful API server |
| **Entity Framework Core** | 10.0.0 | ORM i Pristup Podacima | Sloj apstrakcije baze podataka |
| **MySQL Server** | 8.0+ | Relaciona Baza Podataka | Primarno skladište podataka |
| **SignalR** | 10.0.0 | Real-time Komunikacija | WebSocket podrška za notifikacije |
| **Swagger/OpenAPI** | 10.1.7 | API Dokumentacija | Auto-generisana API dokumentacija |

### Frontend
| Tehnologija | Verzija | Svrha | Napomene |
|---|---|---|---|
| **React** | 19.2.4 | UI Framework | Moderni komponentni UI |
| **Vite** | 8.0.4+ | Build Alat | Brzi development i produkcijski buildovi |
| **React Router** | 7.14.1 | Navigacija | Client-side routing |
| **@microsoft/signalr** | 8.0.0+ | WebSocket Klijent | Real-time notifikacije putem SignalR |
| **TailwindCSS** | 4.0.0+ | Stilizacija | Utility-first CSS framework |
| **Zustand** | 4.4.0+ | Upravljanje Stanjem | Jednostavno za korištenje |

### DevOps i Infrastruktura
| Tehnologija | Verzija | Svrha | Napomene |
|---|---|---|---|
| **Docker** | 18.0+ | Kontejnerizacija | Deployment baziran na kontejnerima |
| **Docker Compose** | 2.0+ | Orkestracija višestrukih kontejnera | Lokalno dev okruženje |
| **GitHub Actions** | Najnoviji | CI/CD Pipeline | Automatizovano testiranje i deployment |
| **GitHub Secrets** | N/A | Upravljanje Okruženjem | Sigurno čuvanje kredencijala |
| **Git** | 2.40+ | Verzionisanje Koda | Upravljanje izvornim kodom |


---

## 3. Strategija Grananja (GitFlow)

### Vrste Grana i Konvencija Imenovanja

#### Glavne Grane
- **`main`** (produkcija)
  - Sadrži samo kod spreman za release
  - Zaštićena: zahtijeva PR review + prolaz testova
  - Svaki commit označen verzijom (v1.0.0, v1.0.1, itd.)
  - Direktni/force pushovi nisu dozvoljeni

- **`develop`** (integracija)
  - Paralelna kopija `main`-a koja služi kao integracijska grana
  - Sve feature grane se kreiraju iz `develop`-a i spajaju nazad u njega putem Pull Requesta.
  - Integracijska grana za završene funkcionalnosti
  - Zaštićena: zahtijeva PR review + prolaz testova
  - Osnova za sve feature/bugfix grane

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

#### Rad na Novoj Funkcionalnosti
```bash
# Kreiranje nove feature grane iz develop
git flow feature start naziv-funkcionalnosti

# Rad na funkcionalnosti, commitanje izmjena
git add .
git commit -m "feat: opis izmjene"

# Push na remote i otvaranje Pull Requesta na GitHubu
git push origin feature/naziv-funkcionalnosti
# NE koristiti "git flow feature finish" — umjesto toga otvori PR na GitHubu
```

#### Release
```bash
# Kreiranje release grane iz main
git flow release start v1.0.0

# Ažuriranje verzija, CHANGELOG-a i sl.
git commit -m "chore: bump version to v1.0.0"

# Push i otvaranje PR-a prema main
git push origin release/v1.0.0
```

#### Hotfix
```bash
# Kreiranje hotfix grane direktno iz main
git flow hotfix start naziv-ispravke

# Ispravka greške
git add .
git commit -m "fix: opis ispravke"

# Push i otvaranje PR-a prema main
git push origin hotfix/naziv-ispravke
```

 **Proces Pull Requesta**
   - Svi testovi prolaze (lokalno + CI/CD???)
   - Code review od strane 1+ člana tima
   - Nema konflikata sa develop granom
   - Prati Definition of Done
   - Komentari riješeni
   - Spajanje putem "Create a merge commit"


### Pravila Zaštite Grana (GitHub Postavke)
- Zahtijevaj pull request review prije spajanja (minimum 1 reviewer)
- Zahtijevaj prolaz status provjera (CI/CD pipeline)
- Zahtijevaj da grane budu ažurne prije spajanja
- Zahtijevaj da code conversation budu riješeni prije spajanja

---

## 5. Strategija Deploymenta

### Pregled Arhitekture

```
┌─────────────────────────────────────────────────────────┐
│                    LOAD BALANCER                        │
│                  (Azure LB / nginx)                     │
└────────────────┬──────────────────┬──────────────────── ┘
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
            │         (MySQL 8.0)        │
            │      Port: 3306 (TLS)      │
            └────────────────────────────┘
```

### Opcije Deploymenta (odlučiti se na jednu)


#### Opcija 1: Docker + Virtuelna Mašina
- **Backend:** Docker kontejner na Linux VM-u
- **Frontend:** Statički hosting na CDN-u ili isti server putem nginx-a
- **Baza Podataka:** MySQL 8.0 na zasebnom VM-u (razdvajanje odgovornosti)
- **Load Balancing:** nginx reverse proxy

#### Opcija 2: Cloud PaaS
- **Backend:** Upravljani app service
- **Frontend:** Statički web hosting
- **Baza Podataka:** Upravljani MySQL servis (npr. AWS RDS for MySQL, Azure Database for MySQL)
- **SSL/TLS:** Auto-upravljano od strane cloud provajdera

#### Opcija 3: Kubernetes
- **Backend:** Kubernetes deployment sa 3+ replika
- **Frontend:** Kubernetes servis sa ingressom
- **Baza Podataka:** Upravljani MySQL servis (npr. Azure Database for MySQL)

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

## 8. Upravljanje Konfiguracijom

### Postavke Specifične za Okruženje

**Razvoj (appsettings.Development.json)**
**Produkcija (appsettings.Production.json)**


### Upravljanje Tajnama
- Razvoj: Lokalne user secrets (`dotnet user-secrets set`)
- Produkcija: Azure Key Vault
- CI/CD: GitHub Secrets

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
