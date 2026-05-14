# AI Usage Log – Sprint 5,6,7

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
| Alat koji je korišten | Claude Sonnet 4.6 (Claude Code) |
| Svrha korištenja | Pomoć pri postavljanju razvojnog okruženja, dijagnostici grešaka i implementaciji filtriranja tiketa (feature/filtriranje-ticketa) |
| Kratak opis zadatka ili upita | Korišten AI za: (1) uklanjanje Docker permission greške, (2) dijagnostiku grešaka pri pokretanju backenda i frontenda (nedostajući NuGet paketi, pogrešni portovi u Vite proxy konfiguraciji, nedostajući using direktivi za DTO namespace-ove), (3) implementaciju filtriranja tiketa po prioritetu, statusu, vrsti i datumu na MyTickets stranici, te (4) dodavanje seed podataka za testiranje. |
| Šta je AI predložio ili generisao | Dijagnozu i ispravke za: `libsimdjson.so.33` grešku (Node.js verzija nekompatibilna sa sistemskom bibliotekom), NETSDK1226 grešku (nedostajući `aspnet-targeting-pack`), pogrešan `.env` putanja u `Program.cs` (`../../.env` → `../.env`), port mismatch u `vite.config.js` (7149 → 5122), nedostajuće `using TelecomSupportSystem.BLL.DTOs.Tickets;` direktive u `ITicketService.cs`, `TicketService.cs` i `TicketController.cs`. Za filtriranje: kompletan rewrite `MyTickets.jsx` sa filter panelom (4 filtera: prioritet, status, vrsta, datum), aktivnim filter chipovima sa individualnim uklanjanjem, kombinovanim filterima putem `useMemo`, te dva odvojena empty state-a (bez tiketa vs. filteri ne odgovaraju). Seed podatke: 9 tiketa raznih prioriteta, statusa i kategorija u `Program.cs`. |
| Šta je tim prihvatio | Sve dijagnostičke ispravke za razvojno okruženje, implementaciju filtriranja sa filter panelom i chip indikatorima aktivnih filtera, seed podatke za testiranje, te PR opis za pull request. |
| Šta je tim izmijenio | Tim je naknadno proširio `MyTickets.jsx` sa Tailwind CSS stilovima, `lucide-react` ikonama, tabularnim prikazom tiketa i `EmptyState` komponentom umjesto inline stila koji je AI generisao. Dodan je i search filter po naslovu tiketa. |
| Šta je tim odbacio | Inline style pristup koji je AI koristio u korist Tailwind CSS klasa koje su konzistentne sa ostatkom projekta. |
| Rizici, problemi ili greške koje su uočene | Seed kod se ponavljao zbog čestih revertiranja `Program.cs` pri prelasku između grana — potrebno je pažljivije upravljanje granama. Vite proxy port je bio pogrešan (7149 umjesto 5122/7148) što je uzrokovalo da sve API greške budu prikazane kao "Invalid credentials" zbog generičkog catch bloka u `Login.jsx`. |
| Ko je koristio alat | Eldar Hadžiselimović |

## Unos #9

| Polje | Detalji |
|------|--------|
| Datum | 29.04.2026 |
| Sprint broj | Sprint 5 |
| Alat koji je korišten | ChatGPT |
| Svrha korištenja | Pomoć pri definisanju promptova i provjeri implementacije za PB-22 “Kreiranje novog tiketa”. |
| Kratak opis zadatka ili upita | Korišten je AI za pripremu promptova za Claude/Copilot radi implementacije backend i frontend dijela feature-a za kreiranje novog tiketa, kao i za provjeru da li backend implementacija zadovoljava acceptance criteria. |
| Šta je AI predložio ili generisao | AI je generisao backend prompt za implementaciju PB-22, dodatni prompt za provjeru repository patterna, validacije, autentifikacije, enum vrijednosti i persistence logike, te frontend prompt za implementaciju forme za kreiranje tiketa. Također je dao smjernice za provjeru backend implementacije prije pusha. |
| Šta je tim prihvatio | Prihvaćeni su promptovi za backend i frontend implementaciju, kao i checklist za provjeru backend dijela prije pusha. |
| Šta je tim izmijenio | Promptovi su prilagođeni stvarnom kontekstu projekta, tako da naglašavaju korištenje postojeće arhitekture, postojećih repozitorija, autentifikacije i validacije. |
| Šta je tim odbacio | Odbačeni su nepotrebni prijedlozi koji nisu bili dio trenutnog scope-a, kao što su obavezno dodavanje novog repository-ja, unit testova i GET endpointa ako nisu potrebni za PB-22. |
| Rizici, problemi ili greške koje su uočene | Uočena je potreba da se ručno provjeri da li postojeći repository zaista poziva SaveChangesAsync, da li Ticket postoji u DbContext-u, da li se userId uzima iz JWT tokena, te da li se enum vrijednosti validiraju bez prihvatanja nevalidnih/numeričkih vrijednosti. |
| Ko je koristio alat | Hana Piralić |

