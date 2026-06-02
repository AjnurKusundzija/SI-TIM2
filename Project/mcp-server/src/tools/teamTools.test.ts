import { describe, it, expect } from "vitest";
import { computeTeamWorkload, teamWorkloadSchema } from "./teamTools.js";
import type { TicketFact, TeamInfo } from "../data/db.js";

const NOW = new Date("2026-05-27T12:00:00.000Z");

function fact(partial: Partial<TicketFact>): TicketFact {
  return {
    ticketId: 1,
    title: "x",
    status: "OPEN",
    priority: "MEDIUM",
    category: "INTERNET",
    teamId: 1,
    teamName: "Internet Tim",
    assignedAgentId: null,
    assignedAgentName: null,
    createdDate: "2026-05-27T08:00:00.000Z",
    closedDate: null,
    firstResponseAt: null,
    lastResponseAt: null,
    ...partial,
  };
}

const TEAMS: TeamInfo[] = [
  { teamId: 1, teamName: "Internet Tim", membersCount: 3 },
  { teamId: 2, teamName: "TV Tim", membersCount: 2 },
];

describe("team.workload", () => {
  it("računa najopterećeniji tim po workloadScore", () => {
    const facts: TicketFact[] = [
      // Internet Tim: 3 otvorena, svi bez odgovora > 2h (kreirani u 08:00, sada 12:00)
      fact({ ticketId: 1, teamId: 1, status: "OPEN" }),
      fact({ ticketId: 2, teamId: 1, status: "OPEN" }),
      fact({ ticketId: 3, teamId: 1, status: "OPEN" }),
      // TV Tim: 1 otvoren, s odgovorom prije 10 min -> nije bez odgovora
      fact({ ticketId: 4, teamId: 2, status: "OPEN", teamName: "TV Tim", lastResponseAt: "2026-05-27T11:50:00.000Z" }),
    ];

    const result = computeTeamWorkload(facts, TEAMS, {}, NOW);

    expect(result.mostLoaded?.teamId).toBe(1);
    expect(result.mostLoaded?.openTickets).toBe(3);
    expect(result.mostLoaded?.ticketsWithoutResponseOver2h).toBe(3);
    // score = 3 + 2*3 = 9
    expect(result.mostLoaded?.workloadScore).toBe(9);
  });

  it("računa prosječno vrijeme prvog odgovora", () => {
    const facts: TicketFact[] = [
      fact({ ticketId: 1, teamId: 2, teamName: "TV Tim", createdDate: "2026-05-27T08:00:00.000Z", firstResponseAt: "2026-05-27T09:00:00.000Z" }),
    ];
    const result = computeTeamWorkload(facts, TEAMS, {}, NOW);
    const tv = result.teams.find((t) => t.teamId === 2);
    expect(tv?.avgFirstResponseMinutes).toBe(60);
  });

  it("vraća mostLoaded = null kada nema opterećenja", () => {
    const result = computeTeamWorkload([], TEAMS, {}, NOW);
    expect(result.mostLoaded).toBeNull();
  });

  it("zod validacija prihvata prag i odbija nevažeći", () => {
    expect(() => teamWorkloadSchema.parse({ noResponseThresholdMinutes: 120 })).not.toThrow();
    expect(() => teamWorkloadSchema.parse({ noResponseThresholdMinutes: -1 })).toThrow();
  });
});
