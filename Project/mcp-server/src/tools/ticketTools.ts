// PB-70 / US-109, US-111 — ticket.search i ticket.analytics (read-only).
import { z } from "zod";
import type { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import type { McpDb, TicketFact } from "../data/db.js";

// ── Zod sheme (validacija inputa) ─────────────────────────────────────────────
export const ticketSearchShape = {
  status: z.enum(["OPEN", "CLOSED", "CLOSURE_REQUESTED"]).optional(),
  priority: z.enum(["LOW", "MEDIUM", "HIGH"]).optional(),
  teamId: z.number().int().optional(),
  agentId: z.number().int().optional(),
  category: z.enum(["INTERNET", "TV", "MOBILE_NETWORK", "BILLING", "TECHNICAL_SUPPORT"]).optional(),
  olderThanMinutes: z.number().int().nonnegative().optional(),
  dateFrom: z.string().optional(),
  dateTo: z.string().optional(),
  limit: z.number().int().positive().max(200).optional(),
};
export const ticketSearchSchema = z.object(ticketSearchShape);
export type TicketSearchInput = z.infer<typeof ticketSearchSchema>;

export const ticketAnalyticsShape = {
  staleThresholdMinutes: z.number().int().positive().optional(),
  dateFrom: z.string().optional(),
  dateTo: z.string().optional(),
};
export const ticketAnalyticsSchema = z.object(ticketAnalyticsShape);
export type TicketAnalyticsInput = z.infer<typeof ticketAnalyticsSchema>;

// ── Pomoćne funkcije ──────────────────────────────────────────────────────────
export function minutesSince(iso: string | null, now: Date): number | null {
  if (!iso) return null;
  const ms = now.getTime() - new Date(iso).getTime();
  return Math.max(0, Math.floor(ms / 60000));
}

/** Minute bez odgovora za otvoreni tiket: od zadnjeg staff odgovora, ili od kreiranja ako odgovora nema. */
export function minutesWithoutResponse(fact: TicketFact, now: Date): number | null {
  if (fact.status !== "OPEN") return null;
  return minutesSince(fact.lastResponseAt ?? fact.createdDate, now);
}

// ── ticket.search (pure) ──────────────────────────────────────────────────────
export function searchTickets(
  facts: TicketFact[],
  input: TicketSearchInput,
  now: Date = new Date()
) {
  const from = input.dateFrom ? new Date(input.dateFrom).getTime() : null;
  const to = input.dateTo ? new Date(input.dateTo).getTime() : null;
  const limit = input.limit ?? 20;

  const filtered = facts.filter((f) => {
    if (input.status && f.status !== input.status) return false;
    if (input.priority && f.priority !== input.priority) return false;
    if (input.category && f.category !== input.category) return false;
    if (input.teamId != null && f.teamId !== input.teamId) return false;
    if (input.agentId != null && f.assignedAgentId !== input.agentId) return false;
    const created = new Date(f.createdDate).getTime();
    if (from != null && created < from) return false;
    if (to != null && created > to) return false;
    if (input.olderThanMinutes != null) {
      const mwr = minutesWithoutResponse(f, now);
      if (mwr == null || mwr < input.olderThanMinutes) return false;
    }
    return true;
  });

  const mapped = filtered
    .map((f) => ({
      ticketId: f.ticketId,
      title: f.title,
      status: f.status,
      priority: f.priority,
      category: f.category,
      teamId: f.teamId,
      teamName: f.teamName,
      assignedAgentId: f.assignedAgentId,
      assignedAgentName: f.assignedAgentName,
      createdDate: f.createdDate,
      lastResponseAt: f.lastResponseAt,
      minutesWithoutResponse: minutesWithoutResponse(f, now),
    }))
    .sort((a, b) => (b.minutesWithoutResponse ?? -1) - (a.minutesWithoutResponse ?? -1))
    .slice(0, limit);

  return { count: mapped.length, tickets: mapped };
}

// ── ticket.analytics (pure) ───────────────────────────────────────────────────
const STOPWORDS = new Set([
  "i", "u", "na", "se", "su", "je", "za", "od", "do", "ne", "mi", "sa", "o", "li",
  "pa", "te", "ili", "a", "the", "of", "to", "and", "ima", "nema", "moj", "moja",
  "duze", "duže", "vise", "više", "sati", "sata", "minuta",
]);

function tokenize(title: string): string[] {
  return Array.from(
    new Set(
      title
        .toLowerCase()
        .replace(/[^\p{L}\p{N}\s]/gu, " ")
        .split(/\s+/)
        .filter((w) => w.length >= 4 && !STOPWORDS.has(w))
    )
  );
}

export function computeTicketAnalytics(
  facts: TicketFact[],
  input: TicketAnalyticsInput = {},
  now: Date = new Date()
) {
  const staleThreshold = input.staleThresholdMinutes ?? 7 * 24 * 60; // 7 dana
  const from = input.dateFrom ? new Date(input.dateFrom).getTime() : null;
  const to = input.dateTo ? new Date(input.dateTo).getTime() : null;

  const scoped = facts.filter((f) => {
    const created = new Date(f.createdDate).getTime();
    if (from != null && created < from) return false;
    if (to != null && created > to) return false;
    return true;
  });

  const openTickets = scoped.filter((f) => f.status === "OPEN");
  const closedTickets = scoped.filter((f) => f.status === "CLOSED");
  const closureRequested = scoped.filter((f) => f.status === "CLOSURE_REQUESTED");
  const staleTickets = openTickets.filter((f) => {
    const age = minutesSince(f.createdDate, now);
    return age != null && age >= staleThreshold;
  });

  // Top kategorije
  const catCounts = new Map<string, number>();
  for (const f of scoped) catCounts.set(f.category, (catCounts.get(f.category) ?? 0) + 1);
  const topCategories = [...catCounts.entries()]
    .map(([category, count]) => ({ category, count }))
    .sort((a, b) => b.count - a.count);

  // Ponavljani problemi izvučeni iz ključnih riječi u naslovima
  const patternMap = new Map<
    string,
    { count: number; categories: Map<string, number>; ticketIds: number[] }
  >();
  for (const f of scoped) {
    for (const token of tokenize(f.title)) {
      let entry = patternMap.get(token);
      if (!entry) {
        entry = { count: 0, categories: new Map(), ticketIds: [] };
        patternMap.set(token, entry);
      }
      entry.count += 1;
      entry.categories.set(f.category, (entry.categories.get(f.category) ?? 0) + 1);
      entry.ticketIds.push(f.ticketId);
    }
  }
  const topProblemPatterns = [...patternMap.entries()]
    .filter(([, e]) => e.count >= 2)
    .map(([pattern, e]) => ({
      pattern,
      count: e.count,
      category: [...e.categories.entries()].sort((a, b) => b[1] - a[1])[0][0],
      sampleTicketIds: e.ticketIds.slice(0, 5),
    }))
    .sort((a, b) => b.count - a.count)
    .slice(0, 10);

  return {
    totalTickets: scoped.length,
    openTickets: openTickets.length,
    closedTickets: closedTickets.length,
    closureRequestedTickets: closureRequested.length,
    staleTickets: staleTickets.length,
    ticketsOlderThanThreshold: staleTickets.length,
    staleThresholdMinutes: staleThreshold,
    topCategories,
    repeatedProblems: topCategories,
    topProblemPatterns,
  };
}

// ── Registracija MCP alata ────────────────────────────────────────────────────
function asTextResult(payload: unknown) {
  return { content: [{ type: "text" as const, text: JSON.stringify(payload) }] };
}

export function registerTicketTools(server: McpServer, db: McpDb): void {
  server.tool(
    "ticket.search",
    "Pretraga tiketa po filterima (status, priority, teamId, agentId, category, olderThanMinutes, dateFrom, dateTo, limit). Read-only.",
    ticketSearchShape,
    async (args) => {
      const facts = await db.getTicketFacts();
      return asTextResult(searchTickets(facts, args as TicketSearchInput));
    }
  );

  server.tool(
    "ticket.analytics",
    "Agregirani podaci o tiketima: ukupno/otvoreni/zatvoreni, zastarjeli, top kategorije i ponavljani problemi (za FAQ coverage analizu). Read-only.",
    ticketAnalyticsShape,
    async (args) => {
      const facts = await db.getTicketFacts();
      return asTextResult(computeTicketAnalytics(facts, args as TicketAnalyticsInput));
    }
  );
}
