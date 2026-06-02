# AI Usage Log – Sprint 10

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
| Datum | 26.05.2026 |
| Sprint broj | Sprint 10 |
| Alat koji je korišten | Claude Code (Anthropic) |
| Svrha korištenja | Implementacija AI funkcionalnosti (PB-57, PB-58) — backend AIService i frontend AI komponente |
| Kratak opis zadatka ili upita | Implementacija `IAIService`/`AIService` s metodama `GetAgentSuggestionAsync` i `GetAdminInsightsAsync`, `AIController` s endpointima `/api/ai/agent-suggestion` i `/api/ai/admin-insights`, DTOs za AI zahtjeve i odgovore, interne knowledge base za 6 telekomunikacijskih kategorija, te frontend `AISuggestionModal`, `AIInsightsPanel` i `AIInsightsCard` komponenti |
| Šta je AI predložio ili generisao | Kompletnu backend implementaciju `AIService` s knowledge base rječnikom za Internet, TV, mobilnu mrežu, naplatu, tehničku podršku i opće probleme; `AgentSuggestionRequestDto`/`AgentSuggestionResponseDto` i `AdminInsightsRequestDto`/`AdminInsightsResponseDto`; `AIController` s role-based autorizacijom; frontend modal s „Kopiraj u poruku" funkcionalnošću; `AIInsightsPanel` s karticama i loading stanjem; `uiStore.js` Zustand store za dijeljeno stanje `aiPanelOpen` između Header i AdminDashboardSection; integraciju AI Uvidi dugmeta u Header komponentu |
| Šta je tim prihvatio | Cjelokupna backend arhitektura AIService s knowledge base pristupom; Zustand store pattern za dijeljeno stanje; modal UX za agent suggestion; inline panel layout ispod KPI kartica za admin insights |
| Šta je tim izmijenio | Tekstovi i kategorije unutar knowledge base prilagođeni telekomunikacijskoj domeni; modal kopira tekst u textarea umjesto auto-slanja; tip vraćenih insights-a poravnat sa stvarnim KPI metrikama |
| Šta je tim odbacio | Prvobitni prijedlog auto-slanja AI odgovora bez agentove potvrde (US-97 zahtijeva ručno „Pošalji"); ideja o eksternoj LLM integraciji u prvoj iteraciji (zadržan interni knowledge base) |
| Rizici, problemi ili greške koje su uočene | Knowledge base ručno održavanje — može zastariti ako se kategorije proširuju; AI suggestion može vratiti opštu poruku kada se kategorija ne poklapa precizno |
| Ko je koristio alat | Uma Mahmutovic |

---

## Unos #2

| Polje | Detalji |
|---|---|
| Datum | 26.05.2026 |
| Sprint broj | Sprint 10 |
| Alat koji je korišten | Claude Code (Anthropic) |
| Svrha korištenja | Kompletni redizajn korisničkog sučelja (PB-59) — Sidebar, Header, AppLayout, AdminDashboardSection |
| Kratak opis zadatka ili upita | Totalni vizualni revamp svih layout komponenti inspirisan modernim dashboard dizajnom: novi Sidebar s navy-800 akcentima, status chipom i kompaktnom navigacijom; novi Header s desktop search barom i notifikacijskim dropdownom; redesigniran AdminDashboardSection s novim StatCard patternima (ikona + trend + broj + labela), dismissabilnim alert bannerom i key highlights sekcijom; navy color palette u index.css s realnim tamnim hex vrijednostima |
| Šta je AI predložio ili generisao | Kompletni redizajn `Sidebar.jsx` s `bg-[#f0f2f5]`, navy-800 logoim i avatarom, aktivnim stavkama s `bg-navy-50`, status chipom (amber/zeleni) koji navigira na filtrirane tikete; redesigniran `Header.jsx` s `bg-[#f4f6f9]`, desktop search barom, AI Uvidi dugmetom i notifikacijskim dropdownom; `AppLayout.jsx` s konzistentnim bojama; `AdminDashboardSection.jsx` s novim `StatCard` componentom (trend badge ArrowUpRight/ArrowDownRight, ikona s mekim pozadinskim bojama, veliki broj, labela dole), dismissabilnim alert bannerom koji je u cijelosti klikabilan, inline AI panelom, key highlights sekcijom s progress barovima; `uiStore.js` s `setAlert(count, url)` za cross-component komunikaciju |
| Šta je tim prihvatio | Cjelokupni dizajn Sidebara s navy akcentima; status chip pattern s amber/zelenom indikacijom; Header layout s konzistentnom pozadinom; StatCard pattern s trend indikatorima; Zustand store za alertTicketCount i alertTicketUrl |
| Šta je tim izmijenio | Hex vrijednosti za navy paletu poravnate s pravim tamnim tonovima umjesto bluish aliasa; pozadina Header-a usklađena s glavnim content područjem; alert banner napravljen u cijelosti klikabilnim |
| Šta je tim odbacio | Prvobitni navy alias kao plava paleta (vraćeno na tamne navy hex vrijednosti); X dugme bez stopPropagation (zamijenjeno verzijom koja sprječava propagaciju klika) |
| Rizici, problemi ili greške koje su uočene | Navy boje u `index.css` su slučajno prepisane sa aliasima za plave boje tokom sessiona — ispravno riješeno vraćanjem tamnih hex vrijednosti; Header i main content imali su različite pozadine — ujednačeno na `bg-[#f4f6f9]`; alert banner nije bio u cijelosti klikabilan — riješeno wrappanjem u `<div onClick>` s `stopPropagation` na X dugmetu |
| Ko je koristio alat | Uma Mahmutovic |

---

## Unos #3

| Polje | Detalji |
|---|---|
| Datum | 26.05.2026 |
| Sprint broj | Sprint 10 |
| Alat koji je korišten | Claude Code (Anthropic) |
| Svrha korištenja | Implementacija PB-61 (Admin CRUD FAQ) prema acceptance kriterijima iz Sprint Backloga 10 (US-104) |
| Kratak opis zadatka ili upita | Proširenje postojećeg FAQ toka (FaqController + FaqService + FaqRepository + frontend Faq stranica) sa admin CRUD funkcionalnošću uz zadržavanje read-only prikaza za obične korisnike; konzistentne poruke validacije za prazno pitanje i odgovor; role-based autorizacija samo za administratora; refresh liste nakon svake akcije i confirm dialog prije brisanja; pisanje pratećih unit, integration i UI testova prema Test Strategy dokumentu |
| Šta je AI predložio ili generisao | Backend: nova DTO-a `CreateFaqDto` i `UpdateFaqDto`; proširen `IFaqRepository`/`FaqRepository` sa `GetAllAsync`, `GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`; proširen `IFaqService`/`FaqService` sa CRUD metodama, konzistentnim porukama validacije kao konstantama (`QuestionRequiredMessage`, `AnswerRequiredMessage`) i trimovanjem inputa; proširen `FaqController` sa endpointima `GET /api/faq/all`, `POST /api/faq`, `PUT /api/faq/{id}`, `DELETE /api/faq/{id}` — svi uz `[Authorize(Roles = "ADMINISTRATOR")]`, dok je public `GET /api/faq` zadržan netaknut za ostale role. Frontend: proširen `faqService.js` sa admin pozivima (`getAllFaqs`, `createFaq`, `updateFaq`, `deleteFaq`); redizajnirana `Faq.jsx` stranica sa admin UX-om — „Dodaj pitanje" dugme u headeru, ikone za uređivanje i brisanje uz svaku stavku, modal forma sa client-side validacijom, `ConfirmDialog` prije brisanja i notification banner za uspjeh/grešku; admin koristi `getAllFaqs` (vidi i neaktivne), ne-admin `getFaqs`. Testovi: `FaqAdminCrudTests.cs` (15 unit testova — validacija, mapiranje, autorizacija, controller status kodovi), `FaqAdminCrudIntegrationTests.cs` (8 end-to-end testova kroz Controller → Service → Repository → InMemory DB), proširenje `Faq.test.jsx` sa admin describe blokom (+8 testova), proširenje `faqService.test.js` (+4 testa za admin endpointe). |
| Šta je tim prihvatio | Cjelokupni backend pristup (DTO + service + controller + role-based authorization); reuse postojećeg `FaqRepository` i `FaqService` umjesto pravljenja paralelnog `AdminFaqController`/`AdminFaqService`; admin UX sa modalom za formu i `ConfirmDialog`-om za delete; refresh liste preko postojećeg `loadFaqs` callback-a nakon svake akcije; konstante poruka validacije u service-u kao izvor istine za testove i frontend |
| Šta je tim izmijenio | Razdvojeni admin i public load tokovi u `Faq.jsx` (`isAdmin ? getAllFaqs : getFaqs`) umjesto jedinstvenog poziva — admin vidi i neaktivne stavke, klijent samo aktivne; test mock konfiguracija za admin describe blok prebačena sa `vi.clearAllMocks()` na `vi.resetAllMocks()` zbog `mockResolvedValueOnce` queue leakage između testova; trimovanje pitanja/odgovora/kategorije u servisu prije persistencije; `getByLabelText` u testovima ograničen sa `selector: 'input'`/`selector: 'textarea'` da ne kolidira sa aria-label-om edit dugmeta |
| Šta je tim odbacio | Prvobitni prijedlog `IsActive` toggle dugmeta umjesto hard delete — zadržan delete koji uklanja stavku iz baze, jer je US-104 eksplicitno tražio brisanje uz potvrdu; ideja o paralelnoj `AdminFaqController` klasi — umjesto toga prošireno postojeće `FaqController` da se ne pravi paralelni sistem; prijedlog server-side render validation poruka u JSON formatu sa multiple errors — zadržan jednostavan `{ poruka: string }` format koji je konzistentan sa ostatkom backend-a |
| Rizici, problemi ili greške koje su uočene | (1) `vi.clearAllMocks()` ne resetuje `mockResolvedValueOnce` queue — ostavljeni leftover mockovi su uzrokovali da test „opens edit form pre-filled" inicijalno padne; ispravljeno prelaskom na `vi.resetAllMocks()` u admin describe bloku. (2) `getByLabelText(/Pitanje/i)` regex je matchao i aria-label dugmeta „Uredi pitanje ..." — ispravljeno koristeći exact label tekst sa `selector: 'input'` ograničenjem. (3) Postojeći FAQ test fajlovi (`FaqUi.test.jsx`, `FaqSystem.test.jsx`, `FaqAcceptance.test.jsx`) su pukli nakon dodavanja `useAuth` dependency u `Faq.jsx` — riješeno dodavanjem AuthContext mocka u sva tri fajla bez izmjene njihovih test scenarija. |
| Ko je koristio alat | Ajnur Kušundžija |

---

## Unos #4

| Polje | Detalji |
|---|---|
| Datum | 26.05.2026 |
| Sprint broj | Sprint 10 |
| Alat koji je korišten | Claude Code (Anthropic) |
| Svrha korištenja | Implementacija PB-62 (Assign to me — samodjelovanje tiketa) prema acceptance kriterijima iz Sprint Backloga 10 (US-105) |
| Kratak opis zadatka ili upita | Implementacija toka u kojem agent jednim klikom preuzima nedodijeljeni tiket sebi bez dodatne potvrde; dugme „Preuzmi tiket" vidljivo isključivo agentu kada je tiket otvoren i nema dodijeljenog agenta/tehničara; backend mora odbiti samodjelovanje za zatvorene tikete i tikete koji su u međuvremenu postali dodijeljeni drugom agentu (race condition); evidencija u istoriji tiketa, audit log i notifikacija klijentu; reuse postojeće forward/assignment logike umjesto paralelnog mehanizma; pisanje pratećih unit, integration i UI testova prema Test Strategy dokumentu |
| Šta je AI predložio ili generisao | Backend: nova metoda `SelfAssignTicketAsync(int ticketId, int agentId)` u `ITicketService`/`TicketService` koja validira da tiket postoji, da nije zatvoren, da nema nijednu postojeću dodjelu, da je pozivalac AGENT, te da agent (ili tiket) ima tim; nakon validacije kreira `TicketUser` zapis sa `AssignmentType.MANUAL`, ažurira `ticket.TeamId` ako agent ima tim, šalje notifikaciju klijentu (`NotificationType.TICKET_ASSIGNED`), dodaje sistemski komentar preko postojećeg `ICommentService.AddSystemCommentAsync` i piše `AuditActionType.TICKET_FORWARDED` audit log; novi endpoint `POST /api/tickets/{id}/self-assign` u `TicketController` ograničen na rolu AGENT sa preslikavanjem `KeyNotFoundException` → 404, `UnauthorizedAccessException` → 403, `InvalidOperationException` → 400. Frontend: dodata funkcija `selfAssignTicket(ticketId)` u `ticketService.js`; dodato „Preuzmi tiket" dugme u `TicketDetail.jsx` koje se prikazuje samo kada je `user.role === 'AGENT' && ticket.status === 'OPEN' && !ticket.assignedAgentId && !ticket.assignedTechnicianId`; klik bez dodatne potvrde poziva `selfAssignTicket`, osvježi tiket preko `getTicketById`, prikaže success notifikaciju i sakrije dugme; backend rejection error se prikazuje kroz `role="alert"` poruku unutar akcijske sekcije. Testovi: `SelfAssignServiceTests.cs` (7 unit testova za sve grane), `SelfAssignIntegrationTests.cs` (5 end-to-end testova uključujući race condition), proširenje `TicketDetail.test.jsx` sa PB-62 describe blokom (+8 UI testova za vidljivost i akciju), proširenje `ticketService.test.js` (+2 testa za POST poziv). |
| Šta je tim prihvatio | Reuse postojećeg `_ticketRepository.AddAssignmentAsync(TicketUser)` mehanizma umjesto pravljenja zasebnog assignment toka; reuse `_commentService.AddSystemCommentAsync` za zapis u istoriji tiketa i `_notificationService.SendNotificationAsync` za obavještavanje klijenta — ista infrastruktura kao i kod forward toka, što garantuje konzistentnost UI prikaza; vidljivost dugmeta zasnovana isključivo na DTO podacima koje frontend već ima (role + status + assignedAgentId + assignedTechnicianId), bez dodatnih API poziva; bez confirm dialoga jer US-105 eksplicitno traži „jedan klik" UX |
| Šta je tim izmijenio | Validacija TeamId u servisu — ako agent nema tim, koristi se `ticket.TeamId`, a ako ni tiket nema tim, akcija se odbija sa `InvalidOperationException` (ovo nije bio dio prvobitnog prijedloga, ali je nužno jer postojeća `TicketUser` shema zahtijeva non-null TeamId); `AssignmentType.MANUAL` umjesto novog enum entry-ja (npr. `SELF_ASSIGNED`) — namjerno ostalo bez nove vrijednosti enum-a kako bi se izbjegla migracija; error poruka u UI-u sa 5-sekundnim auto-dismiss timeout-om umjesto perzistentnog banner-a, da ne ometa naredne akcije agenta |
| Šta je tim odbacio | Prvobitni prijedlog confirm dialoga prije samodjelovanja — odbijen jer US-105 eksplicitno traži jednoklik UX bez dodatnih potvrda; prijedlog da admin također može „preuzeti" tiket sebi — odbijen jer admin već ima forward modal za preraspodjelu (PB-31/US-101), pa bi dodavanje admin self-assign duplo pokrivalo isti use-case; prijedlog da se uvede novi `AssignmentType.SELF_ASSIGNED` enum entry — odbijen radi izbjegavanja DB migracije, korišten postojeći `MANUAL` |
| Rizici, problemi ili greške koje su uočene | (1) Race condition kada se tiket dodijeli između render-a TicketDetail-a i klika na dugme — pokriveno backend validacijom (`ticket.Assignments.Any()` provjera) i integration testom `SelfAssign_ShouldReturnBadRequest_WhenAlreadyAssignedToAnotherAgent`; UI prikazuje jasnu poruku „Tiket je već dodijeljen drugom agentu." (2) Agent bez tima na sistemu može pokušati preuzeti tiket bez tima — backend baca `InvalidOperationException` sa informativnom porukom, ali je ovo edge-case koji bi se trebao spriječiti na nivou seed/admin procesa. (3) `AssignmentType.MANUAL` je dijeljen sa drugim manualnim dodjelama — audit log uz „samodjelovanje" string sa porukom razlikuje slučajeve, ali bi bilo čistije imati zaseban enum entry u budućim sprintovima. (4) Performance test `AuthPerformanceTests.Login_ShouldCompleteWithinTimeLimit_InTestEnvironment` je flaky i nije vezan za ovaj zadatak — preostaje kao postojeća poznata stavka van opsega PB-62. |
| Ko je koristio alat | Ajnur Kušundžija |

---

## Unos #5

| Polje | Detalji |
|---|---|
| Datum | 27.05.2026 |
| Sprint broj | Sprint 10 |
| Alat koji je korišten | Claude Code (Anthropic) |
| Svrha korištenja | Implementacija PB-70 (MCP Admin Copilot) prema acceptance kriterijima iz Sprint Backloga 10 (US-108, US-109, US-110, US-111) |
| Kratak opis zadatka ili upita | Implementacija administratorskog chat interfejsa „MCP Admin Copilot": novi read-only MCP server u TypeScriptu (zvanični Model Context Protocol SDK), backend orkestracioni sloj koji prepoznaje intent pitanja i preko MCP alata dohvaća žive podatke, te Groq modelom (zaseban ključ `GROQ_API_KEY_2`) formatira odgovor na bosanskom; frontend chat panel u stilu postojećih AI/admin komponenti; sve preko Docker Compose-a; uz prateće unit testove na sva tri sloja. Eksplicitno ograničeno na US-108..US-111 (US-114 nije rađen), bez diranja postojećih PB-57/PB-58 AI funkcionalnosti i postojećeg `GROQ_API_KEY` ponašanja |
| Šta je AI predložio ili generisao | MCP server (`Project/mcp-server`): `src/index.ts` (Express + Streamable HTTP transport sa session managementom i `enableJsonResponse`), `src/server.ts`, `src/config.ts`, `src/data/db.ts` (parser ADO.NET connection stringa → `mssql`, read-only `assertReadOnly` zaštita, enum mapiranja, SQL nad `Tickets`/`Teams`/`TicketUsers`/`Comments`/`Faqs`), alati `ticket.search`/`ticket.analytics`/`team.workload`/`faq.search` sa zod validacijom i čistim (testabilnim) compute funkcijama; `Dockerfile`. Backend: `AdminCopilotController` (`POST /api/ai/admin-copilot/query`, 403 za non-admin), `IAdminCopilotService`/`AdminCopilotService` (detekcija intenta team_workload/faq_coverage/tickets_no_response/general/unsupported, poziv MCP alata, Groq formatiranje preko `GROQ_API_KEY_2`, deterministički fallback narativ, logiranje pitanja i alata), `IMcpClient`/`McpClient` (JSON-RPC handshake initialize→initialized→tools/call), DTO-i `AdminCopilotQueryRequestDto`/`AdminCopilotQueryResponseDto`/`AdminCopilotMetricDto`/`AdminCopilotRecommendationDto`/`AdminCopilotSourceDto`/`AdminCopilotMessageDto`; DI registracija. Frontend: `AdminCopilotPanel.jsx`/`AdminCopilotMessage.jsx`, `aiService.adminCopilotQuery`, `uiStore` proširenje, dugme „MCP Copilot" u `Header.jsx` (samo admin), inline panel u `AdminDashboardSection.jsx`. Docker: novi `mcp-server` servis + `GROQ_API_KEY_2`/`MCP_SERVER_URL` za API; `.env.example`. Testovi: MCP 24 (vitest), backend 19 (xUnit/Moq), frontend 12 (Vitest/RTL) |
| Šta je tim prihvatio | Cjelokupna arhitektura sa zasebnim MCP serverom kao posredničkim slojem (AI sloj ne čita bazu direktno); split tool logike na čiste compute funkcije + tanak DB sloj radi testabilnosti; `enableJsonResponse` na Streamable HTTP transportu radi jednostavnijeg C# klijenta; zaseban `AdminCopilotService` (da `AIService` ne naraste); deterministički strukturirani podaci (metrics/recommendations/sources/relatedTickets/faqCoverage) uz Groq narativ; vizualni stil panela usklađen sa `AIInsightsPanel` (navy akcenti, lucide ikone, rounded) |
| Šta je tim izmijenio | Naziv tabele u SQL upitu ispravljen sa `TicketUser` na `TicketUsers` (tabela preimenovana kasnijom EF migracijom — otkriveno tek pri end-to-end testu sa živom bazom, greška „Invalid object name 'TicketUser'"); `team.workload` proširen da tim tiketa izvodi kao `COALESCE(Tickets.TeamId, posljednja TicketUsers.TeamId)` jer seed tiketi nemaju `TeamId` pa bi opterećenje bilo 0; verzija paketa `Microsoft.Extensions.Logging.Abstractions` poravnata na 10.0.6 (NU1605 package downgrade); Moq default setup prebačen u konstruktor testa da test-specifični setupi imaju prioritet |
| Šta je tim odbacio | Automatsko izvršavanje akcija (preraspodjela tiketa, kreiranje FAQ-a) — zadržano isključivo read-only + prijedlozi, prema ograničenjima; stateless MCP transport bez sesije (odbijeno jer SDK zahtijeva initialize handshake po sesiji); izmjena postojećeg `GROQ_API_KEY`/`AIService` ponašanja — novi Copilot koristi isključivo `GROQ_API_KEY_2`; US-114 (van opsega ovog zadatka) |
| Rizici, problemi ili greške koje su uočene | (1) Naziv tabele `TicketUsers` vs `TicketUser` — pre-existing migracija je preimenovala tabelu; riješeno provjerom `ApplicationDbContextModelSnapshot` i verifikovano end-to-end protiv žive baze. (2) MCP Streamable HTTP zahtijeva puni handshake (initialize → notifications/initialized → tools/call) i `Mcp-Session-Id` header — C# klijent to izvodi po pozivu; ako MCP server padne vraća se kontrolisana 503. (3) Ovisnost o Groq dostupnosti — pri grešci Groq-a koristi se deterministički sažetak da odgovor ne bude prazan, dok fali li `GROQ_API_KEY_2` namjerno se vraća jasna 503 greška koja spominje ime varijable. (4) `GROQ_API_KEY_2` u lokalnom `.env` (gitignored) postavljen na istu vrijednost kao `GROQ_API_KEY` da radi lokalno; u verzioniranom `.env.example` ostaje prazan placeholder — stvarni ključ nije commitovan. |
| Ko je koristio alat | Ajnur Kušundžija |

---

## Unos #6

| Polje | Detalji |
|---|---|
| Datum | 27.05.2026 |
| Sprint broj | Sprint 10 |
| Alat koji je korišten | ChatGPT (GPT-5.5) |
| Svrha korištenja | Pomoć pri organizaciji i pisanju Scrum dokumentacije za Sprint 10. |
| Kratak opis zadatka ili upita | AI je korišten za strukturisanje Sprint Goal dokumenta, proširenje Sprint Backloga, organizaciju AI Usage Log unosa, Decision Log napomena i pripremu dokumentacije vezane za AI funkcionalnosti, redizajn korisničkog sučelja i administratorska proširenja. |
| Šta je AI predložio ili generisao | Predložena je struktura Sprint Goal dokumenta, opis sprint cilja, fokus sprinta, očekivani deliverable-i, review kriteriji i održani sastanci. AI je također pomogao u formulisanju dokumentacije tako da bude usklađena sa Sprint 10 backlog stavkama. |
| Šta je tim prihvatio | Prihvaćena je struktura dokumentacije, prošireni opis sprint cilja, organizacija sekcija i većina formulacija vezanih za sprint artefakte. |
| Šta je tim izmijenio | Tim je prilagodio pojedine nazive PB i US stavki, odgovorne osobe i tehničke detalje kako bi dokumentacija odgovarala stvarnoj implementaciji. |
| Šta je tim odbacio | Odbačeni su prijedlozi koji nisu bili direktno vezani za Sprint 10 scope. |
| Rizici, problemi ili greške koje su uočene | Potrebna je dodatna provjera konzistentnosti između Sprint Backloga, Product Backloga i AI Usage Log dokumenata. |
| Ko je koristio alat | Lejan Kozlić |

---


## Unos #7

| Polje | Detalji |
|---|---|
| Datum | 27.05.2026 |
| Sprint broj | Sprint 10 |
| Alat koji je korišten | ChatGPT / GitHub Copilot |
| Svrha korištenja | Dovršetak PB-29 funkcionalnosti pregleda rasporeda timova za administratora. |
| Kratak opis zadatka ili upita | AI je korišten za pomoć pri organizaciji prikaza timova, članova timova, filtera i administratorske preraspodjele agenata kroz sekciju Timovi. |
| Šta je AI predložio ili generisao | Predložena je struktura UI prikaza timova, filteri za pregled agenata po timu, osnovna logika za prikaz članova tima i organizacija backend endpointa za dohvat timova i agenata. |
| Šta je tim prihvatio | Prihvaćen je prikaz timova sa članovima i filterima, kao i logika da administrator može pregledati raspored timova i vršiti preraspodjelu agenata. |
| Šta je tim izmijenio | Tim je prilagodio nazive polja, prikaz filtera i način prikaza članova tima prema postojećem dizajnu aplikacije. |
| Šta je tim odbacio | Odbačeni su prijedlozi koji su uvodili previše kompleksan team management van trenutnog scope-a Sprinta 10. |
| Rizici, problemi ili greške koje su uočene | Potencijalni rizik bio je neusklađenost prikaza timova sa postojećim podacima u bazi i potreba da se ne izgube postojeće dodjele agenata. |
| Ko je koristio alat | Ajdin Dželo|

---

## Unos #8

| Polje | Detalji |
|---|---|
| Datum | 24.05.2026 |
| Sprint broj | Sprint 10 |
| Alat koji je korišten | Claude Code / GitHub Copilot |
| Svrha korištenja | Implementacija PB-60 internih komentara na tiketima. |
| Kratak opis zadatka ili upita | AI je korišten za implementaciju internih komentara koji su vidljivi samo osoblju, odnosno agentima, tehničarima i administratorima, dok su skriveni od klijenata. |
| Šta je AI predložio ili generisao | Predložene su izmjene modela poruka, dodatno polje za označavanje interne bilješke, backend validacija vidljivosti komentara, frontend prikaz internih komentara i vizualno razlikovanje od običnih poruka. |
| Šta je tim prihvatio | Prihvaćena je logika da interni komentari budu dio ticket komunikacije, ali da budu jasno odvojeni od regularnih poruka i potpuno skriveni od klijenta. |
| Šta je tim izmijenio | Tim je prilagodio vizualni prikaz internih komentara i način prikaza oznake “Internal note” kako bi se uklopio u redizajnirani UI. |
| Šta je tim odbacio | Odbačeno je prikazivanje internih komentara klijentu u bilo kojem obliku, čak i kao sistemske poruke. |
| Rizici, problemi ili greške koje su uočene | Glavni rizik bio je slučajno curenje internih komentara klijentima, pa je dodatno provjerena autorizacija i frontend filtriranje poruka. |
| Ko je koristio alat | Eldar Hadžiselimović|

---

## Unos #9

| Polje | Detalji |
|---|---|
| Datum | 25.05.2026 |
| Sprint broj | Sprint 10 |
| Alat koji je korišten | GitHub Copilot / ChatGPT |
| Svrha korištenja | Implementacija PB-63 Agent availability status funkcionalnosti. |
| Kratak opis zadatka ili upita | AI je korišten za pomoć pri implementaciji statusa dostupnosti agenata, gdje agent može postaviti vlastiti availability status, a admin i drugi agenti mogu vidjeti statuse u timskom pregledu. |
| Šta je AI predložio ili generisao | Predložene su izmjene korisničkog modela ili DTO strukture za availability status, backend endpointi za promjenu statusa, frontend UI kontrole za postavljanje dostupnosti i prikaz statusa u timskom pregledu. |
| Šta je tim prihvatio | Prihvaćena je osnovna logika da agent samostalno mijenja svoj status dostupnosti, dok admin i agenti mogu pregledati statuse kroz timski prikaz. |
| Šta je tim izmijenio | Tim je prilagodio nazive statusa, vizualni prikaz badge oznaka i uslove prikaza statusa prema postojećim rolama i dizajnu aplikacije. |
| Šta je tim odbacio | Odbačena je kompleksnija automatizacija statusa dostupnosti, jer nije bila dio trenutnog scope-a sprinta. |
| Rizici, problemi ili greške koje su uočene | Potencijalni rizik bio je da se nedostupni agenti i dalje prikazuju kao kandidati za dodjelu tiketa, pa je dodatno provjerena povezanost availability statusa sa prikazom timova i agenata. |
| Ko je koristio alat | Merisa Ogrić|

Napomena: Ovaj AI Usage Log je zivi dokument i azurira se kroz sprintove.
