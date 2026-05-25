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

Napomena: Ovaj AI Usage Log je zivi dokument i azurira se kroz sprintove.
