# AI Usage Log – Sprint 5

AI Usage Log je obavezan u AI-enabled fazi projekta.

Za svaki relevantan slucaj koristenja AI potrebno je evidentirati:
- Datum
- Sprint broj
- Alat koji je koristen
- Svrha koristenja
- Kratak opis zadatka ili upita
- Sta je AI predlozio ili generisao
- Sta je tim prihvatio
- Sta je tim izmijenio
- Sta je tim odbacio
- Rizici, problemi ili greske koje su uocene
- Ko je koristio alat

AI Usage Log ne sluzi za kaznjavanje koristenja AI, nego za transparentnost i procjenu zrelosti u koristenju alata.

---

## Unos #1

| Polje | Detalji |
|---|---|
| Datum | 25.04.2026 |
| Sprint broj | Sprint 5 |
| Alat koji je koristen | GPT 5.3 medium |
| Svrha koristenja | Pomoc pri pisanju dokumentacije i izradi template-a za Sprint 5 deliverable-e |
| Kratak opis zadatka ili upita | Koristen AI za prijedlog strukture i tekstualnog sadrzaja za Sprint Backlog, Decision Log i AI Usage Log . |
| Sta je AI predlozio ili generisao | Predlozeni naslovi, sekcije, bullet stavke i pocetni tekst za dokumentaciju, ukljucujuci standardizovan format zapisa. |
| Sta je tim prihvatio | Osnovnu strukturu dokumenata i veci dio predlozenog teksta nakon interne provjere. |
| Sta je tim izmijenio | Terminologiju, stil pisanja i pojedine formulacije radi uskladjivanja sa projektom i zahtjevima predmeta. |
| Sta je tim odbacio | Genericke i neprecizne formulacije koje nisu bile direktno vezane za nas projekat. |
| Rizici, problemi ili greske koje su uocene | Rizik od preopstih odgovora i mogucih netacnih formulacija; sve AI prijedloge je bilo potrebno rucno validirati prije usvajanja. |
| Ko je koristio alat | Ajnur Kušundžija |

## Unos #2

| Polje | Detalji |
|---|---|
| Datum | 23.04.2026 |
| Sprint broj | Sprint 5 |
| Alat koji je koristen | Claude Sonnet 4.6 |
| Svrha koristenja | Pomoc pri implementaciji PB-19 (Login korisnika) |
| Kratak opis zadatka ili upita | Koristen AI za implementaciju login funkcionalnosti: JWT autentifikacija na backendu (AuthController, AuthService, refresh tokeni, rate limiting, seed korisnika) i frontend login stranica (Login.jsx, AuthContext.jsx, ProtectedRoute.jsx, api.js sa Axios interceptorom, authService.js). |
| Sta je AI predlozio ili generisao | Kod za AuthService (JWT generacija, BCrypt provjera, refresh token logika), AuthController, frontend Login.jsx stranicu sa React Hook Form, AuthContext sa useAuth(), ProtectedRoute komponentu, Axios instancu sa JWT interceptorom, rate limiting u Program.cs., dodatno ci.yml |
| Sta je tim prihvatio | Vecinu predlozene implementacije: strukturu AuthService-a, JWT claims (NameIdentifier, Email, Role, GivenName, Surname), refresh token mehanizam, frontend AuthContext i ProtectedRoute, te Axios interceptor logiku. |
| Sta je tim izmijenio | Prilagodili smo nazive entiteta i DTO-ova projektu, podesili connection string, Docker Compose konfiguraciju, i ci.yml. Takodjer za JWT key, umjesto _configuration["Jwt:Key"]!, koristimo Environment.GetEnvironmentVariable("JWT_KEY") i .env zbog prevencije curenja informacija putem appsettings.json |
| Sta je tim odbacio | Prijedloge za error poruke koji bi mogli otkriti da li email ili lozinka nisu ispravni (zahtjev US-3), i neke generalizovane pristupe koji nisu odgovarali 3-slojnoj arhitekturi projekta. JWT key |
| Rizici, problemi ili greske koje su uocene | AI je inicijalno predlozio direktno ubacivanje DbContext-a u servis sto je krsilo arhitekturna pravila – ispravljeno kroz repository pattern. Takodjer, JWT key je prebacen u .env zbog sigurnosti informacija. |
| Ko je koristio alat | Uma Mahmutovic |

---

Napomena: Ovaj AI Usage Log je zivi dokument i azurira se kroz sprintove.