## Unos #10

| Polje | Detalji |
|------|--------|
| Datum | 29.04.2026 |
| Sprint broj | Sprint 5 |
| Alat koji je korišten | GitHub Copilot |
| Svrha korištenja | Pomoć pri implementaciji frontend dijela feature-a za kreiranje novog tiketa (PB-22). |
| Kratak opis zadatka ili upita | Korišten je AI za generisanje i strukturiranje frontend komponente za formu za kreiranje tiketa, uključujući validaciju, API integraciju i UX ponašanje. |
| Šta je AI predložio ili generisao | Generisana je struktura komponente za formu (input polja, dropdowni, textarea), validacija unosa (required, whitespace), integracija sa backend endpointom POST /api/ticket, te handling success i error odgovora. |
| Šta je tim prihvatio | Prihvaćena je struktura forme, validacija unosa i način komunikacije sa backendom kroz postojeći API servis. |
| Šta je tim izmijenio | Prijedlozi su prilagođeni postojećem frontend projektu (routing, styling, način poziva API-ja i upravljanje state-om). |
| Šta je tim odbacio | Odbačeni su prijedlozi koji uvode nove biblioteke ili odstupaju od postojećeg načina implementacije u projektu. |
| Rizici, problemi ili greške koje su uočene | Potencijalni rizici uključuju neusklađenost enum vrijednosti između frontend-a i backend-a, nepravilno rukovanje greškama sa servera i potrebu za testiranjem validacije prije slanja zahtjeva. |
| Ko je koristio alat | Hana Piralić |



## Unos #11

| Polje | Detalji |
|---|---|
| Datum | 30.04.2026 |
| Sprint broj | Sprint 5 |
| Alat koji je korišten | Claude Code (claude-sonnet-4-6) |
| Svrha korištenja | Analiza pokrivenosti testovima prema TestStrategy dokumentu i pisanje nedostajućih unit testova za PB-22 (US-8, US-9, US-10) i PB-23 (US-11, US-12, US-13) na backendu i frontendu |
| Kratak opis zadatka ili upita | AI je trebao: napisati backend unit testove i frontend unit testove. |
| Šta je AI predložio ili generisao | backend unit testovi za `TicketControllerTests`, `TicketServiceTests` i `TicketRepositoryTests`, tri nova frontend test fajla: `ticketService.test.js` (5 testova), `CreateTicket.test.jsx` (11 testova: forma, validacija sva 4 polja, uspjeh, reset, greške), `MyTickets.test.jsx` (11 testova: prikaz, OPEN/CLOSED labele, prazno stanje, filtriranje po prioritetu/statusu/tipu, pretraga, "Očisti sve", greška API-a) |
| Šta je tim prihvatio | Sve backend testove; sve tri frontend test datoteke. |
| Šta je tim izmijenio | `MyTickets.test.jsx` je zahtijevao popravku nakon prvog pokretanja — AI je inicijalno koristio `getByText` koji baca grešku kada postoji više podudaranja jer jsdom renderuje i desktop tabelu i mobile kartice istovremeno (CSS media queries se ne primjenjuju); zamijenjeno sa `queryAllByText` pomoćnim funkcijama `present()`/`absent()`. |
| Šta je tim odbacio | Odbačeni su prijedlozi koji uvode nove biblioteke ili odstupaju od postojećeg načina implementacije u projektu. |
| Rizici, problemi ili greške koje su uočene | jsdom ne primjenjuje CSS media queries, pa komponente koje koriste responsive prikaz (desktop tabela + mobile kartice) renderuju oba prikaza istovremeno, što uzrokuje duplikate teksta i greške pri korištenju `getByText`. Rješenje: koristiti `queryAllByText` za provjeru prisustva i odsustva elemenata. |
| Ko je koristio alat | Lejan Kozlić, Uma Mahmutović |

## Unos #12

