# Known Issues and Limitations

## Poznati bugovi

- Produkcijski deployment resetuje bazu pri svakom deploymentu na `main`, jer koristi `docker compose down -v` i briše Docker volumen.
- Backend se može resetovati ako SQL Server nije spreman ni nakon retry mehanizma.
- Dokumentovan je flaky performansni test `AuthPerformanceTests.Login_ShouldCompleteWithinTimeLimit_InTestEnvironment`.

## Tehnička ograničenja

- Produkcijski `docker-compose.yml` i `.env` nisu dio repozitorija i moraju se ručno konfigurirati na serveru.
- CD pipeline nema eksplicitni health check aplikacije nakon deploymenta.
- Deployment zavisi od Docker Hub dostupnosti.
- SQL Server kontejner koristi `MSSQL_ALLOW_RUNNING_AS_ROOT: "1"` zbog hosting ograničenja.
- PB-52 nema dokumentovane automatizovane xUnit/Vitest testove u Sprintu 9; verifikovan je manualno kroz UI i Sprint Review demo.
- Legacy `SubscriptionPackages` tabela je dokumentovano ostala u šemi baze i preporučeno je njeno potpuno uklanjanje u budućim migracijama.
- File upload koristi file system pohranu; cloud storage integracija je odbačena za MVP.
- Za priloge je dokumentovan rizik popunjavanja diska jer nema per-user storage limita.
- AI suggestion koristi internu knowledge base koja se mora ručno održavati i može zastariti.
- MCP Admin Copilot zavisi od MCP servera i Groq dostupnosti; u slučaju pada MCP servera vraća se kontrolisana 503 greška.

## Sigurnosna ograničenja

- Sistem koristi testne korisnike i testne lozinke dokumentovane u README/User Manual dokumentima; lozinke su namijenjene isključivo testiranju.
- Produkcijska baza se resetuje deploymentom i podaci preživljavaju samo kroz seed mehanizam, što nije prikladno za stvarnu produkciju sa trajnim korisničkim podacima.
- Frontend ograničenja nisu dovoljna za sigurnost; dokumentovano je da je potrebna backend validacija za role-based pristup i race condition scenarije.
- Za upload priloga je implementirana whitelist validacija i path traversal zaštita, ali ostaje dokumentovan rizik bez storage quota limita.
- MCP Admin Copilot je read-only i ne smije se predstavljati kao alat koji izvršava administratorske akcije.

## Nezavršene funkcionalnosti

Prema finalnom izvještaju i finalnom backlogu, nisu završene:

- Linked Tickets,
- SLA praćenje i upozorenja,
- Bulk akcije na tiketima,
- Login putem broja telefona,
- mobilna aplikacija,
- CRM integracije,
- napredni BI dashboard,
- automatska AI preraspodjela tiketa.

## Pretpostavke sistema

- Korisnik već ima registrovan profil, a pristupni podaci se nalaze u njegovom ugovoru za paket koji koristi.
- Seed podaci se automatski kreiraju samo u Development okruženju.
- Svi dokumentovani testni korisnici imaju aktivne račune.
- Klijenti mogu vidjeti samo vlastite tikete i nemaju pristup administrativnim funkcijama.
- Agenti i tehničari koriste AI prijedloge kao pomoć, ali odgovor mora biti pregledan i poslan eksplicitnom korisničkom akcijom.
- Za izvještaje po broju, statusu, tipu problema i prosječnom vremenu rješavanja tehničarski user story dio je označen kao out of scope po dogovoru.

## Dijelovi koji se ne smiju predstavljati kao potpuno kompletni

- Sprint 11 backlog stavke PB-64, PB-65, PB-66 i PB-67 ne smiju se predstavljati kao završene.
- PB-52 ne smije se predstavljati kao automatski testiran na istom nivou kao ostale funkcionalnosti; dokumentovana je manualna verifikacija.
- Produkcijski deployment ne smije se predstavljati kao spreman za čuvanje trajnih produkcijskih podataka dok god resetuje bazu.
- MCP Admin Copilot ne smije se predstavljati kao sistem koji samostalno mijenja podatke ili izvršava akcije.
- AI prijedlog odgovora ne smije se predstavljati kao konačan odgovor bez provjere od agenta ili tehničara.
- Mobilna aplikacija, CRM integracije, napredni BI dashboard i automatska AI preraspodjela tiketa nisu dio finalne isporuke.
