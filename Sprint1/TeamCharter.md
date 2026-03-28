

| TEAM CHARTER Softverski inžinjering · 2025/26 Grupa 2  |  Tema 2: Helpdesk i ticketing sistem |
| ----- |

# **1\. Naziv i članovi tima**

**Naziv tima:** Grupa 2

| Ime i prezime | Indeks | Kontakt |
| :---- | :---- | :---- |
| Uma Mahmutović | 19623 | umahmutovi1@etf.unsa.ba |
| Lejan Kozlić | 19279 | lkozlic1@etf.unsa.ba |
| Ajnur Kušundžija | 19621 | akusundzij1@etf.unsa.ba |
| Ajdin Dželo | 19540 | adzelo2@etf.unsa.ba |
| Eldar Hadžiselimović | 19434 | ehadziseli1@etf.unsa.ba |
| Hana Piralić | 19690 | hpiralic1@etf.unsa.ba |
| Lamija Maglić | 19258 | lmaglic1@etf.unsa.ba |
| Merisa Ogrić | 19768 | mogric1@etf.unsa.ba |

# **2\. Način komunikacije**

## **2.1 Kanali komunikacije**

* Primarni kanal: Discord grupni chat (brze poruke i koordinacija)

* Sekundarni kanal: E-mail za formalniju komunikaciju i razmjenu dokumenatan (uključujući i zajednički Google Drive)

* Repozitorij: GitHub (kod, dokumentacija, verzionisanje)

* Alati za zadatke: Github (board, backlog, issues)

## **2.2 Očekivano vrijeme odgovora**

* Na poruke u grupnom chatu: u roku od 4 sata tokom radnih dana

* Na e-mail: u roku od 24 sata

* U slučaju hitnosti (dan pred sastanak): odgovor u roku od 2 sata

* Ako je član nedostupan duže od 8 sati, obavještava tim unaprijed

## **2.3 Zakazivanje i održavanje sastanaka**

* Sedmični timski sastanak: fleksibilno vrijeme interno dogovoreno na sedmičnoj bazi – online putem Discord, po potrebi uživo, na interno dogovorenoj lokaciji

* Sedmični sastanak sa asistentom / Product Ownerom: prema rasporedu (Ponedjeljkom u 19:00 ili Utorkom u 20:00)

* Vanredni sastanci: zakazuju se u grupi po potrebi (u slučaju blokera i slično)

* Sažetak sastanaka bilježi dokumentarista tima

* Odsustvo s dogovorenog sastanka: član je dužan obavijestiti tim unaprijed i naknadno pregledati bilješke

# **3\. Radna pravila tima**

## **3.1 Kada i kako tim radi zajedno**

* Tim radi kontinuirano tokom sedmice u sedmičnim sprintovima

* Svaki sprint počinje planiranjem (sprint planning) i završava review-om na sastanku s PO-om

* Timski rad se odvija asinhrono (svako radi po dogovorenim vremenskim okvirima) i sinhrono (zajednički sastanci)

* Minimalni angažman po članu: Ispunjavanje dodijeljenog zadatka i prisustvo na 70% sastanaka

## **3.2 Dogovor i praćenje zadataka**

* Zadaci se raspoređuju na sprint planningu i upisuju u Github board

* Svaki zadatak ima jasno definisanu odgovornu/e osobu/e, opis i rok

* Status zadataka može se ažurirati svakodnevno (To Do → In Progress → Done)

* Ako član ne može završiti zadatak u roku, obavještava tim kako bi se pronašlo rješenje

* Evidencija rada vodi se kroz historiju na GitHubu i sprint backlog

## **3.3 Dokumentovanje odluka**

* Sve važne projektne, arhitektonske, zahtjevne, procesne i tehničke odluke bilježe se u Decision Log

* Decision Log sadrži: datum, problem/pitanje, razmatrane opcije, odabranu opciju i razlog

* Za svaku odluku donesenu uz pomoć AI alata (u fazi gdje je to dozvoljeno) evidentira se i u AI Usage Logu

* Bilješke sa timskih sastanaka čuvaju se na zajedničkom Google Drive-u

## **3.4 Rješavanje neslaganja**

* Neslaganja se prvo razrješavaju direktnim razgovorom između članova kojih se tiče

* Ako se neslaganje ne može riješiti dvostrano, iznosi se na timski sastanak

* Odluka se donosi glasanjem (natpolovična većina)