| Polje | Detalji |
|---|---|
| Datum | 05.05.2026 |
| Sprint broj | Sprint 6 |
| Alat koji je korišten | Codex / ChatGPT |
| Svrha korištenja | Pomoć pri implementaciji PB-47 FAQ funkcionalnosti, provjeri testova i ispravci UI detalja |
| Kratak opis zadatka ili upita | Korišten je AI za implementaciju FAQ funkcionalnosti: dodavanje backend podrške za često postavljana pitanja, zaštićenog API endpointa, seed podataka, frontend FAQ stranice u autentificiranom dijelu aplikacije, sidebar navigacije i unit testova. Naknadno je korišten za uklanjanje FAQ sekcije sa početnog ekrana, ispravku bosanskih afrikata u UI tekstovima i rješavanje lint greške u `useEffect` logici. |
| Šta je AI predložio ili generisao | AI je generisao `Faq` entitet, `IFaqRepository` i `FaqRepository`, `IFaqService` i `FaqService`, `GetFaqDto`, `FaqController` sa zaštićenim `GET /api/faq` endpointom, registraciju repozitorija i servisa u `Program.cs`, EF konfiguraciju i migraciju za `Faqs` tabelu, te razvojne seed podatke. Na frontendu je generisao `faqService.js`, `/faq` stranicu sa loading, empty, error/retry stanjima i accordion prikazom, protected route, naslov stranice i sidebar link za `CLIENT`, `AGENT` i `ADMINISTRATOR`. Također su generisani backend i frontend unit testovi za FAQ funkcionalnost. |
| Šta je tim prihvatio | Prihvaćena je osnovna 3-slojna backend struktura, zaštićeni endpoint, seed FAQ sadržaj, frontend FAQ stranica unutar postojećeg autentificiranog layouta, sidebar navigacija i testovi za repository, service, controller, frontend service i FAQ komponentu. |
| Šta je tim izmijenio | Uklonjena je FAQ sekcija sa početnog ekrana jer FAQ ne treba biti prikazan na Home stranici. Seed logika je promijenjena iz jednokratnog insertovanja u upsert po `SortOrder`, kako bi se postojeći FAQ redovi u razvojnoj bazi ažurirali i dobili ispravne znakove. UI tekstovi su ispravljeni da koriste bosanske afrikate (`č`, `ć`, `ž`, `š`, `đ`). `useEffect` logika u `Faq.jsx` je izmijenjena kako bi prošla strogo React lint pravilo koje zabranjuje sinhrono pozivanje `setState` unutar efekta. |
| Šta je tim odbacio | Odbačen je prijedlog da FAQ bude vidljiv ili klikabilan na početnom ekranu. Odbačen je i pristup gdje se postojeći seed podaci ne ažuriraju, jer bi to ostavilo neispravne ASCII tekstove u lokalnoj bazi. |
| Rizici, problemi ili greške koje su uočene | Uočeno je da Docker volume zadržava stare FAQ seed podatke, pa izmjene teksta u kodu nisu odmah bile vidljive u UI-u. Riješeno je upsert seed logikom. Također je u CI lint provjeri uočena greška `react-hooks/set-state-in-effect`, pa je inicijalni fetch prebačen na promise callbackove unutar `useEffect`, dok je reset stanja ostavljen samo za retry akciju korisnika. |
| Ko je koristio alat | Ajnur Kušundžija |


## Unos #13

| Polje | Detalji |
|---|---|
| Datum | 05.05.2026 |
| Sprint broj | Sprint 6 |
| Alat koji je korišten | ChatGPT (GPT-5.5) |
| Svrha korištenja | Pomoć pri implementaciji detaljnog prikaza tiketa za klijenta i komunikacije između klijenta i agenta unutar sistema za podršku. |
| Kratak opis zadatka ili upita | AI je korišten za pomoć pri organizaciji TicketDetail stranice, prikazu detalja tiketa, navigaciji između stranica i prilagođavanju prikaza za klijenta bez narušavanja postojeće funkcionalnosti za agente i tehničare. |
| Šta je AI predložio ili generisao | Generisani prijedlozi za strukturu TicketDetail komponente, prikaz statusa, prioriteta, kategorije, komunikacije unutar tiketa i organizaciju frontend ruta i komponenti. |
| Šta je tim prihvatio | Strukturu detaljnog prikaza tiketa za klijenta, organizaciju komunikacije unutar tiketa i dio prijedloga za frontend navigaciju i prikaz podataka. |
| Šta je tim izmijenio | Prilagođeni su nazivi ruta, način prikaza pojedinih elemenata i dio logike kako bi postojeće funkcionalnosti za agente i tehničare ostale nepromijenjene. |
| Šta je tim odbacio | Prijedlozi koji bi zahtijevali promjene postojeće backend logike ili uvođenje dodatnih biblioteka koje nisu potrebne projektu. |
| Rizici, problemi ili greške koje su uočene | Problemi sa React import/export konfiguracijom, pokretanjem frontend aplikacije i povezivanjem frontend-a sa backend servisima tokom razvoja TicketDetail funkcionalnosti. |
| Ko je koristio alat | Lejan Kozlić, Uma Mahmutović |

## Unos #14

