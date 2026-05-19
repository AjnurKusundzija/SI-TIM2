# Sprint Backlog – Sprint 8

Sprint backlog treba biti realan i povezan sa sprint ciljem.

---

## Veza sa sprint ciljem

Implementirati sistem notifikacija koji obavještava korisnike o događajima na tiketima, proširiti korisničke profile sa pregledom paketa i pretplata, omogućiti agentima i tehničarima uvid u vlastitu statistiku rada, te implementirati ocjenjivanje tiketa i ažuriranje statusa tiketa od strane tehničara.

---

## Stavke sprint backloga

| ID | Naziv zadatka ili storyja | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|---|---|---|---|---|---|
| SB-01 | PB-49 Notifikacije | US-58, US-59 | Uma | Done | Implementacija slanja i prikaza notifikacija za sve role, real-time via SignalR |
| SB-02 | PB-36 Ažuriranje statusa tiketa | US-60 | Ajnur | Done | Tehničar mijenja status tiketa koji mu je dodijeljen |
| SB-03 | PB-26 Ocjenjivanje tiketa | US-61 | Ajnur | Done | Korisnik ocjenjuje kvalitet rješenja nakon zatvaranja tiketa |
| SB-04 | PB-42 Statistika agenta i tehničara | US-62, US-63 | Uma | Done | Prikaz lične statistike rada na dashboardu |
| SB-05 | PB-20 Upravljanje korisničkim profilom | US-64, US-65 | Merisa | Done | Korisnik mijenja email i lozinku svog profila |
| SB-06 | PB-34 Pregled korisničkih profila (agent) | US-66, US-67 | Merisa | Done | Agent pregledava profile korisnika i historiju tiketa |
| SB-07 | PB-21 Prikaz paketa i pretplata | US-68 | Eldar | Done | Korisnik vidi svoje aktivne pakete i pretplate |
| SB-08 | Sistemske poruke u chatu pri prosljeđivanju tiketa | US-69 | Uma | Done | Proširenje PB-31: automatska poruka u chatu + real-time broadcast kada se tiket proslijedi |
| SB-09 | Zajednička dodjela tiketa agentu i tehničaru nakon prosljeđivanja | US-70 | Uma | Done | Agent ostaje aktivno dodijeljen nakon prosljeđivanja tehničaru; tiket je vidljiv i agentu i tehničaru |

---

# Detaljni User Stories (US)

---

## PB-49 Notifikacije

### US-58
*Kao korisnik, agent ili tehničar, želim da primam notifikacije o relevantnim događajima na tiketima, kako bih bio pravovremeno obavješten bez potrebe za stalnim provjeravanjem sistema.*

**Acceptance Criteria:**
- Kada je tiket dodijeljen agentu ili tehničaru, sistem mora automatski kreirati notifikaciju tipa `TICKET_ASSIGNED` za tog korisnika
- Kada je tiket proslijeđen drugom agentu ili tehničaru, sistem mora kreirati notifikaciju tipa `TICKET_FORWARDED` za novog vlasnika tiketa
- Kada je tiket proslijeđen, sistem mora kreirati notifikaciju tipa `TICKET_FORWARDED` i za kreatora tiketa (klijenta), kako bi bio obaviješten o promjeni odgovorne osobe
- Kada agent ili tehničar pošalje poruku na tiketu, sistem mora kreirati notifikaciju tipa `TICKET_RESPONSE` za kreatora tiketa
- Kada korisnik pošalje poruku na tiketu, sistem mora kreirati notifikaciju tipa `TICKET_RESPONSE` za trenutnog vlasnika tiketa
- Kada se tiket zatvori, sistem mora kreirati notifikaciju tipa `TICKET_CLOSED` za kreatora tiketa
- Kada tehničar promijeni status tiketa, sistem mora kreirati notifikaciju tipa `STATUS_CHANGED` za kreatora tiketa
- Kada klijent prihvati zatvaranje tiketa, sistem mora kreirati notifikaciju tipa `TICKET_CLOSED` za sve aktivno dodijeljene staff korisnike na tiketu
- Kada klijent odbije zatvaranje tiketa, sistem mora kreirati notifikaciju tipa `STATUS_CHANGED` za sve aktivno dodijeljene staff korisnike na tiketu
- Kada aktivno dodijeljeni agent ili tehničar prisilno zatvori tiket nakon isteka roka, sistem mora kreirati notifikaciju tipa `TICKET_CLOSED` za kreatora tiketa
- Kada je tiket proslijeđen tehničaru, aktivno dodijeljeni staff uključuje i agenta koji je proslijedio tiket i tehničara kojem je tiket proslijeđen
- Notifikacije moraju biti isporučene u realnom vremenu putem SignalR konekcije, bez potrebe za osvježavanjem stranice
- Svaka notifikacija mora sadržavati referencu na odgovarajući tiket (`TicketId`) kako bi redirect bio moguć
- Sistem ne smije kreirati notifikaciju za korisnika koji je sam izvršio tu akciju
- Sistem ne smije duplirati notifikacije istom staff korisniku ako se u historiji dodjela pojavljuje više puta

