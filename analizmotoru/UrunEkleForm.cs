using System;
using System.Windows.Forms;
using System.Data.SqlClient; // SQL kütüphanesini unutmuyoruz

namespace analizmotoru
{
    public partial class UrunEkleForm : Form
    {
        // SQL Bağlantı Adresimiz (Form1 ile aynı)
        SqlConnection baglanti = new SqlConnection(@"Server=.\SQLEXPRESS;Initial Catalog=EczaneDB;Integrated Security=True;TrustServerCertificate=True");

        public UrunEkleForm()
        {
            InitializeComponent();
        }

        // KAYDET BUTONUNA BASILINCA ÇALIŞACAK KOD
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            // 1. Boş alan kontrolü yapalım
            if (txtAd.Text == "" || cmbKategori.Text == "")
            {
                MessageBox.Show("Lütfen ürün adı ve kategori alanlarını doldurunuz!", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Bağlantı kapalıysa aç
                if (baglanti.State == System.Data.ConnectionState.Closed)
                    baglanti.Open();

                // 2. SQL Ekleme Komutu
                string sorgu = "INSERT INTO Urunler (UrunAdi, Kategori, Miad) VALUES (@p1, @p2, @p3)";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);

                // Parametreleri kutucuklardan alıyoruz
                komut.Parameters.AddWithValue("@p1", txtAd.Text);       // txtAd: Ürün adı kutusu
                komut.Parameters.AddWithValue("@p2", cmbKategori.Text); // cmbKategori: Kategori kutusu
                komut.Parameters.AddWithValue("@p3", dtpTarih.Value);   // dtpTarih: Tarih seçici

                // Komutu çalıştır (Veritabanına kaydet)
                komut.ExecuteNonQuery();

                // Bağlantıyı kapat
                baglanti.Close();

                MessageBox.Show("Ürün başarıyla kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 3. İşlem bitince pencereyi kapat
                this.Close();
            }
            catch (Exception hata)
            {
                MessageBox.Show("Bir hata oluştu: " + hata.Message);
            }
        }

        private void cmbKategori_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}