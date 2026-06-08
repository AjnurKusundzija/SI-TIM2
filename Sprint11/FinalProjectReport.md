# Završni izvještaj o radu tima

## Uvod

Tokom trajanja projekta tim je radio na razvoju modernog helpdesk i ticketing sistema namijenjenog telekom okruženju. Projekat je razvijan iterativno kroz Scrum metodologiju rada, pri čemu je svaki sprint imao jasno definisane ciljeve, Product Backlog stavke, acceptance kriterije i očekivane isporuke.

Cilj projekta nije bio samo implementirati osnovni sistem za rad sa tiketima, već razviti kompletno rješenje koje podržava korisnike, agente, tehničare i administratore kroz cijeli životni ciklus korisničke podrške. Tokom razvoja posebna pažnja posvećena je korisničkom iskustvu, automatizaciji procesa, izvještavanju, upravljanju korisnicima i timovima te integraciji AI funkcionalnosti koje unapređuju svakodnevni rad korisnika sistema.

Projekat je realizovan kroz kontinuiranu saradnju članova tima, redovne sprint planning sastanke, sprint review sastanke, retrospektive, code review aktivnosti, testiranje i integraciju frontend i backend komponenti sistema.

---

# Svrha projekta

Svrha projekta bila je razvoj centralizovanog helpdesk sistema koji omogućava efikasno kreiranje, dodjelu, praćenje, obradu i zatvaranje korisničkih zahtjeva kroz strukturirani ticketing sistem.

Sistem je razvijen s ciljem da:

- unaprijedi proces korisničke podrške,
- omogući jednostavniju komunikaciju između korisnika i osoblja,
- poboljša organizaciju rada agenata i tehničara,
- olakša upravljanje korisnicima i timovima,
- pruži administratorima bolji uvid u stanje sistema,
- automatizuje određene poslovne procese,
- omogući korištenje AI funkcionalnosti za podršku radu korisnika.

Na taj način sistem doprinosi efikasnijem radu podrške, većoj transparentnosti procesa i kvalitetnijem korisničkom iskustvu.

---

# Problem koji sistem rješava

Mnoge organizacije i dalje koriste kombinaciju e-mail komunikacije, Excel tabela i nepovezanih alata za evidenciju korisničkih zahtjeva.

Takav pristup dovodi do:

- gubitka informacija,
- sporijeg rješavanja problema,
- nejasne odgovornosti za tiket,
- otežanog praćenja statusa zahtjeva,
- dupliranja aktivnosti,
- otežane analize rada timova,
- ograničenih mogućnosti izvještavanja,
- loše organizacije korisničke podrške.

Razvijeni sistem rješava navedene probleme kroz jedinstvenu platformu koja objedinjuje upravljanje tiketima, komunikacijom, korisnicima, timovima, pretplatama, izvještajima i AI funkcionalnostima.

---

# Glavne korisničke uloge

## Klijent

Klijent predstavlja krajnjeg korisnika sistema.

Omogućene funkcionalnosti:

- kreiranje tiketa,
- pregled vlastitih tiketa,
- komunikacija sa podrškom,
- pregled historije zahtjeva,
- pregled paketa i pretplata,
- korištenje FAQ sadržaja,
- ocjenjivanje zatvorenih tiketa.

---

## Agent

Agent predstavlja prvi nivo korisničke podrške.

Omogućene funkcionalnosti:

- pregled dodijeljenih tiketa,
- komunikacija sa korisnicima,
- preuzimanje tiketa,
- prosljeđivanje tiketa,
- upravljanje prioritetima,
- korištenje AI prijedloga odgovora,
- pregled statusa dostupnosti.

---

## Tehničar

Tehničar rješava kompleksnije tehničke probleme.

Omogućene funkcionalnosti:

- pregled tehničkih tiketa,
- ažuriranje statusa,
- komunikacija kroz tiket,
- korištenje AI prijedloga odgovora,
- rad sa internim komentarima.

---

## Administrator

Administrator ima najviši nivo pristupa sistemu.

Omogućene funkcionalnosti:

- upravljanje korisnicima,
- upravljanje timovima,
- upravljanje paketima,
- upravljanje pretplatama,
- pregled audit logova,
- upravljanje FAQ sadržajem,
- korištenje AI Insights funkcionalnosti,
- korištenje MCP Admin Copilot modula,
- preraspodjela agenata i tehničara,
- pregled izvještaja i statistika.