---

### US-59
*Kao korisnik, agent ili tehničar, želim da upravljam svojim notifikacijama i označim ih kao pročitane, kako bih imao jasan pregled nepročitanih obavještenja.*

**Acceptance Criteria:**
- Kada je korisnik prijavljen u sistem, sistem mora prikazivati broj nepročitanih notifikacija u vidu badge-a u zaglavlju
- Kada korisnik otvori pregled notifikacija, sistem mora prikazati sve notifikacije sortirane po datumu, od najnovijih
- Kada korisnik klikne na notifikaciju, sistem mora označiti notifikaciju kao pročitanu i preusmjeriti ga na odgovarajući tiket (klijent na `/mytickets/:id`, agent/tehničar na `/tickets/:id`)
- Sistem mora omogućiti označavanje svih notifikacija kao pročitane odjednom
- Sistem mora prikazivati notifikacije u realnom vremenu — nova notifikacija se dodaje na vrh liste bez osvježavanja stranice
- Sistem mora prikazivati link "Notifikacije" u bočnoj navigaciji s brojem nepročitanih
- Sistem ne smije prikazivati notifikacije drugih korisnika
- Kada korisnik nema notifikacija, sistem mora prikazati odgovarajuću poruku

---

## PB-36 Ažuriranje statusa tiketa

### US-60
*Kao tehničar, želim da promijenim status tiketa koji mi je dodijeljen, kako bih označio napredak rješavanja problema i informisao agenta i korisnika o trenutnom stanju.*

**Acceptance Criteria:**
- Kada je tehničar prijavljen u sistem i pregleda tiket koji mu je dodijeljen, sistem mora prikazati opciju za promjenu statusa
- Sistem mora ponuditi predefinisane statuse koji su dostupni tehničaru za promjenu (`OPEN` i `CLOSURE_REQUESTED`, prikazano u UI kao "U toku (Otvoren)" i "Čeka se")
- Kada tehničar promijeni status tiketa, sistem mora sačuvati promjenu i prikazati ažurirani status
- Kada tehničar uspješno promijeni status, sistem mora generisati notifikaciju tipa `STATUS_CHANGED` za kreatora tiketa
- Sistem ne smije dozvoliti tehničaru promjenu statusa tiketa koji mu nije dodijeljen
- Sistem mora provjeravati dodjelu preko tehničarske dodjele (`AssignedTechnicianId`), a ne preko agentske dodjele
- Kada je tiket već zatvoren, sistem ne smije dozvoliti promjenu statusa
- Kada tehničar postavi tiket u status `CLOSURE_REQUESTED`, tiket mora ostati vidljiv agentu u listi dodijeljenih tiketa
- Sistem mora prikazati potvrdu uspješne promjene statusa

---

## PB-26 Ocjenjivanje tiketa

### US-61
*Kao korisnik, želim da ocijenim kvalitet rješenja nakon zatvaranja tiketa, kako bih dao povratnu informaciju o pruženoj usluzi.*

**Acceptance Criteria:**
- Kada je tiket zatvoren, sistem mora prikazati korisniku opciju za ocjenjivanje tiketa
- Sistem mora omogućiti ocjenjivanje na skali od 1 do 5
- Korisnik može ostaviti opcionalni komentar uz ocjenu
- Kada korisnik potvrdi ocjenu, sistem mora sačuvati ocjenu vezanu za taj tiket
- Sistem ne smije dozvoliti ocjenjivanje tiketa koji nije zatvoren
- Sistem ne smije dozvoliti ocjenjivanje istog tiketa više puta
- Sistem ne smije prikazivati opciju za ocjenjivanje agentima i tehničarima
- Agent i administrator moraju moći vidjeti ocjenu na zatvorenom tiketu

---

## PB-42 Statistika agenta i tehničara

### US-62
*Kao agent, želim da vidim svoju ličnu statistiku rada unutar sistema, kako bih pratio vlastitu produktivnost i kvalitet pružene usluge.*

