# Sprint Goal – Sprint 10

## Sprint cilj

Cilj sprinta je implementirati AI-potpomognute funkcionalnosti sistema kroz modul prijedloga odgovora za agente i tehničare te modul AI uvida za administratore, izvršiti kompletni redizajn korisničkog sučelja radi poboljšanja korisničkog iskustva i vizualne konzistentnosti, te proširiti administratorske ovlasti nad tiketima kroz mogućnost preraspodjele agenata i tehničara direktno iz prikaza detalja tiketa.

Sprint je fokusiran na integraciju AI funkcionalnosti u svakodnevni workflow helpdesk sistema — agenti i tehničari dobivaju AI prijedloge odgovora zasnovane na internoj knowledge base, dok administratori dobivaju automatski generisane uvide o stanju sistema. Poseban fokus stavljen je na kompletni vizualni revamp koji donosi moderni, konzistentni dizajn s tamnom navy paletom, poboljšanom navigacijom i boljim UX-om za sve role.

---

## Fokus sprinta

- Implementacija AI prijedloga odgovora za agente i tehničare
- Implementacija AI uvida za administratore
- Integracija AI servisa (backend + frontend)
- Kompletni redizajn Sidebara, Headera i AppLayouta
- Redizajn AdminDashboardSection s trend indikatorima i key highlights
- Zustand shared state za AI panel i alert stanje
- Navy color palette s realnim tamnim hex vrijednostima
- Proširenje administratorskih ovlasti nad prosljeđivanjem tiketa
- Zabrana slanja poruka u chat za administratore
- Integracija frontend i backend AI modula
- Proširenje test coverage-a za nove AI i UI funkcionalnosti

---

## Očekivani deliverable-i

- PB-57 AI prijedlog odgovora za agente i tehničare
- PB-58 AI uvidi za administratore
- PB-59 Redizajn korisničkog sučelja
- PB-31 Proširenje — admin preraspodjela tiketa (US-101)
- `AIService` i `IAIService` backend implementacija
- `AIController` s endpointima za agent suggestion i admin insights
- `AISuggestionModal` frontend komponenta
- `AIInsightsPanel` i `AIInsightsCard` frontend komponente
- `uiStore.js` Zustand store
- Redesigniran `Sidebar`, `Header`, `AppLayout`
- Redesigniran `AdminDashboardSection` s StatCard trendom i key highlights
- Navy color palette u `index.css`
- Ažurirani Sprint Backlog
- Decision Log
- AI Usage Log
- Unit testovi i rezultati testiranja
- Dokumentacija implementiranih funkcionalnosti

---

## Sprint Review kriterij

Ocjenjuje se funkcionalnost AI prijedloga odgovora, ispravnost AI uvida na dashboardu, kvalitet vizualnog redizajna i stabilnost integracije novih komponenti sa postojećim sistemom.

Review uključuje provjeru:
- Funkcionalnosti AI prijedloga odgovora u TicketDetail za agente i tehničare
- Ispravnosti AI uvida na admin dashboardu
- Funkcionalnosti „AI Uvidi" dugmeta u headeru
- Vizualne konzistentnosti novog dizajna kroz sve stranice
- Ispravnosti status chipa u sidebaru i navigacije na filtrirane tikete
- Funkcionalnosti admin preraspodjele agenata/tehničara iz TicketDetail
- Zabrane slanja poruka za administratore u chat tiketa
- Stabilnosti frontend i backend integracije
- Pokrivenosti testovima i stabilnosti aplikacije

---

## Održani sastanci

- Sprint planning sastanak
- Koordinacija implementacije AI modula
- Dogovor oko knowledge base strukture i AI servisne arhitekture
- Review sastanci za frontend redizajn
- Koordinacija implementacije Zustand shared state
- Tehnički sastanci za organizaciju AI API endpointa
- Sastanci vezani za vizualni dizajn i UX poboljšanja
- Dogovor oko administratorskih ovlasti i ograničenja u TicketDetail
- Review sastanci za UI konzistentnost
