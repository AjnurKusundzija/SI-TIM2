# Sprint Retrospective – Sprint 5

Retrospektiva je provedena putem anonimnog Google Forms upitnika. Ukupno je odgovorilo **8 članova tima**.

---

## Opće ocjene sprinta

| Kategorija | Prosjek (1–5) |
|---|---|
| Ocjena sprinta u cjelini | **4.1** |
| Razina stresa / opterećenja | **3.3** |
| Zadovoljstvo suradnjom tima | **4.4** |

---

## Što je funkcioniralo dobro

- **Komunikacija i suradnja** istaknuti su kao najveća snaga sprinta — svi su bili dostupni i odgovarali promptno.
- **Alati** (Git, AI agenti, razvojna okruženja) su generalno funkcionirali bez većih tehničkih poteškoća.
- Korištenje **Git brancha** omogućilo je paralelni rad bez konflikata.
- **AI agenti** su bili posebno korisni i pohvaljeni od strane tima.

---

## Osobni uspjesi i highlight momenti

- Postignuta 100% pokrivenost zadataka.
- Uspješan deployment aplikacije.
- Implementacija i testiranje ticket modula (unit testovi).
- Korekcija koda u zadnjem trenutku uz isporuku dobrog dizajna.

---

## Što je usporavalo tim (problemi i frustracije)

- **Odlaganje rada do zadnjeg trenutka** — najčešće navođen problem (5+ odgovora). Većina posla završavana je pri kraju sprinta.
- **Tehničke poteškoće**: konfiguracija portova, environment varijable, deployment, merge konflikti.
- **Zavisnosti između zadataka** — neki članovi nisu mogli krenuti dok drugi nisu završili svoju komponentu.
- **Razumijevanje tuđeg koda** i postavljanje razvojnog okruženja.

---

## Što treba prestati raditi

- Koristiti **AI alate bez pregleda** generisanih promjena — AI ponekad mijenja portove, rute ili konfiguracije koje drugi developeri ne očekuju.
- Pokretati **git komande bez razumijevanja** efekta na repozitorij.
- Slati obavijesti putem **Vibera** koje ostaju nepročitane.

---

## Planiranje i jasnoća scope-a

Većina članova smatra da je sprint bio jasno planiran, no dio navodi da su neke stvari bile nejasne ili da se scope mijenjao tokom sprinta (ODL-2: uklanjanje detaljnog prikaza tiketa, dodavanje PB-22 i PB-23).

---

## Glavni blokeri

- Kasna raspodjela zadataka — taskovi nisu dodijeljeni odmah po konsultacijama.
- Konfiguracija environment varijabli i ključeva (riješeno na vrijeme).
- Problemi s portovima i deploymentom.
- Čekanje na feedback i code review.
- Kasni setup projekta — gubilo se vrijeme na početno podešavanje.

---

## Prijedlozi za sljedeći sprint

### Procesi i organizacija
- **Rasporediti taskove odmah nakon konsultacija** — ne čekati do polovice sprinta.
- **Uvesti konkretne interne deadlineove** za svakog člana, neovisno o finalnom roku.
- **Uvesti kratke daily sync sastanke** (5–10 min) kako bi svi bili u toku s napretkom i problemima.
- Razmotriti uvođenje **alata poput Jira ili Slack** za jasniji pregled rokova i statusa zadataka.

### Tehnički prijedlozi
- **Napisati API dokumentaciju** (lokalna dokumentacija ruta i resursa) kako bi svi znali šta šta radi i s čime komunicira.
- **Više koristiti Pull Request-ove** za code review.
- Svaki član treba elaborirati šta je napravio i šta je izmijenjeno, kako bi ostali bili upućeni u promjene.

### Korištenje AI-a
- **Pametnije koristiti AI** — pregledati svaku AI generisanu promjenu prije prihvatanja, posebno za konfiguracijske fajlove.

---

## Vještine i znanja koji bi timu pomogla

- Bolje razumijevanje **arhitekture projekta** i komunikacije između komponenti (frontend–backend–baza).
- Bolji **individualni menadžment vremena** i navika redovnog rada.
- **Tehničke vještine** vezane za Docker, lokalno okruženje i pokretanje projekta.

---

## Pohvale i dodatni komentari

> *"Zadovoljna sam saradnjom i mislim da kao tim dobro napredujemo."*

---

## TOP akcijske stavke za Sprint 6

| Prioritet | Akcija |
|---|---|
| 1 | Rasporediti taskove odmah nakon konsultacija |
| 2 | Napisati API dokumentaciju (lokalna, barem bazična) |
| 3 | Uvesti kratke sync sastanke (daily ili 2x sedmično) |
| 4 | Pametnije koristiti AI — obavezni pregled svih promjena |
| 5 | Koristiti PR-ove za sve promjene na zajedničkim komponentama |