**Acceptance Criteria:**
- Kada je agent prijavljen i otvori svoju profilnu stranicu, sistem mora prikazati sekciju s ličnom statistikom
- Sistem mora prikazati broj trenutno otvorenih (aktivnih) tiketa dodijeljenih agentu
- Sistem mora prikazati ukupan broj zatvorenih tiketa dodijeljenih agentu
- Sistem mora prikazati broj tiketa koji su u statusu čekanja na zatvaranje
- Tiketi proslijeđeni tehničaru moraju se i dalje računati u statistiku agenta koji je aktivno ostao dodijeljen na tiketu
- Tiketi u statusu `CLOSURE_REQUESTED` ("Čeka se") moraju se računati u broj tiketa koji čekaju zatvaranje
- Sistem mora prikazati prosječno vrijeme prvog odgovora agenta (od kreiranja tiketa do prve poruke agenta)
- Sistem mora prikazati prosječno vrijeme rješavanja tiketa (od kreiranja do zatvaranja)
- Sistem mora prikazati prosječnu ocjenu korisnika na osnovu zatvorenih tiketa
- Kada agent nema nijedan tiket, sistem mora prikazati odgovarajuće poruke umjesto numeričkih vrijednosti
- Klijent ne smije imati pristup statistici agenta

---

### US-63
*Kao tehničar, želim da vidim svoju ličnu statistiku rada unutar sistema, kako bih imao uvid u vlastitu efikasnost i opterećenje.*

**Acceptance Criteria:**
- Kada je tehničar prijavljen i otvori svoju profilnu stranicu, sistem mora prikazati sekciju s ličnom statistikom
- Sistem mora prikazati broj trenutno otvorenih (aktivnih) tiketa dodijeljenih tehničaru
- Sistem mora prikazati ukupan broj zatvorenih tiketa dodijeljenih tehničaru
- Sistem mora prikazati broj tiketa koji su u statusu čekanja na zatvaranje za tehničara
- Sistem mora prikazati prosječno vrijeme prvog odgovora tehničara
- Sistem mora prikazati prosječno vrijeme rješavanja tiketa
- Kada tehničar nema nijedan tiket, sistem mora prikazati odgovarajuće poruke umjesto numeričkih vrijednosti
- Klijent ne smije imati pristup statistici tehničara

---

## PB-20 Upravljanje korisničkim profilom

### US-64
*Kao korisnik, želim da promijenim email adresu svog profila, kako bih održavao tačnost svojih kontakt podataka.*

**Acceptance Criteria:**
- Kada je korisnik prijavljen i otvori stranicu svog profila, sistem mora prikazati opciju za promjenu email adrese
- Kada korisnik unese novu email adresu i potvrdi promjenu, sistem mora validirati format email adrese
- Sistem ne smije dozvoliti promjenu na email adresu koja je već zauzeta u sistemu
- Kada je promjena uspješna, sistem mora prikazati potvrdu i ažurirati prikazanu email adresu
- Kada promjena nije uspješna, sistem mora prikazati odgovarajuću poruku greške

---

### US-65
*Kao korisnik, želim da promijenim lozinku svog profila, kako bih osigurao sigurnost svog naloga.*

**Acceptance Criteria:**
- Kada je korisnik prijavljen i otvori stranicu svog profila, sistem mora prikazati opciju za promjenu lozinke
- Sistem mora zahtijevati unos trenutne lozinke prije postavljanja nove
- Kada korisnik unese pogrešnu trenutnu lozinku, sistem mora prikazati poruku greške i odbiti promjenu
- Nova lozinka mora zadovoljiti minimalne zahtjeve sigurnosti (minimalna dužina)
- Sistem mora zahtijevati potvrdu nove lozinke unosom iste lozinke dva puta
- Kada lozinke ne odgovaraju, sistem mora prikazati odgovarajuću poruku greške
- Kada je promjena uspješna, sistem mora prikazati potvrdu

---

## PB-34 Pregled i uređivanje korisničkih profila (agent)

### US-66
*Kao agent, želim da vidim detalje profila korisnika čiji tiket obrađujem, uključujući historiju tiketa, kako bih imao potpuni kontekst pri rješavanju problema.*

**Acceptance Criteria:**
- Kada agent pregleda tiket, sistem mora prikazati link ili opciju za prikaz profila kreatora tiketa
- Kada agent otvori profil korisnika, sistem mora prikazati osnovne informacije o korisniku (ime, prezime, email, telefon, lokacija)
- Sistem mora prikazati historiju svih tiketa korisnika (otvoreni i zatvoreni)
- Sistem mora omogućiti agentu klik na tiket iz historije radi detaljnog pregleda
- Sistem ne smije dozvoliti klijentu pristup profilima drugih korisnika
- Sistem ne smije prikazivati osjetljive podatke poput lozinke

---

