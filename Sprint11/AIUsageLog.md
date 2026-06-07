# AI Usage Log – Sprint 11

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
| Datum | 06.06.2026 |
| Sprint broj | Sprint 11 |
| Alat koji je korišten | Claude Code (Anthropic, Sonnet) |
| Svrha korištenja | Implementacija PB-46 (Export izvještaja) prema acceptance kriterijima iz Sprint Backloga 11 (US-112) |
| Kratak opis zadatka ili upita | Implementacija client-side CSV export funkcionalnosti za admin izvještaje. Zadatak je uključivao: definisanje user storija i acceptance kriterija kroz clarifying questions, implementaciju `escapeCSV` i `buildReportCSV` helper funkcija na module nivou za svih 7 tipova izvještaja, dodavanje `exportLoading` stanja u komponentu, implementaciju `handleExport` callback-a koji fetchuje svježe podatke i trigeruje browser download, ažuriranje Export dugmeta iz disabled stanja u aktivno s loading indikatorom, te ažuriranje postojećeg US-85 testa i dodavanje URL.createObjectURL mock-a |
| Šta je AI predložio ili generisao | Module-level `escapeCSV` funkcija koja pravilno escapuje zareze, navodnike i nove redove u CSV vrijednostima; `PERIOD_LABELS` konstantu za formatirani prikaz perioda u metadata headeru; `buildReportCSV` funkciju s kompletnom logikom za svih 7 tipova izvještaja (TICKET_COUNT s totalCount i buckets; TICKET_STATUS s postocima; PROBLEM_TYPE s labelama; TEAM_WORKLOAD s dvije tabele — po agentu i pivot period×agent; USER_RATINGS s distribucijom i bucket trendom; FIRST_RESPONSE i AVG_RESOLUTION s agregatima i bucket tabelama); `exportLoading` state; `handleExport` async callback koji validira period, fetchuje svježe podatke putem postojećeg `generateReport` API-ja, gradi CSV s UTF-8 BOM prefixom, kreira `Blob`, koristi `URL.createObjectURL` i trigeruje download; ažurirano Export dugme s loading spinnerom (Loader2 komponenta), navy stilom kada je aktivan i disabled stilom tokom loading-a; ažurirani US-85 test koji provjerava da je dugme enabled i trigeruje `generateReport`; `global.URL.createObjectURL = vi.fn()` mock u beforeEach za jsdom okruženje |
| Šta je tim prihvatio | Cjelokupni client-side pristup bez novog backend endpointa; `escapeCSV` funkcija za sigurno rukovanje specijalnim znakovima; `buildReportCSV` sa svim tipovima izvještaja; UTF-8 BOM za Excel kompatibilnost; default TICKET_COUNT kada nije odabran tip; loading stanje na dugmetu; ažurirani test s URL mock-om |
| Šta je tim izmijenio | Clarifying questions procesom definisani su: svi tipovi exportuju se (ne samo odabrani), CSV sadrži metadata header, dugme je uvijek aktivno, fajl se zove `report.csv`, export uvijek fetchuje svježe podatke prema trenutnim parametrima forme |
| Šta je tim odbacio | Prvobitna ideja o exportovanju samo već generisanog reporta (bez novog fetcha) — odbačena jer korisnik može promijeniti period poslije generisanja a bez klika na Primijeni; ideja o serverside CSV endpointu — odbačena kao prekomplicirana za ovaj scope |
| Rizici, problemi ili greške koje su uočene | jsdom okruženje ne implementira `URL.createObjectURL` — riješeno dodavanjem `vi.fn()` mock-a u `beforeEach` bloku reports describe; UTF-8 BOM mora biti dodan kao `"﻿"` string literal (Unicode escape `﻿`) a ne kao byte sequence u Blob konstruktoru |
| Ko je koristio alat | Uma Mahmutovic |

---

Napomena: Ovaj AI Usage Log je zivi dokument i azurira se kroz sprintove.
