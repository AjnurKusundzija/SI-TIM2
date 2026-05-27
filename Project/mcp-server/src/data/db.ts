// PB-70 / US-109 — pristup živim podacima iz iste SQL Server baze koju koristi backend.
// SVE operacije su READ-ONLY (samo SELECT). assertReadOnly() je zaštita protiv slučajnog
// write upita (INSERT/UPDATE/DELETE/DROP/ALTER/TRUNCATE/MERGE/EXEC/CREATE/GRANT/REVOKE).

import mssql from "mssql";

// ── Enum mapiranja (EF Core čuva enume kao int u bazi) ────────────────────────
export const TICKET_STATUS: Record<number, string> = {
  1: "OPEN",
  2: "CLOSED",
  3: "CLOSURE_REQUESTED",
};

export const PRIORITY: Record<number, string> = {
  1: "LOW",
  2: "MEDIUM",
  3: "HIGH",
};

export const PROBLEM_CATEGORY: Record<number, string> = {
  1: "INTERNET",
  2: "TV",
  3: "MOBILE_NETWORK",
  4: "BILLING",
  5: "TECHNICAL_SUPPORT",
};

const ROLE_CLIENT = 1; // Role.CLIENT — koristi se za razlikovanje "staff" odgovora od klijentskih

// ── Tipovi redova koje vraćamo MCP alatima ────────────────────────────────────
export interface TicketFact {
  ticketId: number;
  title: string;
  status: string;
  priority: string;
  category: string;
  teamId: number | null;
  teamName: string | null;
  assignedAgentId: number | null;
  assignedAgentName: string | null;
  createdDate: string; // ISO
  closedDate: string | null;
  firstResponseAt: string | null;
  lastResponseAt: string | null;
}

export interface TeamInfo {
  teamId: number;
  teamName: string;
  membersCount: number;
}

export interface FaqItem {
  faqId: number;
  question: string;
  answer: string;
  category: string | null;
}

// ── Read-only zaštita ─────────────────────────────────────────────────────────
const FORBIDDEN_SQL =
  /\b(insert|update|delete|drop|alter|truncate|merge|exec|execute|create|grant|revoke|sp_|xp_)\b/i;

export function assertReadOnly(sql: string): void {
  if (FORBIDDEN_SQL.test(sql)) {
    throw new Error(`MCP alati su read-only — odbijen upit koji nije SELECT: ${sql.slice(0, 60)}…`);
  }
}

// ── DB apstrakcija (omogućava lažni DB u testovima) ───────────────────────────
export interface McpDb {
  getTicketFacts(): Promise<TicketFact[]>;
  getTeams(): Promise<TeamInfo[]>;
  getFaqs(): Promise<FaqItem[]>;
}

// ── ADO.NET connection string -> mssql config ─────────────────────────────────
export function parseConnectionString(connectionString: string): mssql.config {
  const parts = connectionString
    .split(";")
    .map((p) => p.trim())
    .filter(Boolean);

  const map = new Map<string, string>();
  for (const part of parts) {
    const eq = part.indexOf("=");
    if (eq === -1) continue;
    const key = part.slice(0, eq).trim().toLowerCase();
    const value = part.slice(eq + 1).trim();
    map.set(key, value);
  }

  const serverRaw = map.get("server") ?? map.get("data source") ?? map.get("address") ?? "localhost";
  let host = serverRaw;
  let port = 1433;
  const commaIdx = serverRaw.indexOf(",");
  if (commaIdx !== -1) {
    host = serverRaw.slice(0, commaIdx).trim();
    port = Number(serverRaw.slice(commaIdx + 1).trim()) || 1433;
  }

  const trust = (map.get("trustservercertificate") ?? "true").toLowerCase() === "true";
  const encrypt = (map.get("encrypt") ?? "false").toLowerCase() === "true";

  return {
    server: host,
    port,
    database: map.get("database") ?? map.get("initial catalog") ?? "",
    user: map.get("user id") ?? map.get("uid") ?? map.get("user") ?? "sa",
    password: map.get("password") ?? map.get("pwd") ?? "",
    options: {
      trustServerCertificate: trust,
      encrypt,
    },
  };
}

// ── Stvarna SQL Server implementacija ─────────────────────────────────────────
export class SqlMcpDb implements McpDb {
  private readonly config: mssql.config;
  private pool: mssql.ConnectionPool | null = null;

  constructor(connectionString: string) {
    this.config = parseConnectionString(connectionString);
  }

  private async getPool(): Promise<mssql.ConnectionPool> {
    if (this.pool && this.pool.connected) return this.pool;
    this.pool = await new mssql.ConnectionPool(this.config).connect();
    return this.pool;
  }

