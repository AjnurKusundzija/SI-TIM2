# AI Usage Log – Sprint 8

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
| Datum | 16.05.2026 |
| Sprint broj | Sprint 8 |
| Alat koji je koristen | Claude Code (claude-sonnet-4-6) |
| Svrha koristenja | Implementacija PB-42: Statistika agenta i tehničara |
| Kratak opis zadatka ili upita | Implementirati prikaz lične statistike rada za agente i tehničare: backend endpoint koji izračunava i vraća broj otvorenih/zatvorenih tiketa, tiketa na čekanju zatvaranja, prosječno vrijeme prvog odgovora, prosječno vrijeme rješavanja i prosječnu ocjenu (samo agenti). Frontend prikaz na zasebnoj stranici i kondenzovani prikaz na Dashboardu. |
| Sta je AI predlozio ili generisao | Backend: `GetMyStatistics` metodu u `UserService`-u sa LINQ upitima za sve metrike, `AgentStatisticsDto`, proširenje `IUserService` i `UserController`. Frontend: `Statistics.jsx` stranicu sa `StatCard` komponentama, proširenje `Dashboard.jsx` sa `MiniStat` komponentama i blokom nedavnih tiketa (`RecentTicketRow`), te `getMyStatistics` i `getMyRecentTickets` u `userService.js`. ESLint ispravka: promjena `{ icon: Icon }` destrukturiranja u `{ icon }` + `const Icon = icon` u `StatCard`, `QuickCard` i `MiniStat` komponentama zbog CI ESLint pravila `no-unused-vars`. |
| Sta je tim prihvatio | Cjelokupnu backend implementaciju (servis, DTO, controller), strukturu `Statistics.jsx` stranice i Dashboard proširenja, te pristup formaterima (`formatMinutes`, `formatHours`, `formatRating`). |
| Sta je tim izmijenio | Vizualni dizajn StatCard komponenti prilagođen Tailwind klasama projekta; rute i navigacija usklađene s postojećom strukturom App.jsx. |
| Sta je tim odbacio | Ništa strukturalno — predložena implementacija je prihvaćena u cjelini nakon provjere. |
| Rizici, problemi ili greske koje su uocene | ESLint CI pravilo `no-unused-vars` nije prepoznavalo JSX alias destrukturiranje `{ icon: Icon }` kao korištenu varijablu. Greška je otkrivena tek pri CI pokretanju — lokalni ESLint nije javljao problem. Dashboard.test.jsx padao na `getByText` jer `QuickCard` renderuje label dva puta (mobilni + desktop layout); ispravno rješenje je `getAllByText(...)[0]`. |
| Ko je koristio alat | Uma Mahmutović |

---

## Unos #2

