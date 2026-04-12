# Risk Register

Ovaj dokument služi za identifikaciju, analizu i planiranje odgovora na rizike u okviru projekta.

## Tabela registra rizika

| ID | Naziv rizika |Opis rizika | Uzrok | Vjerovatnoća | Uticaj | Prioritet rizika | Plan Mitigacije | Odgovorna osoba/uloga | Status |
| :--- | :--- |:--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| R-01 | Tehnička nespremnost | Rizik od nedovoljnog znanja u tehničkom aspektu implementiranja ovog projekta, individualno ili timski | Tehničko neznanje, nespremnost i/ili nezainteresovanost o pojedinačnim dijelovima ili cijelog projekta | Niska | Srednji | Niska | Razgovor, savjetovanje i mentorisanje cijeloga tima i priprema za detaljno istraživanje o dijelu projekta kojeg ne umijemo implementirati| Vođa tima, Developeri projekta | Identifikovano
| R-02 | Loša komunikacija | Nedovoljna komunikacija tokom sprinta cijeloga tima ili pojedinačne osobe unutar tima | Nedostupnost, nezainteresovanost, nekolegijalnost | Niska | Nizak | Nizak | Razgovor unutar tima, sa ili bez odgovorne osobe. Eventualna prijava Product Owneru. Eventualno izbacivanje člana iz tima. | Vođa tima, ostali članovi ekipe | Identifikovano
| R-03 | Loša raspodjela zadataka | Loša raspodjela količine zadataka tokom sprinta jednoj ili više osoba koji rade na sprintu | Loša procjena veličine i obima implementacije zadatka ili Namjerna/nenamjerna podjela veće ili manje količine zadataka članu tima  | Niska | Niska | Niska | Identifikacija ovog rizika, razgovor unutar tima i ponovna preraspodjela zadataka. | Vođa tima, ostali članovi tima| Identifikovano
| R-04 | Neusklađenost sa GDPR-om | Sistem ne omogućava potpuno brisanje, uređivanje ili anonimizaciju ličnih podataka na zahtjev| Loša i neprocjenjena implementacija arhitekture baze podataka | Srednja | Visok | Srednje |  |Developeri u timu | Identifikovan
| R-05 | Neusklađenost sa GDPR-om | Sistem ne omogućava potpuno brisanje, uređivanje ili anonimizaciju ličnih podataka na zahtjev| Loša i neprocjenjena implementacija arhitekture baze podataka | Srednja | Visok | Srednje |  |Developeri u timu | Identifikovan
| R-07 | Spor odziv sistema pod opterećenjem| Sporo slanje i primanje zahtjeva sistema pod opterećenim uvjetima | Loša tehnička implementacija. Nedovoljni memorijski, tehnički aspekti servisa na kojem se hosta naš projekat, sistem | Srednja | Visok | Visok | |Developeri u timu | Identifikovan
| R-08 | Loša upotrebljivost sistema | Implementiranje sistema gdje je korištenje istoga neintuitivno, teško za razumijeti i nepristupačno | Loša implementacija frontenda i korisničkog interfejsa. Nepošivanje standarda za dizajn i implementiranje UI/UX dijela ovoga projekta. Tehnička nespremnost za frontend| Niska | Srednji | Srednji | | Developeri, Frontend developeri, UI/UX Dizajneri sistema| Identifikovan
| R-09 | Neskalabilan sistem | Sistem implementiran samo za nedovoljno vrsti platformi i okruženja kao što su: različiti web browseri, različiti uređaji, itd.| Nedovoljna širina arhitekture koja je namjenjena za ovaj sistem. Loša tehnička implementacija koja je namjenjena samo za statičko prikazivanje sistema. | Niska | Srednje | Niska | | Developeri, Frontend developeri | Identifikovan
| R-09 |  | | |  |  |  | |  | 
| R-10 |  | | |  |  |  | |  | 
| R-11 |  | | |  |  |  | |  | 
| R-12 |  | | |  |  |  | |  | 
| R-13 |  | | |  |  |  | |  | 
---
### Legenda i smjernice:
* **Vjerovatnoća:** (Niska, Srednja, Visoka)
* **Uticaj:** (Nizak, Srednji, Visok)
* **Prioritet rizika:** Obično se računa kao kombinacija (ili proizvod) Vjerovatnoće i Uticaja (npr. Nizak, Srednji, Visok, Kritičan).
* **Status:** (Identifikovan, Aktivan, Ublažen, Zatvoren)