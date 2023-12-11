using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Stockmanagement
{
    public partial class AddProduct : Form
    {
        public AddProduct()
        {
            InitializeComponent();
        }

        //Kategorileri getirir
        public static void FillCategory(ComboBox comboBox1)
        {
            using(SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT KategoriAd FROM Kategori",conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader[0].ToString());
                    }
                    reader.Close();
                }
            }
        }

        //Birimleri getirir
        public static void FillUnit(ComboBox comboBox2)
        {
            using(SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT Birim FROM BirimTablo",conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        comboBox2.Items.Add(reader[0].ToString());
                    }
                    reader.Close();
                }
            }
        }
        
        private bool IsNullOrEmpty(TextBox textbox,string message)
        {
            if(string.IsNullOrEmpty(textbox.Text))
            {
                MessageBox.Show(message);
                return true;
            }
            return false;
        }

        private bool AddNewProduct()
        {
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Urunler (Urun_Ad,Birim,Miktar,Fiyat,Stok,Kategori,EklenmeTarihi,EkleyenKullanici) VALUES" +
                        "(@urunad,@birimad,@miktar,@fiyat,@stok,@kategoriad,@eklenmetarihi,@ekleyenkullanici)", conn))
                    {
                        cmd.Parameters.AddWithValue("@urunad", textBox1.Text);
                        cmd.Parameters.AddWithValue("@birimad", comboBox2.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@miktar", string.IsNullOrEmpty(textBox2.Text) ? (object)DBNull.Value : textBox2.Text);
                        cmd.Parameters.AddWithValue("@fiyat", decimal.Parse(textBox4.Text));
                        cmd.Parameters.AddWithValue("@stok", textBox3.Text);
                        cmd.Parameters.AddWithValue("kategoriad", comboBox1.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@eklenmetarihi", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ekleyenkullanici", CustomUser.Name);
                        int affectedrows = cmd.ExecuteNonQuery();
                        return affectedrows > 0;
                    }
                }
        }

        private void AddProduct_Load(object sender, EventArgs e)
        {
            FillCategory(comboBox1);
            FillUnit(comboBox2);
  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IsNullOrEmpty(textBox1, "Ürün Adı boş bırakılamaz") ||
                IsNullOrEmpty(textBox3, "Stok miktarı boş bırakılamaz") ||
                IsNullOrEmpty(textBox4, "Fiyat seçimi boş bırakılamaz") ||
               (comboBox2.SelectedItem.ToString() == "Gram" && IsNullOrEmpty(textBox2, "Miktar seçimi boş bırakılamaz")))
            {
                return;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Kategori seçimi boş bırakılamaz.");
                return;
            }
            if(comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Birim seçimi boş bırakılamaz.");
                return;
            }

            DialogResult result = MessageBox.Show("Bilgilerin Doğruluğundan emin misiniz?", "", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if(AddNewProduct())
                {
                        MessageBox.Show("Ürün Eklendi!");
                        string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "log.txt");
                        using (StreamWriter writer = File.AppendText(LogFilePath))
                        {
                            writer.WriteLine("Ürün Ekleme");
                            writer.WriteLine(new string('-', 50));
                            writer.WriteLine($"Yeni Ürün Adı : {textBox1.Text}");
                            writer.WriteLine($"Kategori: {comboBox1.SelectedItem.ToString()}");
                            writer.WriteLine($"Birim : {comboBox2.SelectedItem.ToString()}");
                            if (!string.IsNullOrEmpty(textBox2.Text))
                            {
                                writer.WriteLine($"Miktar: {textBox2.Text} Gram");
                            }
                            writer.WriteLine($"Stok : {textBox3.Text} Adet");
                            writer.WriteLine($"Fiyat : {textBox4.Text} TL");
                            writer.WriteLine($"Eklenme Tarihi :{DateTime.Now}");
                            writer.WriteLine($"Ekleyen Kişi :{CustomUser.UserName}");
                            writer.WriteLine(new string('-', 50));
                          
                        }
                        textBox1.Text = null;
                        textBox2.Text = null;
                        textBox3.Text = null;
                        textBox4.Text = null;
                        comboBox1.SelectedItem = null; 
                        comboBox2.SelectedItem = null;
                        this.Close();
                }
                else
                {
                        MessageBox.Show("Ürün eklenirken bir hatayla karşılaşıldı!");
                }
            }
        }
        
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox2.SelectedIndex != -1)
            {
                if(comboBox2.SelectedItem.ToString() == "Adet")
                {
                    textBox2.Enabled = false;
                }
                else { textBox2.Enabled = true; }
            }
        }

        private void AddProduct_FormClosed(object sender, FormClosedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
        }
    }
}
