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

## Unos #3

| Polje | Detalji |
|---|---|
| Datum | 28.04.2026 |
| Sprint broj | Sprint 5 |
| Alat koji je koristen | Claude Code (claude-sonnet-4-6) |
| Svrha koristenja | Generisanje unit testova za PB-19 (login) |
| Kratak opis zadatka ili upita | Na osnovu postojećeg auth sistema (AuthService, AuthController, authService.js, AuthContext, Login, ProtectedRoute), TestStrategy dokumenta i korisničkih priča US-1, US-2, US-3 — AI je trebao napisati unit testove koji pokrivaju sve acceptance kriterije za prijavu, odjavu i prikaz greške. |
| Sta je AI predlozio ili generisao | Backend xUnit test projekt (`TelecomSupportSystem.Tests`) s `AuthServiceTests` (19 testova) i `AuthControllerTests` (7 testova) koristeći Moq za mockiranje repozitorija i konfiguracije; frontend Vitest test fajlovi (`authService.test.js` — 12 testova, `AuthContext.test.jsx` — 3 testa, `ProtectedRoute.test.jsx` — 2 testa, `Login.test.jsx` — 4 testa) koristeći Testing Library. |
| Sta je tim prihvatio | Sve test fajlove i njihovu strukturu; podjelu testova po slojevima (service, controller, frontend servisi, context, komponente). |
| Sta je tim izmijenio | Uklonjen zaseban `vitest.config.js` i konfiguracija je spojena u `vite.config.js`; ispravljen selektor u `Login.test.jsx` (`getByText(/sign in/i)` → `getByRole('button', { name: /sign in/i })`) jer je tekst "Sign in" bio prisutan na više mjesta u DOM-u; uklonjen pogrešan assertion koji provjerava da riječ "email" nije u dokumentu (label "Email" je uvijek prisutan u formi). |
| Sta je tim odbacio | Ništa strukturalno — sve predložene izmjene su prihvaćene nakon provjere i lokalnog testiranja. |
| Rizici, problemi ili greske koje su uocene | Kompatibilnost između `@vitejs/plugin-react` v6 i Vitest v1 zahtijevala je nestandardno rješenje (`process.env.VITEST` uvjet) zbog razlike između OXC (produkcijski build) i esbuild (test okruženje). Instaliran `jsdom` paket koji nije bio u `package.json`. |
| Ko je koristio alat | Uma Mahmutović |
---

## Unos #4

| Polje | Detalji |
|---|---|
| Datum | 30.04.2026 |
| Sprint broj | Sprint 5 |
| Alat koji je koristen | Manus AI |
| Svrha koristenja | Pomoc pri doradi frontend detalja i implementaciji izmjene jezika |
| Kratak opis zadatka ili upita | Koristen AI za prijedloge poboljsanja frontend prikaza, uskladjivanje UI detalja i implementaciju opcije za promjenu jezika u aplikaciji. |
| Sta je AI predlozio ili generisao | Prijedloge za organizaciju frontend komponenti, tekstualne izmjene u korisnickom interfejsu, nacin prikaza opcije za odabir jezika i povezivanje izabranog jezika sa prikazanim tekstom u aplikaciji. |
| Sta je tim prihvatio | Osnovni pristup za uredjivanje frontend detalja i koncept izmjene jezika kroz postojece frontend komponente. |
| Sta je tim izmijenio | Prilagodjeni su nazivi, tekstovi i stilovi stvarnom izgledu aplikacije, kao i nacin integracije promjene jezika sa postojecim frontend kodom. |
| Sta je tim odbacio | Genericke prijedloge koji nisu odgovarali postojecem dizajnu aplikacije ili bi uvodili nepotrebnu kompleksnost. |
| Rizici, problemi ili greske koje su uocene | Rizik da AI predlozi nekonzistentne nazive ili tekstove u odnosu na ostatak aplikacije; sve izmjene su morale biti rucno provjerene kroz postojece UI tokove. |
| Ko je koristio alat | Ajdin Dželo, Ajnur Kušundžija, Uma Mahmutović |


## Unos #5

| Polje | Detalji |
|---|---|
| Datum | 30.04.2026 |
| Sprint broj | Sprint 5 |
| Alat koji je koristen | Manus AI |
| Svrha koristenja | Pomoc pri implementaciji filtriranja tiketa |
| Kratak opis zadatka ili upita | Koristen AI za prijedlog nacina filtriranja tiketa na frontend strani, ukljucujuci filtriranje po relevantnim atributima tiketa i azuriranje prikazane liste nakon promjene filtera. |
| Sta je AI predlozio ili generisao | Logiku za primjenu filtera nad listom tiketa, prijedlog UI kontrola za filtriranje i osnovni tok azuriranja rezultata filtriranja u korisnickom interfejsu. |
| Sta je tim prihvatio | Koncept filtriranja tiketa kroz postojece stanje i komponente, uz prikaz samo tiketa koji odgovaraju odabranim kriterijima. |
| Sta je tim izmijenio | Prilagodjena je implementacija stvarnim poljima tiketa, postojecim nazivima komponenti i nacinu dohvatanja podataka u projektu. |
| Sta je tim odbacio | Prijedloge za dodatne filtere i kompleksnije pretrage koji nisu bili dio trenutnog opsega Sprinta 5. |
| Rizici, problemi ili greske koje su uocene | Potencijalni rizik da filtriranje ne bude uskladjeno sa backend modelom podataka ili da ne pokrije sve kombinacije filtera; potrebno je rucno testiranje osnovnih i negativnih scenarija. |
| Ko je koristio alat | Eldar Hadžiselimović |