| Polje | Detalji |
|---|---|
| Datum | 16.05.2026 |
| Sprint broj | Sprint 8 |
| Alat koji je koristen | Claude Code (claude-sonnet-4-6) |
| Svrha koristenja | Implementacija PB-49: Notifikacije (kompletna implementacija) |
| Kratak opis zadatka ili upita | Implementirati kompletan notifikacijski sistem: backend generisanje notifikacija za sve predviđene događaje na tiketima (dodjela, prosljeđivanje, odgovor, zatvaranje, promjena statusa), real-time isporuka putem SignalR, API endpointi za upravljanje notifikacijama, te frontend prikaz (bell ikona, dropdown, zasebna stranica, redirect na tiket pri kliku). |
| Sta je AI predlozio ili generisao | Backend: `NotificationHub` (SignalR hub sa `JoinUserGroup`), `INotificationPusher` interfejs i `NotificationPusher` implementaciju (apstrakcija da BLL ne ovisi o `IHubContext`), proširenje `NotificationService` poslovnom logikom (`SendNotificationAsync`), `NotificationRepository` s `ExecuteUpdateAsync` za bulk mark-as-read, `NotificationController` s tri endpointa (`GET`, `POST /{id}/read`, `POST /read-all`), JWT konfiguraciju za čitanje `access_token` query parametra (potrebno za SignalR WebSocket handshake), dodavanje `int? TicketId` na `Notification` entitet s EF migracijom, te injektovanje `INotificationService` u `TicketService` i `CommentService` s notifikacionim pozivima po svim predviđenim događajima. Frontend: `notificationService.js` s `createNotificationConnection()` (SignalR konekcija s `accessTokenFactory`), `NotificationContext.jsx` (upravljanje stanjem, real-time push, `markAsRead`, `markAllAsRead`, `reload`), bell ikonu u `Header.jsx` s crvenim badge-om i dropdownom, `Notifications.jsx` stranicu, link u `Sidebar.jsx` s unread badge-om, role-based redirect na tiket pri kliku. Vite proxy ispravku: dodavanje `/notificationhub` s `ws: true` u `vite.config.js`. |
| Sta je tim prihvatio | Cjelokupnu arhitekturu (INotificationPusher apstrakcija, NotificationHub, NotificationContext sa SignalR), sva notifikaciona okidanja po događajima, frontend komponente, role-based redirect logiku, te Vite proxy ispravku. |
| Sta je tim izmijenio | Vizualni stil dropdown komponente i stranice notifikacija prilagođen Tailwind klasama projekta. Pravilo o klijentu koji dobiva TICKET_FORWARDED notifikaciju dodano je naknadno (nije bilo u inicijalnoj specifikaciji). |
| Sta je tim odbacio | Prijedlog za polling kao fallback mehanizam — SignalR s `withAutomaticReconnect()` je dovoljan. |
| Rizici, problemi ili greske koje su uocene | ESLint pravilo `react-hooks/set-state-in-effect`: linter prati pozive funkcija kroz call graph — poziv `load()` unutar efekta triggerovao je grešku jer `load` interno poziva `setNotifications`. Riješeno inline `getNotifications().then(...)` direktno u efektu. SignalR WebSocket nije radio lokalno zbog nedostajuće Vite proxy konfiguracije — notifikacije su stizale samo pri osvježavanju stranice. Ispravka: dodati proxy entry za `/notificationhub` s `ws: true`. |
| Ko je koristio alat | Uma Mahmutović |

---

## Unos #3

| Polje | Detalji |
|---|---|
| Datum | 16.05.2026 |
| Sprint broj | Sprint 8 |
| Alat koji je koristen | Claude Code (claude-sonnet-4-6) |
| Svrha koristenja | Implementacija SB-08: Sistemske poruke u chatu tiketa pri prosljeđivanju |
| Kratak opis zadatka ili upita | Kada agent proslijedi tiket drugom agentu ili tehničaru, automatski dodati sistemsku poruku u chat tiketa koja vidljivo obavještava sve učesnike o prosljeđivanju (npr. "Tiket je proslijeđen tehničaru: Ime Prezime"), uz real-time isporuku svim otvorenim instancama tiketa putem SignalR. |
| Sta je AI predlozio ili generisao | Backend: dodavanje `bool IsSystemMessage` i nullable `int? AuthorId` na `Comment` entitet (sistemske poruke nemaju autora), EF migraciju `AddSystemCommentToComment`, `IChatPusher` interfejs u BLL i `ChatPusher` implementaciju u API (isti arhitekturni pattern kao `INotificationPusher` — razdvajanje BLL od `IHubContext`), `AddSystemCommentAsync` metodu u `ICommentService` i `CommentService` koja kreira komentar i broadcastuje ga putem `IChatPusher`, injektovanje `ICommentService` u `TicketService` i pozive `AddSystemCommentAsync` nakon svakog prosljeđivanja (agentu i tehničaru). Ažuriranje mapiranja u `CommentService` za nullable autora. Frontend: uvjetno renderovanje sistemskih poruka u `TicketDetail.jsx` kao centrirana pill linija s horizontalnim separatorima, bez avatara i bez oznake autora. Ažuriranje 20+ test fajlova: dodavanje `new Mock<ICommentService>().Object` kao 5. parametar `TicketService` konstruktoru i `new Mock<IChatPusher>().Object` kao 4. parametar `CommentService` konstruktoru. |
| Sta je tim prihvatio | Arhitekturni pattern s `IChatPusher` apstrakcijom, implementaciju `AddSystemCommentAsync`, promjene na `Comment` entitetu i migraciju, frontend pill prikaz sistemskih poruka, sve ispravke testova. |
| Sta je tim izmijenio | Vizualni stil pill komponente prilagođen konzistentnosti s ostatkom UI-a (boja, border, veličina fonta). |
| Sta je tim odbacio | Prijedlog za zasebnu "Historija dodjela" sekciju na tiketu — ocijenjeno kao prekomplicirana opcija koja zahtijeva novi UI element i endpoint, dok sistemske poruke u chatu postižu isti cilj jednostavnijim putem. |
| Rizici, problemi ili greske koje su uocene | Perl `sed` zamjena za ažuriranje test konstruktora bila je previše agresivna — slučajno je dodala `new Mock<ICommentService>().Object` i u `CommentService` konstruktor u `Sprint7UserStoriesSystemTests.cs` (koji prima samo 3 parametra). Otkriveno pri `dotnet build` i ručno ispravljeno. FK relacija `Comment.AuthorId → User.UserId` ostaje kao opcionalna veza u EF Core konfiguraciji bez izmjena u `AppDbContext.cs` jer EF Core automatski tretira nullable FK kao opcionalu relaciju. |
| Ko je koristio alat | Uma Mahmutović |

