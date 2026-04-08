using System;
using analizmotoru.Models;

namespace analizmotoru.Services
{
    public class PricingEngine
    {
        public AnalysisResult Analyze(Product product)
        {
            int monthsLeft = product.MonthsToExpiration;

            // 1. Durum: Miadı Dolmak Üzere (Riskli)
            if (monthsLeft <= 3 && monthsLeft > 0)
            {
                return new AnalysisResult
                {
                    Action = "Kritik Miad!",
                    SuggestedDiscount = product.Category != "İlaç" ? 0.40m : 0, // İlaç değilse %40 indirim
                    Reason = $"Son kullanma tarihine sadece {monthsLeft} ay kaldı! Acil çıkış yapılmalı."
                };
            }
            // 2. Durum: Yavaş Giden Dermokozmetik/Vitamin
            else if (monthsLeft <= 6 && product.Sales.Count < 5)
            {
                return new AnalysisResult
                {
                    Action = "Kampanya Önerisi",
                    SuggestedDiscount = 0.15m,
                    Reason = "6 ay içinde miadı dolacak ve satış hızı düşük. %15 indirimle hızlandırılabilir."
                };
            }

            return new AnalysisResult
            {
                Action = "Stok Sağlıklı",
                SuggestedDiscount = 0,
                Reason = "Miad riski yok, satışlar normal."
            };
        }
    }
}