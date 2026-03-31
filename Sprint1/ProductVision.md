# Template: Product Vision

## Naziv projekta:
Telecom Customer Support System

## Problem koji sistem rješava:
Telekom operateri suočavaju se sa nizom konkretnih problema u svakodnevnom radu, uključujući sporo rješavanje korisničkih prijava, neefikasnu komunikaciju između timova i nepovezanost različitih sistema kao što su CRM, tehnička podrška i sistemi za naplatu. Dodatno, upravljanje kvarovima često nije automatizovano niti prioritetizovano, što dovodi do kašnjenja u rješavanju kritičnih problema. Nedostatak centralizovanih i real-time podataka otežava donošenje poslovnih odluka, dok ručni procesi povećavaju mogućnost grešaka i smanjuju efikasnost. Ovi problemi direktno utiču na kvalitet usluge i zadovoljstvo korisnika. Trenutni proces korisničke podrške u telekomunikacijama karakteriše visoka neefikasnost, što se prikazuje kroz sedam ključnih problema:

### Netransparentnost i sporost u rješavanju prijava:
Zbog ručne evidencije i neadekvatnih kanala (e-mail, telefon bez traga), zahtjevi za kvarove (npr. prekid interneta, slab signal) često kasne u obradi ili se potpuno gube. Korisnici nemaju uvid u status svoje prijave, što direktno uzrokuje nezadovoljstvo i prelazak konkurenciji.


### Kolaps interne komunikacije:
Komunikacija između call centra i tehničara na terenu je spora i bez povratne informacije (feedback loop). Informacije se gube u transferu, zbog čega korisnik mora više puta ponavljati isti problem različitim agentima.


### Kritični propusti u upravljanju incidentima:
Bez jasne prioritizacije (hitno vs. nehitno) i automatizacije, kritični kvarovi se tretiraju isto kao rutinski upiti. Nedostatak historije kvarova dovodi do toga da se isti tehnički problemi stalno ponavljaju bez trajnog rješenja.


### Visoka stopa ljudske greške:
Dominacija ručnih procesa pri dodjeli zadataka tehničarima i ručno praćenje statusa značajno usporavaju rad i povećavaju rizik od administrativnih propusta koji direktno utiču na profitabilnost.


### Nedostatak uvida u status prijave unutar aplikacije:
Korisnici nemaju mogućnost da unutar aplikacije provjere status svojih prijava (npr. zaprimljeno, u obradi, riješeno). Zbog toga su primorani ponovo kontaktirati korisničku podršku kako bi dobili informacije, što dodatno opterećuje sistem.


### Nedovoljna automatizacija i skalabilnost sistema:
Sistem korisničke podrške ne može lako podnijeti veliki broj zahtjeva odjednom. Kada ima puno korisnika (npr. kvar u mreži), dolazi do zagušenja, sporog odgovora i pada kvaliteta usluge. Nedostatak automatizacije znači da se sve radi ručno, što dodatno usporava proces i stvara opterećenje za zaposlenike.


---

## Ciljni korisnici :
Sistem je dizajniran da opsluži tri ključne grupe korisnika, od kojih svaka ima specifične potrebe i izazove unutar telekomunikacionog ekosistema:

### 1. Krajnji korisnici (Klijenti)

Rezidencijalni korisnici (Mlađa populacija): Fokusirani na mobilni internet i streaming. Njihov primarni interes je brzina i stabilnost konekcije, te brza prijava smetnji putem digitalnih kanala.

Rezidencijalni korisnici (Starija populacija): Primarno koriste fiksnu telefoniju i TV usluge. Često se suočavaju sa tehničkim poteškoćama u korištenju digitalnih servisa i zahtijevaju jednostavnu komunikaciju sa podrškom.

Poslovni korisnici (Pravna lica): Zahtijevaju najviši nivo pouzdanosti (SLA - Service Level Agreement) i sigurnosti mreže. Svaki zastoj za njih znači finansijski gubitak, zbog čega im je potreban prioritetni tretman i brza tehnička podrška.

### 2. Operativni timovi (Unutrašnji korisnici)