| Polje | Detalji |
|---|---|
| Datum | 05.05.2026 |
| Sprint broj | Sprint 6 |
| Alat koji je korišten | Claude Code |
| Svrha korištenja | Pomoć pri implementaciji pregleda svih tiketa i detaljnog prikaza tiketa za agenta i administratora u TelecomSupport sistemu. |
| Kratak opis zadatka ili upita | AI je korišten za provjeru usklađenosti funkcionalnosti sa Sprint 1-5 dokumentacijom i za pomoć pri implementaciji stranice “Svi tiketi” i stranice detalja tiketa za uloge agent i administrator. |
| Šta je AI predložio ili generisao | Generisani su prijedlozi za prikaz svih tiketa sa paginacijom, detaljni prikaz pojedinačnog tiketa, prikaz statusa, prioriteta, klijenta, datuma i historije aktivnosti, kao i smjernice za autorizaciju putem JWT tokena. |
| Šta je tim prihvatio | Prihvaćena je organizacija stranice “Svi tiketi”, prikaz detalja tiketa za agente i administratore, korištenje postojećeg dizajna aplikacije i smjernica da agent i administrator imaju pristup svim tiketima bez ograničenja. |
| Šta je tim izmijenio | Prilagođeni su nazivi ruta, prikaz UI elemenata i tekstovi na bosanskom jeziku kako bi se uklopili u postojeću frontend strukturu i stil aplikacije. |
| Šta je tim odbacio | Odbijeni su dijelovi prijedloga koji nisu odgovarali postojećoj arhitekturi projekta, tehnologijama koje se stvarno koriste ili bi narušili postojeće funkcionalnosti za druge uloge. |
| Rizici, problemi ili greške koje su uočene | Potencijalni rizici su neusklađenost API endpointa sa postojećim backend rutama, razlike u nazivima polja između backend odgovora i frontend prikaza, kao i potreba da se očuva postojeća autorizacija za ostale korisničke uloge. |
| Ko je koristio alat | Eldar Hadžiselimović |

## Unos #15

| Polje | Detalji |
|---|---|
| Datum | 04.05.2026 |
| Sprint broj | Sprint 6 |
| Alat koji je korišten | Claude Code (claude-sonnet-4-6) |
| Svrha korištenja | Pomoć pri organizaciji i implementaciji unit, integracijskih, performansnih, sistemskih i prihvatnih testova za PB-24 i PB-27 funkcionalnosti ticket sistema. |
| Kratak opis zadatka ili upita | AI je korišten za pomoć pri organizaciji testnih scenarija, validaciji acceptance criteria, povezivanju testova sa user story zahtjevima i strukturiranju testnih fajlova za TicketDetail i komunikaciju kroz tiket. |
| Šta je AI predložio ili generisao | Generisani prijedlozi za testove kao što su `TicketDetailServiceTests.GetTicketByIdAsync_ShouldReturnDto_WhenClientIsOwner`, `TicketDetailServiceTests.GetTicketByIdAsync_ShouldThrowUnauthorized_WhenClientAccessesOtherTicket`, `TicketDetailControllerTests.GetTicketById_ReturnsOk_WhenTicketFound`, `CommentServiceTests.AddCommentAsync_ShouldSucceed_WhenValidContentAndOwner`, `CommentServiceTests.AddCommentAsync_ShouldThrowArgumentException_WhenContentTooLong`, `CommentControllerTests.AddComment_ReturnsBadRequest_WhenContentEmpty`, kao i frontend testove za `TicketDetail.test.jsx`, `CommunicationSystem.test.jsx` i `CommunicationAcceptance.test.jsx`. |
| Šta je tim prihvatio | Strukturu testiranja za TicketDetail i komunikaciju kroz tiket, povezivanje acceptance criteria sa testovima, organizaciju testnih nivoa i dio prijedloga za frontend i backend testne scenarije. |
| Šta je tim izmijenio | Prilagođeni su nazivi testova, organizacija testnih fajlova i dio validacija kako bi odgovarali postojećoj arhitekturi projekta i implementiranim funkcionalnostima sistema. |
| Šta je tim odbacio | Prijedlozi koji nisu bili kompatibilni sa postojećom backend logikom, organizacijom ruta ili tehnologijama korištenim unutar projekta. |
| Rizici, problemi ili greške koje su uočene | Problemi sa autorizacijom pristupa tiketima, validacijom korisničkih uloga, povezivanjem frontend i backend testova i organizacijom testnih scenarija za komunikaciju kroz tiket. |
| Ko je koristio alat | Uma Mahmutović |

## Unos #16

