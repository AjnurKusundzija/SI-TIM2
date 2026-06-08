# AI Usage Summary

## Pregled

Tim je dokumentovao korištenje AI alata kroz sprint AI Usage Log dokumente. Korišteni su ChatGPT, Claude Code / Claude Sonnet, GitHub Copilot, Manus AI, OpenAI Codex i Groq integracija u okviru MCP Admin Copilot funkcionalnosti.

## Za šta je AI korišten

AI je korišten za:

- pisanje i organizaciju Scrum dokumentacije,
- izradu i doradu Sprint Goal, Sprint Backlog, Decision Log, AI Usage Log i Proof of Testing dokumenata,
- implementaciju autentifikacije i autorizacije,
- implementaciju kreiranja, pregleda, filtriranja, detalja, komunikacije, zatvaranja i ocjenjivanja tiketa,
- implementaciju korisničkih profila, promjene emaila i lozinke,
- implementaciju admin dashboarda i izvještaja,
- implementaciju upravljanja korisničkim nalozima,
- implementaciju kataloga paketa i pretplata,
- implementaciju audit log sistema,
- implementaciju priloga na tiketima,
- implementaciju AI prijedloga odgovora i AI Insights,
- redizajn korisničkog interfejsa,
- admin CRUD FAQ,
- Assign to me funkcionalnost,
- MCP Admin Copilot,
- pisanje backend i frontend testova,
- dijagnostiku razvojnih problema, konfiguracije i merge konflikata.

## Šta je prihvaćeno

Tim je prihvatio značajan dio AI prijedloga nakon provjere:

- strukturu više sprint dokumenata i standardizovan format zapisa,
- arhitekturu autentifikacije sa JWT i refresh token mehanizmom,
- frontend AuthContext, ProtectedRoute i Axios interceptor logiku,
- strukturu testova po slojevima,
- backend i frontend tokove za tikete,
- admin dashboard komponente, KPI kartice, Recharts grafove i period filtere,
- backend implementaciju više tipova izvještaja,
- role-based pristup upravljanju korisnicima,
- audit log evidenciju,
- attachment servis sa whitelist validacijom,
- AIService i interne knowledge base tokove,
- MCP server kao zaseban read-only posrednički sloj,
- admin FAQ CRUD pristup,
- samodjelovanje tiketa kroz postojeći assignment mehanizam.

## Šta je izmijenjeno

Tim je AI prijedloge mijenjao kako bi ih uskladio sa projektom:

- prilagođeni su nazivi entiteta, DTO-ova, namespaceova i foldera,
- JWT ključ je prebačen iz konfiguracije u `.env` / environment varijable,
- AI predloženi DbContext pristup je prilagođen repository patternu,
- frontend stilovi su usklađeni sa Tailwind i postojećim dizajnom,
- validacije i poruke grešaka su prilagođene acceptance kriterijima,
- u testovima su mijenjani selektori zbog duplih DOM elemenata i jsdom ograničenja,
- export izvještaja je implementiran client-side bez novog backend endpointa,
- AI prijedlog odgovora kopira tekst u poruku umjesto automatskog slanja,
- MCP Copilot je ograničen na read-only alate i zaseban `GROQ_API_KEY_2`,
- PB-52 je eksplicitno označen kao manualno testiran bez automatizovanih testova.

## Šta je odbačeno

Dokumentovano je odbacivanje:

- generičkih i nepreciznih formulacija u dokumentaciji,
- error poruka koje bi otkrivale da li je email ili lozinka pogrešna,
- prijedloga koji ne prate 3-slojnu arhitekturu,
- inline styling pristupa u korist Tailwind CSS-a,
- dodatnih filtera i funkcionalnosti van sprint scope-a,
- novih biblioteka koje nisu bile potrebne,
- prikaza osjetljivih podataka kao što je `PasswordHash`,
- brisanja priloga nakon uploada,
- cloud storage integracije za MVP,
- auto-slanja AI odgovora bez korisničke potvrde,
- eksterne LLM integracije u prvoj iteraciji AI prijedloga odgovora,
- automatskog izvršavanja administratorskih akcija kroz MCP Copilot,
- paralelnih kontrolera/servisa gdje je postojeća struktura bila dovoljna,
- funkcionalnosti US-114 u okviru MCP zadatka.

## Greške i problemi koje je AI napravio

Dokumentovani problemi uključuju:

- prijedlog direktnog ubacivanja DbContext-a u servis, što je kršilo arhitekturna pravila,
- prijedlog čuvanja JWT ključa u konfiguraciji umjesto sigurnijeg environment pristupa,
- netačne ili preopšte formulacije u dokumentaciji,
- frontend test selektore koji su padali zbog više istih tekstova u DOM-u,
- pogrešan Vite proxy port koji je uzrokovao zbunjujuće API greške,
- prijedloge DTO-a koji su inicijalno sadržavali osjetljive podatke (`PasswordHash`),
- prijedlog non-clickable kartica paketa, kasnije odbačen,
- route problem sa negativnim `PackageId`, riješen prelaskom na pozitivni `SubscriptionId`,
- SQL naziv tabele `TicketUser` umjesto `TicketUsers` u MCP implementaciji,
- potrebu za mockiranjem Recharts `ResponsiveContainer` i `URL.createObjectURL` u testovima,
- prijedloge UI layouta koji su odbačeni jer nisu odgovarali aplikaciji.

## Dijelovi sistema razvijeni uz AI asistenciju koje tim mora znati detaljno objasniti

Tim posebno mora moći objasniti:

- autentifikaciju, JWT, refresh tokene, rate limiting i sigurnosne odluke,
- ticket workflow: kreiranje, pregled, detalji, komunikacija, prioriteti, zatvaranje i dodjela,
- role-based authorization za klijente, agente, tehničare i administratore,
- upravljanje korisnicima, deaktivaciju i reaktivaciju naloga,
- katalog paketa, pretplate i postojeće ograničenje oko legacy `SubscriptionPackages` tabele,
- audit log i zabranu izmjene/brisanja zapisa,
- upload priloga, validaciju formata/veličine, sanitizaciju i autorizaciju pristupa,
- admin dashboard i sve tipove izvještaja,
- CSV export izvještaja,
- AI prijedlog odgovora i AI Insights,
- MCP Admin Copilot arhitekturu, read-only alate, Groq fallback i ograničenja,
- redizajn UI-a i nove frontend komponente,
- test strategiju, automatizovane testove i dijelove koji su testirani samo manualno.
