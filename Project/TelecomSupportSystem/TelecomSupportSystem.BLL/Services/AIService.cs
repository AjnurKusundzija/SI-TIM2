using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using TelecomSupportSystem.BLL.DTOs.AI;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _http;
        private readonly string? _apiKey;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // ── Telekom baza znanja (knowledge base) ──────────────────────────────
        private static readonly Dictionary<string, string> KnowledgeBase = new()
        {
            ["INTERNET"] = """
                PROBLEMI S INTERNETOM — UOBIČAJENA RJEŠENJA:
                1. Nema interneta: Provjeri LED indikatore na ruteru/modemu (napajanje + DSL/WAN + internet trebaju biti zeleni). Isključi ruter iz struje na 30 sekundi, pa uključi. Provjeri da li je fizički kabl od zida do modema čvrsto utaknut. Ako se koristi PPPoE, provjeri korisničko ime i lozinku u WAN postavkama rutera.
                2. Spor internet: Prvo pokreni test brzine na uređaju koji je priključen kablom (ne WiFi). Provjeri da li nema preuzimanja u pozadini. Udaljenost od rutera povećava kašnjenje. Frekvencija 2,4 GHz ima veći domet ali manju brzinu u odnosu na 5 GHz. Zagušenje mreže u vršnim satima (18–22 h) je uobičajeno.
                3. Povremeno prekidanje veze: Provjeri kablove na oštećenja. Smetnje od mikrovalnih pećnica ili susjednih WiFi mreža na istom kanalu. Pokušaj promijeniti WiFi kanal (1, 6 ili 11 za 2,4 GHz). Provjeri da li je firmware rutera ažuriran.
                4. WiFi mrtve zone: Postavi ruter centralno, na povišenom mjestu, dalje od zidova. WiFi extender ili mesh čvor može proširiti pokrivenost. Ako je ruter dvopojasni — koristi 5 GHz za brzinu, 2,4 GHz za veći domet.
                5. Određeni uređaj se ne može spojiti: Zaboravi mrežu i spoji se ponovo. Provjeri IP postavke uređaja (DHCP vs statički IP). MAC filter na ruteru može blokirati uređaj.
                6. Problemi s optičkim ONT uređajem: Provjeri indikatorska svjetla na ONT uređaju — PON i LAN trebaju biti zeleni. Isključi i uključi ONT (ne samo ruter). Kvar na liniji zahtijeva dolazak tehničara na teren.
                Eskalacija: Ako osnovno ponovno pokretanje i provjera kablova ne pomognu, ako ONT pokazuje crveno/naranđasto, ili ako problem traje duže od 24 sata, potrebno je zakazati posjet tehničara.
                """,

            ["TV"] = """
                PROBLEMI S TV USLUGOM — UOBIČAJENA RJEŠENJA:
                1. Nema signala / crni ekran: Provjeri sve kablove (koaksijalni ili HDMI od STB prijemnika do televizora). Provjeri da li je STB uključen. Ulazni izvor na televizoru mora odgovarati izlazu STB uređaja (HDMI 1, 2…). Pokušaj s drugim HDMI kablom.
                2. Zamrzavanje slike / pikselizacija / bufferovanje: Problem s kvalitetom signala. Provjeri stanje koaksijalnog kabla i konektora. Razdjeljnici (splitteri) slabe signal — ukloni nepotrebne. Za IPTV: provjeri brzinu interneta (minimalno 10 Mbps za HD). Ponovo pokreni STB.
                3. Nedostaju kanali nakon skeniranja: Uradi potpuno skeniranje kanala u izborniku STB uređaja. Provjeri da li pretplata uključuje te kanale. Neki kanali zahtijevaju aktivaciju nakon promjene paketa — sačekaj do 30 minuta.
                4. STB se ne pokreće: Iskopčaj napajanje na 60 sekundi, pa uključi. Provjeri HDMI. Ako STB ostane zaglavljen na početnom ekranu, uradi tvorničke postavke (drži tipku za reset 10 sekundi). Trajni problem zahtijeva zamjenu uređaja.
                5. Nema EPG vodiča / vodič je prazan: Osvježavanje EPG-a može trajati do 2 sata nakon ponovnog pokretanja. Provjeri datum i vrijeme na STB uređaju. Ako problem ostaje, kontaktiraj podršku za ponovni prijenos EPG podataka.
                6. Problemi s Smart TV aplikacijom: Provjeri da li televizor ima internetsku vezu. Obriši cache aplikacije ili je ponovo instaliraj. Provjeri podatke za prijavu. Ažuriranje firmwarea na Smart TV može riješiti probleme kompatibilnosti.
                Eskalacija: Razina signala ispod praga zahtijeva dolazak tehničara s mjernim uređajem. Kvar hardvera STB uređaja zahtijeva zamjenu u servisnom centru.
                """,

            ["MOBILE_NETWORK"] = """
                PROBLEMI S MOBILNOM MREŽOM — UOBIČAJENA RJEŠENJA:
                1. Nema signala: Provjeri kartu pokrivenosti za lokaciju korisnika — pokrivenost u zatvorenim prostorima je slabija. Uključi/isključi način rada u avionu. Ponovo pokreni telefon. Pokušaj SIM karticu u drugom telefonu da se isključi kvar na uređaju. Provjeri da li je SIM kartica pravilno umetnuta.
                2. Slab signal u zatvorenom prostoru: Građevinski materijali (beton, metal) slabe signal. WiFi pozivi (VoWiFi) mogu pomoći ako korisnik ima kućni internet. Mrežni repeater za domove/urede.
                3. Spori mobilni podaci: Provjeri da li je dostignut limit podataka (brzina može biti ograničena). APN postavke: provjeri ispravan APN za mrežu. Mrežni način rada: LTE/4G preferiran. Zagušenje u centru grada u vršnim satima.
                4. Padanje poziva: Rubno područje pokrivenosti — premjesti se na bolje mjesto signala. Provjeri da li je problem s određenim brojevima (petlja preusmjeravanja poziva). VoLTE (4G pozivi) mora biti omogućen za HD glas.
                5. Nije moguće upućivanje poziva: Provjeri stanje računa/kredita za prepaid. Blokirani brojevi ili ograničenja poziva. Registracija na mreži: pokušaj ručni odabir mreže u postavkama.
                6. SMS poruke se ne dostavljaju: Provjeri broj SMS centra u postavkama. Format broja primatelja (+387 ili lokalni format). Blokiranje SMS-a na strani primatelja. Roaming — SMS u romingu može zahtijevati aktivaciju roaming SMS usluge.
                7. Problemi s roamingom: Roaming mora biti aktiviran na računu prije putovanja. Ručno odaberi partnersku mrežu. Roaming podataka mora biti omogućen u postavkama telefona. Primjenjuju se ograničenja fer upotrebe u EU.
                Eskalacija: Trajni nestanak signala na inače pokrivenom području može ukazivati na kvar bazne stanice — provjeri status usluge. Kvarovi SIM kartice zahtijevaju zamjenu u servisnom centru.
                """,

            ["BILLING"] = """
                PROBLEMI S NAPLATOM I RAČUNIMA — UOBIČAJENA RJEŠENJA:
                1. Neočekivana stavka na računu: Pregledaj detaljni račun za konkretan kôd stavke. Uobičajeni razlozi: isteklo promotivno razdoblje, roaming upotreba, premium SMS usluge, naknada za kasno plaćanje. Pitaj korisnika za konkretnu stavku.
                2. Naplaćena usluga koja nije korištena: Provjeri datum aktivacije/deaktivacije u sistemu. Ako je usluga bila aktivna u obračunskom periodu, naplata je ispravna. Žalba zahtijeva provjeru sistemskih logova.
                3. Dupli račun: Provjeri brojeve i datume računa. Dupli račun je obično greška pri štampanju/portalu — dugovani iznos je samo jedan. Potvrdi putem stanja na računu.
                4. Uplata nije evidentirana: Bankovni prijenosi traju 1–3 radna dana. Kreditna kartica obično isti dan. Korisnik treba dostaviti referentni broj uplate. Provjeri da li je uplata otišla na stari broj računa.
                5. Naknada za prijevremeni raskid ugovora: Ovisi o preostalim mjesecima ugovora × mjesečna naknada. Provjeri uvjete ugovora za točan izračun. Oprost naknade zahtijeva odobrenje rukovodioca.
                6. Osporavanje stavke: Otvori formalnu žalbu, zabilježi broj tiketa, odobrenje kredita bit će razmotreno u roku 5–10 radnih dana. Kreditiranje se ne može garantovati dok istraga nije završena.
                7. Format računa / PDV: Računi uključuju 17% PDV-a prema zakonu. PDF račun dostupan na korisničkom portalu. Papirnati račun može se zatražiti na zahtjev.
                Eskalacija: Sporovi iznad 50 KM ili koji uključuju prevaru zahtijevaju pregled tima specijalizovanog za naplatu.
                """,

            ["TECHNICAL_SUPPORT"] = """
                TEHNIČKA PODRŠKA — UOBIČAJENA RJEŠENJA:
                1. Konfiguracija rutera: Zadani pristupnik obično je 192.168.1.1 ili 192.168.0.1. Admin lozinka se nalazi na naljepnici ispod rutera. Promijeni WiFi SSID i lozinku u bežičnim postavkama. Za PPPoE: WAN tip = PPPoE, unesi ISP pristupne podatke. Za DHCP: WAN tip = DHCP.
                2. Prosljeđivanje portova (port forwarding): Omogući u ruteru pod NAT/Virtualni server. Potreban je lokalni IP uređaja (prvo postavi statički IP), vanjski port, unutarnji port, protokol (TCP/UDP). DMZ kao posljednje rješenje — sigurnosni rizik.
                3. Postavljanje WiFi extendera / mesh mreže: Postavi na pola puta između rutera i mrtve zone. Uparivanje WPS tipkom: pritisni WPS na ruteru, pa na extenderu u roku 2 minute. Ili se spoji putem preglednika na admin stranicu 192.168.x.x. Koristi isti SSID kao ruter za nesmetano prebacivanje.
                4. Zamjena optičkog ONT uređaja / modema: Za zamjenu ONT uređaja potreban je tehničar. Korisnik može zamijeniti ruter sam — bridge vs routing način rada ovisi o postavkama. GPON serijski broj potreban je za ponovnu dodjelu.
                5. VoIP / kućni telefon na optici: SIP pristupne podatke dostavlja operater. Unesi u VoIP postavke rutera. Provjeri da vatrozid ne blokira SIP (port 5060) ni RTP portove. Problemi s odjekom/kvalitetom: provjeri QoS postavke, daj prioritet VoIP prometu.
                6. Oštećenje kabla ili opreme: Vidljivo oštećenje kabla ili opreme poništava garanciju. Potreban je posjet tehničara radi procjene. Privremeno popravljanje nije preporučeno iz sigurnosnih razloga.
                7. Ažuriranje firmwarea: Pristupi admin panelu rutera > Upravljanje > Ažuriranje firmwarea. Preuzmi s web stranice proizvođača ako automatsko ažuriranje nije dostupno. Ne isključuj napajanje tijekom ažuriranja.
                Eskalacija: Svaki fizički rad na kablu/infrastrukturi, zamjena ONT uređaja ili konfiguracija koju daljinska podrška ne može riješiti zahtijeva dolazak tehničara na teren.
                """
        };

        // ── Constructor ────────────────────────────────────────────────────────
        public AIService(HttpClient http, IConfiguration configuration)
        {
            _http = http;
            _apiKey = configuration["GROQ_API_KEY"];
        }

        // ── PB-57 / US-96: Agent suggestion ───────────────────────────────────
        public async Task<AgentSuggestionResponseDto> GetAgentSuggestionAsync(AgentSuggestionRequestDto request)
        {
            var kb = KnowledgeBase.GetValueOrDefault(request.Category.ToUpper(), "No specific knowledge base available for this category.");

            var conversationHistory = new StringBuilder();
            foreach (var c in request.RecentComments.TakeLast(6))
            {
                var role = c.AuthorRole == "CLIENT" ? "Klijent" : "Agent";
                conversationHistory.AppendLine($"{role}: {c.Content}");
            }

            var prompt = $"""
                Ti si iskusan agent korisničke podrške u telekom kompaniji. Treba da napišeš stručan i ljubazan odgovor klijentu na osnovu opisa problema i baze znanja.

                KATEGORIJA TIKETA: {request.Category}
                NASLOV: {request.Title}
                OPIS PROBLEMA: {request.Description}

                PRETHODNA KOMUNIKACIJA:
                {(conversationHistory.Length > 0 ? conversationHistory.ToString() : "(Nema prethodne komunikacije)")}

                BAZA ZNANJA ZA OVU KATEGORIJU:
                {kb}

                Napiši odgovor na bosanskom jeziku koristeći "Vi" formu. Odgovor treba da:
                1. Potvrdi da si razumio problem
                2. Ponudi konkretne korake za rješavanje ili informacije
                3. Bude koncizan (3-5 rečenica)
                4. Bude profesionalan i ljubazan

                Vrati SAMO tekst odgovora, bez ikakvog uvoda ili zaključka.
                """;

            var text = await CallGeminiAsync(prompt, temperature: 0.5, maxTokens: 1024);
            return new AgentSuggestionResponseDto { Suggestion = text };
        }

        // ── PB-58 / US-98, US-99: Admin insights ──────────────────────────────
        public async Task<AdminInsightsResponseDto> GetAdminInsightsAsync(AdminInsightsRequestDto request)
        {
            var categoryLines = string.Join(", ", request.TopProblemCategories.Select(c => $"{c.Category}: {c.Count}"));
            var agentLines = string.Join(", ", request.TopAgentWorkload.Take(5).Select(a => $"{a.AgentName}: {a.TicketCount}"));

            var closureRate = request.TotalTickets > 0
                ? (double)request.ClosedTickets / request.TotalTickets * 100 : 0;
            var staleRate = request.OpenTickets > 0
                ? (double)request.StaleTickets / request.OpenTickets * 100 : 0;
            var unassignedRate = request.OpenTickets > 0
                ? (double)request.UnassignedTickets / request.OpenTickets * 100 : 0;

            var prompt = $$"""
                Ti si iskusan analitičar kvaliteta usluge u telekom kompaniji. Na osnovu metrika helpdesk sistema napiši detaljan izvještaj za administratora.

                PERIOD: {{request.Period}}

                KLJUČNE METRIKE:
                - Ukupno tiketa: {{request.TotalTickets}} | Otvoreni: {{request.OpenTickets}} | Zatvoreni: {{request.ClosedTickets}}
                - Stopa zatvaranja: {{closureRate:F1}}%
                - Čeka zatvaranje (klijent treba potvrditi): {{request.ClosureRequestedTickets}}
                - Nedodijeljeni tiketi: {{request.UnassignedTickets}} ({{unassignedRate:F1}}% otvorenih)
                - Zastarjeli tiketi (bez aktivnosti 7+ dana): {{request.StaleTickets}} ({{staleRate:F1}}% otvorenih)
                - Prosj. vrijeme prvog odgovora: {{(request.AvgFirstResponseMinutes.HasValue ? $"{request.AvgFirstResponseMinutes:F0} minuta" : "nema podataka")}}
                - Prosj. vrijeme rješavanja: {{(request.AvgResolutionHours.HasValue ? $"{request.AvgResolutionHours:F1} sati" : "nema podataka")}}
                - Prosječna ocjena zadovoljstva: {{(request.AvgRating.HasValue ? $"{request.AvgRating:F2}/5.00" : "nema ocjena")}}
                - Raspodjela po kategorijama: {{(string.IsNullOrEmpty(categoryLines) ? "nema podataka" : categoryLines)}}
                - Opterećenje agenata: {{(string.IsNullOrEmpty(agentLines) ? "nema podataka" : agentLines)}}

                Vrati ISKLJUČIVO validan JSON objekat (bez markdown, bez teksta izvan JSON-a):
                {
                  "narrative": "5-6 rečenica na bosanskom: (1) opći pregled broja i toka tiketa s konkretnim brojevima i postocima, (2) komentar na efikasnost tima na osnovu vremena odgovora i rješavanja, (3) komentar na zadovoljstvo korisnika s ocjenom, (4) dominantne kategorije problema i što to znači za tim, (5) zaključna ocjena stanja sistema.",
                  "anomalies": [
                    {"title": "Koncizan naziv (max 5 riječi)", "description": "Precizno objašnjenje s konkretnim brojem/postotkom i zašto je to problem"}
                  ],
                  "recommendations": [
                    {"title": "Akcioni naslov (max 5 riječi)", "description": "Šta tačno uraditi, ko treba uraditi i koji je očekivani efekt. Budi konkretan.", "teamFilter": "INTERNET|TV|MOBILE_NETWORK|BILLING|TECHNICAL_SUPPORT ili null"}
                  ]
                }

                Stroga pravila:
                - narrative: UVIJEK popuni, minimum 5 rečenica, MORA sadržavati konkretne brojeve iz metrika
                - anomalies: 1-3 anomalije ako postoji bar jedan od uslova: zastarjeli > 15% otvorenih, ocjena < 3.5, prvi odgovor > 60 min, nedodijeljeni > 25% otvorenih, stopa zatvaranja < 40%. Prazna lista [] SAMO ako je sve u redu.
                - recommendations: UVIJEK 2-3 preporuke bazirane na podacima, čak i kad je stanje dobro (preporuke za održavanje ili unapređenje)
                - teamFilter: postavi na kategoriju tima ako se preporuka tiče jedne kategorije, null ako je opća
                """;

            var raw = await CallGeminiAsync(prompt, temperature: 0.3, maxTokens: 2048);

            try
            {
                var result = JsonSerializer.Deserialize<AdminInsightsResponseDto>(raw, _jsonOpts);
                return result ?? FallbackInsights(raw);
            }
            catch
            {
                return FallbackInsights(raw);
            }
        }

        // ── Groq REST call (OpenAI-compatible) ────────────────────────────────
        private async Task<string> CallGeminiAsync(string prompt, double temperature, int maxTokens)
        {
            if (string.IsNullOrWhiteSpace(_apiKey) || _apiKey.StartsWith("YOUR_"))
                throw new InvalidOperationException("GROQ_API_KEY nije konfigurisan. Dodajte ključ u .env fajl.");

            const string url = "https://api.groq.com/openai/v1/chat/completions";

            var body = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature,
                max_tokens = maxTokens
            };

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            using var response = await _http.PostAsJsonAsync(url, body);
            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode == 429)
                    throw new InvalidOperationException("Groq API limit zahtjeva je dostignut. Pričekajte trenutak i pokušajte ponovo.");
                if ((int)response.StatusCode == 401)
                    throw new InvalidOperationException("Groq API ključ nije validan. Provjerite GROQ_API_KEY u .env fajlu.");
                response.EnsureSuccessStatusCode();
            }

            var doc = await response.Content.ReadFromJsonAsync<JsonDocument>();
            var text = doc?.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            return StripCodeFences(text);
        }

        private static string StripCodeFences(string text)
        {
            var t = text.Trim();
            if (t.StartsWith("```"))
            {
                var firstNewline = t.IndexOf('\n');
                if (firstNewline >= 0) t = t[(firstNewline + 1)..];
                if (t.EndsWith("```")) t = t[..^3];
            }
            return t.Trim();
        }

        private static AdminInsightsResponseDto FallbackInsights(string raw) => new()
        {
            Narrative = raw.Length > 600 ? raw[..600] : raw,
            Anomalies = [],
            Recommendations = []
        };
    }
}
