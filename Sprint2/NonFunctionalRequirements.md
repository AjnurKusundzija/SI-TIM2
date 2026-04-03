# Non-Functional Requirements (NFR)

## Uvod

Ovaj dokument definiše nefunkcionalne zahtjeve (NFR) za **Telecom Customer Support System**. Nefunkcionalni zahtjevi opisuju kvalitativne karakteristike sistema – odnosno **kako** sistem treba da se ponaša, a ne **šta** treba da radi. Dokument treba biti **živ dokument i redovno ažuriran** u skladu sa razvojem projekta i povratnim informacijama od stakeholdera.

Nefunkcionalni zahtjevi su organizovani u tri grupe prema Sommerville-u:

- **Zahtjevi proizvoda** – opisuju kvalitativne karakteristike samog softverskog proizvoda (efikasnost, pouzdanost, upotrebljivost, portabilnost)
- **Organizacioni zahtjevi** – proizilaze iz internih standarda, politika i procedura organizacije (npr. standardi kodiranja, metodologija razvoja)
- **Vanjski zahtjevi** – dolaze iz vanjskog okruženja i regulatornih okvira (npr. GDPR, zakonske obaveze, interoperabilnost)



---

### Legenda za oznake

#### Zahtjevi proizvoda

| Oznaka | Kategorija     | Opis                                                                              |
| :----: | :------------- | :-------------------------------------------------------------------------------- |
| `EF`   | Efikasnost     | Zahtjevi vezani za brzinu odziva, propusnost i iskorištenost resursa sistema      |
| `PO`   | Pouzdanost     | Zahtjevi vezani za dostupnost, oporavak od greške i konzistentnost podataka       |
| `UP`   | Upotrebljivost | Zahtjevi vezani za intuitivnost, pristupačnost i korisničko iskustvo              |
| `PT`   | Portabilnost   | Zahtjevi vezani za prenosivost sistema na različite platforme i okruženja         |

#### Organizacioni zahtjevi

| Oznaka | Kategorija       | Opis                                                                              |
| :----: | :--------------- | :-------------------------------------------------------------------------------- |
| `OZ`   | Organizacioni    | Zahtjevi koji proizilaze iz internih standarda, politika i procesa organizacije   |

#### Vanjski zahtjevi

| Oznaka | Kategorija   | Opis                                                                                   |
| :----: | :----------- | :------------------------------------------------------------------------------------- |
| `VZ`   | Vanjski       | Zahtjevi koji dolaze iz regulatornih okvira, zakona i eksternih standarda (npr. GDPR) |

---

- **Prioritet zahtjeva:** 1, 2, 3, 4, 5 (1 je najbitnije, 5 je najmanje bitno)
- **Status:** Identifikovan, U analizi, Potvrđen, Implementiran, Zatvoren

---

## Tabelarni prikaz NFR zahtjeva

### Zahtjevi proizvoda