Korisnička podrška (Agenti): Prva linija kontakta koja prima pozive i poruke. Cilj je imati sve podatke o korisniku na jednom mjestu radi bržeg i tačnijeg rada. Takodjer, agenti služe kao prvi paket za osnovna pitanja oko informacija o paketima, uslugama i slično.

Tehničko osoblje (Serviseri i mrežni inženjeri): Terenske ekipe koje rješavaju fizičke kvarove. Sistem im omogućava efikasnu dodjelu radnih naloga, jasnu prioritizaciju (hitni vs. rutinski kvarovi) i pristup podacima o kvaru direktno sa terena.

### 3. Upravljački sektor (Menadžment i administracija)

Supervizori i menadžment: Donosioci odluka koriste sistem za praćenje rada tima. Fokusiraju se na broj tiketa, vrijeme rješavanja i kvalitet usluge kako bi prepoznali probleme i efikasnije rasporedili resurse.

---

## Vrijednost sistema:
Implementacija sistema donosi višestruku vrijednost kroz digitalnu transformaciju i optimizaciju operativnih procesa telekom operatera:

Korisničko iskustvo: Sistem omogućava brže, transparentnije i pouzdanije rješavanje problema. Korisnici više ne moraju nagađati u kojoj je fazi njihov zahtjev, jer imaju uvid u realnom vremenu. Ovakav pristup direktno smanjuje stopu odlaska korisnika (churn rate) i povećava njihovo povjerenje, posebno u kritičnim situacijama prekida usluge.

Operativna efikasnost i centralizacija: Centralizacija komunikacije skraćuje srednje vrijeme rješavanja kvara (MTTR - Mean Time To Resolution). Automatizacija dodjele zadataka i prioritizacija omogućavaju timovima da urade više sa manje resursa.

Preventivno održavanje i podrška: Uspostavljanjem centralizovane baze znanja o najčešćim kvarovima i incidentima, organizacija može bolje razumjeti ponavljajuće probleme. Analiza prethodnih podataka omogućava prepoznavanje slabih tačaka u sistemu i planiranje preventivnog održavanja prije nego što dođe do većih problema.

Strateško upravljanje zasnovano na podacima: Menadžment dobija alat za analitiku i izvještavanje u realnom vremenu. Umjesto oslanjanja na procjene, odluke se donose na osnovu tačnih podataka o performansama sistema i kvalitetu usluge. Ovo direktno doprinosi dugoročnoj skalabilnosti, smanjenju operativnih troškova i jačanju konkurentske pozicije na tržištu.

---

## Scope MVP verzije
Cilj MVP-a je eliminacija neefikasnog upravljanja zahtjevima kroz centralizovano evidentiranje, praćenje i rješavanje kvarova uz minimalan, ali moćan skup funkcionalnosti:

### Upravljanje korisničkim nalozima:
Registracija i autentifikacija za krajnje korisnike i zaposlenike (Klijenti, Agenti, Tehničari, Menadžment).
Osnovni profili sa podacima o uslugama koje korisnik koristi (Internet, TV, Mobilna).

### Portal za prijavu i evidenciju kvarova:
Digitalni formular za korisnike sa mogućnošću odabira kategorije (npr. nestanak interneta, slab signal, TV smetnje).
Automatsko generisanje jedinstvenog ID-a tiketa za svaki zahtjev.

### Centralizovani Ticketing Dashboard & Workflow:
Pregled svih aktivnih prijava na jednom mjestu za korisničku podršku.
Praćenje životnog ciklusa tiketa: Vizuelni prikaz statusa (Zaprimljeno, U obradi, Na čekanju, Riješen, Arhiviran).
Kategorizacija i prioritizacija: Automatsko razvrstavanje po tipu usluge i prepoznavanje prioritetnih (npr. Poslovni korisnici).

### Sistem komunikacije i dodjele:
Dodjela zadataka: Ručno ili jednostavno automatsko prosljeđivanje tiketa slobodnim agentima ili tehničkom osoblju.
Sistem komunikacije: Interna komunikacija između timova i mogućnost direktnog feedbacka prema korisniku.
Modul za tehničare: Poseban pregled radnih naloga za terenske ekipe sa detaljnim opisom kvara.