  private async readQuery<T>(sql: string): Promise<T[]> {
    assertReadOnly(sql);
    const pool = await this.getPool();
    const result = await pool.request().query(sql);
    return result.recordset as T[];
  }

  async getTicketFacts(): Promise<TicketFact[]> {
    // Posljednja dodjela = najnoviji TicketUser zapis. "Staff" odgovor = komentar autora
    // čija uloga nije CLIENT i koji nije sistemska poruka.
    const sql = `
      SELECT
        t.TicketId       AS ticketId,
        t.Title          AS title,
        t.Status         AS status,
        t.Priority       AS priority,
        t.ProblemCategory AS category,
        COALESCE(t.TeamId, a.TeamId) AS teamId,
        tm.TeamName      AS teamName,
        a.UserId         AS assignedAgentId,
        CASE WHEN au.UserId IS NULL THEN NULL
             ELSE CONCAT(au.FirstName, ' ', au.LastName) END AS assignedAgentName,
        t.CreatedDate    AS createdDate,
        t.ClosedDate     AS closedDate,
        fr.FirstResponseAt AS firstResponseAt,
        lr.LastResponseAt  AS lastResponseAt
      FROM Tickets t
      OUTER APPLY (
        SELECT TOP 1 tu.UserId, tu.TeamId
        FROM TicketUsers tu
        WHERE tu.TicketId = t.TicketId
        ORDER BY tu.AssignmentDate DESC, tu.AssignmentId DESC
      ) a
      LEFT JOIN Teams tm ON tm.TeamId = COALESCE(t.TeamId, a.TeamId)
      LEFT JOIN Users au ON au.UserId = a.UserId
      OUTER APPLY (
        SELECT MIN(c.DateTime) AS FirstResponseAt
        FROM Comments c JOIN Users cu ON cu.UserId = c.AuthorId
        WHERE c.TicketId = t.TicketId AND c.IsSystemMessage = 0 AND cu.Role <> ${ROLE_CLIENT}
      ) fr
      OUTER APPLY (
        SELECT MAX(c.DateTime) AS LastResponseAt
        FROM Comments c JOIN Users cu ON cu.UserId = c.AuthorId
        WHERE c.TicketId = t.TicketId AND c.IsSystemMessage = 0 AND cu.Role <> ${ROLE_CLIENT}
      ) lr;
    `;

    const rows = await this.readQuery<Record<string, unknown>>(sql);
    return rows.map((r) => ({
      ticketId: Number(r.ticketId),
      title: String(r.title ?? ""),
      status: TICKET_STATUS[Number(r.status)] ?? `UNKNOWN_${r.status}`,
      priority: PRIORITY[Number(r.priority)] ?? `UNKNOWN_${r.priority}`,
      category: PROBLEM_CATEGORY[Number(r.category)] ?? `UNKNOWN_${r.category}`,
      teamId: r.teamId == null ? null : Number(r.teamId),
      teamName: r.teamName == null ? null : String(r.teamName),
      assignedAgentId: r.assignedAgentId == null ? null : Number(r.assignedAgentId),
      assignedAgentName: r.assignedAgentName == null ? null : String(r.assignedAgentName),
      createdDate: toIso(r.createdDate),
      closedDate: r.closedDate == null ? null : toIso(r.closedDate),
      firstResponseAt: r.firstResponseAt == null ? null : toIso(r.firstResponseAt),
      lastResponseAt: r.lastResponseAt == null ? null : toIso(r.lastResponseAt),
    }));
  }

  async getTeams(): Promise<TeamInfo[]> {
    const sql = `
      SELECT
        t.TeamId   AS teamId,
        t.TeamName AS teamName,
        (SELECT COUNT(*) FROM Users u WHERE u.TeamId = t.TeamId) AS membersCount
      FROM Teams t;
    `;
    const rows = await this.readQuery<Record<string, unknown>>(sql);
    return rows.map((r) => ({
      teamId: Number(r.teamId),
      teamName: String(r.teamName ?? ""),
      membersCount: Number(r.membersCount ?? 0),
    }));
  }

  async getFaqs(): Promise<FaqItem[]> {
    const sql = `
      SELECT f.FaqId AS faqId, f.Question AS question, f.Answer AS answer, f.Category AS category
      FROM Faqs f
      WHERE f.IsActive = 1;
    `;
    const rows = await this.readQuery<Record<string, unknown>>(sql);
    return rows.map((r) => ({
      faqId: Number(r.faqId),
      question: String(r.question ?? ""),
      answer: String(r.answer ?? ""),
      category: r.category == null ? null : String(r.category),
    }));
  }

  async close(): Promise<void> {
    if (this.pool) {
      await this.pool.close();
      this.pool = null;
    }
  }
}

function toIso(value: unknown): string {
  if (value instanceof Date) return value.toISOString();
  return new Date(String(value)).toISOString();
}
