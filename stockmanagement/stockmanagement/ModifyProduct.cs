using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Stockmanagement
{
    public partial class ModifyProduct : Form
    {
        public ModifyProduct()
        {
            InitializeComponent();
        }


        private void ModifyProduct_Load(object sender, EventArgs e)
        {
            try
            {
                foreach (Control control in this.Controls)
                {
                    control.Enabled = false;
                    listBox1.Enabled = true;
                }

                RemoveProduct.RefreshlistBox(listBox1);
                AddProduct.FillCategory(comboBox1);
                AddProduct.FillUnit(comboBox2);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
              
                foreach (Control control in this.Controls)
                {
                    control.Enabled = true;
                    textBox4.Enabled = false;
                }
                string query = "SELECT * FROM Urunler WHERE Urun_Ad = @urunad";
                using(SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    if (listBox1.SelectedIndex != -1)
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("urunad", listBox1.SelectedItem.ToString());
                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                textBox4.Text = reader["Urun_Ad"].ToString();
                                comboBox1.SelectedItem = reader["Kategori"].ToString();
                                comboBox2.SelectedItem = reader["Birim"].ToString();

                                if (reader["Birim"].ToString() == "Gram")
                                {
                                    textBox1.Text = reader["Miktar"].ToString().Trim();
                                }
                                else
                                {
                                    textBox1.Enabled = false;
                                }
                                textBox2.Text = reader["Stok"].ToString();
                                textBox3.Text = reader["Fiyat"].ToString();
                            }
                            reader.Close();
                        }
                    }
                }
                SaveOriginalValue();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
            {
                if (comboBox2.SelectedItem.ToString() == "Gram")
                {
                    textBox1.Enabled = true;
                }
                else
                {
                    textBox1.Text = null;
                    textBox1.Enabled = false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                LogOldProduct();
                string query =
                    "UPDATE Urunler SET Kategori = @kategori, Birim = @birim, Miktar = @miktar, Stok = @stok, Fiyat = @fiyat,EklenmeTarihi = @zaman, EkleyenKullanici = @ekleyenkullanici WHERE Urun_Ad = @urunad ";
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    if (IsProductModified())
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            DateTime now = DateTime.Now;
                            cmd.Parameters.AddWithValue("@urunad", textBox4.Text);
                            cmd.Parameters.AddWithValue("@kategori", comboBox1.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@birim", comboBox2.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@miktar",
                                string.IsNullOrEmpty(textBox1.Text) ? (object)DBNull.Value : textBox1.Text);
                            cmd.Parameters.AddWithValue("@stok", textBox2.Text);
                            cmd.Parameters.AddWithValue("@fiyat", decimal.Parse(textBox3.Text));
                            cmd.Parameters.AddWithValue("ekleyenkullanici", CustomUser.UserName);
                            cmd.Parameters.AddWithValue("@zaman", now);
                            int rowsChanged = cmd.ExecuteNonQuery();
                            if (rowsChanged > 0)
                            {
                                MessageBox.Show("Ürün başarıyla güncellendi!    ");

                                LogUpdatedProduct(textBox4.Text, comboBox1.SelectedItem.ToString(),
                                    comboBox2.SelectedItem.ToString(), textBox1?.Text, textBox2.Text,
                                    decimal.Parse(textBox3.Text), now);
                            }
                            else
                            {
                                MessageBox.Show("Ürün Güncellenemedi");
                            }


                        }

                    }
                    else
                    {
                        MessageBox.Show("Hiçbir değişiklik yapılmadı");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("bilgiler güncellenirken bir hatayla karşılaşıldı", ex.Message);

            }
        }

        private bool IsProductModified()
        {
            return comboBox1?.SelectedItem?.ToString() != comboBox1.Tag?.ToString() ||
                   comboBox2?.SelectedItem?.ToString() != comboBox2.Tag?.ToString() ||
                   textBox1.Text != textBox1.Tag.ToString() ||
                   textBox2.Text != textBox2.Tag.ToString() ||
                   textBox3.Text != textBox3.Tag.ToString() ||
                   textBox4.Text != textBox4.Tag.ToString();    

        }


        private void SaveOriginalValue()
        {
            comboBox1.Tag = comboBox1.SelectedItem?.ToString();
            comboBox2.Tag = comboBox2.SelectedItem?.ToString();
            textBox1.Tag = textBox1.Text;
            textBox2.Tag = textBox2.Text;
            textBox3.Tag = textBox3.Text;
            textBox4.Tag = textBox4.Text;
        }

        private void ModifyProduct_FormClosing(object sender, FormClosingEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = -1;
            foreach (Control control in this.Controls)
            {
                control.Enabled = false;
                listBox1.Enabled = true;
            }
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;


        }
        private void LogOldProduct()
        {
            string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "log.txt");
            
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT * FROM Urunler WHERE Urun_Ad = @urunad";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@urunad",listBox1.SelectedItem.ToString());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                using (StreamWriter writer = File.AppendText(LogFilePath))
                                {
                                    writer.WriteLine("Ürünün eski bilgileri");
                                    writer.WriteLine(new string('-', 50));
                                    writer.WriteLine($"Ürün Adı :{reader.GetString(1)}");
                                    writer.WriteLine($"Eski Kategori :{reader.GetString(2)}");
                                    writer.WriteLine($"Eski Birim : {reader.GetString(3)}");
                                    if (!reader.IsDBNull(4))
                                    {
                                        writer.WriteLine($"Eski Miktar : {reader.GetInt32(4)} Gram");
                                    }
                                    writer.WriteLine($"Eski Stok: {reader.GetInt32(5)} Adet");
                                    writer.WriteLine($"Eski Fiyat: {reader.GetDecimal(6)} TL");
                                    writer.WriteLine($"Eklenme Tarihi: {reader.GetDateTime(7)}");
                                    writer.WriteLine($"Ekleyen Kişi: {reader.GetString(8)}");
                                    writer.WriteLine(new string('-', 50));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Hata Oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LogUpdatedProduct(string productname, string category, string unit, string quantity, string stock, decimal price, DateTime time)
        {
            try
            {
                string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "log.txt");
                using (StreamWriter writer = File.AppendText(LogFilePath))
                {
                    writer.WriteLine("Ürünün yeni bilgileri"); 
                    writer.WriteLine(new string('-', 50));
                    writer.WriteLine($"Ürün Adı : {productname}");
                    writer.WriteLine($"Yeni Kategori: {category}");
                    writer.WriteLine($"Yeni Birim : {unit}");
                    if (!string.IsNullOrEmpty(quantity))
                    {
                        writer.WriteLine($"Yeni Miktar :{quantity} Gram");
                    }
                    writer.WriteLine($"Yeni Stok : {stock} Adet");
                    writer.WriteLine($"Yeni Fiyat : {price} TL");
                    writer.WriteLine($"Değiştirme Tarihi :{time}");
                    writer.WriteLine($"Değiştiren Kişi :{CustomUser.UserName}");
                    writer.WriteLine(new string('-', 50));
                    writer.WriteLine(new string('-', 50));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Hata Oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