---

# Glavne implementirane funkcionalnosti

## Upravljanje tiketima

- kreiranje tiketa,
- pregled detalja tiketa,
- dodjela tiketa,
- automatska dodjela tiketa,
- promjena statusa,
- upravljanje prioritetima,
- zatvaranje tiketa,
- historija aktivnosti.

## Komunikacija

- komunikacija kroz tiket,
- evidencija poruka,
- interni komentari za osoblje,
- sistemske poruke unutar workflow-a.

## Upravljanje korisnicima

- kreiranje korisnika,
- uređivanje korisnika,
- deaktivacija korisnika,
- pregled korisničkih profila,
- role-based pristup.

## Upravljanje timovima

- pregled timova,
- upravljanje članovima timova,
- availability status agenata,
- pregled opterećenja timova.

## Paketi i pretplate

- katalog paketa,
- dodjela pretplata klijentima,
- pregled pretplata,
- audit log aktivnosti nad pretplatama.

## Audit log

- evidencija aktivnosti korisnika,
- praćenje administrativnih akcija,
- pregled historije promjena.

## FAQ modul

- pregled FAQ sadržaja,
- kreiranje FAQ stavki,
- uređivanje FAQ stavki,
- brisanje FAQ stavki.

## AI funkcionalnosti

- AI prijedlog odgovora za agente,
- AI prijedlog odgovora za tehničare,
- AI Insights za administratore,
- MCP Admin Copilot,
- AI podrška za analizu sistema.

## Izvještavanje

- dashboard statistike,
- administrativni izvještaji,
- CSV export izvještaja.

## Korisnički interfejs

- redizajnirani Sidebar,
- redizajnirani Header,
- redizajnirani Dashboard,
- nova navigacija,
- navy dizajn sistem,
- unaprijeđeno korisničko iskustvo.

---

# Pregled rada kroz sprintove

## Sprint 6

Sprint 6 bio je fokusiran na unapređenje ticket modula i komunikacije.

Implementirano:

- detaljan prikaz tiketa,
- komunikacija kroz tiket,
- pregled svih tiketa za agente i administratore,
- FAQ funkcionalnosti,
- unapređenje autorizacije.

Sprint je uspješno završen i pozitivno ocijenjen od strane Product Ownera.

---

## Sprint 7

Sprint 7 bio je fokusiran na ticket workflow logiku.

Implementirano:

- automatska dodjela tiketa,
- upravljanje prioritetima,
- pregled dodijeljenih tiketa,
- zatvaranje tiketa,
- promjena statusa tiketa,
- prosljeđivanje tiketa.

Sprint je uspješno završen i ocijenjen sa 100%.

---

## Sprint 8

Sprint 8 bio je fokusiran na korisničke profile, statistiku i notifikacije.

Implementirano:

- sistem notifikacija,
- korisnički profili,
- statistika rada agenata,
- statistika rada tehničara,
- ocjenjivanje tiketa,
- pregled paketa i pretplata.

Sprint je uspješno završen uz pozitivne komentare Product Ownera.

---

## Sprint 9

Sprint 9 bio je fokusiran na administrativne funkcionalnosti i upravljanje paketima.

Implementirano:

- upravljanje korisnicima,
- katalog paketa,
- dodjela pretplata klijentima,
- audit log sistem,
- upload priloga,
- pregled timova.

Sprint je ocijenjen sa 100%.

---

## Sprint 10

Sprint 10 predstavljao je jedan od najkompleksnijih sprintova projekta.

Implementirano:

- PB-57 AI prijedlog odgovora za agente i tehničare,
- PB-58 AI Insights za administratore,
- PB-59 kompletan redizajn korisničkog interfejsa,
- PB-31 proširenje administrativne preraspodjele tiketa,
- PB-60 interni komentari,
- PB-61 Admin CRUD FAQ,
- PB-62 Assign To Me funkcionalnost,
- PB-63 Agent Availability Status,
- PB-70 MCP Admin Copilot.

Sprint je dobio 100% bodova, a Product Owner je posebno pohvalio kvalitet implementacije i izgled sistema.

---

## Sprint 11

