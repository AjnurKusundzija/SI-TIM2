// PB-70 / US-111 — faq.search (read-only). Pretraga FAQ stavki s jednostavnim keyword relevance score-om.
import { z } from "zod";
import type { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import type { McpDb, FaqItem } from "../data/db.js";

export const faqSearchShape = {
  query: z.string().optional(),
  category: z.string().optional(),
  keywords: z.array(z.string()).optional(),
  limit: z.number().int().positive().max(50).optional(),
};
export const faqSearchSchema = z.object(faqSearchShape);
export type FaqSearchInput = z.infer<typeof faqSearchSchema>;

export interface FaqSearchResult {
  faqId: number;
  question: string;
  answer: string;
  category: string | null;
  relevanceScore: number;
}

function normalize(text: string): string {
  return text.toLowerCase().replace(/[^\p{L}\p{N}\s]/gu, " ");
}

function collectKeywords(input: FaqSearchInput): string[] {
  const fromQuery = (input.query ?? "")
    .toLowerCase()
    .replace(/[^\p{L}\p{N}\s]/gu, " ")
    .split(/\s+/)
    .filter((w) => w.length >= 3);
  const fromKeywords = (input.keywords ?? []).map((k) => k.toLowerCase()).filter(Boolean);
  return Array.from(new Set([...fromQuery, ...fromKeywords]));
}

export function searchFaqs(faqs: FaqItem[], input: FaqSearchInput = {}): FaqSearchResult[] {
  const limit = input.limit ?? 10;
  const keywords = collectKeywords(input);

  const categoryFiltered = input.category
    ? faqs.filter((f) => (f.category ?? "").toLowerCase().includes(input.category!.toLowerCase()))
    : faqs;

  // Bez ključnih riječi: vrati stavke (po kategoriji) bez rangiranja.
  if (keywords.length === 0) {
    return categoryFiltered.slice(0, limit).map((f) => ({
      faqId: f.faqId,
      question: f.question,
      answer: f.answer,
      category: f.category,
      relevanceScore: 0,
    }));
  }

  const scored = categoryFiltered.map((f) => {
    const q = normalize(f.question);
    const a = normalize(f.answer);
    const c = normalize(f.category ?? "");
    let score = 0;
    for (const kw of keywords) {
      if (q.includes(kw)) score += 2; // pogodak u pitanju nosi veću težinu
      if (a.includes(kw)) score += 1;
      if (c.includes(kw)) score += 1;
    }
    const relevanceScore = Number((score / (keywords.length * 2)).toFixed(3));
    return {
      faqId: f.faqId,
      question: f.question,
      answer: f.answer,
      category: f.category,
      relevanceScore,
    };
  });

  return scored
    .filter((r) => r.relevanceScore > 0)
    .sort((a, b) => b.relevanceScore - a.relevanceScore)
    .slice(0, limit);
}

function asTextResult(payload: unknown) {
  return { content: [{ type: "text" as const, text: JSON.stringify(payload) }] };
}

export function registerFaqTools(server: McpServer, db: McpDb): void {
  server.tool(
    "faq.search",
    "Pretraga FAQ stavki po query/category/keywords; vraća faqId, question, answer, category i relevanceScore. Read-only.",
    faqSearchShape,
    async (args) => {
      const faqs = await db.getFaqs();
      const results = searchFaqs(faqs, args as FaqSearchInput);
      return asTextResult({ count: results.length, results });
    }
  );
}
