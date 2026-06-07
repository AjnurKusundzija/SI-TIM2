# Sprint Backlog – Sprint 11

Sprint backlog treba biti realan i povezan sa sprint ciljem.

---

## Veza sa sprint ciljem

Finalizirati sistem kroz implementaciju CSV exporta izvještaja, Linked Tickets funkcionalnosti, SLA praćenja i upozorenja, bulk akcija na tiketima te proširenja autentikacije putem broja telefona.

---

## Stavke sprint backloga

| ID | Naziv zadatka ili storyja | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|---|---|---|---|---|---|
| SB-01 | PB-46 Export izvještaja | US-112 | Uma | Done | Client-side CSV generisanje za svih 7 tipova izvještaja; metadata header (naziv, period, datum exporta); UTF-8 BOM za Excel; loading spinner tokom fetha; uvijek aktivan Export button |
| SB-02 | PB-64 Linked Tickets — veza između tiketa | US-113, US-114 | [Odgovorna osoba] | Backlog | Bidirekciona veza; tipovi veze (duplikat, nastavak, vezano uz); prevencija cikličnih veza i samopovezivanja; samo agenti i tehničari |
| SB-03 | PB-65 SLA praćenje i upozorenja | US-115, US-116 | [Odgovorna osoba] | Backlog | SLA rokovi po prioritetu; boja-kodirani countdown; notifikacije za blizak rok i breach; evidencija u historiji tiketa; SLA breach counter na dashboardu |
| SB-04 | PB-66 Bulk akcije na tiketima | US-117, US-118 | [Odgovorna osoba] | Backlog | Checkboxes na listi tiketa; bulk zatvaranje/prioritet/dodjela/prosljeđivanje; potvrda za destruktivne akcije; sažetak rezultata; samo admin i agenti |
| SB-05 | PB-67 Login via broj telefona | US-119 | [Odgovorna osoba] | Backlog | Međunarodni format (+387...); jedinstvenost broja; dual login email/telefon; validacija pri registraciji |

---

# Detaljni User Stories (US)

---

## PB-46 Export izvještaja

### US-112
*Kao administrator, želim eksportovati izvještaj u CSV formatu koristeći trenutno odabrane parametre, kako bih mogao analizirati podatke offline ili podijeliti ih sa timom.*

**Acceptance Criteria:**
- Export dugme na stranici izvještaja mora biti uvijek aktivno (nije disabled)
- Klik na Export triggera fetch sa trenutno odabranim tipom izvještaja i periodom iz forme (dropdown-a), bez potrebe da korisnik prethodno klikne „Generiši"
- Ako nije odabran nijedan tip izvještaja, sistem koristi TICKET_COUNT kao podrazumijevani tip
- CSV fajl mora sadržavati metadata header sa: nazivom izvještaja, periodom i datumom exporta, te prazan separator red prije tabelarnih podataka
- Fajl se automatski preuzima sa nazivom `report.csv`
- CSV fajl mora sadržavati UTF-8 BOM za ispravno otvaranje u Microsoft Excelu na Windows platformi
- Export mora raditi za svih 7 tipova izvještaja: TICKET_COUNT, TICKET_STATUS, PROBLEM_TYPE, TEAM_WORKLOAD, USER_RATINGS, FIRST_RESPONSE, AVG_RESOLUTION
- Ako custom period validacija ne prođe (datum početka > datum kraja), sistem prikazuje grešku i ne pokreće export
- Tokom fetcha dugme prikazuje loading spinner i onemogućeno je za ponovni klik
- Ako API poziv ne uspije, Export dugme se vraća u normalno stanje bez rušenja stranice

---

## PB-64 Linked Tickets — veza između tiketa

### US-113
*Kao agent ili tehničar, želim kreirati bidirekcionalnu vezu između tiketa koji se odnose na isti problem ili su na drugi način međusobno zavisni, kako bih mogao pratiti kontekst složenih slučajeva koji obuhvataju više tiketa.*

**Acceptance Criteria:**
- Kada je korisnik agent ili tehničar, sistem mora prikazati opciju za dodavanje veze u sekciji detalja tiketa
- Korisnik mora moći odabrati tip veze: „Duplikat od", „Nastavak od" ili „Vezano uz"
- Korisnik mora moći pretražiti i odabrati ciljni tiket po ID-u ili naslovu
- Kreiranje veze mora biti bidirekciono — oba tiketa prikazuju vezu
- Sistem mora spriječiti samopovezivanje (tiket ne može biti vezan za sebe)
- Sistem mora spriječiti ciklične veze (A→B→A)
- Sistem ne smije prikazati opciju za dodavanje veza klijentima
- Kreiranje veze mora biti evidentirano u historiji tiketa

---

### US-114
*Kao agent, tehničar ili administrator, želim pregledati i ukloniti veze između tiketa direktno iz prikaza detalja tiketa, kako bih imao potpuni kontekst slučaja i mogao upravljati vezama.*

