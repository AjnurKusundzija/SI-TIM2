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
| `VZ`   | Vanjski      | Zahtjevi koji dolaze iz regulatornih okvira, zakona i eksternih standarda (npr. GDPR) |

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
| [NFR-12](#nfr-12)   | Pristupačnost interfejsa za stariju populaciju         | Upotrebljivost |     2     | Identifikovan |
| [NFR-13](#nfr-13)   | Podrška za savremene web browsere                      | Portabilnost   |     1     | Identifikovan |
| [NFR-14](#nfr-14)   | Fleksibilnost baze podataka                            | Portabilnost   |     3     | Identifikovan |
| [NFR-15](#nfr-15)   | Horizontalna skalabilnost backend sistema              | Efikasnost     |     2     | Identifikovan |
| [NFR-16](#nfr-16)   | Asinhrona obrada zahtjevnih operacija                  | Efikasnost     |     3     | Identifikovan |

---

## Detalji NFR zahtjeva

### Zahtjevi proizvoda

---

#### NFR-01

- **Naziv zahtjeva:** Vrijeme odziva pri učitavanju stranica
- **Kategorija:** Efikasnost (`EF`)
- **Opis zahtjeva:** Sve stranice sistema (dashboard, lista tiketa, detalji tiketa) moraju se u potpunosti učitati i biti interaktivne u roku od **2 sekunde** pri normalnom opterećenju sistema koje podrazumijeva do 50 istovremenih korisnika i stabilnu mrežnu vezu. Ovo se odnosi na sve korisničke uloge: klijente, agente, tehničare na terenu i administratore.
- **Kako će se provjeravati:** Testiranje performansi radit će se alatima poput Lighthouse ili k6, mjerenjem vremena učitavanja ključnih stranica u simuliranom okruženju s 50 istovremenih korisnika. Rezultati se dokumentuju u okviru sprint reviewa.
- **Prioritet:** 1
- **Napomena:** Zahtjev je direktno vezan uz Product Vision – sporo učitavanje stranica povećava frustraciju korisnika i smanjuje efikasnost agenata tokom obrade tiketa. Mjeri se pod normalnim, ne maksimalnim opterećenjem.

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
- **Napomena:** Relevantno posebno u scenarijima masovnih kvarova u mreži, gdje veći broj korisnika simultano prijavljuje problem.

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
- **Napomena:** Poslovni korisnici (pravna lica) zahtijevaju visok SLA (Service Level Agreement). Svaki zastoj za njih direktno znači finansijski gubitak.

---

#### NFR-06

- **Naziv zahtjeva:** Oporavak sistema nakon greške
- **Kategorija:** Pouzdanost (`PO`)
- **Opis zahtjeva:** U slučaju neočekivanog pada servera ili kritične greške, sistem mora automatski pokrenuti proces oporavka i postati dostupan korisnicima u roku od **5 minuta**. Tokom oporavka, korisnicima se prikazuje odgovarajuća obavijest o privremenoj nedostupnosti.
- **Kako će se provjeravati:** Testiranje oporavka simulacijom pada servera u testnom okruženju. Mjeri se ukupno vrijeme od detekcije greške do ponovne dostupnosti sistema.
- **Prioritet:** 1
- **Napomena:** Sistem kao samostalno rješenje u MVP fazi se ne oslanja na eksterne backup servise, pa mehanizmi oporavka moraju biti implementirani interno.

---

#### NFR-07

- **Naziv zahtjeva:** Konzistentnost podataka pri prekidu veze
- **Kategorija:** Pouzdanost (`PO`)
- **Opis zahtjeva:** U slučaju prekida WebSocket veze ili gubitka mrežne konekcije tokom aktivne sesije, sistem ne smije dozvoliti gubitak niti korupciju podataka koji su već potvrđeni (npr. kreirani tiketi, promijenjeni statusi). Po ponovnom uspostavljanju veze, prikaz na interfejsu mora odražavati tačno stanje iz baze podataka.
- **Kako će se provjeravati:** Testiranje scenarija s namjernim prekidanjem veze tokom operacija kreiranja i ažuriranja tiketa. Provjera integriteta podataka u bazi nakon ponovnog povezivanja.
- **Prioritet:** 1
- **Napomena:** Kritično za tehničare koji rade s terena na nestabilnoj mreži.

---

#### NFR-08

- **Naziv zahtjeva:** Automatski WebSocket ponovni pokušaj spajanja
- **Kategorija:** Pouzdanost (`PO`)
- **Opis zahtjeva:** Klijentska strana sistema mora automatski pokušavati ponovo uspostaviti WebSocket konekciju u slučaju njenog prekida, bez potrebe za ručnom intervencijom korisnika. Prvi pokušaj ponovnog spajanja mora biti iniciran unutar **3 sekunde** od detekcije prekida, s maksimalno **5 uzastopnih pokušaja** u intervalima od 3 sekunde. Korisnik mora biti obaviješten o gubitku veze vidljivom statusnom porukom unutar **1 sekunde** od detekcije prekida, a po uspješnom ponovnom spajanju poruka se automatski uklanja.
- **Kako će se provjeravati:** Manuelno testiranje simulacijom prekida mrežnog interfejsa putem pregledničkih DevTools alata. Mjeri se: (1) vrijeme od prekida do prve obavijesti korisniku, (2) broj i interval pokušaja reconnecta vidljiv u Network tabu, (3) ispravna sinhronizacija korisničkog sučelja nakon ponovnog uspostavljanja veze.
- **Prioritet:** 2
- **Napomena:** Direktno podržava rad tehničara na terenu, koji ne raspolažu uvijek stabilnim mobilnim internetom.

---

#### NFR-09

- **Naziv zahtjeva:** Intuitivnost interfejsa za agente i tehničare
- **Kategorija:** Upotrebljivost (`UP`)
- **Opis zahtjeva:** Novi agent koji je upoznat s osnovnim principima rada helpdesk sistema mora biti u stanju samostalno pronaći, obraditi i ažurirati tiket unutar sistema u roku od **20 minuta** od prvog pokretanja aplikacije, bez prethodne formalne obuke. Ovo podrazumijeva da navigacija, oznake i tok rada budu logični i konzistentni s uobičajenim poslovnim aplikacijama.
- **Kako će se provjeravati:** Testiranje upotrebljivosti s najmanje 5 ispitanika koji ranije nisu koristili sistem. Mjeri se vrijeme do uspješnog završetka zadatka i broj grešaka tokom procesa.
- **Prioritet:** 1
- **Napomena:** Loša upotrebljivost direktno usporava obradu tiketa i povećava broj grešaka u radu.

---

#### NFR-10

- **Naziv zahtjeva:** Jasne i razumljive poruke o greškama
- **Kategorija:** Upotrebljivost (`UP`)
- **Opis zahtjeva:** Svaka poruka o grešci prikazana korisniku mora biti napisana jezikom razumljivim krajnjem korisniku, bez tehničkih kodova poput HTTP statusnih kodova, mora sadržavati opis problema i konkretan prijedlog daljnjeg postupanja. Poruka mora biti prikazana unutar **500 ms** od nastanka greške, pozicionirana na vrhu aktivne forme ili stranice, minimalne veličine fonta **14 px**, s vizuelno distinktivnim stilom koji je jasno razlikuje od ostalih elemenata interfejsa.
- **Kako će se provjeravati:** Manuelno testiranje svih definisanih scenarija grešaka. Za svaki scenario mjeri se vrijeme prikaza poruke, prisutnost opisnog teksta bez tehničkih kodova, prisutnost prijedloga za postupanje i vizuelna distinktivnost poruke.
- **Prioritet:** 2
- **Napomena:** Posebno bitno za stariju populaciju korisnika i tehničare koji rade na terenu s manjim tehničkim predznanjem.

---

#### NFR-11

- **Naziv zahtjeva:** Responzivan dizajn za desktop i tablet
- **Kategorija:** Upotrebljivost (`UP`)
- **Opis zahtjeva:** Sistem mora biti u potpunosti funkcionalan i vizualno ispravan na desktop rezolucijama (1280×720 i više) te tablet uređajima (768×1024 i više) u modernim web preglednicima. Svi elementi interfejsa – forme, tabele, dashboardi – moraju biti čitljivi i upotrebljivi bez horizontalnog skrolanja ili preklapanja sadržaja.
- **Kako će se provjeravati:** Manuelno testiranje na standardnim desktop rezolucijama i simulaciji tablet prikaza kroz pregledničke DevTools alate za responsive design. Provjera vizuelne konzistentnosti i funkcionalnosti na Chrome i Firefox preglednicima.
- **Prioritet:** 2
- **Napomena:** Mobilna (smartphone) podrška je eksplicitno isključena iz MVP scope-a. Tehničari na terenu mogu koristiti tablete za pristup sistemu.

---

#### NFR-12

- **Naziv zahtjeva:** Pristupačnost interfejsa za stariju populaciju
- **Kategorija:** Upotrebljivost (`UP`)
- **Opis zahtjeva:** Interfejs namijenjen krajnjim korisnicima, posebno forma za prijavu kvara, mora zadovoljiti sljedeće mjerljive kriterije pristupačnosti: (1) minimalna veličina fonta **14 px** za sve elemente forme i navigacije, (2) minimalni kontrast ratio između teksta i pozadine **4.5:1** prema **WCAG 2.1 AA** standardu, (3) tok prijave kvara mora biti realizovan u **najviše 3 koraka**, pri čemu svaki korak smije sadržavati najviše 5 polja za unos.
- **Kako će se provjeravati:** Automatska provjera kontrast omjera putem Google Lighthouse alata. Manuelni pregled veličine fontova u CSS-u. Provjera broja koraka u toku prijave kvara kroz funkcionalno testiranje.
- **Prioritet:** 2
- **Napomena:** Poseban fokus stavljen je na jednostavnost korištenja kako bi se starijim korisnicima olakšao pristup digitalnim servisima.

---

#### NFR-13

- **Naziv zahtjeva:** Podrška za savremene web browsere
- **Kategorija:** Portabilnost (`PT`)
- **Opis zahtjeva:** Sistem mora biti u potpunosti funkcionalan u posljednjim stabilnim verzijama sljedećih web preglednika: **Google Chrome**, **Mozilla Firefox** i **Microsoft Edge**. Sve funkcionalnosti, uključujući WebSocket komunikaciju i real-time prikaz, moraju raditi ispravno bez dodatnih ekstenzija ili podešavanja od strane korisnika.
- **Kako će se provjeravati:** Manuelno testiranje kompletnog toka rada (kreiranje tiketa, promjena statusa, prikaz dashboarda) na svakom od navedenih preglednika, u najnovijoj dostupnoj verziji u trenutku testiranja.
- **Prioritet:** 1
- **Napomena:** Podrška za zastarjele preglednike nije obavezna.

---

#### NFR-14

- **Naziv zahtjeva:** Fleksibilnost baze podataka
- **Kategorija:** Portabilnost (`PT`)
- **Opis zahtjeva:** Svi pristupi bazi podataka u sistemu moraju biti realizovani isključivo putem ORM (Object-Relational Mapping) sloja, bez direktno pisanih SQL upita specifičnih za određenu bazu podataka. Sistem mora biti kompatibilan s najmanje dva relaciona sistema za upravljanje bazama podataka — **PostgreSQL** i **MySQL** — pri čemu promjena između njih ne smije zahtijevati izmjenu izvornog koda, već isključivo izmjenu konfiguracijskih parametara konekcije.
- **Kako će se provjeravati:** Pregled koda verificira odsutnost SQL upita pisanih izvan ORM sloja i odsutnost bazi-specifičnih funkcija ili sintakse. Funkcionalno testiranje osnovnih operacija provodi se na PostgreSQL i MySQL instancama korištenjem iste baze koda uz promjenu isključivo konfiguracijskih parametara.
- **Prioritet:** 3
- **Napomena:** Apstrakcija kroz ORM sloj smanjuje rizik tehnološkog zaključavanja i olakšava eventualnu migraciju.

---

#### NFR-15

- **Naziv zahtjeva:** Horizontalna skalabilnost backend sistema
- **Kategorija:** Efikasnost (`EF`)
- **Opis zahtjeva:** Backend dio sistema mora biti projektovan tako da podržava horizontalno skaliranje dodavanjem novih serverskih instanci bez izmjene izvornog koda aplikacije. Korisničke sesije i privremeni podaci ne smiju biti vezani isključivo za memoriju jedne instance aplikacije.
- **Kako će se provjeravati:** Deploy testne verzije sistema na najmanje dvije backend instance i simulacija rada s load balancerom. Provjerava se da li korisnički zahtjevi mogu biti obrađeni preko više instanci bez gubitka funkcionalnosti i bez grešaka u radu sesije.
- **Prioritet:** 2
- **Napomena:** Ovaj zahtjev omogućava lakše proširenje sistema u slučaju rasta broja korisnika ili povećanog broja prijava kvarova.

---

#### NFR-16

- **Naziv zahtjeva:** Asinhrona obrada zahtjevnih operacija
- **Kategorija:** Efikasnost (`EF`)
- **Opis zahtjeva:** Operacije koje mogu trajati duže, poput generisanja izvještaja, izvoza podataka ili obrade većeg broja notifikacija, trebaju se izvršavati asinhrono kako ne bi blokirale glavni tok rada korisnika i usporavale interfejs sistema.
- **Kako će se provjeravati:** Testiranjem funkcionalnosti koje uključuju duže procese i provjerom da korisnički interfejs ostaje responzivan dok se obrada izvršava u pozadini.
- **Prioritet:** 3
- **Napomena:** Time se poboljšava korisničko iskustvo i smanjuje opterećenje sistema kod zahtjevnijih operacija.

---

## Organizacioni zahtjevi

Organizacioni zahtjevi opisuju pravila, standarde i tehničke odluke kojih se tim treba pridržavati tokom razvoja sistema. Oni ne definišu šta sistem radi, nego pod kojim uslovima se razvija, održava i isporučuje.

---

### Tabelarni prikaz organizacionih zahtjeva

| ID                | Naziv zahtjeva                                    | Kategorija    | Prioritet | Status        |
| :---------------- | :------------------------------------------------ | :------------ | :-------: | :------------ |
| [NFR-17](#nfr-17) | Razvoj sistema kroz zajednički GitHub repozitorij | Organizacioni |     1     | Identifikovan |
| [NFR-18](#nfr-18) | Dogovoreni tehnološki stack                       | Organizacioni |     1     | Identifikovan |
| [NFR-19](#nfr-19) | Standardizovan način evidentiranja zadataka       | Organizacioni |     2     | Identifikovan |
| [NFR-20](#nfr-20) | Održavanje ažurne projektne dokumentacije         | Organizacioni |     2     | Identifikovan |
| [NFR-21](#nfr-21) | Poštivanje standarda kodiranja                    | Organizacioni |     2     | Identifikovan |
| [NFR-22](#nfr-22) | Jasna struktura projekta i razdvajanje slojeva    | Organizacioni |     2     | Identifikovan |
| [NFR-23](#nfr-23) | Praćenje promjena kroz commit historiju           | Organizacioni |     3     | Identifikovan |
| [NFR-24](#nfr-24) | Usklađenost rada sa sprintovima i rokovima        | Organizacioni |     1     | Identifikovan |
| [NFR-25](#nfr-25) | Minimalna pokrivenost testovima                   | Organizacioni |     2     | Identifikovan |
| [NFR-26](#nfr-26) | Automatska provjera kvaliteta koda u CI procesu   | Organizacioni |     3     | Identifikovan |

---

### Detalji organizacionih zahtjeva

#### NFR-17

- **Naziv zahtjeva:** Razvoj sistema kroz zajednički GitHub repozitorij
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Sav izvorni kod, dokumentacija i ostali projektni materijali moraju se voditi kroz zajednički GitHub repozitorij kako bi svi članovi tima imali pristup istim, ažurnim verzijama fajlova.
- **Kako će se provjeravati:** Pregledom repozitorija, foldera i dostupnih projektnih fajlova.
- **Prioritet:** 1
- **Napomena:** Ovim se izbjegava rad u više nepovezanih verzija istog dokumenta ili koda.

---

#### NFR-18

- **Naziv zahtjeva:** Dogovoreni tehnološki stack
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Sistem treba biti razvijen kao web aplikacija koristeći **C# / ASP.NET** za backend, **React** za frontend i **PostgreSQL** ili **SQL** za bazu podataka, u skladu s dogovorom tima.
- **Kako će se provjeravati:** Pregledom strukture projekta, konfiguracije i korištenih tehnologija u repozitoriju.
- **Prioritet:** 1
- **Napomena:** Korištenje istog tehnološkog stacka svim članovima olakšava razvoj i održavanje sistema.

---

#### NFR-19

- **Naziv zahtjeva:** Standardizovan način evidentiranja zadataka
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Zadatke na projektu potrebno je pratiti kroz backlog stavke, issue zadatke i statuse rada, tako da bude jasno šta je planirano, šta je u toku i šta je završeno.
- **Kako će se provjeravati:** Pregledom issue zadataka, backloga i statusa rada unutar GitHub okruženja.
- **Prioritet:** 2
- **Napomena:** Ovo pomaže timu da lakše prati napredak i raspodjelu obaveza.

---

#### NFR-20

- **Naziv zahtjeva:** Održavanje ažurne projektne dokumentacije
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Projektna dokumentacija treba biti redovno ažurirana i usklađena sa stvarnim stanjem projekta. Kada se promijeni zahtjev, odluka ili dogovor, to treba biti vidljivo i u dokumentima.
- **Kako će se provjeravati:** Poređenjem dokumentacije sa backlogom, issue zadacima i aktuelnim projektnim odlukama.
- **Prioritet:** 2
- **Napomena:** Dobra dokumentacija olakšava komunikaciju u timu i sa Product Ownerom.

---

#### NFR-21

- **Naziv zahtjeva:** Poštivanje standarda kodiranja
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Kod treba biti pisan uredno i dosljedno, uz jasna imena klasa, metoda, funkcija, varijabli i komponenti, kako bi svi članovi tima mogli lako razumjeti i nastaviti rad na istom dijelu sistema.
- **Kako će se provjeravati:** Pregledom koda i internim code review pristupom unutar tima.
- **Prioritet:** 2
- **Napomena:** Dosljedan stil kodiranja smanjuje zabune i olakšava održavanje projekta.

---

#### NFR-22

- **Naziv zahtjeva:** Jasna struktura projekta i razdvajanje slojeva
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Projekt treba biti organizovan pregledno, sa jasnim razdvajanjem backend, frontend i baza sloja, kao i logičnim rasporedom fajlova i foldera unutar repozitorija.
- **Kako će se provjeravati:** Pregledom strukture projekta i rasporeda foldera.
- **Prioritet:** 2
- **Napomena:** Dobra organizacija projekta olakšava snalaženje i ubrzava razvoj.

---

#### NFR-23

- **Naziv zahtjeva:** Praćenje promjena kroz commit historiju
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Sve značajne izmjene u kodu i dokumentaciji trebaju biti evidentirane kroz commit historiju, tako da se može pratiti ko je šta radio i kada je promjena napravljena.
- **Kako će se provjeravati:** Pregledom commit historije i opisa commit poruka.
- **Prioritet:** 3
- **Napomena:** Ovo je korisno za pregled razvoja projekta i lakše vraćanje na starije verzije po potrebi.

---

#### NFR-24

- **Naziv zahtjeva:** Usklađenost rada sa sprintovima i rokovima
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Rad na projektu treba biti usklađen sa sprintovima, sedmičnim planiranjem i rokovima definisanim u okviru predmeta, kako bi tim mogao na vrijeme pripremiti dokumentaciju i isporuke.
- **Kako će se provjeravati:** Poređenjem planiranih i završenih zadataka po sprintovima.
- **Prioritet:** 1
- **Napomena:** Poštivanje rokova je važno da bi tim mogao redovno predstavljati napredak i završavati obaveze bez kašnjenja.

---

#### NFR-25

- **Naziv zahtjeva:** Minimalna pokrivenost testovima
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Ključna poslovna logika backend sistema mora biti pokrivena unit testovima u minimalnom obimu od **60%**, s ciljem povećanja pouzdanosti i lakšeg održavanja sistema tokom daljeg razvoja.
- **Kako će se provjeravati:** Korištenjem alata za mjerenje pokrivenosti testovima i pregledom izvještaja u okviru CI/CD procesa ili lokalnog testnog okruženja.
- **Prioritet:** 2
- **Napomena:** Ovaj zahtjev doprinosi održivosti sistema i smanjuje rizik od regresija pri budućim izmjenama.

---

#### NFR-26

- **Naziv zahtjeva:** Automatska provjera kvaliteta koda u CI procesu
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Svaka značajnija izmjena koda treba prolaziti automatsku provjeru build procesa, pokretanje testova i osnovnu statičku analizu koda prije spajanja u glavnu granu repozitorija.
- **Kako će se provjeravati:** Pregledom CI pipeline konfiguracije i provjerom da pull request ne može biti spojen ako build ili testovi nisu uspješno prošli.
- **Prioritet:** 3
- **Napomena:** Ovim se postiže stabilniji razvojni proces i lakše održavanje kvaliteta projekta tokom rada više članova tima.

---

## Vanjski zahtjevi

Vanjski zahtjevi nastaju iz obaveza koje dolaze izvan samog tima i tehničke implementacije. To su zahtjevi vezani za zaštitu podataka, pristupačnost, transparentnost rada sistema i mogućnost praćenja važnih aktivnosti.

---

### Tabelarni prikaz vanjskih zahtjeva

| ID                | Naziv zahtjeva                                  | Kategorija | Prioritet | Status        |
| :---------------- | :---------------------------------------------- | :--------- | :-------: | :------------ |
| [NFR-23](#nfr-27) | Zaštita korisničkih i ličnih podataka           | Vanjski    |     1     | Identifikovan |
| [NFR-24](#nfr-28) | Ograničen pristup prema korisničkim ulogama     | Vanjski    |     1     | Identifikovan |
| [NFR-25](#nfr-29) | Transparentan prikaz statusa tiketa             | Vanjski    |     2     | Identifikovan |
| [NFR-26](#nfr-30) | Evidencija važnih aktivnosti u sistemu          | Vanjski    |     1     | Identifikovan |
| [NFR-27](#nfr-31) | Mogućnost revizije promjena nad tiketima        | Vanjski    |     1     | Identifikovan |
| [NFR-28](#nfr-32) | Pristupačan i razumljiv korisnički interfejs    | Vanjski    |     2     | Identifikovan |
| [NFR-29](#nfr-33) | Jednaka dostupnost funkcionalnosti korisnicima  | Vanjski    |     2     | Identifikovan |
| [NFR-30](#nfr-34) | Sigurno čuvanje operativnih i korisničkih podata| Vanjski    |     1     | Identifikovan |
| [NFR-35](#nfr-35) | Enkripcija podataka u prenosu                   | Vanjski    |     1     | Identifikovan |
| [NFR-36](#nfr-36) | Sigurno upravljanje lozinkama                   | Vanjski    |     1     | Identifikovan |
| [NFR-37](#nfr-37) | Anonimizacija i pravo na brisanje podataka      | Vanjski    |     1     | Identifikovan |
| [NFR-38](#nfr-38) | Minimalizacija prikaza ličnih podataka          | Vanjski    |     2     | Identifikovan |

---

### Detalji vanjskih zahtjeva

#### NFR-27

- **Naziv zahtjeva:** Zaštita korisničkih i ličnih podataka
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Sistem mora štititi lične i korisničke podatke od neovlaštenog pristupa, izmjene ili zloupotrebe, posebno podatke o korisnicima, tiketima i komunikaciji unutar sistema.
- **Kako će se provjeravati:** Pregledom pravila pristupa, strukture podataka i načina na koji sistem rukuje osjetljivim informacijama.
- **Prioritet:** 1
- **Napomena:** Zaštita podataka je obavezna jer sistem obrađuje informacije koje ne smiju biti javno dostupne.

---

#### NFR-28

- **Naziv zahtjeva:** Ograničen pristup prema korisničkim ulogama
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Korisnici sistema trebaju imati pristup samo onim opcijama i podacima koji su im potrebni za njihov posao ili korištenje sistema. Klijent, agent, tehničar i administrator ne trebaju imati isti nivo pristupa.
- **Kako će se provjeravati:** Testiranjem korisničkih naloga sa različitim ulogama.
- **Prioritet:** 1
- **Napomena:** Ovim se smanjuje mogućnost grešaka i neovlaštenih radnji unutar sistema.

---

#### NFR-29

- **Naziv zahtjeva:** Transparentan prikaz statusa tiketa
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Sistem treba korisniku jasno prikazivati u kojoj se fazi njegov tiket nalazi, kako bi imao uvid u tok obrade bez dodatnog kontaktiranja podrške.
- **Kako će se provjeravati:** Testiranjem prikaza tiketa iz perspektive krajnjeg korisnika.
- **Prioritet:** 2
- **Napomena:** Transparentnost povećava povjerenje korisnika i smanjuje opterećenje helpdeska.

---

#### NFR-30

- **Naziv zahtjeva:** Evidencija važnih aktivnosti u sistemu
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Sistem treba bilježiti važne aktivnosti kao što su kreiranje tiketa, promjena statusa, dodjela tiketa, zatvaranje tiketa i izmjene važnih korisničkih podataka.
- **Kako će se provjeravati:** Pregledom statusa aktivnosti ili historije događaja u sistemu.
- **Prioritet:** 1
- **Napomena:** Ova evidencija je važna za praćenje rada i lakše otkrivanje eventualnih problema.

---

#### NFR-31

- **Naziv zahtjeva:** Mogućnost revizije promjena nad tiketima
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Za svaku važnu promjenu nad tiketom treba biti moguće utvrditi ko je izvršio radnju.
- **Kako će se provjeravati:** Pregledom historije promjena na konkretnim tiketima.
- **Prioritet:** 1
- **Napomena:** Ovo je važno radi odgovornosti, kontrole i eventualne interne provjere rada.

---

#### NFR-32

- **Naziv zahtjeva:** Pristupačan i razumljiv korisnički interfejs
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Interfejs sistema treba biti dovoljno jasan i jednostavan da ga mogu koristiti i korisnici sa manjim tehničkim znanjem, bez nepotrebno komplikovanih koraka i nejasnih poruka.
- **Kako će se provjeravati:** Pregledom ekrana, formi i osnovnih korisničkih tokova.
- **Prioritet:** 2
- **Napomena:** Ovo je posebno važno jer sistem koriste različiti profili korisnika.

---

#### NFR-33

- **Naziv zahtjeva:** Jednaka dostupnost funkcionalnosti korisnicima
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Sistem ne smije neopravdano ograničavati pristup funkcionalnostima korisnicima koji imaju istu ulogu i iste dozvole, nego im treba pružiti jednak i konzistentan nivo pristupa.
- **Kako će se provjeravati:** Poređenjem prava pristupa između korisnika istih uloga.
- **Prioritet:** 2
- **Napomena:** Ovim se osigurava fer i dosljedno korištenje sistema.

---

#### NFR-34

- **Naziv zahtjeva:** Sigurno čuvanje operativnih i korisničkih podataka
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Podaci o tiketima, korisnicima, statusima i komunikaciji trebaju se čuvati na način koji smanjuje rizik od gubitka, neovlaštene izmjene ili oštećenja podataka.
- **Kako će se provjeravati:** Pregledom načina pohrane podataka i osnovnih sigurnosnih mjera u sistemu.
- **Prioritet:** 1
- **Napomena:** Pouzdano čuvanje podataka je važno za kontinuitet rada i vjerodostojnost sistema.

---

#### NFR-35

- **Naziv zahtjeva:** Enkripcija podataka u prenosu
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Sav mrežni saobraćaj između klijentske aplikacije, backend servisa i eventualnih integrisanih servisa mora biti zaštićen korištenjem **HTTPS/TLS 1.2 ili novijeg** protokola. Sistem ne smije dozvoliti pristup preko nezaštićenog HTTP protokola u produkcijskom okruženju.
- **Kako će se provjeravati:** Pregledom konfiguracije servera i mrežnog saobraćaja putem browser DevTools i sigurnosnog testa kojim se potvrđuje da su svi zahtjevi preusmjereni na HTTPS i da nema miješanog nesigurnog sadržaja.
- **Prioritet:** 1
- **Napomena:** Ovaj zahtjev je osnovna tehnička mjera zaštite korisničkih podataka i prijavnih informacija tokom prenosa.

---

#### NFR-36

- **Naziv zahtjeva:** Sigurno upravljanje lozinkama
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Ako sistem koristi vlastitu autentifikaciju korisnika, lozinke se ne smiju čuvati u čistom tekstu, nego isključivo kao kriptografski hash korištenjem sigurnih algoritama poput **bcrypt** ili **Argon2**. Sistem mora zahtijevati lozinku minimalne dužine **8 karaktera**, uz najmanje jedno veliko slovo, jedno malo slovo i jedan broj.
- **Kako će se provjeravati:** Pregledom implementacije autentifikacije, strukture baze podataka i validacijskih pravila prilikom registracije ili promjene lozinke. Testiranjem pokušaja unosa neispravnih lozinki.
- **Prioritet:** 1
- **Napomena:** Ovim se smanjuje rizik kompromitacije korisničkih naloga i povećava sigurnost sistema.

---

#### NFR-37

- **Naziv zahtjeva:** Anonimizacija i pravo na brisanje podataka
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Sistem mora omogućiti anonimizaciju ili brisanje ličnih podataka korisnika na zahtjev, u skladu s važećim pravilima zaštite podataka. Pri tome se mora očuvati integritet poslovno važnih zapisa, tako da stari tiketi mogu ostati u sistemu bez direktno identifikacionih podataka.
- **Kako će se provjeravati:** Pregledom funkcionalnosti za anonimizaciju ili brisanje podataka i testiranjem scenarija u kojem se zahtjev korisnika izvršava bez narušavanja historije tiketa i osnovnih izvještaja sistema.
- **Prioritet:** 1
- **Napomena:** Ovaj zahtjev dodatno konkretizuje usklađenost sistema sa GDPR principima i zaštitom privatnosti korisnika.

---

#### NFR-38

- **Naziv zahtjeva:** Minimalizacija prikaza ličnih podataka
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Sistem treba prikazivati samo onaj skup ličnih podataka koji je nužan za izvršavanje zadatka određene korisničke uloge. Na primjer, tehničar na terenu ne treba imati pristup svim korisničkim podacima ako mu za intervenciju trebaju samo ime, adresa, kontakt telefon i osnovne informacije o usluzi.
- **Kako će se provjeravati:** Pregledom ekrana i dozvola za svaku korisničku ulogu, te testiranjem da li različiti korisnici vide samo podatke koji su im potrebni za rad.
- **Prioritet:** 2
- **Napomena:** Ovim se smanjuje izloženost osjetljivih podataka i primjenjuje princip najmanjih privilegija.

---
