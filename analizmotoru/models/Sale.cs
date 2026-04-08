using System;

namespace analizmotoru.Models
{
    public class Sale
    {
        // İlacın/Ürünün satıldığı tarih
        public DateTime SoldAt { get; set; }

        // Satış fiyatı (İndirimli mi satıldı görmek için)
        public decimal SoldPrice { get; set; }

        // Kaç adet satıldı?
        public int Quantity { get; set; }
    }
}