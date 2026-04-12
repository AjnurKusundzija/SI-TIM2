# Risk Register

Ovaj dokument služi za identifikaciju, analizu i planiranje odgovora na rizike u okviru projekta.

## Tabela registra rizika

| ID | Naziv rizika |Opis rizika | Uzrok | Vjerovatnoća | Uticaj | Prioritet rizika | Plan Mitigacije | Odgovorna osoba/uloga | Status |
| :--- | :--- |:--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| R-01 | Tehnička nespremnost | Rizik od nedovoljnog znanja u tehničkom aspektu implementiranja ovog projekta, individualno ili timski | Tehničko neznanje, nespremnost i/ili nezainteresovanost o pojedinačnim dijelovima ili cijelog projekta | Niska | Srednji | Niska | Razgovor, savjetovanje i mentorisanje cijeloga tima i priprema za detaljno istraživanje o dijelu projekta kojeg ne umijemo implementirati| Vođa tima, Developeri projekta | Identifikovano
| R-02 | Loša komunikacija | Nedovoljna komunikacija tokom sprinta cijeloga tima ili pojedinačne osobe unutar tima | Nedostupnost, nezainteresovanost, nekolegijalnost | Niska | Nizak | Nizak | Razgovor unutar tima, sa ili bez odgovorne osobe. Eventualna prijava Product Owneru. Eventualno izbacivanje člana iz tima. | Vođa tima, ostali članovi ekipe | Identifikovano
| R-03 | Loša raspodjela zadataka | Loša raspodjela količine zadataka tokom sprinta jednoj ili više osoba koji rade na sprintu | Loša procjena veličine i obima implementacije zadatka ili Namjerna/nenamjerna podjela veće ili manje količine zadataka članu tima  | Niska | Niska | Niska | Identifikacija ovog rizika, razgovor unutar tima i ponovna preraspodjela zadataka. | Vođa tima, ostali članovi tima| Identifikovano
| R-04 | Neusklađenost sa GDPR-om | Sistem ne omogućava potpuno brisanje, uređivanje ili anonimizaciju ličnih podataka na zahtjev. Rizik od curenja osjetljivih ličnih podataka. | Loša i neprocjenjena implementacija arhitekture baze podataka | Srednja | Visok | Srednje |  |Developeri u timu | Identifikovan
| R-05 | Spor odziv sistema pod opterećenjem| Sporo slanje i primanje zahtjeva sistema pod opterećenim uvjetima | Loša tehnička implementacija. Nedovoljni memorijski, tehnički aspekti servisa na kojem se hosta naš projekat, odnosno cijeli sistem | Srednja | Visok | Visok | |Developeri u timu | Identifikovan
| R-06 | Loša upotrebljivost sistema | Implementiranje sistema gdje je korištenje istoga neintuitivno, teško za razumijeti i nepristupačno | Loša implementacija frontenda i korisničkog interfejsa. Nepošivanje standarda za dizajn i implementiranje UI/UX dijela ovoga projekta. Tehnička nespremnost za frontend| Niska | Srednji | Srednji | | Developeri, Frontend developeri, UI/UX Dizajneri sistema| Identifikovan
| R-07 | Neskalabilan sistem | Sistem implementiran samo za nedovoljno vrsti platformi i okruženja kao što su: različiti web browseri, različiti uređaji, itd.| Nedovoljna širina arhitekture koja je namjenjena za ovaj sistem. Loša tehnička implementacija koja je namjenjena samo za statičko prikazivanje sistema. | Niska | Srednje | Niska | | Developeri, Frontend developeri | Identifikovan
| R-08 |  Neovlašteni pristup podacima | Dolazak do situacije gdje neovlaštena uloga ima pristup informacijama(na primjer, korisnik ima pristup admin dashboardu, admin ima pristup ličnim podacima korisnika, itd.) | Loša tehnička implementacija autorizacijskog dijela projekta. | Niska |  Visok | Srednje  | | Developeri, backend developeri, arhitekta za bazu podataka  | Identifikovan
| R-09 | Pogrešna automatizacija i generisanje svih vrsta izvještaja | Pogrešan prikaz izvještaja koji su dostupni u admin dashboardu i ostalo | Loš preračun i tehnička implementacija za generisanje svih vrsta izvještaja za agente, admine i tehničare | Srednje  | Srednje  |  Srednje| | Developeri, Backend developeri | Identifikovan
| R-10 | Spam i korištenje neprimjerenog jezika | Namjerno/nenamjerno slanje većeg broja tiketa i korištenje neprimjerenog jezika tokom pisanja tiketa | Loša tehnička implementacija za filtriranje neprimjerenih riječi i za validaciju poruke u tiketu. Loša tehnička implementacija za spriječavanje spama sa tiketima | Srednje  | Srednje  | Srednja  | | Developeri (Backend, Frontend) | Identifikovan 
| R-11 |  | | |  |  |  | y|  | 
| R-12 |  | | |  |  |  | |  | 
---
### Legenda i smjernice:
* **Vjerovatnoća:** (Niska, Srednja, Visoka)
* **Uticaj:** (Nizak, Srednji, Visok)
* **Prioritet rizika:** Obično se računa kao kombinacija (ili proizvod) Vjerovatnoće i Uticaja (npr. Nizak, Srednji, Visok, Kritičan).
* **Status:** (Identifikovan, Aktivan, Ublažen, Zatvoren)