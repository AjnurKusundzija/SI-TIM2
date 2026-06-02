# Sprint Goal – Sprint 10

## Sprint cilj

Cilj Sprinta 10 je implementirati AI-potpomognute funkcionalnosti sistema kroz modul prijedloga odgovora za agente i tehničare, modul AI uvida za administratore, kompletni redizajn korisničkog sučelja i proširenje administratorskih ovlasti nad tiketima kroz mogućnost preraspodjele agenata i tehničara direktno iz prikaza detalja tiketa.

Sprint je fokusiran na unapređenje svakodnevnog rada unutar helpdesk sistema kroz integraciju AI funkcionalnosti u postojeći ticket workflow. Agenti i tehničari dobijaju mogućnost korištenja AI prijedloga odgovora zasnovanih na sadržaju tiketa, historiji komunikacije i internoj knowledge base logici, dok administratori dobijaju AI uvide koji pomažu u analizi stanja sistema, opterećenja timova i mogućih problema u radu podrške.

Poseban fokus sprinta stavljen je na PB-57, odnosno AI prijedlog odgovora za agente i tehničare. Ova funkcionalnost treba omogućiti korisnicima sa odgovarajućim rolama da iz detaljnog prikaza tiketa pokrenu generisanje prijedloga odgovora, pregledaju predloženi tekst, po potrebi ga izmijene i tek nakon toga pošalju korisniku. AI prijedlog ne zamjenjuje agenta ili tehničara, nego služi kao pomoćni alat za brže, jasnije i konzistentnije odgovaranje na korisničke zahtjeve.

Dodatni cilj sprinta je implementacija PB-58, odnosno AI uvida za administratore. Ova funkcionalnost omogućava administratorima da na osnovu postojećih metrika sistema dobiju sažete uvide o stanju helpdesk procesa, potencijalnim problemima, opterećenju timova, broju otvorenih tiketa, sporim odgovorima i trendovima koji zahtijevaju pažnju. AI uvidi trebaju biti prikazani kroz administratorski dashboard i povezani sa postojećim metrikama sistema.

Sprint također uključuje PB-59, odnosno kompletni redizajn korisničkog sučelja. Cilj redizajna je unaprijediti vizualnu konzistentnost aplikacije, poboljšati preglednost navigacije, modernizovati Sidebar, Header i AppLayout, te uskladiti dashboard i ostale stranice sa novom navy paletom boja. Redizajn treba omogućiti ugodnije korištenje sistema za sve role: administratore, agente, tehničare i klijente.

U okviru redizajna posebna pažnja posvećena je organizaciji informacija na dashboardu, boljem prikazu kartica, trend indikatora, status chipa, key highlights sekcija i navigacije prema filtriranim tiketima. Cilj je da korisnik može brže razumjeti stanje sistema, pronaći relevantne funkcionalnosti i izvršiti potrebne akcije bez nepotrebnog pretraživanja interfejsa.

Sprint obuhvata i proširenje PB-31 kroz US-101, gdje administrator dobija mogućnost preraspodjele agenata i tehničara direktno iz TicketDetail prikaza. Ova funkcionalnost omogućava administratoru da efikasnije upravlja tiketima, promijeni odgovornu osobu i reaguje na promjene u dostupnosti ili opterećenju timova bez prelaska na zasebne administrativne ekrane.

Posebno je važno da se administratorske ovlasti jasno ograniče. Administrator može nadzirati i preraspodijeliti tiket, ali ne učestvuje u komunikaciji sa korisnikom kroz chat. Zbog toga sprint uključuje i zabranu slanja poruka administratorima u ticket chat, kako bi se jasno razdvojile administratorske i operativne uloge agenata i tehničara.

Tehnički fokus sprinta uključuje implementaciju backend AI servisa, `AIService` i `IAIService`, razvoj `AIController` endpointa za AI prijedloge i administratorske uvide, integraciju AI funkcionalnosti sa frontend komponentama, te implementaciju `AISuggestionModal`, `AIInsightsPanel` i `AIInsightsCard` komponenti. Dodatno se uvodi `uiStore.js` Zustand store za zajedničko upravljanje stanjem AI panela i alert komponenti.

Sprint također obuhvata stabilizaciju postojećeg sistema nakon većih UI izmjena, provjeru kompatibilnosti novog dizajna sa postojećim funkcionalnostima, validaciju role-based pristupa, proširenje test coverage-a za AI i UI funkcionalnosti, te dokumentovanje svih implementiranih promjena kroz Sprint Backlog, Decision Log, AI Usage Log i Proof of Testing.

Kroz Sprint 10 tim nastavlja razvijati sistem prema modernijem, inteligentnijem i korisnički ugodnijem helpdesk rješenju. Implementacijom AI podrške, administratorskih uvida i redizajniranog interfejsa sistem postaje efikasniji za rad podrške, pregledniji za administratore i jednostavniji za krajnje korisnike.

---

## Fokus sprinta