| Polje | Detalji |
|---|---|
| Datum | 05.05.2026 |
| Sprint broj | Sprint 6 |
| Alat koji je korišten | ChatGPT (GPT-5.5) |
| Svrha korištenja | Pomoć pri organizaciji Sprint 6 aktivnosti, povezivanju user story zahtjeva sa PB zadacima i usklađivanju ticket funkcionalnosti sa sprint ciljevima. |
| Kratak opis zadatka ili upita | AI je korišten za pomoć pri definisanju Sprint Goal-a, povezivanju PB-24 funkcionalnosti sa user story zahtjevima, organizaciji sprint backlog stavki i usklađivanju ticket funkcionalnosti sa planiranim sprint aktivnostima. |
| Šta je AI predložio ili generisao | Generisani prijedlozi za organizaciju sprint backlog stavki, povezivanje detaljnog prikaza tiketa sa sprint ciljevima, raspodjelu zadataka po članovima tima i definisanje fokusnih tačaka Sprinta 6. |
| Šta je tim prihvatio | Organizaciju Sprint 6 backlog stavki, povezivanje PB-24 funkcionalnosti sa sprint ciljevima i dio prijedloga za raspodjelu ticket funkcionalnosti između članova tima. |
| Šta je tim izmijenio | Prilagođeni su nazivi backlog stavki, statusi zadataka i dio opisa sprint aktivnosti kako bi odgovarali stvarnoj organizaciji tima i implementiranim funkcionalnostima. |
| Šta je tim odbacio | Prijedlozi koji nisu odgovarali postojećoj organizaciji sprintova ili planiranoj raspodjeli zadataka unutar tima. |
| Rizici, problemi ili greške koje su uočene | Problemi sa organizacijom branch workflow-a, merge procesom na main granu i usklađivanjem ticket funkcionalnosti između frontend i backend dijela sistema. |
| Ko je koristio alat | Lejan Kozlić |

## Unos #17

| Polje | Detalji |
|---|---|
| Datum | 05.05.2026 |
| Sprint broj | Sprint 6 |
| Alat koji je korišten | Gemini 3.1 Pro|
| Svrha korištenja | Pomoć pri proširenju ticket sistema sa real-time komunikacijom između klijenta i agenta, SignalR integracijom i proširenjem autorizacije pristupa tiketima za agente i administratore. |
| Kratak opis zadatka ili upita | AI je korišten za definisanje i generisanje detaljnog prompta za implementaciju funkcionalnosti gdje agent može pregledati detalje svakog tiketa, kliknuti na tiket iz prikaza “Svi tiketi”, te ostvariti real-time komunikaciju sa klijentom kroz sistem komentara unutar TicketDetail stranice koristeći SignalR. |
| Šta je AI predložio ili generisao | Generisan je detaljan implementacijski prompt koji uključuje izmjene backend autorizacije za pristup detaljima tiketa, omogućavanje pristupa svim tiketima za AGENT i ADMINISTRATOR uloge, klikabilne kartice/redove tiketa u frontend prikazu, SignalR Hub za komunikaciju po ticket grupama, automatsko osvježavanje komentara bez refresh-a, organizaciju TicketDetail stranice kao chat interfejsa, validaciju maksimalne dužine komentara (1000 karaktera), te filtriranje uvredljivih riječi zamjenom znakovima `*`. |
| Šta je tim prihvatio | Prihvaćen je koncept real-time komunikacije putem SignalR-a, organizacija komentara unutar TicketDetail stranice, pristup svim tiketima za agente i administratore, kao i prijedlog da se prvi opis problema tretira kao inicijalni komentar u razgovoru. |
| Šta je tim izmijenio | Prilagođene su SignalR rute, nazivi DTO objekata, frontend struktura komponenti i postojeća logika autorizacije kako bi se uklopili u postojeću arhitekturu projekta i JWT autentifikaciju. |
| Šta je tim odbacio | Ništa. |
| Rizici, problemi ili greške koje su uočene | Uočeni su potencijalni problemi sa duplim prikazom komentara zbog istovremenog lokalnog state update-a i SignalR event-a, problemi sa autorizacijom pristupa ticket grupama, kao i potreba za pravilnim cleanup-om SignalR konekcija pri napuštanju stranice. |
| Ko je koristio alat | Ajdin Dželo |

## Unos #18

