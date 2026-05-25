# AI Usage Log – Sprint 9

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
| Datum | 21.05.2026 |
| Sprint broj | Sprint 9 |
| Alat koji je korišten | ChatGPT |
| Svrha korištenja | Pomoć pri izradi Scrum dokumentacije i definisanju sprint artefakata |
| Kratak opis zadatka ili upita | Generisanje Sprint Goal dokumenta, proširenje deliverable-a, fokus sprinta, review kriterija i sastanaka za Sprint 9 |
| Šta je AI predložio ili generisao | AI je generisao strukturirani Sprint Goal dokument sa detaljno opisanim ciljevima sprinta, fokusom sprinta, očekivanim deliverable-ima, review kriterijima i održanim sastancima |
| Šta je tim prihvatio | Prihvaćen je najveći dio generisane strukture dokumenta, uključujući organizaciju sekcija, opis sprint ciljeva i većinu deliverable-a |
| Šta je tim izmijenio | Izmijenjeni su pojedini detalji vezani za scope sprinta, nazive funkcionalnosti i dio tehničkih formulacija kako bi bile usklađene sa projektom i Product Backlog stavkama |
| Šta je tim odbacio | Odbačeni su dijelovi koji nisu bili direktno povezani sa planiranim funkcionalnostima Sprinta 9 |
| Rizici, problemi ili greške koje su uočene | Potrebna dodatna provjera konzistentnosti između Sprint Goal dokumenta, Sprint Backloga i Product Backlog stavki kako bi se izbjegla neusklađenost funkcionalnosti i oznaka |
| Ko je koristio alat | Lejan Kozlić |

---

## Unos #2

| Polje | Detalji |
|---|---|
| Datum | 25.05.2026 |
| Sprint broj | Sprint 9 |
| Alat koji je korišten | Claude Code (Anthropic) |
| Svrha korištenja | Implementacija admin dashboarda s ključnim metrikama (PB-45) |
| Kratak opis zadatka ili upita | Generisanje React komponente za admin dashboard koja prikazuje KPI kartice (otvoreni tiketi, prosječni odgovor, prosječno rješavanje, ocjene, zastarjeli tiketi), grafove (pie chart po statusu, bar chart po tipu problema i opterećenju agenata) i sekciju aktivnih korisnika po ulozi |
| Šta je AI predložio ili generisao | Kompletnu `AdminDashboardSection` komponentu s `StatCard` podkomponentom, Recharts integraciju (PieChart, BarChart), shared period filter s preset dugmadima i custom date range, drill-down navigacijom na ticket listu, skeleton loading state i error handling |
| Šta je tim prihvatio | Cjelokupna struktura komponente, StatCard pattern, period filter logika, Recharts grafovi, drill-down navigacija |
| Šta je tim izmijenio | Prilagođene boje i labele statusima i kategorijama problema, dodan `closedInPeriodCount` prikaz kao description na StatCard, usklađeni Tailwind class nazivi s ostatkom projekta (navy-600/700 color scheme) |
| Šta je tim odbacio | — |
| Rizici, problemi ili greške koje su uočene | Recharts `ResponsiveContainer` zahtijeva mociranje u testovima; `StatCard` s `disabled={!onClick}` okida ESLint upozorenje za button bez explicit tipa |
| Ko je koristio alat | Uma Mahmutovic |

---

## Unos #3

