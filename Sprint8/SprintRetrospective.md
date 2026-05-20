# Sprint Retrospective – Sprint 8

## Opće ocjene sprinta

| Kategorija | Prosjek (1–5) |
|---|---|
| Ocjena sprinta u cjelini | 5.0 |
| Razina stresa / opterećenja | 3.5 |
| Zadovoljstvo suradnjom tima | 4.8 |

---

# Što je funkcioniralo dobro

- Komunikacija i suradnja unutar tima bile su na veoma visokom nivou tokom cijelog sprinta.
- Većina članova istaknula je da su članovi tima bili dostupni, odgovarali pravovremeno i pomagali jedni drugima pri implementaciji funkcionalnosti.
- Sprint ciljevi i scope bili su jasno definisani od početka sprinta, što je omogućilo stabilniji workflow i lakšu organizaciju rada.
- Integracija frontend i backend funkcionalnosti prošla je uspješno bez većih problema.
- Implementacija notifikacijskog sistema uspješno je povezana sa postojećim ticket workflow sistemom.
- Funkcionalnosti vezane za profile korisnika, statistiku agenata i tehničara te upravljanje tiketima uspješno su implementirane i integrisane u sistem.
- AI alati i razvojna okruženja generalno su pomogli pri implementaciji novih funkcionalnosti i ubrzali razvoj pojedinih komponenti sistema.
- Testiranje aplikacije i validacija implementiranih funkcionalnosti prošli su uspješno.
- Tim je pokazao dobru organizaciju pri raspodjeli zadataka i koordinaciji između članova tokom rada na zajedničkim komponentama.
- Workflow rada na branch-evima i Pull Request pristup omogućili su relativno stabilan razvoj bez ozbiljnijih problema pri integraciji.
- Veći dio funkcionalnosti završen je na vrijeme i bez značajnijih tehničkih dugova.
- Sprint je završen uz veoma visok nivo zadovoljstva članova tima i pozitivnu atmosferu tokom rada.

---

# Osobni uspjesi i highlight momenti

- Uspješna implementacija real-time notifikacijskog sistema.
- Implementacija statistike rada agenata i tehničara na profilnim stranicama.
- Integracija ocjenjivanja tiketa nakon zatvaranja.
- Implementacija upravljanja korisničkim profilom i pregledom korisničkih podataka.
- Uspješno povezivanje novih funkcionalnosti sa postojećim ticket workflow sistemom.
- Završavanje kompleksnijih PB zadataka bez većih problema u integraciji.
- Bolje razumijevanje arhitekture sistema i komunikacije između komponenti aplikacije.
- Stabilna saradnja između frontend i backend dijela tima tokom implementacije sprint funkcionalnosti.
- Uspješno testiranje i validacija novih funkcionalnosti.
- Napredak u organizaciji rada i koordinaciji tima kroz sprint.
- Pozitivna atmosfera i visok nivo međusobne pomoći unutar tima.
- Veća sigurnost članova tima pri radu sa Git workflow-om i integracijom promjena.

---

# Što je usporavalo tim (problemi i frustracije)

- Merge konflikti između različitih branch-eva povremeno su usporavali integraciju promjena.
- Dio vremena izgubljen je na usklađivanje frontend i backend implementacija.
- Pojedine funkcionalnosti zahtijevale su dodatna testiranja zbog povezanosti sa postojećim workflow sistemom.
- Povremeno čekanje na review i potvrdu određenih implementacija produžavalo je završetak pojedinih taskova.
- Integracija većeg broja funkcionalnosti unutar istog sprinta povećala je kompleksnost koordinacije između članova tima.
- Dio vremena potrošen je na stabilizaciju i provjeru SignalR notifikacijskog sistema.
- Pojedine konsultacije i organizacijske aktivnosti skratile su efektivno vrijeme rada tokom sprinta.
- Neki članovi tima naveli su povećan nivo opterećenja zbog većeg broja međusobno povezanih funkcionalnosti.
- Povremeno je bilo potrebno dodatno usklađivanje oko poslovne logike pojedinih funkcionalnosti kako bi implementacija ostala konzistentna unutar sistema.

---

# Što treba prestati raditi

- Oslanjati se na završavanje većih taskova pred kraj sprinta.
- Merge-ati veće promjene bez prethodne koordinacije sa ostalim članovima tima.
- Oslanjati se isključivo na usmenu ili neformalnu komunikaciju bez ažuriranja task statusa.
- Ostaviti integraciju većih funkcionalnosti za posljednje dane sprinta.
- Raditi izmjene nad istim komponentama bez prethodnog dogovora između članova tima.
- Prihvatati AI generisane prijedloge bez detaljne validacije logike i sigurnosnih aspekata.
- Zanemarivati dodatno testiranje funkcionalnosti koje utiču na ticket workflow i autorizaciju.

---

# Planiranje i jasnoća scope-a

