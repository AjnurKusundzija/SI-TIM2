# Decision Log - UPDATE ZA SPRINT 11

Decision Log se koristi za evidentiranje važnih projektnih, zahtjevnih, arhitektonskih, tehničkih i procesnih odluka.

Decision Log treba pokazati da tim ne radi nasumično, nego svjesno donosi i prati odluke.

---

## Odluka #1

| Polje | Detalji |
|---|---|
| **ID odluke** | DL-S11-01 |
| **Datum** | 06.06.2026 |
| **Kratak naziv odluke** | CSV export implementiran client-side bez novog backend endpointa |
| **Opis problema ili pitanja** | Za PB-46 (Export izvještaja), tim je razmatrao da li CSV generisanje obaviti na backendu (novi endpoint koji vraća CSV fajl) ili na frontendu (browser generisanje iz postojećih API podataka) |
| **Razmatrane opcije** | 1. Backend CSV endpoint — novi `GET /api/reports/export` koji vraća `text/csv` response; 2. Client-side generisanje — frontend dohvaća podatke kroz postojeći `POST /api/reports/generate` i gradi CSV u browseru |
| **Odabrana opcija** | Client-side CSV generisanje u browseru |
| **Razlog izbora** | Postojeći `POST /api/reports/generate` endpoint već vraća sve potrebne podatke u strukturiranom JSON formatu; nije potrebna nikakva backend izmjena; client-side `Blob` + `URL.createObjectURL` je standardna web tehnika; nema server-side memorijskog opterećenja za generisanje fajla; MVP pristup — minimalan scope |
| **Posljedice odluke** | CSV fajl se kreira u browseru — za vrlo velike skupove podataka (npr. alltime period s desetinama hiljada tiketa) fajl može biti ograničen količinom podataka koju backend vraća u JSON odgovoru; server-side opcija bi bila bolja za streaming velikih fajlova, ali nije potrebna za trenutni scope |
| **Status odluke** | Prihvaćena |

---

## Odluka #2

| Polje | Detalji |
|---|---|
| **ID odluke** | DL-S11-02 |
| **Datum** | 06.06.2026 |
| **Kratak naziv odluke** | UTF-8 BOM uključen u CSV export za Excel kompatibilnost |
| **Opis problema ili pitanja** | Generirani CSV fajl sadrži bosanski tekst sa specijalnim znakovima (č, ć, đ, š, ž). Windows Excel otvara UTF-8 CSV fajlove bez BOM-a u pogrešnom encodingu što uzrokuje iskrivljene znakove |
| **Razmatrane opcije** | 1. UTF-8 bez BOM — standardno ali problematično u Excelu na Windowsu; 2. UTF-8 s BOM (U+FEFF) — Excel ga prepoznaje kao signal za UTF-8 encoding; 3. Windows-1252 encoding — kompatibilno s Excelom ali gubi unicode znakove van te code page |
| **Odabrana opcija** | UTF-8 s BOM (U+FEFF prefiks na CSV sadržaju) |
| **Razlog izbora** | Sistem koristi bosanski jezik s dijakritičkim znakovima koji zahtijevaju UTF-8; BOM je standardna metoda za naznačavanje UTF-8 encodinga u CSV fajlovima za Windows alate; ne utječe na čitanje u LibreOffice, Google Sheets ili text editorima |
| **Posljedice odluke** | Manji overhead (3 bajta) na početku fajla; u rijetkim slučajevima programatskog čitanja CSV-a BOM može uzrokovati neočekivani karakter u prvoj ćeliji — trivijalno za strip |
| **Status odluke** | Prihvaćena |

---

Napomena: Ovaj Decision Log je živi dokument i ažurira se kroz sprintove.