---

## Unos #4

| Polje | Detalji |
|---|---|
| Datum | 17.05.2026 |
| Sprint broj | Sprint 8 |
| Alat koji je koristen | Claude Code (claude-opus-4-7) |
| Svrha koristenja | Implementacija PB-26: Ocjenjivanje tiketa |
| Kratak opis zadatka ili upita | Implementirati funkcionalnost koja korisniku omogucava da nakon zatvaranja tiketa ocijeni kvalitet rjesenja na skali od 1 do 5 i opcionalno ostavi komentar, uz prikaz ocjene agentu i administratoru na zatvorenom tiketu. |
| Sta je AI predlozio ili generisao | Backend: prosirenje postojeceg `Rating` modela i repozitorija/servisa za cuvanje ocjene vezane za tiket, validacije da se moze ocijeniti samo zatvoren tiket, da korisnik moze ocijeniti samo vlastiti tiket i da isti tiket ne moze biti ocijenjen vise puta. Predlozeni su API endpointi za dodavanje i dohvat ocjene, DTO strukture za request/response i mapiranje ocjene u detaljni prikaz tiketa. Frontend: prikaz forme za ocjenjivanje na `TicketDetail` stranici samo za klijenta i samo kada je tiket zatvoren, `StarRating` komponenta za izbor ocjene 1-5, opcionalno polje za komentar, success/error poruke i prikaz postojece ocjene agentu/adminu. |
| Sta je tim prihvatio | Osnovnu backend arhitekturu sa servisnom validacijom, povezivanje ocjene sa zatvorenim tiketom, ogranicenje na jednu ocjenu po tiketu, role-based prikaz forme i prikaz ocjene u detaljima tiketa. |
| Sta je tim izmijenio | Nazivi DTO klasa, endpoint ruta i UI tekstovi prilagodjeni su postojecim konvencijama projekta. Frontend prikaz je uskladjen sa postojecim Tailwind stilovima i vec postojecom `StarRating` komponentom. |
| Sta je tim odbacio | Prijedloge za anonimno ocjenjivanje i posebnu stranicu za sve ocjene, jer nisu dio trenutnog scope-a PB-26. Odbaceno je i omogucavanje izmjene ocjene nakon slanja, zbog acceptance kriterija da isti tiket ne smije biti ocijenjen vise puta. |
| Rizici, problemi ili greske koje su uocene | Potrebno je pazljivo validirati vlasnistvo tiketa i status `Closed`, jer bi propust omogucio ocjenjivanje tudjih ili nezatvorenih tiketa. Postoji rizik od duplog slanja forme na frontendu, pa backend validacija jedinstvene ocjene ostaje obavezna zastita. |
| Ko je koristio alat | Ajnur Kusundzija |