Većina članova tima smatra da je sprint bio veoma dobro planiran i da su ciljevi i očekivani deliverable-i bili jasno definisani od početka sprinta. Funkcionalnosti planirane za Sprint 8 bile su logično povezane sa prethodnim sprintovima i predstavljale su nastavak razvoja ticket workflow sistema i korisničkog iskustva.

Jasno definisan scope omogućio je:
- stabilniju organizaciju rada,
- efikasniju raspodjelu zadataka,
- manji broj nesporazuma tokom implementacije,
- lakšu koordinaciju između frontend i backend dijela sistema.

Ipak, pojedini tehnički detalji i integracijski aspekti postajali su jasniji tek tokom same implementacije, što je zahtijevalo dodatne konsultacije i manje prilagodbe workflow logike.

---

# Glavni blokeri

- Merge konflikti između branch-eva.
- Integracija više funkcionalnosti unutar istih modula sistema.
- Dodatno vrijeme potrebno za testiranje i validaciju notifikacijskog sistema.
- Povremeno čekanje na review i potvrdu implementacija.
- Koordinacija izmjena između frontend i backend dijela sistema.
- Dodatna provjera autorizacije i pristupa podacima po korisničkim rolama.
- Stabilizacija pojedinih workflow funkcionalnosti nakon integracije.

Ipak, nijedan od navedenih problema nije značajno ugrozio završetak sprinta niti funkcionalnost sistema.

---

# Prijedlozi za sljedeći sprint

## Procesi i organizacija

- Ranije raspodijeliti kompleksnije taskove i preciznije definisati odgovornosti članova tima.
- Uvesti dodatne kratke sync sastanke radi lakše koordinacije većih funkcionalnosti.
- Nastaviti održavati visok nivo komunikacije i međusobne pomoći unutar tima.
- Jasnije planirati integraciju funkcionalnosti koje koriste iste module sistema.
- Nastaviti koristiti Pull Request review za zajedničke komponente i veće izmjene.

## Tehnički prijedlozi

- Dodatno unaprijediti administratorski dashboard i statističke izvještaje.
- Razmotriti implementaciju AI recommendation sistema za agente i administratore.
- Proširiti notifikacijski sistem dodatnim tipovima događaja i filtriranjem.
- Nastaviti unapređivati autorizaciju i sigurnosne provjere unutar sistema.
- Dodatno standardizovati backend workflow logiku i organizaciju endpointa.
- Proširiti testiranje real-time funkcionalnosti i SignalR integracije.

## Korištenje AI-a

- AI alati pokazali su se korisnim pri implementaciji i organizaciji određenih dijelova sistema.
- Potrebno je nastaviti detaljno validirati AI generisani kod prije merge-a.
- AI koristiti kao podršku pri implementaciji, dokumentaciji i analizi, ali ne kao zamjenu za code review i testiranje.
- Razmotriti potencijalnu integraciju AI funkcionalnosti u buduće administratorske i analitičke module sistema.

---

# Vještine i znanja koja bi timu pomogla

- Naprednije Git i branch management prakse.
- Bolje razumijevanje real-time komunikacije i SignalR arhitekture.
- Više iskustva sa integracijom kompleksnijih workflow sistema.
- Dodatno iskustvo sa autorizacijom i sigurnosnim pravilima u web aplikacijama.
- Bolja organizacija integracije većih funkcionalnosti između frontend i backend dijela sistema.
- Više iskustva sa testiranjem i validacijom real-time funkcionalnosti.
- Naprednije planiranje taskova i raspodjele rada kroz sprint.

---

# Pohvale i dodatni komentari

- Timska saradnja i komunikacija ocijenjene su veoma visoko.
- Članovi tima istaknuli su veoma pozitivnu atmosferu tokom rada.
- Tim je pokazao visok nivo međusobne pomoći, profesionalnosti i organizacije.
- Sprint je završen uspješno uz implementaciju velikog broja funkcionalnosti.
- Većina članova smatra da tim kontinuirano napreduje kroz svaki naredni sprint.
- Posebno je pohvaljena koordinacija između frontend i backend dijela tima.
- Tim je pokazao stabilnost pri radu na većem broju međusobno povezanih funkcionalnosti.
- Sprint 8 dodatno je unaprijedio kvalitet i stabilnost sistema podrške.

---

# TOP akcijske stavke za Sprint 9

| Prioritet | Akcija |
|---|---|
| 1 | Ranije raspodijeliti kompleksnije taskove i preciznije definisati odgovornosti |
| 2 | Smanjiti merge konflikte kroz češće sync-ovanje branch-eva |
| 3 | Dodatno unaprijediti dokumentaciju workflow logike i API endpointa |
| 4 | Proširiti testiranje real-time funkcionalnosti i notifikacija |
| 5 | Nastaviti koristiti Pull Request review za sve veće izmjene i zajedničke komponente |