- Implementacija AI prijedloga odgovora za agente i tehničare
- Implementacija AI uvida za administratore
- Integracija AI servisa na backendu i frontendu
- Generisanje AI odgovora na osnovu sadržaja tiketa i historije komunikacije
- Pregled, uređivanje i potvrđivanje AI prijedloga prije slanja
- AI analiza stanja sistema na administratorskom dashboardu
- Implementacija `AIService` i `IAIService`
- Implementacija `AIController` endpointa
- Implementacija `AISuggestionModal` komponente
- Implementacija `AIInsightsPanel` i `AIInsightsCard` komponenti
- Kompletni redizajn Sidebara, Headera i AppLayouta
- Redizajn AdminDashboardSection komponente
- Implementacija trend indikatora i key highlights sekcija
- Uvođenje `uiStore.js` Zustand shared state logike
- Implementacija navy color palette u `index.css`
- Poboljšanje navigacije i vizualne konzistentnosti sistema
- Proširenje administratorskih ovlasti nad prosljeđivanjem tiketa
- Admin preraspodjela agenata i tehničara iz TicketDetail prikaza
- Zabrana slanja poruka administratorima u ticket chat
- Role-based validacija AI i administratorskih funkcionalnosti
- Integracija novih komponenti sa postojećim ticket workflow sistemom
- Refaktorisanje UI komponenti i layout strukture
- Proširenje test coverage-a za AI i UI funkcionalnosti
- Stabilizacija sistema nakon redizajna

---

## Očekivani deliverable-i

- PB-57 AI prijedlog odgovora za agente i tehničare
- PB-58 AI uvidi za administratore
- PB-59 Redizajn korisničkog sučelja
- PB-31 Proširenje — admin preraspodjela tiketa kroz US-101
- `AIService` i `IAIService` backend implementacija
- `AIController` sa endpointima za agent suggestion i admin insights
- `AISuggestionModal` frontend komponenta
- `AIInsightsPanel` frontend komponenta
- `AIInsightsCard` frontend komponenta
- `uiStore.js` Zustand store
- Redizajniran `Sidebar`
- Redizajniran `Header`
- Redizajniran `AppLayout`
- Redizajniran `AdminDashboardSection`
- Implementirani StatCard trend indikatori
- Implementirana key highlights sekcija
- Implementirana navy color palette u `index.css`
- AI response suggestion workflow unutar TicketDetail prikaza
- AI insights prikaz na administratorskom dashboardu
- Funkcionalnost „AI Uvidi“ dugmeta u Header komponenti
- Admin preraspodjela agenata i tehničara iz TicketDetail prikaza
- Zabrana slanja poruka administratorima u chat tiketa
- Role-based validacija AI funkcionalnosti
- Role-based validacija administratorskih akcija
- Refaktorisane dashboard i layout komponente
- Integracija AI modula sa postojećim ticket workflow sistemom
- Ažurirani Sprint Backlog
- Decision Log
- AI Usage Log
- Unit testovi i rezultati testiranja
- Dokumentacija implementiranih funkcionalnosti

---

## Sprint Review kriterij

Ocjenjuje se funkcionalnost AI prijedloga odgovora, ispravnost AI uvida na administratorskom dashboardu, kvalitet vizualnog redizajna, stabilnost integracije novih komponenti sa postojećim sistemom i ispravnost proširenih administratorskih ovlasti nad tiketima.

Posebna pažnja biće posvećena tome da AI funkcionalnosti budu korisne, ali kontrolisane. Agent ili tehničar mora imati mogućnost pregleda i uređivanja AI prijedloga prije slanja, dok administrator mora imati pristup AI uvidima, ali ne i mogućnost slanja poruka kroz ticket chat.

Review uključuje provjeru:

- Funkcionalnosti AI prijedloga odgovora u TicketDetail prikazu za agente i tehničare
- Ispravnosti generisanja prijedloga na osnovu sadržaja tiketa
- Mogućnosti uređivanja AI prijedloga prije slanja
- Ispravnosti AI uvida na admin dashboardu
- Funkcionalnosti „AI Uvidi“ dugmeta u Header komponenti
- Prikaza AI insights panela i AI insights kartica
- Vizualne konzistentnosti novog dizajna kroz sve stranice
- Ispravnosti redizajniranog Sidebara, Headera i AppLayouta
- Ispravnosti status chipa u Sidebaru
- Navigacije na filtrirane tikete iz dashboard i sidebar prikaza
- Funkcionalnosti redizajniranog AdminDashboardSection prikaza
- Funkcionalnosti trend indikatora i key highlights sekcija
- Funkcionalnosti admin preraspodjele agenata i tehničara iz TicketDetail prikaza
- Zabrane slanja poruka za administratore u chat tiketa
- Ispravnosti role-based autorizacije za AI funkcionalnosti
- Ispravnosti role-based autorizacije za administratorske akcije
- Stabilnosti frontend i backend AI integracije
- Funkcionalnosti Zustand shared state logike
- Stabilnosti dashboard i layout komponenti
- Responzivnosti i UX konzistentnosti sistema
- Pokrivenosti testovima i stabilnosti aplikacije

---

## Održani sastanci

- Sprint planning sastanak
- Koordinacija implementacije AI modula
- Dogovor oko knowledge base strukture i AI servisne arhitekture
- Dogovor oko načina generisanja AI prijedloga odgovora
- Tehnički sastanci za organizaciju AI API endpointa
- Review sastanci za frontend redizajn
- Koordinacija implementacije Zustand shared state logike
- Sastanci vezani za AI response workflow
- Sastanci vezani za vizualni dizajn i UX poboljšanja
- Dogovor oko navy color palette i UI standarda
- Dogovor oko administratorskih ovlasti i ograničenja u TicketDetail prikazu
- Review sastanci za UI konzistentnost
- Koordinacija frontend i backend AI integracije
- Sastanci vezani za optimizaciju dashboard komponenti
- Review sastanci za AI insight funkcionalnosti
- Tehnički sastanci za testiranje AI i redesign modula
- Koordinacija dokumentacije Sprint Backloga, Decision Loga i AI Usage Loga