---

## Unos #5

| Polje | Detalji |
|---|---|
| Datum | 17.05.2026 |
| Sprint broj | Sprint 8 |
| Alat koji je koristen | Claude Code (claude-opus-4-7) |
| Svrha koristenja | Implementacija PB-36 / US-60: Azuriranje statusa tiketa od strane tehnicara |
| Kratak opis zadatka ili upita | Implementirati backend i frontend tako da tehnicar moze promijeniti status tiketa koji mu je dodijeljen, uz postovanje pravila pristupa (samo dodijeljeni tehnicar, samo otvoreni tiketi, predefinisani dozvoljeni statusi), generisanje `STATUS_CHANGED` notifikacije za kreatora tiketa i prikaz potvrde uspjeha — bez narusavanja postojecih tokova (closure workflow, forwarding, notifikacije). |
| Sta je AI predlozio ili generisao | Backend: novi `UpdateTicketStatusDto` (TicketStatus enum), prosirenje `ITicketService` i `TicketService` metodom `UpdateTicketStatusAsync(ticketId, newStatus, userId, role)` sa svim biznis pravilima (role == TECHNICIAN, posljednji assignee == current user, ticket != CLOSED, novi status u dozvoljenoj listi `{OPEN, CLOSURE_REQUESTED}`, no-op kada je isti status, postavljanje `ClosureRequested*` polja pri prelasku na CLOSURE_REQUESTED, postavljanje `ClosureRequestStatus = REJECTED` pri povratku na OPEN), novi endpoint `POST /api/tickets/{id}/status` u `TicketController`-u koji mapira izuzetke na 404/403/400, prosirenje `TicketDetailDto` sa `AssignedAgentId` (potrebno da frontend zna treba li prikazati kontrolu), 13 jedinicnih testova u `TicketStatusUpdateTests.cs` (servis: uspjesna promjena + notifikacija, zabrana neasignuee/closed/non-technician/non-allowed-status, KeyNotFound, no-op, REJECTED tranzicija; kontroler: 200/403/400/404/401 mapiranja). Frontend: `updateTicketStatus` u `ticketService.js`, mali dropdown blok u `TicketDetail.jsx` koji se prikazuje samo kada je `user.role === 'TECHNICIAN' && ticket.status !== 'CLOSED' && ticket.assignedAgentId === user.userId`, inline success/error poruka, osvjezavanje stanja tiketa bez full reloada. |
| Sta je tim prihvatio | Cjelokupnu backend arhitekturu (DTO, servisnu metodu sa svim validacijama, kontroler endpoint, prosirenje TicketDetailDto sa AssignedAgentId), sve testove, frontend dropdown UI i logiku osvjezavanja stanja. |
| Sta je tim izmijenio | Nista strukturalno — implementacija je prihvacena nakon ručne provjere end-to-end toka kroz UI (klijent kreira tiket → agent prosljedjuje tehnicaru → tehnicar mijenja status → klijent dobija STATUS_CHANGED notifikaciju). |
| Sta je tim odbacio | Prijedlog da se prosiri `TicketStatus` enum sa dodatnim vrijednostima tipa `IN_PROGRESS`/`PENDING` — odbaceno jer bi zahtijevalo EF migraciju, novu logiku u svim postojecim filterima/statistikama i nije nuzno za AC. Takodjer odbaceno dozvoljavanje tehnicaru da direktno postavi `CLOSED` — zatvaranje u ovom sistemu mora ici kroz client-confirm tok (`request-closure` → `accept-closure`). |
| Rizici, problemi ili greske koje su uocene | UI guard (`assignedAgentId === user.userId`) sakriva kontrolu od tehnicara koji nisu trenutni assignee, ali backend takodjer provjerava — bez backend provjere kontrola bi se mogla zaobici direktnim API pozivom. Postojeci `GetTicketByIdAsync` za TECHNICIAN-a dozvoljava pristup ako je IKAD bio dodijeljen (`Assignments.Any(a => a.UserId == userId)`), sto znaci da bivsi assignee moze otvoriti detalj, ali nasa provjera "posljednji assignee" u `UpdateTicketStatusAsync` ispravno odbija takvog korisnika. Zabilezeni postojeci CS8602 warning u `TicketService.cs:271` i nedostatak assignee-provjere u `CloseTicketAsync` — oba su van scope-a US-60. |
| Ko je koristio alat | Ajnur Kusundzija |