| Polje | Detalji |
|---|---|
| Datum | 10.05.2026 |
| Sprint broj | Sprint 7 |
| Alat koji je korišten | Claude Code (claude-opus-4-7) |
| Svrha korištenja | Pisanje testova za US-25 (Automatska dodjela tiketa, PB-30) prema test strategiji iz Sprint 3, te ažuriranje dokumentacije ProofOfTesting.md u Sprintu 7 i TestStrategy.md u Sprintu 3 sa rezultatima testiranja. |
| Kratak opis zadatka ili upita | AI je trebao pročitati Sprint3/TestStrategy.md, analizirati postojeću implementaciju US-25, napisati nove backend testove i ažurirati dokumentaciju za Sprint 7. |
| Šta je AI predložio ili generisao | Generisani su backend testovi u TelecomSupportSystem.Tests: AutoAssignServiceTests.cs, AutoAssignRepositoryTests.cs, AutoAssignIntegrationTests.cs i AutoAssignPerformanceTests.cs, ukupno 25 test slučajeva. Također je popunjen Sprint7/ProofOfTesting.md i dodani su evidence redovi u Sprint3/TestStrategy.md. |
| Šta je tim prihvatio | Prihvaćen je paket od 25 backend testova, struktura dokumenta ProofOfTesting.md, mapiranje testova na AC1–AC6, evidence stavke u TestStrategy.md i objašnjenje da AC3 koristi postojeći endpoint za prikaz dodijeljenih tiketa. |
| Šta je tim izmijenio | Tim je odbacio inicijalni pristup sa posebnom AssignmentRules tabelom i prihvatio jednostavniji pristup preko Team.SpecializedCategory. Frontend je izmijenjen tako da agentski sidebar ima odvojene linkove /tickets i /assigned. Status „Nedodijeljen“ riješen je kroz postojeći status i AssignmentMessage u DTO-u, bez dodavanja novog TicketStatus.UNASSIGNED enuma. |
| Šta je tim odbacio | Odbačeni su AssignmentRule entitet, AssignmentRules tabela, dodatni UNASSIGNED status, poseban admin ekran za upravljanje pravilima dodjele, IsSystem flag i dodatni assignment-rules endpointi. Također nije dodan novi frontend test fajl jer postojeći Tickets.test.jsx već pokriva potrebne izmjene. |
| Rizici, problemi ili greške koje su uočene | Lokalno okruženje ima .NET 8 SDK, dok projekat cilja net10.0, pa testovi nisu mogli biti lokalno pokrenuti. Validacija će se izvršiti u CI-u ili Docker okruženju s .NET 10 SDK-om. Uočeni su mogući problemi s različitim pristupima implementaciji US-25 i ESLint greška react-hooks/set-state-in-effect, koja je riješena refaktorisanjem Promise.all().then().catch().finally() patternom. |
| Ko je koristio alat | Eldar Hadžiselimović |

## Unos #19

| Polje | Detalji |
|---|---|
| Datum | 10.05.2026 |
| Sprint broj | Sprint 7 |
| Alat koji je korišten | ChatGPT (GPT-5.5) |
| Svrha korištenja | Pomoć pri organizaciji Sprint 7 dokumentacije, rješavanju GitHub konflikata i usklađivanju sprint dokumenata sa dogovorenim PB i US stavkama. |
| Kratak opis zadatka ili upita | AI je korišten za pomoć pri definisanju Sprint Goal-a, Decision Log odluke, objašnjenju merge konflikata i pravilnom postavljanju PR-a između docs branch-a i develop grane. |
| Šta je AI predložio ili generisao | Prijedloge za Sprint Goal Sprinta 7, Decision Log odluku o ticket workflow logici, objašnjenje razlike između current i incoming changes, te smjernice za rješavanje konflikata u dokumentima. |
| Šta je tim prihvatio | Strukturu Sprint Goal-a, dio opisa Decision Log odluke, postupak rješavanja konflikata i način spajanja dokumentacijskog branch-a u develop. |
| Šta je tim izmijenio | Tekstovi su prilagođeni stvarnim dogovorima tima, raspodjeli PB zadataka i pravilima projekta. |
| Šta je tim odbacio | Prijedlozi koji nisu odgovarali stvarnom sprint scope-u ili bi pogrešno prikazali planirane user story izmjene. |
| Rizici, problemi ili greške koje su uočene | Merge konflikti u sprint dokumentima, nejasnoća oko base/compare grana i potreba da se pažljivo razlikuju current i incoming promjene. |
| Ko je koristio alat | Lejan Kozlić |

## Unos #20

| Polje | Detalji |
|---|---|
| Datum | 09.05.2026 |
| Sprint broj | Sprint 7 |
| Alat koji je korišten | Claude Code (claude-opus-4-7) |
| Svrha korištenja | Pomoć pri analizi postojeće arhitekture projekta i implementaciji funkcionalnosti za interno upravljanje prioritetima tiketa. |
| Kratak opis zadatka ili upita | AI je korišten za definisanje backend i frontend izmjena potrebnih za dodavanje internog prioriteta tiketa, uključujući model, DTO strukture, API endpoint, autorizaciju, validaciju i prikaz na stranici detalja tiketa. |
| Šta je AI predložio ili generisao | Prijedloge za dodavanje internog prioriteta na ticket model, definisanje enum vrijednosti prioriteta, kreiranje endpointa za ažuriranje prioriteta, ograničavanje pristupa na Agent/Admin role, sakrivanje internog prioriteta od klijenata, frontend dropdown za izbor prioriteta i prikaz success/error notifikacija. |
| Šta je tim prihvatio | Koncept internog prioriteta tiketa, predefined listu prioriteta, role-based ograničenje za agente i administratore, prikaz prioriteta na TicketDetail stranici i sakrivanje ove funkcionalnosti od običnih korisnika. |
| Šta je tim izmijenio | Nazivi DTO-a, endpointa, enum vrijednosti i UI prikaza prilagođeni su postojećoj arhitekturi projekta i ranije korištenim konvencijama za tikete. |
| Šta je tim odbacio | Prijedlozi koji bi uvodili nepotrebne biblioteke, mijenjali postojeći authorization flow ili otkrivali interni prioritet korisnicima bez odgovarajuće role. |
| Rizici, problemi ili greške koje su uočene | Potencijalni problemi sa migracijama baze, mapiranjem DTO-a, autorizacijom endpointa i očuvanjem odvojenosti internog prioriteta od korisničkog prikaza tiketa. |
| Ko je koristio alat | Ajdin Dželo |

