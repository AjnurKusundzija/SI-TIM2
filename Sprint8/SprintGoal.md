# Sprint Goal – Sprint 8

## Sprint cilj

Cilj sprinta je implementirati sistem notifikacija koji pravovremeno obavještava sve uloge o događajima na tiketima, proširiti korisničko iskustvo kroz upravljanje profilom i pregled paketa te pretplata, omogućiti agentima i tehničarima uvid u vlastitu statistiku rada, te implementirati ocjenjivanje tiketa i ažuriranje statusa tiketa od strane tehničara.

Sprint je fokusiran na zaokruživanje korisničkog i tehničarskog iskustva unutar sistema — korisnik dobiva mogućnost ocjenjivanja usluge i upravljanja vlastitim profilom, tehničar može ažurirati status dodijeljenih tiketa, a svi korisnici sistema dobivaju obavještenja o relevantnim promjenama bez potrebe za ručnim praćenjem.

Poseban fokus stavljen je na implementaciju notifikacijskog modula čija je infrastruktura pripremljena u Sprint 7, ali čija poslovna logika i frontend prikaz nisu bili implementirani. Sprint 8 donosi kompletnu implementaciju generisanja, prikaza i upravljanja notifikacijama za sve korisničke role.

Dodatno, sprint uključuje proširenje profilnih stranica agenta i tehničara sa ličnom statistikom rada (broj tiketa, prosječna vremena, ocjene), što postavlja temelj za izvještajni modul planiran za Sprint 11.

---

## Fokus sprinta

- Implementacija notifikacijskog sistema (generisanje, prikaz, upravljanje)
- Ocjenjivanje tiketa od strane korisnika nakon zatvaranja
- Ažuriranje statusa tiketa od strane tehničara
- Statistika rada na profilnoj stranici agenta i tehničara
- Upravljanje korisničkim profilom (email, lozinka)
- Pregled korisničkih profila i historije tiketa od strane agenta
- Prikaz aktivnih paketa i pretplata za korisnike
- Integracija notifikacija sa postojećim ticket workflow-om
- Proširenje autorizacije i pristupa po korisničkim rolama
- Implementacija i proširenje unit testova za nove funkcionalnosti

---

## Očekivani deliverable-i

- PB-49 Notifikacije — implementiran notifikacijski sistem za sve role
- PB-36 Ažuriranje statusa tiketa — tehničar mijenja status dodijeljenih tiketa
- PB-26 Ocjenjivanje tiketa — korisnik ocjenjuje zatvoreni tiket (1–5, opcionalni komentar)
- PB-42 Statistika agenta i tehničara — lična statistika na profilnoj stranici
- PB-20 Upravljanje korisničkim profilom — promjena email adrese i lozinke
- PB-34 Pregled korisničkih profila (agent) — agent vidi profil, historiju tiketa i pakete korisnika
- PB-21 Prikaz paketa i pretplata — korisnik vidi svoje aktivne pakete i pretplate
- Ažurirani Sprint Backlog
- Decision Log
- AI Usage Log
- Unit testovi i rezultati testiranja
- Dokumentacija implementiranih funkcionalnosti

---

## Sprint Review kriterij

Ocjenjuje se funkcionalna implementacija notifikacijskog sistema, ispravnost generisanja notifikacija po svim predviđenim događajima, kvalitet implementacije korisničkih profila i statistike, te uspješnost integracije ocjenjivanja tiketa i ažuriranja statusa.

Review uključuje provjeru:
- Ispravnosti generisanja notifikacija po svim tipovima (`TICKET_ASSIGNED`, `TICKET_FORWARDED`, `STATUS_CHANGED`, `TICKET_RESPONSE`, `TICKET_CLOSED`)
- Ispravnosti prikaza i upravljanja notifikacijama (badge, označavanje kao pročitano)
- Funkcionalnosti ocjenjivanja tiketa i ograničenja (jednom po tiketu, samo zatvoreni tiketi)
- Ispravnosti ažuriranja statusa tiketa od strane tehničara i generisanja odgovarajuće notifikacije
- Prikaza statistike na profilnoj stranici agenta i tehničara
- Funkcionalnosti promjene email adrese i lozinke korisnika
- Prikaza korisničkih profila i historije tiketa od strane agenta
- Prikaza paketa i pretplata korisnika
- Ispravnosti autorizacije po korisničkim rolama za sve nove funkcionalnosti
- Pokrivenosti testovima i stabilnosti aplikacije

---

## Održani sastanci

- Sprint planning sastanak
- Koordinacija implementacije notifikacijskog sistema
- Dogovor oko redefinisanja PB-42
- Review sastanci za backend i frontend integraciju

