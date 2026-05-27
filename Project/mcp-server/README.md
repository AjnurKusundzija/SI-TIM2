# Telecom MCP Server (PB-70 / US-109)

Read-only Model Context Protocol server koji izlaže žive podatke iz iste SQL Server baze
koju koristi backend. Implementiran zvaničnim **@modelcontextprotocol/sdk** TypeScript SDK-om
preko Streamable HTTP transporta.

## Alati (read-only)

| Alat              | Opis                                                                 |
| ----------------- | -------------------------------------------------------------------- |
| `ticket.search`   | Pretraga tiketa po statusu, prioritetu, timu, agentu, kategoriji, starosti |
| `ticket.analytics`| Agregati: ukupno/otvoreno/zatvoreno, zastarjeli, top kategorije, ponavljani problemi |
| `team.workload`   | Opterećenje po timu + najopterećeniji tim (workloadScore)            |
| `faq.search`      | Pretraga FAQ stavki s keyword relevance score-om                     |

Sve operacije su isključivo `SELECT` — `assertReadOnly()` odbija svaki write upit.

## Env varijable

- `PORT` — default `3001`
- `MCP_DB_CONNECTION_STRING` — ADO.NET connection string (fallback: `ConnectionStrings__DefaultConnection`)

## Pokretanje

Pokreće se preko Docker Compose-a (`docker compose up -d`), servis `mcp-server`.
Endpoint: `POST http://mcp-server:3001/mcp`.

## Skripte

```bash
npm install
npm test         # vitest — testovi logike alata i read-only zaštite
npm run build    # tsc -> dist/
npm start        # node dist/index.js
```