* Vođa tima ima pravo razrješavajućeg glasa u slučaju izjednačenja

* Ako se neslaganje ne može riješiti interno, delegira se nastavnom osoblju

# **4\. Početne odgovornosti članova tima**

## **4.1 Koordinacija sprinta**

* Sprint planning se održava na početku svake sedmice

* Sprint cilj se dogovara zajednički i zapisuje u sprint backlog prije početka rada

* Napredak se prati svakodnevno kroz ažuriranje statusa zadataka na Github board-u

* Na sedmičnom sastanku s Product Ownerom tim zajedno predstavlja rezultate i predlaže plan za naredni sprint

* Svaki bloker ili kašnjenje prijavljuje se timu čim se pojavi, a ne tek na kraju sedmice

## **4.2 Backlog i zahtjevi**

* Product Backlog se vodi kao živi dokument i ažurira nakon svakog novog saznanja ili povratne informacije od PO-a

* User stories se pišu zajedno kao tim, uz jasne kriterije prihvatljivosti koji su mjerljivi i testabilni

* Prioritizacija backlog stavki vrši se u dogovoru s Product Ownerom na sedmičnim sastancima

* Otvorena pitanja i pretpostavke o zahtjevima bilježe se i razjašnjavaju s PO-om prije implementacije

* Svaka izmjena scope-a ili zahtjeva evidentira se u backlogu sa kratkim obrazloženjem

## **4.3 Arhitektura i tehničke odluke**

* Arhitektonske odluke donose se zajednički na timskim sastancima

* Sve važne tehničke odluke (izbor tehnologije, struktura sistema, pristupi rješavanju) bilježe se u Decision Log

* Nijedna velika tehnička promjena ne provodi se bez prethodnog dogovora unutar tima

* U AI-enabled fazi, svako korištenje AI za tehničke prijedloge evidentira se u AI Usage Logu

## **4.4 Testiranje i kvalitet**

* Tim se drži dogovorene Definition of Done – stavka nije završena dok ne zadovoljava sve kriterije

* Testovi se pišu paralelno s implementacijom, a ne nakon završetka razvoja

* Svaka implementirana stavka prolazi pregled unutar tima prije nego se označi kao Done

* Rezultati testiranja dokumentuju se i dostupni su za pregled na svakom sprint reviewu

* Poznati bugovi i tehnički problemi evidentiraju se u backlogu i adresiraju u trenutnom ili narednim sprintovima

## **4.5 Dokumentacija i evidencije**

* Sprint Review Summary i Sprint Retrospective Summary pišu se nakon svakog sprinta, prije narednog sastanka

* Decision Log i AI Usage Log ažuriraju se kontinuirano

* Svi artefakti čuvaju se u zajedničkom repozitoriju i/ili Google Drive-u, organizovani po sprintovima

* Dokumentacija mora biti pregledna i razumljiva svim članovima tima

* Na kraju semestra tim zajedno priprema korisničku i tehničku dokumentaciju za završnu demonstraciju

# **5\. Pravila u slučaju neispunjavanja obaveza**

## **5.1 Kako se član upozorava**

* Korak 1 – Razgovor: Vođa tima razgovara s članom koji nije ispunio obavezu i zajedno traže rješenje. Rok za popravak se dogovara.

* Korak 2 – Pisano upozorenje: Ako problem potraje, vođa šalje pisano upozorenje (e-mail, poruka u grupi) i bilježi incident.

* Korak 3 – Timska diskusija: Problem se iznosi na timski sastanak kako bi cijeli tim bio informisan i dogovorene se eventualne izmjene u raspodjeli zadataka.

## **5.2 Kada se problem prijavljuje nastavnom osoblju**

* Problem se prijavljuje nastavnom osoblju kada: interni koraci nisu dali rezultat nakon razumnog perioda (npr. 1-2 sprinta), i kada nerad jednog člana direktno ugrožava kvalitet i rokove projekta.

* Prijava mora biti pravovremena i dokumentovana.

* Uz prijavu se dostavljaju konkretni primjeri: koje obaveze nisu izvršene, u kojim sprintovima, i koji su poduzeti interni koraci.

## **5.3 Evidencija problema u timu**

* Svaki zabilježeni incident evidentira se u timu (Problem Log): datum, opis, poduzeti koraci, ishod.

* Problem Log je interni dokument tima, ali može biti dostavljen nastavnom osoblju na zahtjev.

* Cilj evidencije je transparentnost i pravovremeno rješavanje.

