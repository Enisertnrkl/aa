using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stockmanagement
{
    public partial class RemoveProduct : Form
    {
        public RemoveProduct()
        {
            InitializeComponent();
        }


        public static void RefreshlistBox(ListBox listBox)
        {
            try
            {
                string query = "SELECT * FROM Urunler";
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        listBox.Items.Clear();
                        while (reader.Read())
                        {
                            listBox.Items.Add(reader["Urun_Ad"]);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RemoveProduct_Load(object sender, EventArgs e)
        {
            RefreshlistBox(listBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string item;
            int affectedrows = 0;
            string query = "DELETE FROM Urunler WHERE Urun_Ad = @urunad";
            string query2 = "INSERT INTO SilinenUrunler (Urun_Ad, Kategori, Birim, Miktar, Stok, Fiyat,SilinmeTarihi,SilenKullanici) " +
                             "SELECT Urun_Ad, Kategori, Birim, Miktar, Stok, Fiyat,@silinmetarihi,@silenkullanici FROM Urunler WHERE Urun_Ad = @urunad";
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {

                conn.Open();
                using (SqlCommand cmd2 = new SqlCommand(query2, conn))
                {
                    item = listBox1.SelectedItem.ToString();
                    cmd2.Parameters.AddWithValue("@urunad",item);
                    cmd2.Parameters.AddWithValue("@silinmetarihi",DateTime.Now.ToString());
                    cmd2.Parameters.AddWithValue("@silenkullanici", CustomUser.UserName);
                    cmd2.ExecuteNonQuery();
                }
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    cmd.Parameters.AddWithValue("urunad",item);
                    affectedrows = cmd.ExecuteNonQuery();
                }
                if(affectedrows >0)
                {
                    string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "log.txt");
                    MessageBox.Show("ürün başarıyla silindi");
                    using(StreamWriter write = File.AppendText(LogFilePath))
                    {
                        write.WriteLine("Ürün Silme");
                        write.WriteLine(new string('-',50));
                        write.WriteLine($"Silinen Ürün Adı :{item}");
                        write.WriteLine($"Ürünün Silinme Tarihi : {DateTime.Now}");
                        write.WriteLine($"Ürünü Silen Kullanıcı : {CustomUser.UserName}");
                        write.WriteLine(new string('-', 50));
                        write.WriteLine(new string('-', 50));

                    }
                    RefreshlistBox(listBox1);
                }
            }
  
        }
        private void RemoveProduct_FormClosed(object sender, FormClosedEventArgs e)
        {
       
            Menu menu = new Menu();
            menu.Show();
        }
    }
}