| Polje | Detalji |
|---|---|
| Datum | 25.05.2026 |
| Sprint broj | Sprint 9 |
| Alat koji je korišten | Claude Code (Anthropic) |
| Svrha korištenja | Implementacija modula izvještaja (PB-38, PB-39, PB-40, PB-41, PB-43, PB-44) i redizajn UI-a izvještaja |
| Kratak opis zadatka ili upita | Kompletiranje svih 7 tipova izvještaja: TICKET_COUNT, TICKET_STATUS, PROBLEM_TYPE, TEAM_WORKLOAD, USER_RATINGS, FIRST_RESPONSE, AVG_RESOLUTION — uključujući backend DTOs, servisne metode, repository metode i frontend prikaz. Zatim iterativni redizajn selektora tipa izvještaja (dropdown → tab bar → accordion → pill chips) |
| Šta je AI predložio ili generisao | Backend: novi DTOs (`TicketCountReportDto`, `AvgResolutionReportDto`, `ResolutionBucketDto`, `WorkloadPeriodRowDto` itd.), `GetAgentResolvedDetailsAsync` repository metodu za pivot tablicu, `BuildTeamWorkloadReportAsync` s period×agent pivotom, `BuildAvgResolutionReport` i `BuildUserRatingsReport` s bucket trendovima. Frontend: `renderReportTable` s mini-karticama za aggregate statistike i konzistentno stiliziranim tabelama (`bg-gray-50` header, `hover:bg-gray-50` redovi), horizontalno scrollabilni pill chip selector s auto-fetch na klik, `fetchReport`/`handleSelectChip` handleri, "Primijeni" dugme u period filteru za reports mod |
| Šta je tim prihvatio | Kompletna backend implementacija svih report tipova, pill chip UI, mini-kartice za aggregate stats, poboljšano stiliziranje tabela, auto-fetch logika |
| Šta je tim izmijenio | Iterativno odbijeni tab bar i accordion u korist pill chips; vraćen disabled Export gumb desno od naslova sekcije |
| Šta je tim odbacio | Tab bar layout (izgledao kao navbar), accordion layout |
| Rizici, problemi ili greške koje su uočene | Test `Reports.test.jsx` je pao jer je tražio tekst `'Generisanje izvještaja'` koji je preimenovan u `'Izvještaji'` — popravljen promjenom assertion-a na chip label `'Broj tiketa'`; linter je više puta revertorao izmjene što je uzrokovalo ponovnu primjenu istih promjena |
| Ko je koristio alat | Uma Mahmutovic |

---
## Unos #4

| Polje | Detalji |
|---|---|
| Datum | 25.05.2026 |
| Sprint broj | Sprint 9 |
| Alat koji je korišten | Claude Code (Anthropic, model Opus 4.7) |
| Svrha korištenja | Full-stack implementacija PB-52 (US-76 Upravljanje katalogom paketa i US-77 Dodjela paketa klijentima) kroz backend (.NET 10, EF Core) i frontend (React 19, Vite, Tailwind), uključujući EF migracije, audit log funkcionalnosti, role-based authorization i rješavanje merge konflikata sa develop granom. |
| Kratak opis zadatka ili upita | AI alat korišten je za implementaciju novog kataloga paketa i pretplata odvojenog od legacy SubscriptionPackages sistema, razvoj CRUD operacija nad paketima i pretplatama, validaciju duplicate dodjela, implementaciju audit log sistema za promjene pretplata, razvoj administratorskog UI-a za upravljanje paketima i pretplatama, kao i rješavanje merge konflikata nastalih integracijom sa develop granom. |
| Šta je AI predložio ili generisao | AI je generisao nove DAL entity klase CatalogPackage, ClientSubscription i SubscriptionAuditLog zajedno sa DbContext konfiguracijom i EF migracijom `20260523103345_AddCatalogPackagesAndSubscriptions`. Generisani su repozitoriji i servisi (`CatalogPackageRepository`, `ClientSubscriptionRepository`, `SubscriptionAuditLogRepository`, `CatalogPackageService`, `ClientSubscriptionService`), REST API kontroleri (`PackageCatalogController`, `ClientSubscriptionController`), DTO klase za pakete i pretplate, server-side validacija i HTTP 409 Conflict logika za duplicate dodjele i brisanje aktivnih paketa. Na frontend strani generisane su stranice `PackageManagement.jsx`, `ClientSubscriptionsSection.jsx`, novi servisi `packageCatalogService.js`, seed podaci za katalog paketa, kao i merge rješenja za konfliktne fajlove (`Sidebar.jsx`, `App.jsx`, `UserProfile.jsx`). |
| Šta je tim prihvatio | Tim je prihvatio većinu predložene arhitekture, uključujući odvajanje novih tabela od legacy sistema, audit log implementaciju na servisnom nivou, administratorsku autorizaciju preko `[Authorize(Roles = "ADMINISTRATOR")]`, UI gating kroz `canManageSubscriptions` prop, refresh-key pattern za rerender liste nakon CRUD operacija i layout organizaciju korisničkog profila sa sekcijama za osnovne podatke i pretplate. |
| Šta je tim izmijenio | Izmijenjena je logika tako da `PackageService.GetMyPackagesAsync` koristi isključivo `ClientSubscriptions` tabelu umjesto legacy `SubscriptionPackages` sistema. Kartice paketa na klijentskoj stranici ponovo su postavljene kao klikabilne sa detaljnim prikazom paketa. `GetPackageByIdAsync` metoda preusmjerena je na novi subscription model. Dodatno je unaprijeđen UI `ClientSubscriptionsSection` komponente kroz ikonice po tipu paketa, prikaz datuma, skeleton loader i prazna stanja sa CTA porukama. |
| Šta je tim odbacio | Odbačena je početna AI sugestija da kartice paketa budu non-clickable bez detaljnog prikaza, kao i prijedlog paralelnog prikaza dvije sekcije za pretplate radi izbjegavanja duplikacije podataka i komplikovanja korisničkog interfejsa. |
| Rizici, problemi ili greške koje su uočene | Uočeni su problemi sa frontend rutama prilikom korištenja negativnog `PackageId` kao route diskriminatora, što je izazivalo 404 greške. Problem je riješen refaktorisanjem na pozitivni `SubscriptionId`. Pojavio se i 500 status na `GET /api/users/{id}` zbog neprimijenjene migracije i nerebuildanog Docker kontejnera. Dodatno su riješene ESLint greške vezane za `setState` pozive unutar `useEffect` hookova. Nakon merge-a sa develop granom pojavili su se problemi sa nedostajućim helperima i state varijablama unutar `UserProfile.jsx`, što je riješeno spajanjem kompletnih JSX blokova. Također je evidentirano da legacy `SubscriptionPackages` tabela još uvijek postoji u šemi baze podataka i preporučeno je njeno potpuno uklanjanje u budućim migracijama. |
| Ko je koristio alat | Eldar Hadžiselimović |