## Unos #6

| Polje | Detalji |
|---|---|
| Datum | 30.04.2026 |
| Sprint broj | Sprint 5 |
| Alat koji je koristen | Manus AI |
| Svrha koristenja | Pomoc pri implementaciji PB-22 (Kreiranje novog tiketa) |
| Kratak opis zadatka ili upita | Koristen AI za prijedlog implementacije funkcionalnosti kreiranja novog tiketa na osnovu Sprint 5 backloga i korisnickih prica US-8, US-9 i US-10. |
| Sta je AI predlozio ili generisao | Prijedlog forme za prijavu problema, polja za unos naslova i opisa tiketa, odabir tipa i prioriteta tiketa, validaciju obaveznih polja i tok spremanja novog tiketa. |
| Sta je tim prihvatio | Osnovni tok kreiranja tiketa, strukturu forme i validaciju obaveznih podataka u skladu sa acceptance kriterijima za PB-22. |
| Sta je tim izmijenio | Prilagodjeni su nazivi polja, poruke validacije i povezivanje forme sa postojecim frontend i backend dijelovima projekta. |
| Sta je tim odbacio | Genericke prijedloge koji nisu bili vezani za trenutni sprint backlog, kao i dodatne opcije za tiket koje nisu bile dio opsega US-8, US-9 i US-10. |
| Rizici, problemi ili greske koje su uocene | Rizik da AI predlozi polja ili tokove koji nisu uskladjeni sa postojecim modelom tiketa; implementacija je zahtijevala rucnu provjeru obaveznih polja, prioriteta, tipa tiketa i potvrde uspjesnog kreiranja. |
| Ko je koristio alat | Hana Piralić, Lamija Maglić |

## Unos 7
| Polje | Detalji |
|---|---|
| Datum | 29.04.2026. |
| Sprint broj | Sprint 5 |
| Alat koji je korišten | Claude Sonnet 4.6 |
| Svrha korištenja | Pomoć pri implementaciji PB-22 (Pregled vlastitih tiketa) |
| Kratak opis zadatka ili upita | "Radim US-11 — pregled vlastitih tiketa. Imam Ticket.cs entitet, TicketService.cs je prazan, MojiTiketi.jsx već postoji na frontendu ali ima greške. Šta trebam prepraviti da zadovolji US-11?" |
| Šta je AI predložio ili generisao | Identificirao sve komponente koje nedostaju i generisao: ITicketRepository s GetByCreatorIdAsync metodom, TicketRepository s EF Core upitom (WHERE CreatorId = userId), ITicketService interfejs, TicketService s mapiranjem na MyTicketDto, TicketController s GET /api/ticket/my-tickets endpointom koji userId čita isključivo iz JWT claims-a, te MyTicketDto smješten u DTOs/Tickets/. Dodatno, identifikovao dva buga u postojećem frontendu. |
| Šta je tim prihvatio | Cijelu backend arhitekturu (ITicketRepository, TicketRepository, ITicketService, TicketService, TicketController), sigurnosni pristup gdje se userId čita iz JWT-a a ne iz parametra zahtjeva, ispravku ticket.ticketId na frontendu, te dodavanje ProtectedRoute na /mojitiketi rutu. |
| Šta je tim izmijenio | MyTicketDto smješten u postojeći DTOs/Tickets/ folder umjesto lokacije koju je AI inicijalno predložio. Namespace usklađen sa konvencijom projekta. Uklonjena redundantna `const { user } = useAuth()` linija iz MojiTiketi.jsx jer api instanca već automatski dodaje JWT header putem interceptora. |
| Šta je tim odbacio | Razmatrano je korištenje već postojećeg GetTicketDto umjesto kreiranja novog, ali odlučeno je da se napravi zaseban MyTicketDto radi jasnoće i razdvajanja odgovornosti. |
| Rizici, problemi ili greške koje su uočene | Bez posebnih rizika. AI je generisao kompletan kod koji je tim pregledao i prilagodio konvencijama projekta. Konačna odluka o strukturi fajlova donijeta je od strane tima. |
| Ko je koristio alat | Merisa Ogrić |

## Unos 8
| Polje | Detalji |
|---|---|
| Datum | 29.04.2026. |
| Sprint broj | Sprint 5 |
| Alat koji je korišten | Claude Sonnet 4.6 |
| Svrha korištenja |  |
| Kratak opis zadatka ili upita | |
| Šta je AI predložio ili generisao |  |
| Šta je tim prihvatio ||
| Šta je tim izmijenio ||
| Šta je tim odbacio | |
| Rizici, problemi ili greške koje su uočene |  |
| Ko je koristio alat | Eldar Hadžiselimović |



Napomena: Ovaj AI Usage Log je zivi dokument i azurira se kroz sprintove.