## Unos #21

| Polje | Detalji |
|---|---|
| Datum | 11.05.2026 |
| Sprint broj | Sprint 7 |
| Alat koji je korišten | Claude Code (claude-opus-4-7) |
| Svrha korištenja | Pomoć pri analizi postojeće arhitekture projekta i implementaciji kompletnog workflow-a za zatvaranje tiketa za korisnike, agente i tehničare. |
| Kratak opis zadatka ili upita | AI je korišten za definisanje backend i frontend izmjena potrebnih za zatvaranje tiketa, slanje zahtjeva za zatvaranje, prihvatanje ili odbijanje zahtjeva od strane korisnika i prinudno zatvaranje tiketa nakon isteka roka. |
| Šta je AI predložio ili generisao | Prijedloge za proširenje statusa tiketa na Open, Resolved, Closure Requested i Closed, validaciju dozvoljenih promjena stanja, endpoint za korisničko zatvaranje vlastitog riješenog tiketa, endpoint za slanje closure request-a, prihvatanje i odbijanje zahtjeva, te logiku za zatvaranje nakon 7 dana bez odgovora korisnika. |
| Šta je tim prihvatio | Workflow u kojem korisnik može zatvoriti vlastiti riješeni tiket, agent ili tehničar može poslati zahtjev za zatvaranje, korisnik može prihvatiti ili odbiti zahtjev, a ovlašteni agent može zatvoriti tiket nakon isteka definisanog roka. |
| Šta je tim izmijenio | Nazivi statusa, DTO strukture, endpointi, validacije i UI kontrole prilagođeni su postojećoj arhitekturi projekta, postojećim korisničkim rolama i ranije definisanim konvencijama za ticket sistem. |
| Šta je tim odbacio | Prijedlozi koji bi omogućili zatvaranje tiketa bez provjere vlasništva, role ili trenutnog statusa tiketa, kao i prijedlozi koji bi uvodili nepotrebne biblioteke ili narušili postojeći authorization flow. |
| Rizici, problemi ili greške koje su uočene | Potencijalni problemi sa validacijom state transition logike, provjerom vlasništva tiketa, računanjem perioda od 7 dana, čuvanjem closure metadata podataka i pravilnim prikazom akcija za različite korisničke role. |
| Ko je koristio alat | Ajdin Dželo |

## Unos #22

| Polje | Detalji |
|---|---|
| Datum | 12.05.2026 |
| Sprint broj | Sprint 7 |
| Alat koji je korišten | GitHub Copilot |
| Svrha korištenja | Implementacija pregleda i historije dodijeljenih tiketa za agente (US-53 i US-54) u okviru PB-48. |
| Kratak opis zadatka ili upita | Tim je implementirao funkcionalnosti koje agentima omogućavaju pregled trenutno otvorenih dodijeljenih tiketa i historije zatvorenih tiketa kojima su upravljali. AI je korišten za generisanje backend logike, API endpointa i frontend komponenti za pregled dodijeljenih tiketa. |
| Šta je AI predložio ili generisao | Generisani su prijedlozi za proširenje ITicketService i TicketService interfejsa metodama GetOpenAssignedTicketsAsync i GetClosedAssignedTicketsAsync, zajedno sa ITicketRepository i TicketRepository implementacijama koje koriste EF Core LINQ upite za filtriranje tiketa po korisniku i statusu. AI je također generisao API endpoint-e GET /api/tickets/assigned/open i GET /api/tickets/assigned/closed u TicketController-u sa pristupom ograničenim na AGENT rolu. Na frontend strani generisane su funkcije getOpenAssignedTickets() i getClosedAssignedTickets() u ticketService.js, kao i AssignedTickets.jsx stranica sa tab navigacijom između otvorenih i zatvorenih tiketa, filtriranjem po kategoriji i prioritetu, pretragom po naslovu i prikazom datuma zatvaranja tiketa. |
| Šta je tim prihvatio | Tim je prihvatio backend arhitekturu sa TicketRepository, TicketService i TicketController implementacijom, kao i sigurnosni pristup u kojem se userId preuzima iz JWT claims-a. Prihvaćena je i frontend AssignedTickets.jsx komponenta sa tab navigacijom, filtriranjem i rutom /assigned u aplikaciji. |
| Šta je tim izmijenio | DTO struktura i namespace MyTicketDto klase prilagođeni su postojećoj organizaciji projekta i smješteni u postojeći DTOs/Tickets folder. Prilagođena je i konfiguracija relacija u AppDbContext-u kako bi Ticket koristio cascade delete, dok User i Team relacije koriste Restrict pravila radi očuvanja integriteta podataka. |
| Šta je tim odbacio | Odbačeni su prijedlozi koji nisu bili dio definisanog sprint scope-a ili bi uvodili nepotrebne izmjene u postojeću arhitekturu sistema. |
| Rizici, problemi ili greške koje su uočene | Uočeno je da TicketUser entitet već postoji u projektu bez odgovarajućeg DbSet-a u AppDbContext-u, što je izazivalo probleme prilikom migracija. Problem je riješen dodavanjem DbSet<TicketUser> TicketUsers i konfiguracijom relacija. Također, inicijalni namespace prijedlog za MyTicketDto nije bio usklađen sa konvencijama projekta i ručno je prilagođen. |
| Ko je koristio alat | Merisa Ogrić |

