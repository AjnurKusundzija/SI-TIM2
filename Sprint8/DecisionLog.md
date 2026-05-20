# Decision Log - UPDATE ZA SPRINT 8

Decision Log se koristi za evidentiranje važnih projektnih, zahtjevnih, arhitektonskih, tehničkih i procesnih odluka.

Decision Log treba pokazati da tim ne radi nasumično, nego svjesno donosi i prati odluke.

---

## Odluka #1

| Polje | Detalji |
|---|---|
| **ID odluke** | ODL-S8-1 |
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
| **ID odluke** | ODL-S8-2 |
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
| **ID odluke** | ODL-S8-3 |
| **Datum** | 16.05.2026 |
| **Kratak naziv odluke** | Dodavanje PB-50 kao zasebne stavke za metriku prvog odgovora u admin izvještajima |
| **Opis problema ili pitanja** | Redefinisanjem PB-42, originalna funkcionalnost "Prosječno vrijeme prvog odgovora" kao admin metrika nije izgubljena, ali nije bila smještena ni u jednu PB stavku. Potrebno ju je formalno evidentirati za Sprint 11. |
| **Razmatrane opcije** | 1. Uključiti metriku u PB-45 Admin Dashboard bez zasebne stavke. <br> 2. Dodati PB-50 kao eksplicitnu stavku za ovu metriku u Sprint 11. |
| **Odabrana opcija** | 2. Dodati PB-50 kao eksplicitnu stavku za ovu metriku u Sprint 11. |
| **Razlog izbora** | Admin Dashboard (PB-45) je već složena stavka (L). Odvajanje metrike prvog odgovora u zasebnu stavku osigurava bolji granularitet planiranja, jasniji scope i lakše testiranje u okviru Sprint 11. Podaci za izračun već postoje u bazi (`Comment.DateTime` i `Ticket.CreatedDate`) pa nema potrebe za dodatnom migracijom. |
| **Posljedice odluke** | PB-50 "Prosječno vrijeme prvog odgovora — izvještaj za admina" dodana je u Product Backlog sa prioritetom 2 i složenošću S za Sprint 11. |
| **Status odluke** | aktivna |

---


## Odluka #4

| Polje | Detalji |
|---|---|
| **ID odluke** | ODL-S8-4 |
| **Datum** | 16.05.2026 |
| **Kratak naziv odluke** | Klijent dobija `TICKET_FORWARDED` notifikaciju pri prosljeđivanju tiketa |
| **Opis problema ili pitanja** | Originalna implementacija je slala `TICKET_FORWARDED` notifikaciju samo novom agentu ili tehničaru. Klijent nije bio informisan da je odgovorna osoba za njegov tiket promijenjena. |
| **Razmatrane opcije** | 1. Samo novi agent/tehničar dobija notifikaciju (originalno). <br> 2. I novi agent/tehničar i klijent (kreator tiketa) dobijaju notifikaciju. |
| **Odabrana opcija** | 2. I klijent dobija `TICKET_FORWARDED` notifikaciju. |
| **Razlog izbora** | Klijent mora biti informisan o promjeni odgovorne osobe bez potrebe za osvježavanjem stranice. Ovo je osnovna zahtijevana transparentnost helpdesk sistema. |
| **Posljedice odluke** | U `TicketService.ExecuteForwardAsync` i `ForwardTicketToTechnicianAsync` dodato je slanje notifikacije na `ticket.CreatorId` uz već postojeće slanje na novog agenta/tehničara. |
| **Status odluke** | aktivna |

---

## Odluka #5

| Polje | Detalji |
|---|---|
| **ID odluke** | ODL-S8-5 |
| **Datum** | 16.05.2026 |
| **Kratak naziv odluke** | Uvođenje sistemskih poruka u chatu tiketa pri prosljeđivanju |
| **Opis problema ili pitanja** | Notifikacija informiše primatelja, ali ne ostavlja trajan trag u chatu tiketa. Klijent i ostali učesnici nemaju uvid u historiju prosljeđivanja direktno u razgovoru. |
| **Razmatrane opcije** | 1. Samo notifikacija, bez zapisa u chatu. <br> 2. Notifikacija + automatska sistemska poruka u chatu ("Tiket je proslijeđen agentu: Ime Prezime"). <br> 3. Posebna sekcija "Historija dodjela" na tiketu. |
| **Odabrana opcija** | 2. Notifikacija + sistemska poruka u chatu. |
| **Razlog izbora** | Sistemska poruka u chatu ostavlja trajan, vidljiv zapis svim učesnicima. Implementaciono je najjednostavnija opcija jer koristi postojeću infrastrukturu komentara. Opcija 3 zahtijeva poseban UI element i API endpoint. |
| **Posljedice odluke** | Dodat je `bool IsSystemMessage` i nullable `int? AuthorId` na `Comment` entitet. Kreirana EF migracija `AddSystemCommentToComment`. `CommentService` dobija `AddSystemCommentAsync` metodu i `IChatPusher` za real-time broadcast. Frontend prikazuje sistemske poruke kao centriranu pill liniju bez avatara. |
| **Status odluke** | aktivna |

