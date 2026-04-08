using System;

namespace analizmotoru.Models
{
    public class AnalysisResult
    {
        // Motorun kararı (Örn: "Kritik Miad", "Normal")
        public string Action { get; set; }

        // Önerilen indirim oranı (Örn: 0.20m)
        public decimal SuggestedDiscount { get; set; }

        // Kararın teknik açıklaması
        public string Reason { get; set; }
    }
}