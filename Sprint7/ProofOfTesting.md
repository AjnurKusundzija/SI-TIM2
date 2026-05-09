# Proof of Testing — Sprint 7
---

## Ukupni rezultati

| Nivo | US | Alat | Broj testova | Rezultat |
| --- | --- | --- | --- | --- |


| **Ukupno Sprint 7** | **US-?? do US-??** | | **?? novih testova** | **PASS** |
| **Ukupno projekat** | **US-?? do US-??** | | **?? backend + ?? frontend = ??** | **PASS** |

---

## PB-?? — Prijava i upravljanje sesijama

### Pokriveni AC (??)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |


### Fajlovi sa testovima



---

## PB-?? — Kreiranje tiketa

### Pokriveni AC (??)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |


### Fajlovi sa testovima



---

## PB-?? — Pregled vlastitih tiketa

### Pokriveni AC (??)



### Fajlovi sa testovima



---

## PB-?? — Detaljan prikaz tiketa

### Pokriveni AC (??)

| Nivo | US | AC | Test koji pokriva | Status |


### Fajlovi sa testovima



---

## PB-?? — Komunikacija kroz tiket

### Pokriveni AC (??)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |


### Fajlovi sa testovima



---

## PB-?? — Pregled svih tiketa

### Pokriveni AC (??)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |


### Fajlovi sa testovima



---

## PB-?? — Pretraga i filtriranje tiketa

### Pokriveni AC (??)

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |


### Fajlovi sa testovima



---

## Zajedničke UI komponente

Testovi zajedničkih komponenti (`components/common`) pokrivaju izoliranu renderabilnost, interakciju i edge-case ponašanje UI gradivnih blokova koji se koriste kroz više stranica.

| Nivo | US | AC | Test koji pokriva | Status |
| --- | --- | --- | --- | --- |


### Fajlovi sa testovima


---

## Veza sa Test Strategijom

| Test strategija nivo | US | PB | Dokaz | Status |
| --- | --- | --- | --- | --- |



## Lokalno pokretanje testova:

Iz root direktorija: 

### Frontend:
cd ../Project/TelecomSupportSystem && dotnet test TelecomSupportSystem.Tests/ --logger "console;verbosity=normal" 2>&1

### Backend:
cd ../Project/frontend && npx vitest run 2>&1