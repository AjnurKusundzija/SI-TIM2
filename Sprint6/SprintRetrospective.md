# Sprint Retrospective – Sprint 6

---

## Opće ocjene sprinta

| Kategorija | Prosjek (1–5) |
|---|---|
| Ocjena sprinta u cjelini | 4.8 |
| Razina stresa / opterećenja | 2.0 |
| Zadovoljstvo suradnjom tima | 4.0 |

> Najuspješniji sprint do sada po ocjeni — i najmanji stres. Tim je vidno sazrio u načinu rada.

---

## Što je funkcioniralo dobro

- Komunikacija između članova bila je primjetno bolja nego u prethodnim sprintovima — koordinacija je tekla bez većih zastoja.
- Git workflow i organizacija branch-eva uglavnom su funkcionirali bez konflikata.
- AI alati su i dalje korisni — korišteni za dokumentaciju, testiranje i implementaciju ticket funkcionalnosti.
- Zainteresovanost i zalaganje svih članova tima — bez primjedbi, svi su ulagali trud.
- Backlog stavke su bile logično isplanirane, inkrement je bio smislen i kohezivan.

---

## Osobni uspjesi i highlight momenti

- Uspješna integracija ticket detalja i komunikacije kroz tiket.
- Implementacija WebSocket funkcionalnosti.
- Završavanje većine planiranih funkcionalnosti bez velikih blokera pred kraj sprinta.
- Timski rad i međusobno povjerenje — istaknuto kao posebna vrijednost ovog sprinta.

> "Kada neko napravi 'grešku', bez ikakvih primjedbi ili ljutnje prihvati istu, čak se izvini, i pokuša je riješiti. A od ostalih članova ekipe se ne može osjetiti ni trunka osuđivanja."

---

## Što je usporavalo tim (problemi i frustracije)

- Zavisnosti između PB-ova — dio članova nije mogao krenuti dok drugi nisu završili svoju komponentu, problem koji se ponavlja iz Sprinta 5.
- Prvomajski praznici — izgubljeno je nekoliko radnih dana, što je stisnulo vremenski okvir.
- Povremeni merge konflikti i problemi s autorizacijom ruta i frontend-backend integracijom.
- Nejasnoće oko UI-a po rolama — nije bilo precizno definirano kako UI treba izgledati za različite role, što će je zahtjevalo detaljniju analizu za sprint.
- Čekanje na feedback i code review i dalje prisutno kao usporavajući faktor.

---

## Što treba prestati raditi

- Prihvatati AI generirane izmjene bez detaljnog pregleda prije pushanja — posebno konfiguracije i rute.
- Ostavljati posao za dan prije meetinga — prepoznato kao najštetnija navika tima.

---

## Planiranje i jasnoća scope-a

Većina članova smatra da je sprint bio jasno planiran. Jedina primjedba odnosi se na UI definiciju po rolama koja nije bila precizno određena na početku, što je uzrokovalo dodatni posao. Za Sprint 7 preporučuje se preciznije definisanje UI/UX zahtjeva po rolama još u fazi planiranja.

---

## Glavni blokeri

- Zavisnosti između taskova — blokirale su paralelni rad pojedinih članova.
- Prvomajski praznici  — skratili efektivni radni period sprinta.
- Problemi s merge konfliktima — (riješeno kroz koordinaciju tima).
- Nejasna UI specifikacija po rolama 

---

## Prijedlozi za sljedeći sprint

### Procesi i organizacija
- Uvesti interne rokove — prijedlog: ako se rok ne ispoštuje, zadatak automatski prelazi na drugu osobu (interni "minus" za propuštanje).
- Kraći sync sastanci tokom sedmice za praćenje napretka i ranu detekciju blokatora.
- Svaki član na Discordu/Viberu treba kratko napisati šta je uradio i priložiti screenshot — transparentnost bez overhead-a.
- Ranije raspoređivanje taskova — odmah po konsultacijama, ne čekati.

### Tehnički prijedlozi
- Obavezni code review (PR-ovi) prije svakog merge-a na zajedničke brancheve.
- Svako piše kratke testove za svoju komponentu — ne prepuštati testiranje na kraj.
- Precizno definisati UI po rolama na početku sljedećeg sprinta, prije implementacije.

### Korištenje AI-a
- Nastaviti pametno koristiti AI alate, uz obavezan pregled svake generirane promjene — posebno konfiguracije, portova i ruta.

---

## Vještine i znanja koji bi timu pomogla

- Bolje razumijevanje pokretanja projekta lokalno, kroz Docker i sl. — smanjilo bi onboarding friku na početku sprinta.
- Prompt inženjering i review AI koda — bolje iskorištavanje AI alata uz manji rizik.
- Backend autorizacija i organizacija frontend-backend integracije.

---

## Pohvale i dodatni komentari

> "Nažalost, i opet kažem, NAŽALOST, čovjek shvati koliko su njegovi problemi nebitni tek kada vidi probleme drugih ljudi. Tek kada sam vidio kako može nastati razdor u ekipi... shvatio sam da ipak super funkcionišemo. Siguran sam da bi mi bilo iz ekipe pomogao bez ikakvog osuđivanja."

> "Smatram da je timski rad bio dosta bolji nego u ranijim sprintovima i da je organizacija implementacije ticket sistema bila uspješna."

---

## TOP akcijske stavke za Sprint 7

| Prioritet | Akcija |
|---|---|
| 1 | Uvesti interne rokove s automatskim prelaskom zadatka ako se ne ispoštuju |
| 2 | Obavezni code review (PR) prije svakog merge-a na zajedničke brancheve |
| 3 | Precizno definisati UI po rolama na početku sprinta |
| 4 | Kraći sync sastanci 2x sedmično (Discord/Viber update s kratkim opisom i screenshotom) |
| 5 | Svako piše testove za svoju komponentu — ne prepuštati na kraj |
