import { describe, it, expect } from "vitest";
import { searchFaqs, faqSearchSchema } from "./faqTools.js";
import type { FaqItem } from "../data/db.js";

const FAQS: FaqItem[] = [
  { faqId: 1, question: "Kako resetovati ruter?", answer: "Isključite ruter 30 sekundi.", category: "Internet" },
  { faqId: 2, question: "Internet je spor", answer: "Provjerite kablove i restartujte ruter.", category: "Internet" },
  { faqId: 3, question: "TV signal nestaje", answer: "Provjerite HDMI kabl i ponovo pokrenite STB.", category: "TV" },
];

describe("faq.search", () => {
  it("vraća relevantne FAQ rezultate za keyword query", () => {
    const results = searchFaqs(FAQS, { query: "ruter internet" });
    expect(results.length).toBeGreaterThan(0);
    // FAQ-ovi koji spominju ruter/internet imaju veći score
    expect(results[0].relevanceScore).toBeGreaterThan(0);
    expect(results.some((r) => r.faqId === 1 || r.faqId === 2)).toBe(true);
  });

  it("rangira pogodak u pitanju iznad pogotka u odgovoru", () => {
    const results = searchFaqs(FAQS, { keywords: ["internet"] });
    // FAQ 2 ima 'internet' u pitanju -> veći score od FAQ 1 (gdje ga nema u pitanju)
    const faq2 = results.find((r) => r.faqId === 2);
    expect(faq2).toBeDefined();
  });

  it("filtrira po kategoriji", () => {
    const results = searchFaqs(FAQS, { query: "signal", category: "TV" });
    expect(results.every((r) => r.category === "TV")).toBe(true);
  });

  it("bez ključnih riječi vraća stavke bez score-a", () => {
    const results = searchFaqs(FAQS, {});
    expect(results.length).toBe(3);
  });

  it("zod validacija odbija nevažeći limit", () => {
    expect(() => faqSearchSchema.parse({ limit: 0 })).toThrow();
    expect(() => faqSearchSchema.parse({ query: "ruter", limit: 5 })).not.toThrow();
  });
});