---

## Unos #6

| Polje | Detalji |
|---|---|
| Datum | 19.05.2026 |
| Sprint broj | Sprint 8 |
| Alat koji je koristen | OpenAI Codex (GPT-5) |
| Svrha koristenja | Refinement i implementacija zajedničke dodjele agent+tehničar, closure notifikacija i ažuriranje Sprint 8 dokumentacije |
| Kratak opis zadatka ili upita | Ispraviti logiku dodjele tako da tiket nakon prosljeđivanja tehničaru ostaje dodijeljen i agentu i tehničaru; omogućiti agentu da vidi tiket u dodijeljenim tiketima kada tehničar postavi status "Čeka se"; prikazati u detaljima tiketa odvojeno `Agent` i `Tehničar`; uključiti taj status u statistiku agenta; dodati notifikacije aktivno dodijeljenom staffu kada klijent prihvati ili odbije zatvaranje, te klijentu kada assigned staff prisilno zatvori tiket. |
| Sta je AI predlozio ili generisao | Backend: zajedničku helper logiku za aktivne assigneeje u `TicketRepository` i `TicketService`, pravilo da `FORWARDED_TO_TECHNICIAN` zadržava prethodnog agenta kao aktivnog assigneeja, proširenje `TicketDetailDto` sa `AssignedTechnicianName` i `AssignedTechnicianId`, mapiranje odvojenih agent/tehničar vrijednosti, closure notifikacije u `AcceptClosureAsync` i `RejectClosureAsync`, dodatnu provjeru da agent/tehničar mora biti aktivno dodijeljen za `ForceCloseAsync`. Frontend: uklanjanje defaultnog `OPEN` filtera na dodijeljenim tiketima, prikaz `Tehničar` ispod `Agent`, usklađivanje labela statusa na "Čeka se", i disable force-close akcije za staff koji nije aktivno dodijeljen. Testovi: novi `TicketClosureServiceTests`, dopune `AllTicketsRepositoryTests`, `TicketDetailServiceTests`, `TicketDetail.test.jsx` i `TicketsAssignmentStatusUi.test.jsx`. Dokumentacija: dopune `SprintBacklog`, `DecisionLog`, `AIUsageLog` i `ProofOfTesting`. |
| Sta je tim prihvatio | Pravilo aktivne zajedničke dodjele za agent+tehničar, bez nove migracije baze; slanje closure notifikacija samo aktivno dodijeljenom staffu; prikaz odvojenih imena agenta i tehničara u UI; uključivanje "Čeka se" tiketa u agentovu listu dodijeljenih tiketa i statistiku. |
| Sta je tim izmijenio | Terminologija u UI i dokumentaciji usklađena je na "Čeka se". Acceptance kriteriji su dodatno razdvojeni u US-70 za zajedničku dodjelu i US-71 za closure notifikacije radi lakšeg praćenja. |
| Sta je tim odbacio | Odbijena je opcija dodavanja nove tabele/kolone za aktivne assigneeje jer postojeća historija dodjela može jednoznačno podržati pravilo bez migracije. Odbijeno je i slanje notifikacija svim korisnicima iz kompletne historije dodjela jer bi stvaralo nepotreban šum. |
| Rizici, problemi ili greske koje su uocene | Prvi pokušaj backend testiranja u sandboxu pao je zbog MSBuild named-pipe ograničenja (`SocketException: Permission denied`); testovi su ponovo pokrenuti uz odobreno izvršavanje izvan sandboxa. Tok "posljednja dodjela" je morao ostati važeći za forward na drugog agenta, dok je samo forward na tehničara poseban slučaj. |
| Ko je koristio alat | Uma Mahmutović |
