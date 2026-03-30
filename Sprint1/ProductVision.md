# Template: Product Vision

## Naziv projekta:
Telecom Customer Support System

## Problem koji sistem rješava:
Telekom operateri suočavaju se sa nizom konkretnih problema u svakodnevnom radu, uključujući sporo rješavanje korisničkih prijava, neefikasnu komunikaciju između timova i nepovezanost različitih sistema kao što su CRM, tehnička podrška i sistemi za naplatu. Dodatno, upravljanje kvarovima često nije automatizovano niti prioritetizovano, što dovodi do kašnjenja u rješavanju kritičnih problema. Nedostatak centralizovanih i real-time podataka otežava donošenje poslovnih odluka, dok ručni procesi povećavaju mogućnost grešaka i smanjuju efikasnost. Ovi problemi direktno utiču na kvalitet usluge i zadovoljstvo korisnika. Trenutni proces korisničke podrške u telekomunikacijama karakteriše visoka fragmentacija i neefikasnost, što se manifestuje kroz sedam ključnih identifikovanih barijera:

### Netransparentnost i sporost u rješavanju prijava:
Zbog ručne evidencije i neadekvatnih kanala (e-mail, telefon bez traga), zahtjevi za kvarove (npr. prekid interneta, slab signal) često kasne u obradi ili se potpuno gube. Korisnici nemaju uvid u status svoje prijave, što direktno uzrokuje nezadovoljstvo i prelazak konkurenciji.

### Informacioni silosi (Nepovezani sistemi):
Podaci su rasuti između CRM-a, tehničke podrške i billing sistema. Ovakva nepovezanost uzrokuje dupliranje podataka, greške u informacijama i onemogućava zaposlenicima da imaju "360° pogled" na korisnika i njegove aktivne usluge (internet, TV, mobilna).

### Kolaps interne komunikacije:
Komunikacija između call centra i tehničara na terenu je spora i bez povratne informacije (feedback loop). Informacije se gube u transferu, zbog čega korisnik mora više puta ponavljati isti problem različitim agentima.

### Kritični propusti u upravljanju incidentima:
Bez jasne prioritizacije (hitno vs. nehitno) i automatizacije, kritični kvarovi se tretiraju isto kao rutinski upiti. Nedostatak historije kvarova dovodi do toga da se isti tehnički problemi stalno ponavljaju bez trajnog rješenja.

### Operativno "sljepilo" menadžmenta:
Zbog nedostatka real-time podataka i automatizovanih izvještaja, donosioci odluka nemaju uvid u ključne metrike poput vremena rješavanja (MTTR) ili identifikacije "uskih grla" u procesima, što onemogućava bilo kakvu optimizaciju.

### Neorganizovano upravljanje uslugama:
Budući da podaci o korisniku koji koristi više usluga nisu objedinjeni, često dolazi do grešaka u naplati i lošeg korisničkog iskustva pri pokušaju nadogradnje ili promjene paketa.

### Visoka stopa ljudske greške:
Dominacija ručnih procesa pri dodjeli zadataka tehničarima i ručno praćenje statusa značajno usporavaju rad i povećavaju rizik od administrativnih propusta koji direktno utiču na profitabilnost.

---

## Ciljni korisnici :
Sistem je dizajniran da opsluži tri ključne grupe korisnika, od kojih svaka ima specifične potrebe i izazove unutar telekomunikacionog ekosistema:

### 1. Krajnji korisnici (Klijenti)

Rezidencijalni korisnici (Mlađa populacija): Fokusirani na mobilni internet i streaming. Njihov primarni interes je brzina i stabilnost konekcije, te brza prijava smetnji putem digitalnih kanala.

Rezidencijalni korisnici (Starija populacija): Primarno koriste fiksnu telefoniju i TV usluge. Često se suočavaju sa tehničkim poteškoćama u korištenju digitalnih servisa i zahtijevaju jednostavnu komunikaciju sa podrškom.