**Acceptance Criteria:**
- Sekcija detalja tiketa mora prikazivati listu svih linked tiketa sa: tipom veze, ID-jem tiketa, naslovom i trenutnim statusom
- Klik na linked tiket mora navigirati na prikaz tog tiketa
- Agent ili tehničar može ukloniti vezu; uklanjanje je bidirekciono (uklanja se s oba tiketa)
- Administrator može pregledati veze ali ne može uklanjati (read-only prikaz za admina)
- Sistem ne smije prikazati sekciju linked tiketa klijentima
- Ako tiket nema nijednu vezu, sekcija ne prikazuje praznu listu nego je skrivena ili prikazuje poruku „Nema vezanih tiketa"

---

## PB-65 SLA praćenje i upozorenja

### US-115
*Kao agent ili administrator, želim vidjeti preostalo SLA vrijeme za svaki tiket na osnovu prioriteta i tipa problema, kako bih mogao prioritizirati rad i spriječiti prekoračenja rokova.*

**Acceptance Criteria:**
- Sistem mora definisati SLA rokove po prioritetu: CRITICAL — 2 sata, HIGH — 8 sati, NORMAL — 24 sata, LOW — 72 sata
- Svaki tiket mora prikazivati preostalo SLA vrijeme u satima i minutama
- SLA se računa od datuma kreiranja tiketa
- Sistem mora prikazivati SLA status s boja-kodiranjem: zelena (>50% preostalo), žuta (20–50% preostalo), crvena (<20% ili prekoračeno)
- Sistem mora evidentirati SLA breach u historiji tiketa s tačnim timestampom prekoračenja
- Lista tiketa mora prikazivati SLA indikator uz svaki tiket za agente i administratore
- Zatvoreni tiketi ne prikazuju SLA countdown

---

### US-116
*Kao agent ili administrator, želim primiti upozorenje kada SLA rok ističe ili je prekoračen, kako bih mogao odmah reagovati i spriječiti negativan uticaj na kvalitet usluge.*

**Acceptance Criteria:**
- Sistem mora obavijestiti dodijeljenog agenta kada SLA ima manje od 20% preostalih vremena (notifikacija putem postojećeg NotificationHub-a)
- Sistem mora poslati zasebnu notifikaciju kada SLA bude prekoračen
- Administratorski dashboard mora prikazivati ukupan broj tiketa s prekoračenim SLA
- Broj prekoračenih SLA tiketa mora biti vidljiv u admin Sidebar status chipu uz postojeće alertove
- Sistem ne smije dodjeljivati nove tikete agentima čiji bi workload uzrokovao SLA prekoračenje
- SLA upozorenja moraju biti vizualno jasno razlikovana od regularnih notifikacija

---

## PB-66 Bulk akcije na tiketima

### US-117
*Kao agent ili administrator, želim odabrati više tiketa odjednom koristeći checkboxe na listi tiketa, kako bih mogao izvršiti akcije na više tiketa istovremeno.*

**Acceptance Criteria:**
- Lista tiketa mora prikazivati checkbox uz svaki red za agente i administratore
- „Odaberi sve" checkbox mora biti dostupan za odabir/poništenje odabira svih vidljivih tiketa
- Broj odabranih tiketa mora biti prikazan kada je odabran barem jedan
- Toolbar za bulk akcije mora se prikazati kada je odabran barem jedan tiket
- Klijenti ne smiju vidjeti checkboxe niti bulk akcijske kontrole
- Odabrani tiketi moraju biti vizualno istaknuti u listi

---

### US-118
*Kao agent ili administrator, želim izvršiti bulk akcije nad odabranim tiketima (zatvaranje, promjena prioriteta, dodjela agentu, prosljeđivanje timu), kako bih efikasnije upravljao većim brojem tiketa.*

**Acceptance Criteria:**
- Dostupne bulk akcije moraju uključivati: zatvaranje tiketa, promjenu prioriteta, dodjelu agentu i prosljeđivanje timu
- Sistem mora tražiti potvrdu prije destruktivnih operacija (zatvaranje, prosljeđivanje)
- Nakon izvršavanja, sistem mora prikazati sažetak: broj uspješnih i neuspješnih operacija
- Ako neki tiketi ne mogu biti obrađeni (npr. već zatvoreni), sistem mora prikazati koji su i zašto
- Bulk akcije moraju poštovati ista autorizacijska pravila kao individualne akcije na tiketima
- Nakon bulk akcije, lista tiketa mora biti osvježena bez potrebe za ručnim reloadom

---

## PB-67 Login via broj telefona

### US-119
*Kao klijent, želim se prijaviti u sistem koristeći broj telefona umjesto emaila, kako bih imao više fleksibilnosti u načinu pristupa i mogao koristiti isti identifikator kao što je naveden u ugovoru.*

**Acceptance Criteria:**
- Login forma mora prihvatiti broj telefona u međunarodnom formatu (+387...) kao alternativu email adresi
- Sistem mora validirati jedinstvenost broja telefona pri registraciji i ažuriranju profila
- Klijent se može prijaviti i emailom i brojem telefona uz istu lozinku
- Sistem ne smije dozvoliti prijavu brojem telefona koji ne pripada nijednom nalogu
- Postojeći korisnici bez broja telefona moraju i dalje moći koristiti email za prijavu
- Polje za unos mora vizualno naznačiti da se može unijeti email ili telefon

---

Napomena: Ovaj Sprint Backlog je živ dokument i ažurira se kroz sprint.