### US-67
*Kao agent, želim da vidim pakete i pretplate korisnika na njegovom profilu, kako bih mogao pružiti relevantniju podršku vezanu za usluge koje korisnik koristi.*

**Acceptance Criteria:**
- Kada agent pregleda profil korisnika, sistem mora prikazati listu aktivnih paketa i pretplata tog korisnika
- Sistem mora prikazati tip paketa (Internet, TV, mobilni) i status pretplate
- Kada korisnik nema aktivnih paketa, sistem mora prikazati odgovarajuću poruku
- Sistem ne smije dozvoliti agentu izmjenu paketa ili pretplata korisnika kroz ovaj prikaz

---

## PB-21 Prikaz paketa i pretplata

### US-68
*Kao korisnik, želim da vidim svoje aktivne pakete i pretplate unutar sistema, kako bih imao uvid u usluge koje koristim bez potrebe za kontaktiranjem podrške.*

**Acceptance Criteria:**
- Kada je korisnik prijavljen i otvori sekciju za pakete i pretplate, sistem mora prikazati listu svih aktivnih paketa
- Sistem mora prikazati tip paketa (Internet, TV, mobilni), naziv i status pretplate
- Sistem mora prikazati osnovne informacije o svakom paketu
- Kada korisnik nema aktivnih paketa, sistem mora prikazati odgovarajuću poruku
- Sistem ne smije prikazivati pakete i pretplate drugih korisnika
- Agenti i tehničari ne smiju imati pristup ovoj sekciji za vlastiti nalog

---

## PB-31 (proširenje) — Sistemske poruke u chatu pri prosljeđivanju

### US-69
*Kao korisnik, želim da vidim u chatu tiketa poruku koja me obavještava kada je tiket proslijeđen drugoj osobi, kako bih imao jasan pregled toka rješavanja bez potrebe za zasebnom provjerom.*

**Acceptance Criteria:**
- Kada agent proslijedi tiket drugom agentu, sistem mora automatski dodati sistemsku poruku u chat tiketa u formatu: `"Tiket je proslijeđen agentu: Ime Prezime"`
- Kada agent proslijedi tiket tehničaru, sistem mora automatski dodati sistemsku poruku u chat tiketa u formatu: `"Tiket je proslijeđen tehničaru: Ime Prezime"`
- Sistemska poruka mora biti vidljiva svim učesnicima (klijent, agent, tehničar, admin) u realnom vremenu bez osvježavanja stranice
- Sistemska poruka mora biti vizualno različita od regularnih poruka — prikazana kao centrirana pill linija, bez avatara i bez oznake autora
- Sistemska poruka ne smije biti prikazana kao poruka korisnika niti smije imati polje za autora

---

## PB-31 / PB-36 / PB-42 (proširenje) — Zajednička dodjela agentu i tehničaru

### US-70
*Kao agent, želim da tiket koji proslijedim tehničaru ostane dodijeljen i meni i tehničaru, kako bih mogao nastaviti pratiti rješavanje tiketa i vidjeti ga u svojim dodijeljenim tiketima i statistikama.*

**Acceptance Criteria:**
- Kada agent proslijedi tiket tehničaru, sistem mora zadržati agenta kao aktivno dodijeljenu osobu na tiketu
- Kada agent proslijedi tiket tehničaru, sistem mora dodati tehničara kao aktivno dodijeljenu osobu na tiketu
- Kada agent otvori listu dodijeljenih tiketa, tiket proslijeđen tehničaru mora biti prikazan i agentu
- Kada tehničar otvori listu dodijeljenih tiketa, isti tiket mora biti prikazan i tehničaru
- Kada tehničar postavi status tiketa na `CLOSURE_REQUESTED` ("Čeka se"), tiket mora ostati prikazan agentu u dodijeljenim tiketima
- Lista dodijeljenih tiketa ne smije po defaultu sakriti tikete u statusu "Čeka se"
- Detalji tiketa moraju prikazati odvojene vrijednosti `Agent: Ime Prezime` i ispod toga `Tehničar: Ime Prezime` kada je tiket proslijeđen tehničaru
- Aktivno dodijeljeni agent i aktivno dodijeljeni tehničar moraju moći učestvovati u closure workflowu u skladu s pravilima sistema
- Kada se tiket proslijedi drugom agentu, prethodni agent više ne smije ostati aktivno dodijeljen osim ako je posljednja dodjela prosljeđivanje tehničaru
- Statistika agenta mora uključiti tikete koji su proslijeđeni tehničaru, ali su i dalje aktivno vezani za tog agenta

---

Napomena: Ovaj Sprint Backlog je živ dokument i ažurira se kroz sprint.
