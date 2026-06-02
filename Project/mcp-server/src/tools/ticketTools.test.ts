import { describe, it, expect } from "vitest";
import {
  computeTicketAnalytics,
  searchTickets,
  ticketSearchSchema,
  minutesWithoutResponse,
} from "./ticketTools.js";
import type { TicketFact } from "../data/db.js";

const NOW = new Date("2026-05-27T12:00:00.000Z");

function fact(partial: Partial<TicketFact>): TicketFact {
  return {
    ticketId: 1,
    title: "Internet ne radi",
    status: "OPEN",
    priority: "HIGH",
    category: "INTERNET",
    teamId: 1,
    teamName: "Internet Tim",
    assignedAgentId: 10,
    assignedAgentName: "Amina H",
    createdDate: "2026-05-27T08:00:00.000Z",
    closedDate: null,
    firstResponseAt: null,
    lastResponseAt: null,
    ...partial,
  };
}

const FACTS: TicketFact[] = [
  fact({ ticketId: 1, title: "Internet ne radi", category: "INTERNET", status: "OPEN", createdDate: "2026-05-27T08:00:00.000Z" }),
  fact({ ticketId: 2, title: "Internet je spor", category: "INTERNET", status: "OPEN", createdDate: "2026-05-20T08:00:00.000Z" }),
  fact({ ticketId: 3, title: "TV signal nestaje", category: "TV", status: "CLOSED", closedDate: "2026-05-25T08:00:00.000Z", firstResponseAt: "2026-05-24T10:00:00.000Z" }),
  fact({ ticketId: 4, title: "Pogrešan iznos na računu", category: "BILLING", status: "OPEN", createdDate: "2026-05-27T11:30:00.000Z", lastResponseAt: "2026-05-27T11:45:00.000Z" }),
];

describe("ticket.analytics", () => {
  it("vraća agregate nad podacima", () => {
    const result = computeTicketAnalytics(FACTS, {}, NOW);
    expect(result.totalTickets).toBe(4);
    expect(result.openTickets).toBe(3);
    expect(result.closedTickets).toBe(1);
  });

  it("detektuje zastarjele tikete prema pragu", () => {
    // ticket 2 kreiran prije 7 dana — stale ako je prag 7 dana
    const result = computeTicketAnalytics(FACTS, { staleThresholdMinutes: 7 * 24 * 60 }, NOW);
    expect(result.staleTickets).toBeGreaterThanOrEqual(1);
  });

  it("izvlači ponavljane probleme (top kategorije) i top patterns", () => {
    const result = computeTicketAnalytics(FACTS, {}, NOW);
    expect(result.topCategories[0].category).toBe("INTERNET");
    expect(result.topCategories[0].count).toBe(2);
    // "internet" se ponavlja u 2 naslova
    const internetPattern = result.topProblemPatterns.find((p) => p.pattern === "internet");
    expect(internetPattern?.count).toBe(2);
  });
});

describe("ticket.search", () => {
  it("filtrira po statusu i kategoriji", () => {
    const result = searchTickets(FACTS, { status: "OPEN", category: "INTERNET" }, NOW);
    expect(result.count).toBe(2);
    expect(result.tickets.every((t) => t.status === "OPEN" && t.category === "INTERNET")).toBe(true);
  });

  it("olderThanMinutes vraća samo tikete bez odgovora preko praga", () => {
    const result = searchTickets(FACTS, { olderThanMinutes: 120 }, NOW);
    // ticket 4 ima odgovor prije 15 min -> ispada; ticket 1 i 2 nemaju odgovor i stariji su
    expect(result.tickets.some((t) => t.ticketId === 4)).toBe(false);
    expect(result.tickets.some((t) => t.ticketId === 1)).toBe(true);
  });

  it("računa minutesWithoutResponse za otvoreni tiket", () => {
    const f = fact({ status: "OPEN", lastResponseAt: null, createdDate: "2026-05-27T10:00:00.000Z" });
    expect(minutesWithoutResponse(f, NOW)).toBe(120);
  });

  it("zatvoreni tiket nema minutesWithoutResponse", () => {
    const f = fact({ status: "CLOSED" });
    expect(minutesWithoutResponse(f, NOW)).toBeNull();
  });
});

describe("ticket.search zod validacija", () => {
  it("prihvata validan input", () => {
    expect(() => ticketSearchSchema.parse({ status: "OPEN", limit: 10 })).not.toThrow();
  });

  it("odbija nevažeći status", () => {
    expect(() => ticketSearchSchema.parse({ status: "NOPE" })).toThrow();
  });

  it("odbija negativan limit", () => {
    expect(() => ticketSearchSchema.parse({ limit: -5 })).toThrow();
  });
});