---
## Unos #5

| Polje | Detalji |
|---|---|
| Datum | 21.05.2026 |
| Sprint broj | Sprint 9 |
| Alat koji je korišten | Claude Code (Anthropic, model Opus 4.7) |
| Svrha korištenja | Full-stack implementacija PB-51 funkcionalnosti upravljanja korisničkim nalozima |
| Kratak opis zadatka ili upita | AI alat korišten je za implementaciju administratorskog i agentskog upravljanja korisnicima kroz backend (.NET, EF Core) i frontend (React, Tailwind), uključujući kreiranje korisnika, uređivanje korisničkih profila, deaktivaciju i reaktivaciju naloga, role-based authorization, audit log evidenciju i validaciju aktivnih korisnika pri dodjeli tiketa. Poseban fokus stavljen je na reuse postojećeg korisničkog profila i implementaciju posebnog prikaza za agente i tehničare sa statističkim podacima umjesto korisničkih paketa i historije tiketa. |
| Šta je AI predložio ili generisao | AI je generisao backend i frontend implementaciju za upravljanje klijentima, agentima i tehničarima, uključujući role-based API autorizaciju, validaciju aktivnih korisnika, audit log funkcionalnosti i filtriranje deaktiviranih naloga. Predložena je nadogradnja postojećeg detaljnog prikaza korisnika umjesto kreiranja potpuno novog UI-a. Generisana je logika koja sprječava korisnike da uređuju vlastito ime, prezime i broj telefona, dok administratori i agenti mogu uređivati te podatke drugim korisnicima. Za agente i tehničare generisan je poseban profilni prikaz sa statističkim komponentama koje koriste postojeće dashboard/statistics komponente sistema. Također je generisana backend validacija koja sprječava dodjelu tiketa deaktiviranim agentima čak i u slučaju kada je agent deaktiviran nakon učitavanja liste dostupnih agenata. |
| Šta je tim prihvatio | Prihvaćen je role-based pristup upravljanju korisnicima, reuse postojećih profile komponenti, audit log evidencija, validacija deaktiviranih korisnika i poseban statistički prikaz za agente i tehničare. Prihvaćena je i backend validacija dodjele tiketa samo aktivnim agentima. |
| Šta je tim izmijenio | Dodatno su prilagođeni UI detalji i validacijska pravila kako bi bili usklađeni sa postojećim dizajnom sistema i postojećim korisničkim workflow-ima. |
| Šta je tim odbacio | Odbačen je prijedlog korištenja istog detaljnog prikaza za sve tipove korisnika bez odvajanja statistike zaposlenika od korisničkih podataka i paketa. |
| Rizici, problemi ili greške koje su uočene | Identifikovan je potencijalni race condition scenario prilikom dodjele tiketa agentima koji mogu biti deaktivirani između učitavanja liste i potvrde dodjele. Također je uočena potreba za dodatnom backend validacijom kako frontend ograničenja ne bi bila jedina zaštita role-based pristupa. |
| Ko je koristio alat | Ajdin Dželo |

---

Napomena: Ovaj AI Usage Log je zivi dokument i azurira se kroz sprintove.
