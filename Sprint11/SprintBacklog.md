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
| SB-03 | PB-65 SLA praćenje i upozorenja | US-115, US-116 | Uma | Done | SLA rokovi po prioritetu; boja-kodirani countdown; notifikacije za blizak rok i breach; evidencija u historiji tiketa; SLA breach counter na dashboardu |
| SB-05 | PB-67 Login via broj telefona | US-119 | Uma | Done | Međunarodni format (+387...); dual login email/telefon; `EmailOrBiHPhoneAttribute` validacija; `GetByPhoneAsync` lookup u `AuthService`; placeholder i label na Login formi |

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
- SLA upozorenja moraju biti vizualno jasno razlikovana od regularnih notifikacija

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
