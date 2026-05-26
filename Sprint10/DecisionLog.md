# Decision Log - UPDATE ZA SPRINT 10

Decision Log se koristi za evidentiranje važnih projektnih, zahtjevnih, arhitektonskih, tehničkih i procesnih odluka.

Decision Log treba pokazati da tim ne radi nasumično, nego svjesno donosi i prati odluke.

---

## Odluka #1

| Polje | Detalji |
|---|---|
| **ID odluke** | DL-S10-01 |
| **Datum** | 26.05.2026 |
| **Kratak naziv odluke** | AI servis koristi internu knowledge base umjesto vanjskog LLM API-ja |
| **Opis problema ili pitanja** | Za AI prijedloge odgovora i admin uvide, tim je razmatrao da li koristiti vanjski LLM (OpenAI, Claude API) ili internu knowledge base s predefinisanim rješenjima |
| **Razmatrane opcije** | 1. Vanjski LLM API (OpenAI GPT / Anthropic Claude) — dinamični, kontekstualni odgovori; 2. Interna knowledge base s heurističkim pravilima — deterministički, offline, bez troškova |
| **Odabrana opcija** | Interna knowledge base s heurističkim pravilima |
| **Razlog izbora** | Nema troškova API poziva; nema latencije externe mreže; deterministički i testabilni odgovori; telekomunikacijska domena je dobro definisana i pokriva se s 6 predefinisanih kategorija problema; MVP pristup |
| **Posljedice odluke** | Odgovori su manje personalizovani od LLM generisanih; proširenje na novu kategoriju zahtijeva izmjenu koda; admin insights su temeljeni na heurističkim pravilima a ne na dubokoj analizi podataka |
| **Status odluke** | Prihvaćena |

---

## Odluka #2

| Polje | Detalji |
|---|---|
| **ID odluke** | DL-S10-02 |
| **Datum** | 26.05.2026 |
| **Kratak naziv odluke** | Zustand store za dijeljeno stanje između Header i AdminDashboard komponenti |
| **Opis problema ili pitanja** | AI Uvidi dugme je u Header komponenti (AppLayout), dok se panel i dashboard podaci nalaze u AdminDashboardSection (renderisan kroz Outlet). Potreban je mehanizam za dijeljenje stanja između ove dvije nezavisne grane stabla komponenti |
| **Razmatrane opcije** | 1. React Context — dodat novi kontekst; 2. Prop drilling kroz AppLayout — nije izvedivo jer su komponente u različitim granama; 3. Zustand global store — minimalan boilerplate, već korišten u projektu |
| **Odabrana opcija** | Zustand store (`uiStore.js`) |
| **Razlog izbora** | Zustand je već prisutan u projektu (koristi se za ostalo UI stanje); nema prop drilling; minimalan boilerplate; state je dostupan iz bilo koje komponente bez kontekst providera |
| **Posljedice odluke** | Globalno stanje za UI; potrebno voditi računa o čišćenju stanja pri navigaciji između stranica |
| **Status odluke** | Prihvaćena |

---

## Odluka #3

| Polje | Detalji |
|---|---|
| **ID odluke** | [PLACEHOLDER] |
| **Datum** | [PLACEHOLDER] |
| **Kratak naziv odluke** | [PLACEHOLDER] |
| **Opis problema ili pitanja** | [PLACEHOLDER] |
| **Razmatrane opcije** | [PLACEHOLDER] |
| **Odabrana opcija** | [PLACEHOLDER] |
| **Razlog izbora** | [PLACEHOLDER] |
| **Posljedice odluke** | [PLACEHOLDER] |
| **Status odluke** | [PLACEHOLDER] |

---

Napomena: Ovaj Decision Log je živi dokument i ažurira se kroz sprintove.
