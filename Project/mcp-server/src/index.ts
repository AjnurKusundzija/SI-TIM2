// PB-70 / US-109 — MCP server preko Streamable HTTP transporta (zvanični MCP TypeScript SDK).
// Endpoint: POST/GET/DELETE /mcp. Pokreće se isključivo preko Docker Compose-a.
import express, { type Request, type Response } from "express";
import { randomUUID } from "node:crypto";
import { StreamableHTTPServerTransport } from "@modelcontextprotocol/sdk/server/streamableHttp.js";
import { isInitializeRequest } from "@modelcontextprotocol/sdk/types.js";
import { buildMcpServer } from "./server.js";
import { SqlMcpDb } from "./data/db.js";
import { loadConfig } from "./config.js";

const config = loadConfig();
const db = new SqlMcpDb(config.connectionString);

const app = express();
app.use(express.json());

// Aktivni transporti po sesiji (Streamable HTTP zahtijeva session management).
const transports: Record<string, StreamableHTTPServerTransport> = {};

app.get("/health", (_req: Request, res: Response) => {
  res.json({ status: "ok", service: "telecom-mcp-server" });
});

app.post("/mcp", async (req: Request, res: Response) => {
  try {
    const sessionId = req.headers["mcp-session-id"] as string | undefined;
    let transport: StreamableHTTPServerTransport;

    if (sessionId && transports[sessionId]) {
      transport = transports[sessionId];
    } else if (!sessionId && isInitializeRequest(req.body)) {
      transport = new StreamableHTTPServerTransport({
        sessionIdGenerator: () => randomUUID(),
        enableJsonResponse: true, // odgovori kao application/json (jednostavnije za backend klijenta)
        onsessioninitialized: (sid) => {
          transports[sid] = transport;
        },
      });
      transport.onclose = () => {
        if (transport.sessionId) delete transports[transport.sessionId];
      };
      const server = buildMcpServer(db);
      await server.connect(transport);
    } else {
      res.status(400).json({
        jsonrpc: "2.0",
        error: { code: -32000, message: "Bad Request: nedostaje validan session ID." },
        id: null,
      });
      return;
    }

    await transport.handleRequest(req, res, req.body);
  } catch (err) {
    console.error("[mcp] greška pri obradi zahtjeva:", err);
    if (!res.headersSent) {
      res.status(500).json({
        jsonrpc: "2.0",
        error: { code: -32603, message: "Interna greška MCP servera." },
        id: null,
      });
    }
  }
});

const handleSessionRequest = async (req: Request, res: Response) => {
  const sessionId = req.headers["mcp-session-id"] as string | undefined;
  if (!sessionId || !transports[sessionId]) {
    res.status(400).send("Nevažeći ili nedostajući session ID.");
    return;
  }
  await transports[sessionId].handleRequest(req, res);
};

app.get("/mcp", handleSessionRequest);
app.delete("/mcp", handleSessionRequest);

app.listen(config.port, () => {
  console.log(`[mcp] Telecom MCP server sluša na portu ${config.port} (POST /mcp)`);
});