## Unos #23

| Polje | Detalji |
|---|---|
| Datum | 14.05.2026 |
| Sprint broj | Sprint 7 |
| Alat koji je korišten | Claude Code (claude-opus-4-7) |
| Svrha korištenja | Pomoć pri pisanju testova osnovnih i dodatnih nivoa testiranja. |
| Kratak opis zadatka ili upita | Korišten AI za prijedloge i izradu skica testova za osnovne (unit) i dodatne nivoe (integracijski, sistemski, performansni i prihvatni) prema TestStrategy dokumentu i aktuelnim PB/US stavkama. |
| Šta je AI predložio ili generisao | Predložene testne scenarije i strukturu test fajlova po nivoima testiranja, uključujući pokrivenost kritičnih tokova i rubnih slučajeva. |
| Šta je tim prihvatio | Okvirnu strukturu testova i veći dio predloženih scenarija kao bazu za implementaciju. |
| Šta je tim izmijenio | Prilagođeni nazivi testova, organizacija po folderima i mapiranje na postojeće PB/US zahtjeve. |
| Šta je tim odbacio | Prijedloge koji nisu bili u scope-u Sprinta 7 ili bi zahtijevali nove biblioteke/alate. |
| Rizici, problemi ili greške koje su uočene | Rizik da AI predloži testove koji ne odgovaraju stvarnoj implementaciji; svaki scenarij je zahtijevao ručnu validaciju. |
| Ko je koristio alat | Ajnur Kušundžija |

## Unos #24

| Polje | Detalji |
|---|---|
| Datum | 14.05.2026 |
| Sprint broj | Sprint 7 |
| Alat koji je korišten | Codex / ChatGPT (GPT-5.5) |
| Svrha korištenja | Pomoć pri implementiranju unit testova. |
| Kratak opis zadatka ili upita | Korišten AI za izradu prijedloga i skica unit testova prema postojećoj TestStrategy dokumentaciji i aktuelnim PB/US zahtjevima. |
| Šta je AI predložio ili generisao | Predložene strukture test fajlova, pokrivenost glavnih tokova i rubnih slučajeva za relevantne module. |
| Šta je tim prihvatio | Veći dio predloženih test scenarija i strukturu testova. |
| Šta je tim izmijenio | Prilagođeni nazivi testova, mockovi i organizacija testova u skladu sa postojećom arhitekturom. |
| Šta je tim odbacio | Prijedloge koji nisu odgovarali trenutnom scope-u sprinta ili uvodili nepotrebne zavisnosti. |
| Rizici, problemi ili greške koje su uočene | Potreba za ručnom validacijom scenarija kako bi se uskladili sa stvarnom implementacijom. |
| Ko je koristio alat | Hana Piralić |

## Unos #25

| Polje | Detalji |
|---|---|
| Datum | 14.05.2026 |
| Sprint broj | Sprint 7 |
| Alat koji je korišten | Codex / ChatGPT (GPT-5.5) |
| Svrha korištenja | Pomoć pri implementiranju unit testova. |
| Kratak opis zadatka ili upita | Korišten AI za izradu prijedloga i skica unit testova prema postojećoj TestStrategy dokumentaciji i aktuelnim PB/US zahtjevima. |
| Šta je AI predložio ili generisao | Predložene strukture test fajlova, pokrivenost glavnih tokova i rubnih slučajeva za relevantne module. |
| Šta je tim prihvatio | Veći dio predloženih test scenarija i strukturu testova. |
| Šta je tim izmijenio | Prilagođeni nazivi testova, mockovi i organizacija testova u skladu sa postojećom arhitekturom. |
| Šta je tim odbacio | Prijedloge koji nisu odgovarali trenutnom scope-u sprinta ili uvodili nepotrebne zavisnosti. |
| Rizici, problemi ili greške koje su uočene | Potreba za ručnom validacijom scenarija kako bi se uskladili sa stvarnom implementacijom. |
| Ko je koristio alat | Lamija Maglić |

Napomena: Ovaj AI Usage Log je zivi dokument i azurira se kroz sprintove.
