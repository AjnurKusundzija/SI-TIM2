# Decision Log - UPDATE ZA SPRINT 8

Decision Log se koristi za evidentiranje važnih projektnih, zahtjevnih, arhitektonskih, tehničkih i procesnih odluka.

Decision Log treba pokazati da tim ne radi nasumično, nego svjesno donosi i prati odluke.

---

## Odluka #1

| Polje | Detalji |
|---|---|
| **ID odluke** | ODL-S8-2 |
| **Datum** | 16.05.2026 |
| **Kratak naziv odluke** | Redefinisanje PB-42 iz metrike izvještaja u statistiku agenta i tehničara |
| **Opis problema ili pitanja** | PB-42 je originalno definisan kao "Prosječno vrijeme prvog odgovora" — agregirana metrika za admin izvještaje. Međutim, bez Admin Dashboarda (PB-45, planiranog za Sprint 11) ova metrika nema gdje biti prikazana, što je činilo PB-42 besmislenim u kontekstu Sprint 8. |
| **Razmatrane opcije** | 1. Zadržati PB-42 u originalnom obliku i odgoditi ga za Sprint 11. <br> 2. Redefinisati PB-42 kao prikaz lične statistike rada za agente i tehničare na profilnoj stranici. <br> 3. Prikazati metriku direktno na tiketu (klijent/tehničar view). |
| **Odabrana opcija** | 2. Redefinisati PB-42 kao prikaz lične statistike rada za agente i tehničare na profilnoj stranici. |
| **Razlog izbora** | Statistika rada (broj otvorenih/zatvorenih tiketa, prosječno vrijeme prvog odgovora, prosječna ocjena) je korisna agentima i tehničarima odmah, bez potrebe za admin dashboardom. Ujedno postavlja temelj za izvještajni modul u Sprint 11. |
| **Posljedice odluke** | PB-42 je redefinisan sa novim nazivom "Statistika agenta i tehničara" i novim acceptance kriterijima. Originalna metrika "Prosječno vrijeme prvog odgovora" za admin izvještaje dodata je kao nova stavka PB-50 u Sprint 11. |
| **Status odluke** | aktivna |

---

## Odluka #2

| Polje | Detalji |
|---|---|
| **ID odluke** | ODL-S8-3 |
| **Datum** | 16.05.2026 |
| **Kratak naziv odluke** | Dodavanje PB-49 Notifikacije kao nove backlog stavke |
| **Opis problema ili pitanja** | Notifikacije nisu bile eksplicitno definisane kao Product Backlog stavka, iako je infrastruktura (entitet `Notification`, `NotificationType` enum, repozitorij, servis i kontroler kao skeleton) bila pripremljena u Sprint 7. Bez formalne PB stavke, implementacija nije mogla biti planirana ni praćena. |
| **Razmatrane opcije** | 1. Implementirati notifikacije kao dio postojeće PB stavke bez zasebnog evidentiranja. <br> 2. Dodati PB-49 kao novu eksplicitnu stavku u Product Backlog. |
| **Odabrana opcija** | 2. Dodati PB-49 kao novu eksplicitnu stavku u Product Backlog. |
| **Razlog izbora** | Notifikacije su zasebna, složena funkcionalnost (L složenosti) koja obuhvata backend logiku generisanja, API endpointe i frontend prikaz. Evidentiranje kao zasebna PB stavka osigurava pravilno planiranje, dodjelu i praćenje implementacije. |
| **Posljedice odluke** | PB-49 Notifikacije dodana je u Product Backlog sa prioritetom 1 i složenošću L za Sprint 8. Infrastruktura iz Sprint 7 (skeleton entiteti i klase) bit će iskorištena kao osnova implementacije. |
| **Status odluke** | aktivna |

---

## Odluka #3

| Polje | Detalji |
|---|---|
| **ID odluke** | ODL-S8-4 |
| **Datum** | 16.05.2026 |
| **Kratak naziv odluke** | Dodavanje PB-50 kao zasebne stavke za metriku prvog odgovora u admin izvještajima |
| **Opis problema ili pitanja** | Redefinisanjem PB-42, originalna funkcionalnost "Prosječno vrijeme prvog odgovora" kao admin metrika nije izgubljena, ali nije bila smještena ni u jednu PB stavku. Potrebno ju je formalno evidentirati za Sprint 11. |
| **Razmatrane opcije** | 1. Uključiti metriku u PB-45 Admin Dashboard bez zasebne stavke. <br> 2. Dodati PB-50 kao eksplicitnu stavku za ovu metriku u Sprint 11. |
| **Odabrana opcija** | 2. Dodati PB-50 kao eksplicitnu stavku za ovu metriku u Sprint 11. |
| **Razlog izbora** | Admin Dashboard (PB-45) je već složena stavka (L). Odvajanje metrike prvog odgovora u zasebnu stavku osigurava bolji granularitet planiranja, jasniji scope i lakše testiranje u okviru Sprint 11. Podaci za izračun već postoje u bazi (`Comment.DateTime` i `Ticket.CreatedDate`) pa nema potrebe za dodatnom migracijom. |
| **Posljedice odluke** | PB-50 "Prosječno vrijeme prvog odgovora — izvještaj za admina" dodana je u Product Backlog sa prioritetom 2 i složenošću S za Sprint 11. |
| **Status odluke** | aktivna |

---

Napomena: Ovaj Decision Log je živi dokument i ažurira se kroz sprintove.