### Sistem izvještaja:
Generisanje različitih vrsta izvještaja i statistika.

---

## Šta ne ulazi u MVP
Kako bi se razvoj primarno fokusirao na rješavanje kritičnih problema i osigurala brža isporuka stabilnog rješenja, sljedeće funkcionalnosti su svjesno izostavljene iz prve faze:

Sistem za online plaćanje
Integracija sa payment gateway sistemima i mogućnost online plaćanja računa neće biti dio MVP verzije. Fokus ostaje isključivo na upravljanju korisničkom podrškom i rješavanju kvarova, dok će se finansijske funkcionalnosti razmatrati u kasnijim fazama razvoja.

Višejezična podrška (Multilingual support)
MVP će podržavati samo jedan jezik. Implementacija višejezičnosti (npr. engleski, njemački i drugi jezici) planirana je za naredne faze kako bi se omogućila šira dostupnost sistema različitim korisničkim grupama.

Napredna statistika i AI analitika
Iako MVP uključuje osnovne izvještaje, napredna analitika poput prediktivnih modela, AI-driven uvida (npr. predviđanje kvarova, analiza ponašanja korisnika, automatska optimizacija resursa) neće biti implementirana u ovoj fazi. Ove funkcionalnosti zahtijevaju kompleksniju obradu podataka i biće dio budućih unapređenja sistema.

Napredni sistemi notifikacija
Funkcionalnosti poput automatizovanih SMS-ova, push notifikacija na telefonima ili kompleksnih e-mail kampanja ostaju van opsega. Obavještenja će biti ograničena na osnovne statusne promjene unutar samog sistema.

---

## Ključna ograničenja i pretpostavke

### Ograničenja (Constraints)
Vremensko i razvojno ograničenje: Razvoj MVP-a strogo je limitiran trajanjem sprinta. Ovo zahtijeva beskompromisan fokus na osnovne funkcionalnosti i odlaganje bilo kakve "nice-to-have" kompleksnosti.

Tehnička arhitektura: Sistem je inicijalno ograničen na web platformu. Isključena je mobilna podrška i duboka integracija sa eksternim billing ili CRM sistemima u ovoj fazi (sistem funkcioniše kao standalone rješenje). Također, sistem mora podržavati rad u realnom vremenu, što znači da se sve promjene (npr. status tiketa ili ažuriranja od strane tehničara) odmah prikazuju svim korisnicima bez osvježavanja stranice. Ovo zahtijeva stabilnu dvosmjernu komunikaciju (npr. WebSocket), podršku za veći broj istovremenih konekcija i osnovne mehanizme za očuvanje konzistentnosti podataka i reconnect u slučaju prekida veze.

Podaci i validacija: Zbog nedostupnosti realnih produkcionih podataka, za razvoj se koriste isključivo testni i simulirani setovi podataka. To može uticati na preciznost predviđanja opterećenja sistema u realnim uslovima.

Sigurnosna i pravna regulativa: Sigurnost i pravna regulativa: Obrada korisničkih podataka mora biti u skladu sa GDPR pravilima (Zakonska odluka o zaštiti podataka – zakon koji štiti privatnost i lične podatke korisnika). Pristup osjetljivim informacijama je ograničen prema korisničkim ulogama, što utiče na način na koji je sistem dizajniran.

### Pretpostavke (Assumptions)
Ažurnost tehničara: Pretpostavlja se da će tehničko osoblje i agenti redovno ažurirati statuse prijava u realnom vremenu te da će sistem prihvatiti kao svoj primarni alat za rad.

Infrastrukturna podrška: Pretpostavlja se da tehničko osoblje na terenu ima stabilan pristup mobilnom internetu kako bi mogli slati povratne informacije izravno sa lokacije kvara.

Stabilnost zahtjeva: Razvoj se bazira na pretpostavci da neće biti radikalnih promjena u poslovnim zahtjevima tokom trajanja MVP ciklusa.

Interno korištenje: Sistem je dizajniran za upotrebu unutar jedne telekom organizacije, bez potrebe za multitenancy podrškom ili kompleksnim eksternim API integracijama u prvoj fazi.
