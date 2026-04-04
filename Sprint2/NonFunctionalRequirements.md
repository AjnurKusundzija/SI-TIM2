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
- **Opis zahtjeva:** Klijentska strana sistema mora automatski pokušavati ponovo uspostaviti WebSocket konekciju u slučaju njenog prekida, bez potrebe za ručnom intervencijom korisnika. Prvi pokušaj ponovnog spajanja mora biti iniciran unutar **3 sekunde** od detekcije prekida, s maksimalno **5 uzastopnih pokušaja** u intervalima od 3 sekunde. Korisnik mora biti obaviješten o gubitku veze vidljivom statusnom porukom unutar **1 sekunde** od detekcije prekida, a po uspješnom ponovnom spajanju poruka se automatski uklanja.
- **Kako će se provjeravati:** Manuelno testiranje simulacijom prekida mrežnog interfejsa putem pregledničkih DevTools alata. Mjeri se: (1) vrijeme od prekida do prve obavijesti korisniku, (2) broj i interval pokušaja reconnecta vidljiv u Network tabu, (3) ispravna sinhronizacija korisničkog sučelja nakon ponovnog uspostavljanja veze.
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
- **Opis zahtjeva:** Svaka poruka o grešci prikazana korisniku mora biti napisana jezikom razumljivim krajnjem korisniku (bez tehničkih kodova poput HTTP statusnih kodova), mora sadržavati opis problema i konkretan prijedlog daljnjeg postupanja (npr. "Pokušajte ponovo" ili "Kontaktirajte podršku"). Poruka mora biti prikazana unutar **500ms od nastanka greške**, pozicionirana na vrhu aktivne forme ili stranice, minimalne veličine fonta **14px**, s vizuelno distinktivnim stilom (crvena boja pozadine) koji je jasno razlikuje od ostalih elemenata interfejsa.
- **Kako će se provjeravati:** Manuelno testiranje svih definisanih scenarija grešaka (neuspješna prijava, greška pri kreiranju tiketa, gubitak veze i sl.). Za svaki scenario mjeri se: (1) vrijeme prikaza poruke od nastanka greške, (2) prisutnost opisnog teksta bez tehničkih kodova, (3) prisutnost prijedloga za postupanje, (4) vizuelna distinktivnost poruke. Testiranje se provodi na svim podržanim preglednicima.
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
- **Opis zahtjeva:** Interfejs namijenjen krajnjim korisnicima (posebno forma za prijavu kvara) mora zadovoljiti sljedeće mjerljive kriterije pristupačnosti: (1) minimalna veličina fonta **14px** za sve elemente forme i navigacije, (2) minimalni kontrast ratio između teksta i pozadine **4.5:1** prema **WCAG 2.1 AA standardu**, (3) tok prijave kvara mora biti realizovan u **najviše 3 koraka**, pri čemu svaki korak smije sadržavati najviše 5 polja za unos.
- **Kako će se provjeravati:** Automatska provjera kontrast omjera putem Google Lighthouse alata (ciljna ocjena pristupačnosti minimalno 90/100). Manuelni pregled veličine fontova u CSS-u. Provjera broja koraka u toku prijave kvara kroz funkcionalno testiranje. Po mogućnosti, testiranje prihvatljivosti s najmanje 3 ispitanika iz ciljne grupe (65+ godina).
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
- **Opis zahtjeva:** Serverska komponenta sistema mora biti funkcionalna na **Linux (Ubuntu 22.04 LTS)** i **Windows Server 2019** operativnim sistemima bez izmjena izvornog koda. Sve putanje do fajlova i sistemske konfiguracije moraju biti definirane isključivo putem konfiguracijskih fajlova i varijabli okruženja (environment variables), bez hard-kodiranih putanja ili komandi.
- **Kako će se provjeravati:** Deployment i funkcionalno testiranje kompletnog toka rada (kreiranje tiketa, promjena statusa, WebSocket komunikacija) na Ubuntu 22.04 LTS i Windows Server 2019 okruženju. Pregledom koda (code review) provjerava se odsutnost hard-kodiranih putanja.
- **Prioritet:** 2
- **Napomena:** Naručilac sistema (BH Telecom) može imati specifičnu serversku infrastrukturu. Nezavisnost od OS-a osigurava fleksibilnost migracije bez dodatnih troškova prilagodbe.
 
---
 
#### NFR-15
 
