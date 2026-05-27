// PB-70 / US-110 — team.workload (read-only). Određuje opterećenje po timu i najopterećeniji tim.
import { z } from "zod";
import type { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import type { McpDb, TicketFact, TeamInfo } from "../data/db.js";
import { minutesSince } from "./ticketTools.js";

export const teamWorkloadShape = {
  noResponseThresholdMinutes: z.number().int().positive().optional(),
};
export const teamWorkloadSchema = z.object(teamWorkloadShape);
export type TeamWorkloadInput = z.infer<typeof teamWorkloadSchema>;

export interface TeamWorkloadRow {
  teamId: number;
  teamName: string;
  openTickets: number;
  membersCount: number;
  ticketsWithoutResponseOver2h: number;
  avgFirstResponseMinutes: number | null;
  workloadScore: number;
}

export function computeTeamWorkload(
  facts: TicketFact[],
  teams: TeamInfo[],
  input: TeamWorkloadInput = {},
  now: Date = new Date()
): { teams: TeamWorkloadRow[]; mostLoaded: TeamWorkloadRow | null; criterion: string } {
  const threshold = input.noResponseThresholdMinutes ?? 120; // 2h

  const rows: TeamWorkloadRow[] = teams.map((team) => {
    const teamTickets = facts.filter((f) => f.teamId === team.teamId);
    const open = teamTickets.filter((f) => f.status === "OPEN");

    const withoutResponseOver2h = open.filter((f) => {
      const mins = minutesSince(f.lastResponseAt ?? f.createdDate, now);
      return mins != null && mins >= threshold;
    }).length;

    const firstResponseDurations = teamTickets
      .filter((f) => f.firstResponseAt)
      .map((f) => {
        const created = new Date(f.createdDate).getTime();
        const responded = new Date(f.firstResponseAt as string).getTime();
        return Math.max(0, Math.floor((responded - created) / 60000));
      });
    const avgFirstResponseMinutes =
      firstResponseDurations.length > 0
        ? Math.round(
            firstResponseDurations.reduce((a, b) => a + b, 0) / firstResponseDurations.length
          )
        : null;

    // Score: otvoreni tiketi + dvostruka težina za tikete bez odgovora preko 2h.
    const workloadScore = open.length + 2 * withoutResponseOver2h;

    return {
      teamId: team.teamId,
      teamName: team.teamName,
      openTickets: open.length,
      membersCount: team.membersCount,
      ticketsWithoutResponseOver2h: withoutResponseOver2h,
      avgFirstResponseMinutes,
      workloadScore,
    };
  });

  const sorted = [...rows].sort(
    (a, b) => b.workloadScore - a.workloadScore || b.openTickets - a.openTickets
  );
  const mostLoaded = sorted.length > 0 && sorted[0].workloadScore > 0 ? sorted[0] : null;

  return {
    teams: sorted,
    mostLoaded,
    criterion:
      "workloadScore = otvoreni tiketi + 2 × tiketi bez odgovora duže od 2h; pri izjednačenju veći broj otvorenih tiketa.",
  };
}

function asTextResult(payload: unknown) {
  return { content: [{ type: "text" as const, text: JSON.stringify(payload) }] };
}

export function registerTeamTools(server: McpServer, db: McpDb): void {
  server.tool(
    "team.workload",
    "Opterećenje po timu: openTickets, membersCount, ticketsWithoutResponseOver2h, avgFirstResponseMinutes, workloadScore + najopterećeniji tim. Read-only.",
    teamWorkloadShape,
    async (args) => {
      const [facts, teams] = await Promise.all([db.getTicketFacts(), db.getTeams()]);
      return asTextResult(computeTeamWorkload(facts, teams, args as TeamWorkloadInput));
    }
  );
}
