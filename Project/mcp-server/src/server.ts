// PB-70 / US-109 — sastavljanje MCP servera sa svim read-only alatima.
import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import type { McpDb } from "./data/db.js";
import { registerTicketTools } from "./tools/ticketTools.js";
import { registerTeamTools } from "./tools/teamTools.js";
import { registerFaqTools } from "./tools/faqTools.js";

export function buildMcpServer(db: McpDb): McpServer {
  const server = new McpServer({
    name: "telecom-mcp-server",
    version: "1.0.0",
  });

  registerTicketTools(server, db);
  registerTeamTools(server, db);
  registerFaqTools(server, db);

  return server;
}