Poslovni korisnici (Pravna lica): Zahtijevaju najviši nivo pouzdanosti (SLA) i sigurnosti mreže. Svaki zastoj za njih znači finansijski gubitak, zbog čega im je potreban prioritetni tretman i brza tehnička podrška.

### 2. Operativni timovi (Unutrašnji korisnici)

Korisnička podrška (Agenti L1 i L2): Prva linija kontakta koja prima pozive i poruke. Njihov cilj je integrisan pregled svih informacija o korisniku (billing, CRM, historija kvarova) kako bi izbjegli dupliranje posla i pružili tačne informacije.

Tehničko osoblje (Serviseri i mrežni inženjeri): Terenske ekipe koje rješavaju fizičke kvarove. Sistem im omogućava efikasnu dodjelu radnih naloga, jasnu prioritizaciju (hitni vs. rutinski kvarovi) i pristup podacima o kvaru direktno sa terena.

### 3. Upravljački sektor (Menadžment i administracija)

Supervizori i menadžment: Donosioci strateških odluka koji koriste sistem za praćenje performansi tima (KPI). Njihov fokus je na real-time izvještajima o broju otvorenih tiketa, vremenu rješavanja i kvaliteti pružene usluge, kako bi identifikovali "uska grla" i optimizovali resurse.

---

## Vrijednost sistema:
Implementacija sistema donosi višestruku vrijednost kroz digitalnu transformaciju i optimizaciju operativnih procesa telekom operatera:

Superiorno korisničko iskustvo i lojalnost: Sistem omogućava brže, transparentnije i pouzdanije rješavanje problema. Korisnici više ne moraju nagađati u kojoj je fazi njihov zahtjev, jer imaju uvid u realnom vremenu. Ovakav pristup direktno smanjuje stopu odlaska korisnika (churn rate) i povećava njihovo povjerenje, posebno u kritičnim situacijama prekida usluge.

Operativna efikasnost i centralizacija : Eliminisanjem informacionih silosa (povezivanjem podrške, tehničke službe i billinga), sistem sprečava dupliranje podataka i administrativne greške. Centralizacija komunikacije dramatično skraćuje srednje vrijeme rješavanja kvara (MTTR - Mean Time To Resolution). Automatizacija dodjele zadataka i pametna prioritizacija omogućavaju timovima da urade više sa manje manuelnog angažmana.

Preventivno održavanje i proaktivna podrška: Stvaranjem centralizovane baze znanja o najčešćim kvarovima i incidentima, organizacija prelazi iz reaktivnog u proaktivni mod rada. Analiza historijskih podataka omogućava identifikaciju slabih tačaka u mrežnoj infrastrukturi, što pomaže u planiranju preventivnog održavanja prije nego što dođe do masovnih pritužbi.

Strateško upravljanje zasnovano na podacima: Menadžment dobija moćan alat za analitiku i izvještavanje u realnom vremenu. Umjesto oslanjanja na procjene, odluke se donose na osnovu egzaktnih podataka o performansama sistema, "uskim grlima" u procesima i kvalitetu usluge. Ovo direktno doprinosi dugoročnoj skalabilnosti, smanjenju operativnih troškova i jačanju konkurentske pozicije na tržištu.

---

## Scope MVP verzije
Cilj MVP-a je eliminacija neefikasnog upravljanja zahtjevima kroz centralizovano evidentiranje, praćenje i rješavanje kvarova uz minimalan, ali moćan skup funkcionalnosti:

### Upravljanje korisničkim nalozima:
Registracija i autentifikacija za krajnje korisnike i zaposlenike (Agenti, Tehničari, Menadžment).
Osnovni profili sa podacima o uslugama koje korisnik koristi (Internet, TV, Mobilna).

### Portal za prijavu i evidenciju kvarova:
Digitalni formular za korisnike sa mogućnošću odabira kategorije (npr. nestanak interneta, slab signal, TV smetnje).
Automatsko generisanje jedinstvenog ID-a tiketa za svaki zahtjev.