- **Naziv zahtjeva:** Fleksibilnost baze podataka
- **Kategorija:** Portabilnost (`PT`)
- **Opis zahtjeva:** Svi pristupi bazi podataka u sistemu moraju biti realizovani isključivo putem ORM (Object-Relational Mapping) sloja, bez direktno pisanih SQL upita specifičnih za određenu bazu podataka. Sistem mora biti kompatibilan s najmanje dva relaciona sistema za upravljanje bazama podataka — **PostgreSQL** i **MySQL** — pri čemu promjena između njih ne smije zahtijevati izmjenu izvornog koda, već isključivo izmjenu konfiguracijskih parametara konekcije.
- **Kako će se provjeravati:** Pregled koda (code review) verificira odsutnost SQL upita pisanih izvan ORM sloja i odsutnost bazi-specifičnih funkcija ili sintakse. Funkcionalno testiranje osnovnih operacija (kreiranje, čitanje, ažuriranje i brisanje tiketa) provodi se na PostgreSQL i MySQL instancama korištenjem iste baze koda uz promjenu isključivo konfiguracijskih parametara.
- **Prioritet:** 3
- **Napomena:** Naručilac sistema možda nije finalizovao odabir konkretnog RDBMS-a. Apstrakcija kroz ORM sloj smanjuje rizik tehnološkog zaključavanja (vendor lock-in) i olakšava eventualnu migraciju bez potrebe za prepisom podatkovnog sloja aplikacije.
 
---


## Organizacioni zahtjevi

Organizacioni zahtjevi opisuju pravila, standarde i tehničke odluke kojih se tim treba pridržavati tokom razvoja sistema. Oni ne definišu šta sistem radi, nego pod kojim uslovima se razvija, održava i isporučuje.

---

### Tabelarni prikaz organizacionih zahtjeva