Sprint 11 predstavlja završni sprint projekta.

Završene su:

- PB-46 Export izvještaja,
- PB-65 SLA praćenje i upozorenja,
- PB-67 Login putem broja telefona.

Odgođene stavke (Deferred) — nisu implementirane:

- PB-64 Linked Tickets,
- PB-66 Bulk akcije na tiketima.

---

# Šta je završeno

Tim je uspješno završio sve funkcionalnosti planirane kroz Sprint 6, Sprint 7, Sprint 8, Sprint 9, Sprint 10 i Sprint 11.

Završeni su:

- ticket workflow,
- komunikacijski modul,
- korisnički moduli,
- timski moduli,
- paketni moduli,
- audit log sistem,
- FAQ sistem,
- AI funkcionalnosti,
- MCP Admin Copilot,
- CSV export izvještaja,
- SLA praćenje i upozorenja,
- login putem broja telefona,
- redizajn korisničkog interfejsa,
- dokumentacija,
- testiranje.

---

# Šta je djelimično završeno

Djelimično završene funkcionalnosti odnose se na Sprint 11 backlog stavke koje su planirane, ali nisu implementirane:

- Linked Tickets (PB-64, Deferred),
- Bulk akcije na tiketima (PB-66, Deferred).

---

# Šta nije završeno

Nisu realizovane funkcionalnosti koje nisu bile dio završene implementacije:

- Linked Tickets (Deferred),
- Bulk akcije nad tiketima (Deferred),
- mobilna aplikacija,
- CRM integracije,
- napredni BI dashboard,
- automatska AI preraspodjela tiketa.

---

# Glavne tehničke odluke

Najvažnije tehničke odluke tokom projekta bile su:

- ASP.NET Core backend arhitektura,
- React frontend aplikacija,
- Entity Framework Core ORM,
- Service-Repository arhitektura,
- REST API komunikacija,
- Role-Based Authorization,
- Docker okruženje,
- poseban AI servisni sloj,
- MCP server kao zaseban servis,
- audit log arhitektura.

---

# Najveći problemi tokom razvoja i način rješavanja

## Merge konflikti

Pojavljivali su se zbog paralelnog razvoja više funkcionalnosti.

Rješavani su kroz Pull Request review proces i dodatno integraciono testiranje.

## Migracije baze podataka

Više funkcionalnosti zahtijevalo je izmjene šeme baze podataka.

Problem je riješen standardizacijom procesa migracija i dodatnim provjerama prije merge-a.

## Integracija AI funkcionalnosti

AI funkcionalnosti zahtijevale su dodatna testiranja i prilagođavanje postojećoj arhitekturi sistema.

Rješenje je bilo odvajanje AI logike u poseban servisni sloj.

## Redizajn korisničkog interfejsa

Sprint 10 zahtijevao je refaktorisanje velikog broja zajedničkih komponenti.

Problem je riješen kroz postepenu migraciju na novi dizajn sistem i dodatno testiranje.

---

# Šta bi tim unaprijedio da se projekat nastavlja

Ako bi se razvoj projekta nastavio, fokus bi bio na:

- Linked Tickets modulu (PB-64, Deferred),
- Bulk akcijama na tiketima (PB-66, Deferred),
- automatizovanim testovima za PB-52 (upravljanje paketima),
- proširenju AI funkcionalnosti,
- mobilnoj aplikaciji,
- CRM integracijama,
- naprednom BI dashboardu,
- dodatnoj optimizaciji performansi,
- E2E testovima (Playwright/Cypress),
- cloud storage integraciji za upload fajlova.

---

# Zaključak

Projekat je uspješno rezultirao razvojem modernog helpdesk i ticketing sistema koji podržava upravljanje tiketima, komunikaciju, korisnike, timove, pakete, pretplate, audit logove, izvještavanje i AI funkcionalnosti.

Tim je kroz više sprintova kontinuirano unapređivao sistem, uspješno rješavao tehničke izazove i implementirao sve ključne funkcionalnosti planirane za završene sprintove.

Posebno značajan rezultat projekta predstavljaju AI funkcionalnosti, MCP Admin Copilot modul i kompletan redizajn korisničkog interfejsa, koji sistemu daju dodatnu vrijednost i predstavljaju osnovu za buduća proširenja i unapređenja.