---

## Odluka #6

| Polje | Detalji |
|---|---|
| **ID odluke** | ODL-S8-6 |
| **Datum** | 19.05.2026 |
| **Kratak naziv odluke** | Prosljeđivanje tehničaru ne uklanja agenta kao aktivnog assigneeja |
| **Opis problema ili pitanja** | Postojeća logika je tretirala posljednju dodjelu kao jedinog aktivnog vlasnika tiketa. Kada agent proslijedi tiket tehničaru, tehničar postaje posljednji assignee, pa agent više nije vidio tiket u dodijeljenim tiketima, nije vidio "Čeka se" tiket u svom toku rada i taj tiket nije ulazio u agentovu statistiku. |
| **Razmatrane opcije** | 1. Zadržati "posljednja dodjela je jedini vlasnik" pravilo. <br> 2. Uvesti zasebnu tabelu/flag za aktivne assigneeje. <br> 3. Tretirati `FORWARDED_TO_TECHNICIAN` kao poseban slučaj u kojem ostaju aktivni i prethodni agent i novi tehničar. |
| **Odabrana opcija** | 3. `FORWARDED_TO_TECHNICIAN` zadržava prethodnog agenta kao aktivnog assigneeja i dodaje tehničara kao drugog aktivnog assigneeja. |
| **Razlog izbora** | Ova opcija najbolje odgovara poslovnom toku: agent ostaje odgovoran za praćenje tiketa, dok tehničar rješava terenski dio. Implementaciono ne zahtijeva novu migraciju niti promjenu modela podataka, nego samo jasnije tumačenje historije dodjela. |
| **Posljedice odluke** | Repozitorijski upiti za dodijeljene tikete, nedavne tikete i statistiku koriste zajedničku logiku aktivnih assigneeja. Detalji tiketa prikazuju odvojeno `Agent` i `Tehničar`. Forward na drugog agenta i dalje prebacuje vlasništvo na novog agenta i ne zadržava prethodnog agenta aktivnim. |
| **Status odluke** | aktivna |

---

## Odluka #7

| Polje | Detalji |
|---|---|
| **ID odluke** | ODL-S8-7 |
| **Datum** | 19.05.2026 |
| **Kratak naziv odluke** | Closure workflow obavještava sve aktivno dodijeljene učesnike |
| **Opis problema ili pitanja** | Notifikacije su postojale za promjenu statusa i zatvaranje tiketa, ali closure workflow nije potpuno pokrivao staff korisnike. Kada klijent prihvati ili odbije zatvaranje, agent i tehničar moraju znati da li je tiket završen ili se obrada nastavlja. |
| **Razmatrane opcije** | 1. Slati notifikaciju samo klijentu ili samo korisniku koji je zadnji dodijeljen. <br> 2. Slati notifikaciju svim korisnicima iz kompletne historije dodjela. <br> 3. Slati notifikaciju samo aktivno dodijeljenim staff korisnicima, uz posebno pravilo za agent+tehničar dodjelu. |
| **Odabrana opcija** | 3. Notifikacije se šalju aktivno dodijeljenim staff korisnicima; kod prisilnog zatvaranja notifikacija ide klijentu. |
| **Razlog izbora** | Aktivni assigneeji su jedini korisnici koji trenutno imaju operativnu odgovornost za tiket. Slanje cijeloj historiji dodjela bi stvaralo šum, dok slanje samo posljednjem assigneeju propušta agenta koji i dalje prati tiket nakon prosljeđivanja tehničaru. |
| **Posljedice odluke** | `AcceptClosureAsync` šalje `TICKET_CLOSED` aktivnom staffu, `RejectClosureAsync` šalje `STATUS_CHANGED` aktivnom staffu, a `ForceCloseAsync` šalje `TICKET_CLOSED` klijentu. Agent/tehničar koji nije aktivno dodijeljen ne može prisilno zatvoriti tiket; administrator zadržava privilegiju prema postojećem role modelu. |
| **Status odluke** | aktivna |

---

Napomena: Ovaj Decision Log je živi dokument i ažurira se kroz sprintove.