### Centralizovani Ticketing Dashboard & Workflow:
Pregled svih aktivnih prijava na jednom mjestu za korisničku podršku.
Praćenje životnog ciklusa tiketa: Vizuelni prikaz statusa (Zaprimljeno, U obradi, Na čekanju, Riješen, Arhiviran).
Pametna kategorizacija i prioritizacija: Automatsko razvrstavanje po tipu usluge i prepoznavanje prioritetnih (npr. Poslovni korisnici).

### Sistem komunikacije i dodjele:
Dodjela zadataka: Ručno ili jednostavno automatsko prosljeđivanje tiketa slobodnim agentima ili tehničkom osoblju.
Sistem komentara: Interna komunikacija između timova i mogućnost direktnog feedbacka prema korisniku.
Modul za tehničare: Poseban pregled radnih naloga za terenske ekipe sa detaljnim opisom kvara.

---

## Šta ne ulazi u MVP
Kako bi se razvoj primarno fokusirao na rješavanje kritičnih problema i osigurala brža isporuka stabilnog rješenja, sljedeće funkcionalnosti su svjesno izostavljene iz prve faze:

Mobilna aplikacija (Native): MVP verzija će biti isključivo web-bazirana (optimizovana za desktop i mobile browser-e). Posebne aplikacije za iOS i Android platforme za korisnike i terenske tehničare planirane su za kasnije faze.

AI Chatbot i Automatizacija Odgovora: Neće biti implementirana vještačka inteligencija za automatsko odgovaranje na upite. Sva komunikacija u ovoj fazi odvija se direktno između agenata i korisnika.

Napredni sistemi notifikacija: Funkcionalnosti poput automatizovanih SMS-ova, push notifikacija na telefonima ili kompleksnih e-mail kampanja ostaju van opsega. Obavještenja će biti ograničena na osnovne statusne promjene unutar samog sistema.

---

## Ključna ograničenja i pretpostavke

### Ograničenja (Constraints)
Vremensko i razvojno ograničenje: Razvoj MVP-a strogo je limitiran trajanjem sprinta. Ovo zahtijeva beskompromisan fokus na osnovne funkcionalnosti i odlaganje bilo kakve "nice-to-have" kompleksnosti.

Tehnička arhitektura: Sistem je inicijalno ograničen na web platformu. Isključena je nativna mobilna podrška i duboka integracija sa eksternim billing ili CRM sistemima u ovoj fazi (sistem funkcioniše kao standalone rješenje).

Podaci i validacija: Zbog nedostupnosti realnih produkcionih podataka, za razvoj se koriste isključivo testni i simulirani setovi podataka. To može uticati na preciznost predviđanja opterećenja sistema u realnim uslovima.

Sigurnosna i pravna regulativa: Obrada podataka o korisnicima i njihovim kvarovima podliježe strogoj usklađenosti sa GDPR-om. Pristup osjetljivim informacijama je ograničen prema ulogama (Role-based access control), što utiče na dizajn korisničkog interfejsa.

### Pretpostavke (Assumptions)
Ažurnost operativaca: Pretpostavlja se da će tehničko osoblje i agenti redovno ažurirati statuse prijava u realnom vremenu te da će sistem prihvatiti kao svoj primarni alat za rad.

Kvalitet unosa: Pretpostavlja se da su osnovni podaci o korisnicima (uvezeni u sistem) tačni i validni za identifikaciju usluga.

Infrastrukturna podrška: Pretpostavlja se da tehničko osoblje na terenu ima stabilan pristup mobilnom internetu kako bi mogli slati povratne informacije izravno sa lokacije kvara.

Stabilnost zahtjeva: Razvoj se bazira na pretpostavci da neće biti radikalnih promjena u poslovnim zahtjevima tokom trajanja MVP ciklusa.

Interno korištenje: Sistem je dizajniran za upotrebu unutar jedne telekom organizacije, bez potrebe za multitenancy podrškom ili kompleksnim eksternim API integracijama u prvoj fazi.