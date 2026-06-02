// PB-70 / US-109 — MCP server konfiguracija.
// Connection string se čita iz iste baze koju koristi backend:
//   MCP_DB_CONNECTION_STRING  (preferirano, postavlja se u docker-compose.yml)
//   ConnectionStrings__DefaultConnection  (fallback — isti naziv kao backend)

export interface McpConfig {
  port: number;
  connectionString: string;
}

export function loadConfig(env: NodeJS.ProcessEnv = process.env): McpConfig {
  const port = Number(env.PORT ?? "3001");
  const connectionString =
    env.MCP_DB_CONNECTION_STRING ??
    env.ConnectionStrings__DefaultConnection ??
    "";

  if (!connectionString) {
    throw new Error(
      "Nije postavljen connection string. Postavite MCP_DB_CONNECTION_STRING ili ConnectionStrings__DefaultConnection."
    );
  }

  return { port, connectionString };
}
