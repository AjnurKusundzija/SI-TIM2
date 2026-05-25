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
| Alat koji je korišten | Claude Code (Anthropic) — Opus 4.7 |
| Svrha korištenja | Generisanje automatizovanih testova za Sprint 9 user stories (PB-45, PB-50, PB-51) i ažuriranje Proof of Testing dokumenta |
| Kratak opis zadatka ili upita | Dodati nove backend (xUnit + Moq + EF InMemory + Stopwatch) i frontend (Vitest + React Testing Library) testove za US-71, US-72, US-73, US-74, US-75, US-82, US-83, US-84, US-85, US-86, US-87, US-88, US-89, bez izmjene postojećeg produkcijskog koda i bez izmjene postojećih testova. Pokriti unit, integracijski, sistemski, sigurnosni, performansni i acceptance/smoke nivo. Ažurirati `Sprint9/ProofOfTesting.md` po uzoru na `Sprint7/ProofOfTesting.md` |
| Šta je AI predložio ili generisao | 10 novih backend test fajlova u `Project/TelecomSupportSystem/TelecomSupportSystem.Tests/Sprint9/` (87 test metoda, sa Theory inline data > 100 slučajeva) — `UserAccountManagementServiceTests`, `UserAccountManagementControllerTests`, `UserAccountManagementIntegrationTests`, `UserAccountManagementSecurityTests`, `FirstResponseReportTests`, `FirstResponseReportIntegrationTests`, `AdminDashboardServiceTests`, `AdminDashboardIntegrationTests`, `AdminDashboardPerformanceTests`, `Sprint9UserStoriesSystemTests`. Pored toga 4 nova frontend test fajla u `Project/frontend/src/test/` (40 testova) — `Sprint9CreateUser.test.jsx`, `Sprint9UsersList.test.jsx`, `Sprint9AdminDashboard.test.jsx`, `Sprint9FirstResponse.test.jsx`. Kompletno prepisan `Sprint9/ProofOfTesting.md` sa tabelama pokrivenih AC za svaki US, vezom sa Test Strategijom, gap evidencijom i komandama za lokalno pokretanje |
| Šta je tim prihvatio | Prihvaćeni svi generisani test fajlovi (nijedna postojeća produkcijska niti test datoteka nije mijenjana, samo dodani novi fajlovi); prihvaćena cijela struktura `Sprint9/ProofOfTesting.md` |
| Šta je tim izmijenio | Nakon prvog test run-a popravljena 2 frontend testa: precizniji string match za inline validation poruke u `Sprint9CreateUser.test.jsx`, i dokumentovan gap (auto-reload preko useEffect) u `Sprint9AdminDashboard.test.jsx` umjesto da se mijenja produkcijski kod |
| Šta je tim odbacio | Odbačen prijedlog da se popravi produkcijski kod kada bi to bilo potrebno za prolaz nekog AC (npr. audit log za izmjenu korisnika u US-74 ili confirm modal za US-89) — umjesto toga, ti slučajevi su evidentirani kao GAP u `ProofOfTesting.md` |
| Rizici, problemi ili greške koje su uočene | (1) `.NET SDK` nije bio instaliran u trenutnom okruženju pa backend testovi nisu pokrenuti lokalno tokom prve sesije — naknadno instaliran .NET 10.0.300 SDK i potrebno ih je verifikovati. (2) Otkriven gap u `AdminDashboardSection.jsx` (auto-reload na promjenu filtera prije Primijeni — krši AC „ne smije pozvati API“). (3) Otkriven gap: audit log za `Users` izmjene nije implementiran u produkcijskom kodu (US-74 AC). Nije pravljen fix, već gap dokumentovan |
| Ko je koristio alat | Ajnur Kušundžija |

---

Napomena: Ovaj AI Usage Log je zivi dokument i azurira se kroz sprintove.
