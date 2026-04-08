using System;
using System.Collections.Generic;

namespace analizmotoru.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; } // "İlaç", "Vitamin", "Dermokozmetik"
        public decimal BasePrice { get; set; }
        public DateTime CreatedAt { get; set; }

        // Eczane için kritik alan: Son Kullanma Tarihi
        public DateTime ExpirationDate { get; set; }

        public List<Sale> Sales { get; set; } = new List<Sale>();

        // Ürünün son kullanma tarihine kaç ay kaldığını hesaplar
        public int MonthsToExpiration =>
            ((ExpirationDate.Year - DateTime.Now.Year) * 12) + ExpirationDate.Month - DateTime.Now.Month;
    }
}