| ID                | Naziv zahtjeva                                    | Kategorija    | Prioritet | Status        |
| :---------------- | :------------------------------------------------ | :------------ | :-------: | :------------ |
| [NFR-16](#nfr-16) | Razvoj sistema kroz zajednički GitHub repozitorij | Organizacioni |     1     | Identifikovan |
| [NFR-17](#nfr-17) | Dogovoreni tehnološki stack                       | Organizacioni |     1     | Identifikovan |
| [NFR-18](#nfr-18) | Standardizovan način evidentiranja zadataka       | Organizacioni |     2     | Identifikovan |
| [NFR-19](#nfr-19) | Održavanje ažurne projektne dokumentacije         | Organizacioni |     2     | Identifikovan |
| [NFR-20](#nfr-20) | Poštivanje standarda kodiranja                    | Organizacioni |     2     | Identifikovan |
| [NFR-21](#nfr-21) | Jasna struktura projekta i razdvajanje slojeva    | Organizacioni |     2     | Identifikovan |
| [NFR-22](#nfr-22) | Praćenje promjena kroz commit historiju           | Organizacioni |     3     | Identifikovan |
| [NFR-23](#nfr-23) | Usklađenost rada sa sprintovima i rokovima        | Organizacioni |     1     | Identifikovan |

---

### Detalji organizacionih zahtjeva

#### NFR-16

- **Naziv zahtjeva:** Razvoj sistema kroz zajednički GitHub repozitorij
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Sav izvorni kod, dokumentacija i ostali projektni materijali moraju se voditi kroz zajednički GitHub repozitorij kako bi svi članovi tima imali pristup istim, ažurnim verzijama fajlova.
- **Kako će se provjeravati:** Pregledom repozitorija, foldera i dostupnih projektnih fajlova.
- **Prioritet:** 1
- **Napomena:** Ovim se izbjegava rad u više nepovezanih verzija istog dokumenta ili koda.

---

#### NFR-17

- **Naziv zahtjeva:** Dogovoreni tehnološki stack
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Sistem treba biti razvijen kao web aplikacija koristeći **C# / ASP.NET** za backend, **React** za frontend i **PostgreSQL** ili **SQL** za bazu podataka, u skladu s dogovorom tima.
- **Kako će se provjeravati:** Pregledom strukture projekta, konfiguracije i korištenih tehnologija u repozitoriju.
- **Prioritet:** 1
- **Napomena:** Korištenje istog tehnološkog stacka svim članovima olakšava razvoj i održavanje sistema.

---

#### NFR-18

- **Naziv zahtjeva:** Standardizovan način evidentiranja zadataka
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Zadatke na projektu potrebno je pratiti kroz backlog stavke, issue zadatke i statuse rada, tako da bude jasno šta je planirano, šta je u toku i šta je završeno.
- **Kako će se provjeravati:** Pregledom issue zadataka, backloga i statusa rada unutar GitHub okruženja.
- **Prioritet:** 2
- **Napomena:** Ovo pomaže timu da lakše prati napredak i raspodjelu obaveza.

---

#### NFR-19

- **Naziv zahtjeva:** Održavanje ažurne projektne dokumentacije
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Projektna dokumentacija treba biti redovno ažurirana i usklađena sa stvarnim stanjem projekta. Kada se promijeni zahtjev, odluka ili dogovor, to treba biti vidljivo i u dokumentima.
- **Kako će se provjeravati:** Poređenjem dokumentacije sa backlogom, issue zadacima i aktuelnim projektnim odlukama.
- **Prioritet:** 2
- **Napomena:** Dobra dokumentacija olakšava komunikaciju u timu i sa Product Ownerom.

---

#### NFR-20

- **Naziv zahtjeva:** Poštivanje standarda kodiranja
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Kod treba biti pisan uredno i dosljedno, uz jasna imena klasa, metoda, funkcija, varijabli i komponenti, kako bi svi članovi tima mogli lako razumjeti i nastaviti rad na istom dijelu sistema.
- **Kako će se provjeravati:** Pregledom koda i internim code review pristupom unutar tima.
- **Prioritet:** 2
- **Napomena:** Dosljedan stil kodiranja smanjuje zabune i olakšava održavanje projekta.

---

#### NFR-21

- **Naziv zahtjeva:** Jasna struktura projekta i razdvajanje slojeva
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Projekt treba biti organizovan pregledno, sa jasnim razdvajanjem backend, frontend i baza sloja, kao i logičnim rasporedom fajlova i foldera unutar repozitorija.
- **Kako će se provjeravati:** Pregledom strukture projekta i rasporeda foldera.
- **Prioritet:** 2
- **Napomena:** Dobra organizacija projekta olakšava snalaženje i ubrzava razvoj.

---

#### NFR-22

- **Naziv zahtjeva:** Praćenje promjena kroz commit historiju
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Sve značajne izmjene u kodu i dokumentaciji trebaju biti evidentirane kroz commit historiju, tako da se može pratiti ko je šta radio i kada je promjena napravljena.
- **Kako će se provjeravati:** Pregledom commit historije i opisa commit poruka.
- **Prioritet:** 3
- **Napomena:** Ovo je korisno za pregled razvoja projekta i lakše vraćanje na starije verzije po potrebi.

---

#### NFR-23

- **Naziv zahtjeva:** Usklađenost rada sa sprintovima i rokovima
- **Kategorija:** Organizacioni (`OZ`)
- **Opis zahtjeva:** Rad na projektu treba biti usklađen sa sprintovima, sedmičnim planiranjem i rokovima definisanim u okviru predmeta, kako bi tim mogao na vrijeme pripremiti dokumentaciju i isporuke.
- **Kako će se provjeravati:** Poređenjem planiranih i završenih zadataka po sprintovima.
- **Prioritet:** 1
- **Napomena:** Poštivanje rokova je važno da bi tim mogao redovno predstavljati napredak i završavati obaveze bez kašnjenja.

---

## Vanjski zahtjevi

Vanjski zahtjevi nastaju iz obaveza koje dolaze izvan samog tima i tehničke implementacije. To su zahtjevi vezani za zaštitu podataka, pristupačnost, transparentnost rada sistema i mogućnost praćenja važnih aktivnosti.

---

### Tabelarni prikaz vanjskih zahtjeva

| ID                | Naziv zahtjeva                                  | Kategorija | Prioritet | Status        |
| :---------------- | :---------------------------------------------- | :--------- | :-------: | :------------ |
| [NFR-24](#nfr-24) | Zaštita korisničkih i ličnih podataka           | Vanjski    |     1     | Identifikovan |
| [NFR-25](#nfr-25) | Ograničen pristup prema korisničkim ulogama     | Vanjski    |     1     | Identifikovan |
| [NFR-26](#nfr-26) | Transparentan prikaz statusa tiketa             | Vanjski    |     2     | Identifikovan |
| [NFR-27](#nfr-27) | Evidencija važnih aktivnosti u sistemu          | Vanjski    |     1     | Identifikovan |
| [NFR-28](#nfr-28) | Mogućnost revizije promjena nad tiketima        | Vanjski    |     1     | Identifikovan |
| [NFR-29](#nfr-29) | Pristupačan i razumljiv korisnički interfejs    | Vanjski    |     2     | Identifikovan |
| [NFR-30](#nfr-30) | Jednaka dostupnost funkcionalnosti korisnicima  | Vanjski    |     2     | Identifikovan |
| [NFR-31](#nfr-31) | Sigurno čuvanje operativnih i korisničkih podata| Vanjski    |     1     | Identifikovan |

---

### Detalji vanjskih zahtjeva

#### NFR-24

- **Naziv zahtjeva:** Zaštita korisničkih i ličnih podataka
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Sistem mora štititi lične i korisničke podatke od neovlaštenog pristupa, izmjene ili zloupotrebe, posebno podatke o korisnicima, tiketima i komunikaciji unutar sistema.
- **Kako će se provjeravati:** Pregledom pravila pristupa, strukture podataka i načina na koji sistem rukuje osjetljivim informacijama.
- **Prioritet:** 1
- **Napomena:** Zaštita podataka je obavezna jer sistem obrađuje informacije koje ne smiju biti javno dostupne.

---

#### NFR-25

- **Naziv zahtjeva:** Ograničen pristup prema korisničkim ulogama
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Korisnici sistema trebaju imati pristup samo onim opcijama i podacima koji su im potrebni za njihov posao ili korištenje sistema. Klijent, agent, tehničar i administrator ne trebaju imati isti nivo pristupa.
- **Kako će se provjeravati:** Testiranjem korisničkih naloga sa različitim ulogama.
- **Prioritet:** 1
- **Napomena:** Ovim se smanjuje mogućnost grešaka i neovlaštenih radnji unutar sistema.

---

#### NFR-26

- **Naziv zahtjeva:** Transparentan prikaz statusa tiketa
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Sistem treba korisniku jasno prikazivati u kojoj se fazi njegov tiket nalazi, kako bi imao uvid u tok obrade bez dodatnog kontaktiranja podrške.
- **Kako će se provjeravati:** Testiranjem prikaza tiketa iz perspektive krajnjeg korisnika.
- **Prioritet:** 2
- **Napomena:** Transparentnost povećava povjerenje korisnika i smanjuje opterećenje helpdeska.

---

#### NFR-27

- **Naziv zahtjeva:** Evidencija važnih aktivnosti u sistemu
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Sistem treba bilježiti važne aktivnosti kao što su kreiranje tiketa, promjena statusa, dodjela tiketa, zatvaranje tiketa i izmjene važnih korisničkih podataka.
- **Kako će se provjeravati:** Pregledom logova aktivnosti ili historije događaja u sistemu.
- **Prioritet:** 1
- **Napomena:** Ova evidencija je važna za praćenje rada i lakše otkrivanje eventualnih problema.

---

#### NFR-28

- **Naziv zahtjeva:** Mogućnost revizije promjena nad tiketima
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Za svaku važnu promjenu nad tiketom treba biti moguće utvrditi ko je izvršio radnju i kada se ta radnja dogodila.
- **Kako će se provjeravati:** Pregledom historije promjena na konkretnim tiketima.
- **Prioritet:** 1
- **Napomena:** Ovo je važno radi odgovornosti, kontrole i eventualne interne provjere rada.

---

#### NFR-29

- **Naziv zahtjeva:** Pristupačan i razumljiv korisnički interfejs
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Interfejs sistema treba biti dovoljno jasan i jednostavan da ga mogu koristiti i korisnici sa manjim tehničkim znanjem, bez nepotrebno komplikovanih koraka i nejasnih poruka.
- **Kako će se provjeravati:** Pregledom ekrana, formi i osnovnih korisničkih tokova.
- **Prioritet:** 2
- **Napomena:** Ovo je posebno važno jer sistem koriste različiti profili korisnika.

---

#### NFR-30

- **Naziv zahtjeva:** Jednaka dostupnost funkcionalnosti korisnicima
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Sistem ne smije neopravdano ograničavati pristup funkcionalnostima korisnicima koji imaju istu ulogu i iste dozvole, nego im treba pružiti jednak i konzistentan nivo pristupa.
- **Kako će se provjeravati:** Poređenjem prava pristupa između korisnika istih uloga.
- **Prioritet:** 2
- **Napomena:** Ovim se osigurava fer i dosljedno korištenje sistema.

---

#### NFR-31

- **Naziv zahtjeva:** Sigurno čuvanje operativnih i korisničkih podataka
- **Kategorija:** Vanjski (`VZ`)
- **Opis zahtjeva:** Podaci o tiketima, korisnicima, statusima i komunikaciji trebaju se čuvati na način koji smanjuje rizik od gubitka, neovlaštene izmjene ili oštećenja podataka.
- **Kako će se provjeravati:** Pregledom načina pohrane podataka i osnovnih sigurnosnih mjera u sistemu.
- **Prioritet:** 1
- **Napomena:** Pouzdano čuvanje podataka je važno za kontinuitet rada i vjerodostojnost sistema.