| ID                  | Naziv zahtjeva                                         | Kategorija     | Prioritet | Status        |
| :------------------ | :----------------------------------------------------- | :------------- | :-------: | :------------ |
| [NFR-01](#nfr-01)   | Vrijeme odziva pri učitavanju stranica                 | Efikasnost     |     1     | Identifikovan |
| [NFR-02](#nfr-02)   | Real-time ažuriranje statusa tiketa                    | Efikasnost     |     1     | Identifikovan |
| [NFR-03](#nfr-03)   | Propusnost – broj istovremenih korisnika               | Efikasnost     |     2     | Identifikovan |
| [NFR-04](#nfr-04)   | Brzina kreiranja tiketa                                | Efikasnost     |     2     | Identifikovan |
| [NFR-05](#nfr-05)   | Dostupnost sistema (uptime)                            | Pouzdanost     |     1     | Identifikovan |
| [NFR-06](#nfr-06)   | Oporavak sistema nakon greške                          | Pouzdanost     |     1     | Identifikovan |
| [NFR-07](#nfr-07)   | Konzistentnost podataka pri prekidu veze               | Pouzdanost     |     1     | Identifikovan |
| [NFR-08](#nfr-08)   | Automatski WebSocket reconnect                         | Pouzdanost     |     2     | Identifikovan |
| [NFR-09](#nfr-09)   | Intuitivnost interfejsa za agente i tehničare          | Upotrebljivost |     1     | Identifikovan |
| [NFR-10](#nfr-10)   | Jasne i razumljive poruke o greškama                   | Upotrebljivost |     2     | Identifikovan |
| [NFR-11](#nfr-11)   | Responzivan dizajn za desktop i tablet                 | Upotrebljivost |     2     | Identifikovan |
| [NFR-12](#nfr-12)   | Pristupačnost interfejsa za stariju populaciju         | Upotrebljivost |     3     | Identifikovan |
| [NFR-13](#nfr-13)   | Podrška za savremene web browsere                      | Portabilnost   |     1     | Identifikovan |
| [NFR-14](#nfr-14)   | Nezavisnost od operativnog sistema na serveru          | Portabilnost   |     2     | Identifikovan |
| [NFR-15](#nfr-15)   | Apstrakcija sloja baze podataka                        | Portabilnost   |     3     | Identifikovan |

---

## Detalji NFR zahtjeva

### Zahtjevi proizvoda

---

#### NFR-01

- **Naziv zahtjeva:** Vrijeme odziva pri učitavanju stranica
- **Kategorija:** Efikasnost (`EF`)
- **Opis zahtjeva:** Sve stranice sistema (dashboard, lista tiketa, detalji tiketa) moraju se u potpunosti učitati i biti interaktivne u roku od **2 sekunde** pri normalnom opterećenju sistema koji podrazumijeva do 50 istovremenih korisnika i stabilnoj mrežnoj vezi. Ovo se odnosi na sve korisničke uloge: klijente, agente, tehničare na terenu i administratore.
- **Kako će se provjeravati:** Testiranje perfomansi radit će se alatima poput Lighthouse ili k6, mjerenjem vremena učitavanja ključnih stranica u simuliranom okruženju s 50 istovremenih korisnika. Rezultati se dokumentuju u okviru sprint reviewa.
- **Prioritet:** 1
- **Napomena:** Zahtjev je direktno vezan uz Product Vision – sporo učitavanje stranica povećava frustraciju korisnika i smanjuje efikasnost agenata tokom obrade tiketa. Mjeri se pod normalnim, ne vršnim opterećenjem.

---

#### NFR-02

- **Naziv zahtjeva:** Stvarno (real-time) ažuriranje statusa tiketa
- **Kategorija:** Efikasnost (`EF`)
- **Opis zahtjeva:** Svaka promjena statusa tiketa (npr. s "Zaprimljeno" na "U obradi") mora biti vidljiva svim aktivnim korisnicima koji prate taj tiket u roku od **1 sekunde** od trenutka promjene, bez potrebe za ručnim osvježavanjem stranice. Komunikacija se ostvaruje putem WebSocket protokola.
- **Kako će se provjeravati:** Manuelno testiranje s dva aktivna korisnička naloga istovremeno otvorena na različitim preglednicima, mjerenjem kašnjenja promjene statusa. Automatizovani test simulira više istovremenih WebSocket konekcija.
- **Prioritet:** 1
- **Napomena:** Sistem mora podržavati rad u realnom vremenu. Implementacija zahtijeva stabilnu WebSocket infrastrukturu.

---

#### NFR-03

- **Naziv zahtjeva:** Propusnost – broj istovremenih korisnika
- **Kategorija:** Efikasnost (`EF`)
- **Opis zahtjeva:** Sistem mora ostati funkcionalan i stabilan pri radu s najmanje **100 istovremenih aktivnih korisnika** bez degradacije performansi. Pod degradacijom se podrazumijeva povećanje vremena odziva za više od 50% u odnosu na normalne uslove ili pojava grešaka na korisničkom interfejsu.
- **Kako će se provjeravati:** Load testiranje alatom k6 ili sličnim, uz simulaciju 100 istovremenih sesija tokom perioda od 5 minuta. Mjeri se prosječno i maksimalno vrijeme odziva, kao i stopa grešaka (error rate).
- **Prioritet:** 2
- **Napomena:** Relevantno posebno u scenarijima masovnih kvarova u mreži, gdje veći broj korisnika simultano prijavljuje problem. Product Vision navodi nedovoljnu skalabilnost kao ključan problem trenutnog sistema.

---

#### NFR-04

- **Naziv zahtjeva:** Brzina kreiranja tiketa
- **Kategorija:** Efikasnost (`EF`)
- **Opis zahtjeva:** Nakon što korisnik popuni i pošalje formular za prijavu kvara, sistem mora kreirati tiket, dodijeliti mu jedinstven ID i prikazati potvrdu korisniku u roku od **3 sekunde**. Zahtjev se odnosi na normalne mrežne uslove i standardno opterećenje baze podataka.
- **Kako će se provjeravati:** Manuelno i automatizovano testiranje koristeći Selenium, slanjem zahtjeva za kreiranje tiketa i mjerenjem trajanja od klika na "Pošalji" do prikaza potvrde s generisanim ID-om tiketa.
- **Prioritet:** 2
- **Napomena:** Kreiranje tiketa je centralna funkcionalnost MVP-a. Sporije kreiranje direktno povećava frustraciju korisnika koji prijavljuju kvar.

---

#### NFR-05

- **Naziv zahtjeva:** Dostupnost sistema (uptime)
- **Kategorija:** Pouzdanost (`PO`)
- **Opis zahtjeva:** Sistem mora biti dostupan korisnicima najmanje **99% vremena** u toku radne sedmice (ponedjeljak–petak, 07:00–22:00). Planirani zastoji za održavanje moraju biti unaprijed najavljeni i trajati kraće od 1 sata.
- **Kako će se provjeravati:** Praćenje dostupnosti putem monitoring alata (npr. UptimeRobot ili Better Stack). Evidencija svih zastoja s vremenskim oznakama i uzrocima. Izvještaji o dostupnosti pregledaju se na sprint reviewu.
- **Prioritet:** 1
- **Napomena:** Poslovni korisnici (pravna lica) zahtijevaju visok SLA(Service Level Agreement). Svaki zastoj za njih direktno znači finansijski gubitak, što je navedeno u Product Vision dokumentu.

---

#### NFR-06

- **Naziv zahtjeva:** Oporavak sistema nakon greške
- **Kategorija:** Pouzdanost (`PO`)
- **Opis zahtjeva:** U slučaju neočekivanog pada servera ili kritične greške, sistem mora automatski pokrenuti proces oporavka i postati dostupan korisnicima u roku od **5 minuta**. Tokom oporavka, korisnicima se prikazuje odgovarajuća obavijest o privremenoj nedostupnosti.
- **Kako će se provjeravati:** Testiranje oporavka simulacijom pada servera u testnom okruženju. Mjeri se ukupno vrijeme od detekcije greške do ponovne dostupnosti sistema.
- **Prioritet:** 1
- **Napomena:** Usko vezano s NFR-05. Sistem kao samostalno rješenje u MVP fazi se ne oslanja na externe backup servise, pa mehanizmi oporavka moraju biti implementirani interno.

---

#### NFR-07

- **Naziv zahtjeva:** Konzistentnost podataka pri prekidu veze
- **Kategorija:** Pouzdanost (`PO`)
- **Opis zahtjeva:** U slučaju prekida WebSocket veze ili gubitka mrežne konekcije tokom aktivne sesije, sistem ne smije dozvoliti gubitak niti korupciju podataka koji su već potvrđeni (npr. kreirani tiketi, promijenjeni statusi). Po ponovnom uspostavljanju veze, prikaz na interfejsu mora odražavati tačno stanje iz baze podataka.
- **Kako će se provjeravati:** Testiranje scenarija s namjernim prekidanjem veze tokom operacija kreiranja i ažuriranja tiketa. Provjera integriteta podataka u bazi nakon ponovnog povezivanja.
- **Prioritet:** 1
- **Napomena:** Product Vision eksplicitno navodi kao ograničenje potrebu za "osnovnim mehanizmima za očuvanje konzistentnosti podataka i ponovnog povezivanja u slučaju prekida veze". Kritično za tehničare koji rade s terena na nestabilnoj mreži.

---

#### NFR-08

- **Naziv zahtjeva:** Automatski WebSocket ponovni pokušaj spajanja
- **Kategorija:** Pouzdanost (`PO`)
- **Opis zahtjeva:** Klijentska strana sistema mora automatski pokušavati ponovo uspostaviti WebSocket konekciju u slučaju njenog prekida, bez potrebe za ručnom intervencijom korisnika. Korisnik mora biti obaviješten o gubitku veze putem statusne poruke.
- **Kako će se provjeravati:** Manuelno testiranje simulacijom prekida mrežnog interfejsa u pregledavačkim DevTools alatima. Provjera da li se konekcija automatski obnavlja i da li se korisničko sučelje ispravno sinhronizira.
- **Prioritet:** 2
- **Napomena:** Direktno podržava rad tehničara na terenu, koji prema pretpostavkama iz Product Vision dokumenta ne raspolažu uvijek stabilnim mobilnim internetom.

---

#### NFR-09

- **Naziv zahtjeva:** Intuitivnost interfejsa za agente i tehničare
- **Kategorija:** Upotrebljivost (`UP`)
- **Opis zahtjeva:** Novi agent koji je upoznat s osnovnim principima rada helpdesk sistema mora biti u stanju samostalno pronaći, obraditi i ažurirati tiket unutar sistema u roku od **20 minuta** od prvog pokretanja aplikacije, bez prethodne formalne obuke. Ovo podrazumijeva da navigacija, oznake i tok rada budu logični i konzistentni s uobičajenim poslovnim aplikacijama.
- **Kako će se provjeravati:** Testiranje upotrebljivosti s najmanje 5 ispitanika koji ranije nisu koristili sistem (po mogućnosti to će biti osobe s iskustvom u korisničkoj podršci). Po potrebi A/B testiranje ukoliko rezultati ne budu zadovoljavajući. Mjeri se vrijeme do uspješnog završetka zadatka i broj grešaka tokom procesa.
- **Prioritet:** 1
- **Napomena:** Agenti su svakodnevni operativni korisnici sistema. Loša upotrebljivost direktno usporava obradu tiketa i povećava broj grešaka u radu, što je identificirano kao jedan od ključnih problema u Product Vision dokumentu.

---

#### NFR-10

- **Naziv zahtjeva:** Jasne i razumljive poruke o greškama
- **Kategorija:** Upotrebljivost (`UP`)
- **Opis zahtjeva:** Svaka poruka o grešci prikazana korisniku mora biti napisana jasnim jezikom (bez tehničkih kodova), mora sadržavati opis problema i, gdje je to moguće, prijedlog kako korisnik treba postupiti. Poruke moraju biti prikazane na vidljivom dijelu ekrana i stilski se razlikuju od ostalih elemenata interfejsa (poruke upozorenja).
- **Kako će se provjeravati:** Pregled svih definisanih scenarija grešaka (neuspješna prijava, greška pri kreiranju tiketa, gubitak veze i sl.) i provjera prikaza poruka kroz manuelno testiranje. Ocjenjuje se jasnoća poruka bez tehničke pozadine.
- **Prioritet:** 2
- **Napomena:** Posebno bitno za stariju populaciju korisnika i tehničare koji rade na terenu s manjim tehničkim predznanjem. Poruke o greškama trebaju biti logovane interno, ali ne i prikazane korisniku.

---

#### NFR-11

- **Naziv zahtjeva:** Responzivan dizajn za desktop i tablet
- **Kategorija:** Upotrebljivost (`UP`)
- **Opis zahtjeva:** Sistem mora biti u potpunosti funkcionalan i vizualno ispravan na desktop rezolucijama (1280×720 i više) te tablet uređajima (768×1024 i više) u modernim web preglednicima. Svi elementi interfejsa – forme, tabele, dashboardi – moraju biti čitljivi i upotrebljivi bez horizontalnog skrolanja ili preklapanja sadržaja.
- **Kako će se provjeravati:** Manuelno testiranje na standardnim desktop rezolucijama i simulaciji tablet prikaza kroz pregledničke DevTools alate za responsive design. Provjera vizuelne konzistentnosti i funkcionalnosti na Chrome i Firefox preglednicima.
- **Prioritet:** 2
- **Napomena:** Product Vision definiše sistem kao isključivo web platformu. Mobilna (smartphone) podrška je eksplicitno isključena iz MVP scope-a. Tehničari na terenu mogu koristiti tablete za pristup sistemu.

---

#### NFR-12

- **Naziv zahtjeva:** Pristupačnost interfejsa za stariju populaciju
- **Kategorija:** Upotrebljivost (`UP`)
- **Opis zahtjeva:** Interfejs namijenjen krajnjim korisnicima (posebno forma za prijavu kvara) mora koristiti font veličine najmanje 14px, dovoljno visok kontrast između teksta i pozadine (minimalan kontrast ratio prema Nielsenovim principima upotrebljivosti), te jednostavnu navigaciju s minimalnim brojem koraka za prijavu kvara (ne više od 3 koraka prema dobrim UX praksama).
- **Kako će se provjeravati:** Provjera kontrasta pomoću Google Lighthouse alata. Manuelni pregled veličine fontova u CSS -u i složenosti toka prijave. Po mogućnosti, kratki test prihvatljivosti s predstavnicima starije populacije korisnika.
- **Prioritet:** 2
- **Napomena:** Poseban fokus stavljen je na jednostavnost korištenja kako bi se starijim rezidentima olakšao pristup digitalnim servisima i spriječila njihova digitalna izolacija.

---

#### NFR-13

- **Naziv zahtjeva:** Podrška za savremene web browsere
- **Kategorija:** Portabilnost (`PT`)
- **Opis zahtjeva:** Sistem mora biti u potpunosti funkcionalan u posljednjim stabilnim verzijama sljedećih web preglednika: **Google Chrome**, **Mozilla Firefox** i **Microsoft Edge**. Sve funkcionalnosti, uključujući WebSocket komunikaciju i real-time prikaz, moraju raditi ispravno bez dodatnih ekstenzija ili podešavanja od strane korisnika.
- **Kako će se provjeravati:** Manuelno testiranje kompletnog toka rada (kreiranje tiketa, promjena statusa, prikaz dashboarda) na svakom od navedenih preglednika, u najnovijoj dostupnoj verziji u trenutku testiranja.
- **Prioritet:** 1
- **Napomena:** Sistem je web-based i nema kontrolu nad tim koji preglednik koriste krajnji korisnici ili zaposlenici. Podrška za zastarjele preglednike nije obavezna.

---

#### NFR-14

- **Naziv zahtjeva:** Nezavisnost od operativnog sistema na serveru
- **Kategorija:** Portabilnost (`PT`)
- **Opis zahtjeva:** Kod ovog zahtjeva nam je cilj da aplikacija "ne bira stranu" kada je u pitanju operativni sistem. Pošto BH Telecom možda već ima spremne Linux servere, a možda preferira Windows, sistem moramo razviti tako da radi na oba bez diranja samog koda.
- **Kako će se provjeravati:** Izbjegavamo bilo kakve fiksne putanje ili specifične sistemske komande. Sve što se tiče okruženja rješavamo kroz konfiguracione fajlove i sistemske varijable. Na kraju ćemo to testirati tako što ćemo isti kod pokušati "podići" na Ubuntu i Windows okruženju.
- **Prioritet:** 2
- **Napomena:** Naručilac sistema (BH Telecom) može imati specifičnu serversku infrastrukturu. Nezavisnost od OS-a osigurava fleksibilnost migracije bez dodatnih troškova prilagodbe.

---

#### NFR-15

- **Naziv zahtjeva:** Fleksibilnost baze podataka
- **Kategorija:** Portabilnost (`PT`)
- **Opis zahtjeva:** Ovdje igramo na sigurno za budućnost. Kako još uvijek nismo 100% sigurni koju će bazu podataka klijent na kraju koristiti (npr. PostgreSQL, MySQL ili nešto treće), uvodimo ORM sloj. To je "prevodilac" koji omogućava da pišemo kod u objektima, a on se sam brine o SQL upitima.
- **Kako će se provjeravati:** Kroz pregled koda (code review) osiguravamo da su svi upiti prema bazi izolovani unutar ORM-a. Cilj je eliminisati bilo kakav SQL kod koji je vezan isključivo za jednu vrstu baze podataka.
- **Prioritet:** 3
- **Napomena:** Ako se za godinu dana odluči da se prelazi na drugi tip baze, ne želimo da moramo prepisivati stotine upita kroz cijelu aplikaciju. Cilj je da u tom slučaju samo promijenimo par linija u konfiguraciji konekcije. Tokom revizije koda ćemo paziti da niko ne piše "sirove" SQL upite koji su specifični samo za jednu bazu, jer bi nas to kasnije "zaključalo" za tu tehnologiju.

---
