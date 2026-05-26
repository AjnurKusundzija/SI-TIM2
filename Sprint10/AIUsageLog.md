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
| Šta je tim izmijenio | [PLACEHOLDER — navesti konkretne izmjene] |
| Šta je tim odbacio | [PLACEHOLDER — navesti odbačene prijedloge] |
| Rizici, problemi ili greške koje su uočene | [PLACEHOLDER — navesti uočene rizike i probleme] |
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
| Šta je tim izmijenio | [PLACEHOLDER — navesti konkretne izmjene] |
| Šta je tim odbacio | [PLACEHOLDER — navesti odbačene prijedloge] |
| Rizici, problemi ili greške koje su uočene | Navy boje u `index.css` su slučajno prepisane sa aliasima za plave boje tokom sessiona — ispravno riješeno vraćanjem tamnih hex vrijednosti; Header i main content imali su različite pozadine — ujednačeno na `bg-[#f4f6f9]`; alert banner nije bio u cijelosti klikabilan — riješeno wrappanjem u `<div onClick>` s `stopPropagation` na X dugmetu |
| Ko je koristio alat | Uma Mahmutovic |

---

## Unos #3

| Polje | Detalji |
|---|---|
| Datum | [PLACEHOLDER] |
| Sprint broj | Sprint 10 |
| Alat koji je korišten | [PLACEHOLDER] |
| Svrha korištenja | [PLACEHOLDER] |
| Kratak opis zadatka ili upita | [PLACEHOLDER] |
| Šta je AI predložio ili generisao | [PLACEHOLDER] |
| Šta je tim prihvatio | [PLACEHOLDER] |
| Šta je tim izmijenio | [PLACEHOLDER] |
| Šta je tim odbacio | [PLACEHOLDER] |
| Rizici, problemi ili greške koje su uočene | [PLACEHOLDER] |
| Ko je koristio alat | [PLACEHOLDER] |

---

Napomena: Ovaj AI Usage Log je zivi dokument i azurira se kroz sprintove.
