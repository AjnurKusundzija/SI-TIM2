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
