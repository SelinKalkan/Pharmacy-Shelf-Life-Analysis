using System;
using System.Drawing;
using System.Windows.Forms;

namespace analizmotoru
{
    public partial class GirisFormu : Form
    {
        // Kontrolleri Kodla Oluşturuyoruz
        private TextBox txtKullaniciAdi;
        private TextBox txtSifre;
        private Button btnGiris;
        private Button btnCikis;
        private Panel solPanel;
        private Label lblBaslik;
        private Label lblAltBaslik;

        public GirisFormu()
        {
            InitializeComponent_Elle(); // Tasarımı Çiz
        }

        // --- TASARIM KODLARI (MODERN GÖRÜNÜM) ---
        private void InitializeComponent_Elle()
        {
            this.Size = new Size(750, 450);
            this.FormBorderStyle = FormBorderStyle.None; // Çerçevesiz Modern Ekran
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // 1. SOL TARAFTAKİ RENKLİ PANEL
            solPanel = new Panel();
            solPanel.Dock = DockStyle.Left;
            solPanel.Width = 300;
            solPanel.BackColor = Color.Crimson; // ANA TEMA RENGİNİZ

            PictureBox pbLogo = new PictureBox();
            try
            {
                pbLogo.Image = Image.FromFile("logo.png");
            }
            catch
            {
                pbLogo.ImageLocation = "https://cdn-icons-png.flaticon.com/512/3063/3063176.png";
            }

            pbLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pbLogo.Size = new Size(100, 100);
            pbLogo.Location = new Point(100, 80);
            solPanel.Controls.Add(pbLogo);

            Label lblLogo = new Label();
            lblLogo.Text = "Eczane\nStok\nSistemi";
            lblLogo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblLogo.ForeColor = Color.White;
            lblLogo.AutoSize = true;
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            lblLogo.Location = new Point(95, 200);
            solPanel.Controls.Add(lblLogo);

            this.Controls.Add(solPanel);

            // 2. BAŞLIKLAR
            lblBaslik = new Label();
            lblBaslik.Text = "HOŞ GELDİNİZ";
            lblBaslik.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblBaslik.ForeColor = Color.DimGray;
            lblBaslik.Location = new Point(350, 50);
            lblBaslik.AutoSize = true;
            this.Controls.Add(lblBaslik);

            lblAltBaslik = new Label();
            lblAltBaslik.Text = "Lütfen bilgilerinizi giriniz";
            lblAltBaslik.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblAltBaslik.ForeColor = Color.Gray;
            lblAltBaslik.Location = new Point(355, 90);
            lblAltBaslik.AutoSize = true;
            this.Controls.Add(lblAltBaslik);

            // 3. KULLANICI ADI KUTUSU
            Label lblUser = new Label();
            lblUser.Text = "Kullanıcı Adı:";
            lblUser.Location = new Point(350, 140);
            lblUser.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.Controls.Add(lblUser);

            txtKullaniciAdi = new TextBox();
            txtKullaniciAdi.Location = new Point(350, 165);
            txtKullaniciAdi.Size = new Size(300, 30);
            txtKullaniciAdi.Font = new Font("Segoe UI", 12);
            this.Controls.Add(txtKullaniciAdi);

            // 4. ŞİFRE KUTUSU
            Label lblPass = new Label();
            lblPass.Text = "Şifre:";
            lblPass.Location = new Point(350, 210);
            lblPass.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.Controls.Add(lblPass);

            txtSifre = new TextBox();
            txtSifre.Location = new Point(350, 235);
            txtSifre.Size = new Size(300, 30);
            txtSifre.Font = new Font("Segoe UI", 12);
            txtSifre.PasswordChar = '●';
            this.Controls.Add(txtSifre);

            // 5. GİRİŞ BUTONU (YEŞİL YERİNE KIRMIZI/CRIMSON YAPILDI)
            btnGiris = new Button();
            btnGiris.Text = "GİRİŞ YAP";
            btnGiris.Location = new Point(350, 300);
            btnGiris.Size = new Size(300, 45);
            btnGiris.BackColor = Color.Crimson; // YEŞİL RENK BURADA DEĞİŞTİ
            btnGiris.ForeColor = Color.White;
            btnGiris.FlatStyle = FlatStyle.Flat;
            btnGiris.FlatAppearance.BorderSize = 0;
            btnGiris.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnGiris.Cursor = Cursors.Hand;
            btnGiris.Click += new EventHandler(btnGiris_Click);
            this.Controls.Add(btnGiris);

            // 6. ÇIKIŞ (X) BUTONU (KAPANMA SORUNU İÇİN GÜNCELLENDİ)
            btnCikis = new Button();
            btnCikis.Text = "X";
            btnCikis.Location = new Point(700, 0);
            btnCikis.Size = new Size(50, 40);
            btnCikis.BackColor = Color.White;
            btnCikis.ForeColor = Color.Black;
            btnCikis.FlatStyle = FlatStyle.Flat;
            btnCikis.FlatAppearance.BorderSize = 0;
            btnCikis.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnCikis.Cursor = Cursors.Hand;

            // Uygulamayı tamamen kapatması için event güncellendi
            btnCikis.Click += (s, e) => {
                Application.Exit();
            };

            btnCikis.MouseEnter += (s, e) => { btnCikis.BackColor = Color.Red; btnCikis.ForeColor = Color.White; };
            btnCikis.MouseLeave += (s, e) => { btnCikis.BackColor = Color.White; btnCikis.ForeColor = Color.Black; };
            this.Controls.Add(btnCikis);

            // Form üzerindeyken ENTER tuşuna basıldığında Giriş butonunu çalıştırır
            this.AcceptButton = btnGiris;
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            string kAdi = txtKullaniciAdi.Text;
            string sifre = txtSifre.Text;

            if (kAdi == "admin" && sifre == "1234")
            {
                Form1 anaEkran = new Form1();
                anaEkran.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Hatalı Kullanıcı Adı veya Şifre!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}