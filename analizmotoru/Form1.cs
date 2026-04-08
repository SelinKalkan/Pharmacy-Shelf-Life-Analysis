using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Web.WebView2.WinForms;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace analizmotoru
{
    public partial class Form1 : Form
    {
        // --- AYARLAR ---
        private const string API_KEY = "BURAYA_API_KEY_YAPISTIR";

        // DÜZELTME: 'gemini-1.5-flash' yerine en klasik ve garantili 'gemini-pro' modelini yazdık.
        // Bu model hata vermeden çalışır.
        private const string API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key=" + API_KEY;

        // --- DEĞİŞKENLER ---
        SqlConnection baglanti = new SqlConnection(@"Server=.\SQLEXPRESS;Initial Catalog=EczaneDB;Integrated Security=True;TrustServerCertificate=True");
        List<Product> products = new List<Product>();

        // İstatistik Kartları
        Label lblKartToplam;
        Label lblKartRiskli;
        Label lblKartGuvenli;

        // Paneller
        Panel pnlNobetci;
        Panel pnlAIMerkezi;
        Panel pnlChat;
        Panel pnlAnaliz;

        WebView2 webViewHarita;

        // AI Chat
        RichTextBox rtbChatGecmisi;
        TextBox txtChatGiris;

        // Butonlar
        Button btnNobetci;
        Button btnAI;

        public class Product
        {
            public string Name { get; set; }
            public string Category { get; set; }
            public DateTime ExpirationDate { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Tablo Görünümü
            dgvRapor.EnableHeadersVisualStyles = false;
            dgvRapor.ColumnHeadersDefaultCellStyle.BackColor = Color.Crimson;
            dgvRapor.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRapor.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvRapor.DefaultCellStyle.SelectionBackColor = Color.Crimson;
            dgvRapor.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvRapor.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRapor.RowHeadersVisible = false;
            dgvRapor.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRapor.BackgroundColor = Color.White;

            ModernArayuzUygula();
        }

        private void ModernArayuzUygula()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Text = "Eczane Stok Takip - AI Powered";
            this.BackColor = Color.White;
            this.WindowState = FormWindowState.Maximized;

            Panel solMenu = new Panel();
            solMenu.Dock = DockStyle.Left;
            solMenu.Width = 260;
            solMenu.BackColor = Color.Crimson;
            this.Controls.Add(solMenu);

            Label lblLogo = new Label();
            lblLogo.Text = "💊 Eczane\nSistemi";
            lblLogo.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblLogo.ForeColor = Color.White;
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            lblLogo.AutoSize = false;
            lblLogo.Size = new Size(260, 150);
            lblLogo.Location = new Point(0, 30);
            solMenu.Controls.Add(lblLogo);

            // --- BUTONLAR ---
            Font menuFont = new Font("Segoe UI", 12, FontStyle.Bold);

            if (btnEkle != null)
            {
                btnEkle.Parent = solMenu; btnEkle.Dock = DockStyle.Top; btnEkle.Height = 70;
                btnEkle.Font = menuFont; btnEkle.ForeColor = Color.White; btnEkle.FlatStyle = FlatStyle.Flat;
                btnEkle.FlatAppearance.BorderSize = 0; btnEkle.Text = "➕  Yeni Ürün Ekle"; btnEkle.BackColor = Color.Crimson; btnEkle.BringToFront();
            }

            if (btnListele != null)
            {
                btnListele.Parent = solMenu; btnListele.Dock = DockStyle.Top; btnListele.Height = 70;
                btnListele.Font = menuFont; btnListele.ForeColor = Color.White; btnListele.FlatStyle = FlatStyle.Flat;
                btnListele.FlatAppearance.BorderSize = 0; btnListele.Text = "🔍  İlaçları Listele"; btnListele.BackColor = Color.Crimson; btnListele.BringToFront();
            }

            btnNobetci = new Button();
            btnNobetci.Parent = solMenu; btnNobetci.Dock = DockStyle.Top; btnNobetci.Height = 70;
            btnNobetci.Font = menuFont; btnNobetci.ForeColor = Color.White; btnNobetci.FlatStyle = FlatStyle.Flat;
            btnNobetci.FlatAppearance.BorderSize = 0; btnNobetci.Text = "🌙  Nöbetçi Eczaneler"; btnNobetci.BackColor = Color.Crimson;
            btnNobetci.Click += (s, e) => HaritayiAc();
            btnNobetci.BringToFront();

            btnAI = new Button();
            btnAI.Parent = solMenu; btnAI.Dock = DockStyle.Top; btnAI.Height = 70;
            btnAI.Font = menuFont; btnAI.ForeColor = Color.White; btnAI.FlatStyle = FlatStyle.Flat;
            btnAI.FlatAppearance.BorderSize = 0; btnAI.Text = "🤖  AI Merkezi"; btnAI.BackColor = Color.Crimson;
            btnAI.Click += (s, e) => AIMerkeziniAc();
            btnAI.BringToFront();

            if (btnSil != null)
            {
                btnSil.Parent = solMenu; btnSil.Dock = DockStyle.Bottom; btnSil.Height = 70;
                btnSil.Font = menuFont; btnSil.ForeColor = Color.White; btnSil.BackColor = Color.FromArgb(150, 0, 0);
                btnSil.FlatStyle = FlatStyle.Flat; btnSil.FlatAppearance.BorderSize = 0; btnSil.Text = "🗑️  Seçiliyi Sil";
            }

            // --- KARTLARI OLUŞTUR VE DEĞİŞKENLERE ATA ---
            lblKartToplam = KartEkle("Toplam Ürün", "0", Color.Orange, 280);
            lblKartRiskli = KartEkle("Riskli Ürünler", "0", Color.Crimson, 510);
            lblKartGuvenli = KartEkle("Stok Durumu", "%0", Color.Teal, 740);

            dgvRapor.Location = new Point(280, 130);
            dgvRapor.Size = new Size(this.Width - 320, this.Height - 200);

            // --- PANELLER ---
            pnlNobetci = new Panel { Location = dgvRapor.Location, Size = dgvRapor.Size, BackColor = Color.White, Visible = false, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            this.Controls.Add(pnlNobetci);
            webViewHarita = new WebView2 { Dock = DockStyle.Fill };
            pnlNobetci.Controls.Add(webViewHarita);

            pnlAIMerkezi = new Panel { Location = dgvRapor.Location, Size = dgvRapor.Size, BackColor = Color.WhiteSmoke, Visible = false, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            this.Controls.Add(pnlAIMerkezi);
            AIMerkeziTasariminiYap();

            pnlChat = new Panel { Location = dgvRapor.Location, Size = dgvRapor.Size, BackColor = Color.White, Visible = false, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            this.Controls.Add(pnlChat);
            ChatEkraniTasarla();

            pnlAnaliz = new Panel { Location = dgvRapor.Location, Size = dgvRapor.Size, BackColor = Color.AliceBlue, Visible = false, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            this.Controls.Add(pnlAnaliz);
        }

        private async Task<string> GeminiyeSor(string kullaniciSorusu)
        {
            if (API_KEY == "BURAYA_API_KEY_YAPISTIR") return "Lütfen kodun içine API Key'inizi yapıştırın!";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string sistemMesaji = "Sen uzman bir eczacı asistanısın. İlaçlar, sağlık ve stok yönetimi hakkında kısa, net ve profesyonel cevaplar ver. Kullanıcı ile Türkçe konuş.";
                    string tamSoru = $"{sistemMesaji}\n\nKullanıcı Sorusu: {kullaniciSorusu}";

                    var requestBody = new
                    {
                        contents = new[]
                        {
                            new { parts = new[] { new { text = tamSoru } } }
                        }
                    };

                    string jsonContent = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(API_URL, content);
                    string responseString = await response.Content.ReadAsStringAsync();

                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);

                    if (jsonResponse.error != null)
                    {
                        string hataMesaji = jsonResponse.error.message;
                        return "Hata: " + hataMesaji;
                    }

                    if (jsonResponse.candidates != null && jsonResponse.candidates.Count > 0)
                    {
                        string cevap = jsonResponse.candidates[0].content.parts[0].text;
                        return cevap;
                    }
                    else
                    {
                        return "Yapay zeka boş cevap döndü.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Bağlantı Hatası: " + ex.Message;
            }
        }

        private async void MesajGonder()
        {
            string soru = txtChatGiris.Text.Trim();
            if (string.IsNullOrEmpty(soru)) return;

            ChatEkle("SEN: " + soru, Color.Black);
            txtChatGiris.Clear();
            txtChatGiris.Enabled = false;

            ChatEkle("🤖 AI Düşünüyor...", Color.Gray);
            string cevap = await GeminiyeSor(soru);
            ChatEkle("🤖 UZMAN: " + cevap, Color.Blue);

            rtbChatGecmisi.ScrollToCaret();
            txtChatGiris.Enabled = true;
            txtChatGiris.Focus();
        }

        private void AIMerkeziTasariminiYap()
        {
            Label lbl = new Label { Text = "Yapay Zeka Asistan Merkezi", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.DarkSlateBlue, AutoSize = true, Location = new Point(50, 50) };
            pnlAIMerkezi.Controls.Add(lbl);

            Button btnSohbet = AICenterButonuOluştur("💬 Eczacı Asistanıyla Sohbet Et", "Tüm soruların için gerçek yapay zeka.", Color.MediumPurple, 120);
            btnSohbet.Click += (s, e) => ChatEkraniniAc();
            pnlAIMerkezi.Controls.Add(btnSohbet);

            Button btnEtkilesim = AICenterButonuOluştur("⚠️ İlaç Etkileşim Kontrolü", "İki ilacın risk analizi.", Color.Orange, 250);
            btnEtkilesim.Click += (s, e) => MessageBox.Show("Bu modül de yakında yapay zekaya bağlanacak!");
            pnlAIMerkezi.Controls.Add(btnEtkilesim);

            Button btnAnaliz = AICenterButonuOluştur("📊 İşletme Sağlık Analizi", "Stok puanı hesapla.", Color.Teal, 380);
            btnAnaliz.Click += (s, e) => AnalizRaporunuGoster();
            pnlAIMerkezi.Controls.Add(btnAnaliz);
        }

        private Button AICenterButonuOluştur(string baslik, string aciklama, Color renk, int yKonum)
        {
            Button btn = new Button();
            btn.Size = new Size(600, 100); btn.Location = new Point(50, yKonum);
            btn.FlatStyle = FlatStyle.Flat; btn.BackColor = Color.White;
            btn.FlatAppearance.BorderColor = renk; btn.FlatAppearance.BorderSize = 2;
            btn.TextAlign = ContentAlignment.TopLeft; btn.Padding = new Padding(10);
            btn.Font = new Font("Segoe UI", 14, FontStyle.Bold); btn.ForeColor = renk;
            btn.Text = baslik + "\n\r";
            ToolTip ipucu = new ToolTip(); ipucu.SetToolTip(btn, aciklama);
            return btn;
        }

        private void ChatEkraniTasarla()
        {
            rtbChatGecmisi = new RichTextBox();
            rtbChatGecmisi.Location = new Point(20, 20);
            rtbChatGecmisi.Size = new Size(pnlChat.Width - 40, pnlChat.Height - 100);
            rtbChatGecmisi.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbChatGecmisi.ReadOnly = true; rtbChatGecmisi.Font = new Font("Segoe UI", 12);
            rtbChatGecmisi.BackColor = Color.WhiteSmoke;
            rtbChatGecmisi.Text = "🤖 SİSTEM: Merhaba! Ben Google Gemini destekli Eczacı Asistanıyım. Her şeyi sorabilirsin.\n\n";
            pnlChat.Controls.Add(rtbChatGecmisi);

            txtChatGiris = new TextBox();
            txtChatGiris.Location = new Point(20, pnlChat.Height - 60);
            txtChatGiris.Size = new Size(pnlChat.Width - 140, 40);
            txtChatGiris.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtChatGiris.Font = new Font("Segoe UI", 14);
            txtChatGiris.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) MesajGonder(); };
            pnlChat.Controls.Add(txtChatGiris);

            Button btnGonder = new Button();
            btnGonder.Text = "GÖNDER"; btnGonder.Location = new Point(pnlChat.Width - 110, pnlChat.Height - 62);
            btnGonder.Size = new Size(90, 35); btnGonder.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnGonder.BackColor = Color.DodgerBlue; btnGonder.ForeColor = Color.White; btnGonder.FlatStyle = FlatStyle.Flat;
            btnGonder.Click += (s, e) => MesajGonder();
            pnlChat.Controls.Add(btnGonder);

            Button btnGeri = new Button();
            btnGeri.Text = "🔙 Menüye Dön"; btnGeri.Location = new Point(pnlChat.Width - 150, 10);
            btnGeri.Size = new Size(120, 30); btnGeri.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGeri.Click += (s, e) => AIMerkeziniAc();
            pnlChat.Controls.Add(btnGeri); btnGeri.BringToFront();
        }

        private void ChatEkle(string mesaj, Color renk)
        {
            rtbChatGecmisi.SelectionStart = rtbChatGecmisi.TextLength;
            rtbChatGecmisi.SelectionLength = 0;
            rtbChatGecmisi.SelectionColor = renk;
            rtbChatGecmisi.AppendText(mesaj + "\n\n");
            rtbChatGecmisi.SelectionColor = rtbChatGecmisi.ForeColor;
        }

        private void GizleHepsini()
        {
            dgvRapor.Visible = false;
            if (pnlNobetci != null) pnlNobetci.Visible = false;
            if (pnlAIMerkezi != null) pnlAIMerkezi.Visible = false;
            if (pnlChat != null) pnlChat.Visible = false;
            if (pnlAnaliz != null) pnlAnaliz.Visible = false;
        }

        private void AIMerkeziniAc() { GizleHepsini(); pnlAIMerkezi.Visible = true; pnlAIMerkezi.BringToFront(); }
        private void ChatEkraniniAc() { GizleHepsini(); pnlChat.Visible = true; pnlChat.BringToFront(); txtChatGiris.Focus(); }

        private async void HaritayiAc()
        {
            if (pnlNobetci != null)
            {
                GizleHepsini(); pnlNobetci.Visible = true; pnlNobetci.BringToFront();
                await webViewHarita.EnsureCoreWebView2Async(); webViewHarita.CoreWebView2.Navigate("https://www.google.com/maps/search/nöbetçi+eczane+çerkezköy");
            }
        }

        private Label KartEkle(string baslik, string deger, Color renk, int xKonum)
        {
            Panel pnl = new Panel(); pnl.Location = new Point(xKonum, 20); pnl.Size = new Size(200, 80); pnl.BackColor = Color.White;
            Panel serit = new Panel { Dock = DockStyle.Left, Width = 10, BackColor = renk }; pnl.Controls.Add(serit);
            Label lblB = new Label { Text = baslik, Location = new Point(20, 15), Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, AutoSize = true }; pnl.Controls.Add(lblB);
            Label lblD = new Label { Text = deger, Location = new Point(20, 35), Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.Black, AutoSize = true }; pnl.Controls.Add(lblD);
            this.Controls.Add(pnl); return lblD;
        }

        private void KartlariHesapla()
        {
            if (products == null) return;
            int toplam = products.Count;
            int riskli = 0;
            foreach (var u in products) if ((u.ExpirationDate - DateTime.Now).TotalDays < 90) riskli++;

            if (lblKartToplam != null) lblKartToplam.Text = toplam + " Adet";
            if (lblKartRiskli != null) lblKartRiskli.Text = riskli + " Adet";
            if (lblKartGuvenli != null && toplam > 0)
            {
                lblKartGuvenli.Text = "%" + ((toplam - riskli) * 100 / toplam) + " Güvenli";
            }
            else if (lblKartGuvenli != null) lblKartGuvenli.Text = "%0 Güvenli";
        }

        private void btnListele_Click(object sender, EventArgs e)
        {
            GizleHepsini();
            dgvRapor.Visible = true;
            TabloyuHazirla();
            products.Clear();
            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();
                SqlCommand komut = new SqlCommand("Select * From Urunler", baglanti);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    Product yeniUrun = new Product(); yeniUrun.Name = dr["UrunAdi"].ToString();
                    yeniUrun.Category = dr["Kategori"].ToString(); yeniUrun.ExpirationDate = DateTime.Parse(dr["Miad"].ToString());
                    products.Add(yeniUrun);
                    TimeSpan fark = yeniUrun.ExpirationDate - DateTime.Now; int gunFarki = (int)fark.TotalDays;
                    string durumMetni = (gunFarki < 0) ? "İmha" : (gunFarki < 90) ? "İade" : "Normal";
                    dgvRapor.Rows.Add(yeniUrun.Name, yeniUrun.Category, yeniUrun.ExpirationDate.ToShortDateString(), gunFarki + " Gün", durumMetni, "İşlem");
                    SatiriRenklendir(dgvRapor.Rows.Count - 1, durumMetni);
                }
                baglanti.Close();
                KartlariHesapla();
            }
            catch (Exception ex) { baglanti.Close(); MessageBox.Show("Hata: " + ex.Message); }
        }

        private void btnEkle_Click(object sender, EventArgs e) { UrunEkleForm yeniPencere = new UrunEkleForm(); yeniPencere.ShowDialog(); }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dgvRapor.CurrentRow != null && MessageBox.Show("Silinsin mi?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                baglanti.Open(); new SqlCommand("DELETE FROM Urunler WHERE UrunAdi = '" + dgvRapor.CurrentRow.Cells[0].Value.ToString() + "'", baglanti).ExecuteNonQuery(); baglanti.Close();
            }
        }

        private void AnalizRaporunuGoster() { GizleHepsini(); pnlAnaliz.Visible = true; pnlAnaliz.Controls.Clear(); Label lbl = new Label { Text = "DETAYLI ANALİZ", Font = new Font("Segoe UI", 20, FontStyle.Bold), Location = new Point(50, 50), AutoSize = true }; pnlAnaliz.Controls.Add(lbl); Button btnGeri = new Button { Text = "🔙", Location = new Point(10, 10), Size = new Size(40, 40) }; btnGeri.Click += (s, e) => AIMerkeziniAc(); pnlAnaliz.Controls.Add(btnGeri); int riskli = 0; foreach (var u in products) if ((u.ExpirationDate - DateTime.Now).TotalDays < 90) riskli++; Label lblSonuc = new Label { Text = $"Puan: {100 - (riskli * 10)}/100\nRiskli: {riskli}", Font = new Font("Segoe UI", 16), AutoSize = true, Location = new Point(50, 120) }; pnlAnaliz.Controls.Add(lblSonuc); }
        private void SatiriRenklendir(int index, string durum) { if (durum.Contains("İmha")) { dgvRapor.Rows[index].DefaultCellStyle.BackColor = Color.FromArgb(50, 50, 50); dgvRapor.Rows[index].DefaultCellStyle.ForeColor = Color.White; } else if (durum.Contains("İade")) dgvRapor.Rows[index].DefaultCellStyle.BackColor = Color.Orange; }

        private void TabloyuHazirla()
        {
            dgvRapor.Rows.Clear();
            dgvRapor.ColumnCount = 6;
            dgvRapor.Columns[0].Name = "Ürün Adı";
            dgvRapor.Columns[1].Name = "Kategori";
            dgvRapor.Columns[2].Name = "SKT";
            dgvRapor.Columns[3].Name = "Kalan Süre";
            dgvRapor.Columns[4].Name = "DURUM";
            dgvRapor.Columns[5].Name = "YAPILACAK İŞLEM";
        }
    }
}