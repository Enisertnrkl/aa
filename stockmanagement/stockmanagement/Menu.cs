using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using stockmanagement;

namespace Stockmanagement
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        //datagridview'i doldurur
        private void FillData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                    using(SqlDataAdapter da = new SqlDataAdapter("SELECT Urun_Ad,Kategori,Birim,Miktar,Stok,Fiyat,EklenmeTarihi,EkleyenKullanici FROM Urunler",conn))
                {
                    da.Fill(dt);
                }
                dataGridView1.DataSource = dt;
            }

        }
        private void Fillcombobox()
        {
            using(SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT KategoriAd FROM Kategori",conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader[0].ToString());
                        }
                    }
                }
            }
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            Fillcombobox();
            if(RoleInfo.GetRole(CustomUser.UserName) is "User")
            {
                ürünDüzenleToolStripMenuItem.Enabled = false; ürünSilToolStripMenuItem.Enabled = false; 
            }
            if (RoleInfo.GetRole(CustomUser.UserName) is "User" or "Admin")
            {
                ürünSatışToolStripMenuItem.Enabled = false;
            }
            FillData();
        }

        private void Menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Çıkış yapmak istediğine emin misin?","Onay",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                CustomUser.UserName = string.Empty; 
                CustomUser.Password = string.Empty;
                Login login = new Login();
                login.Show();
                this.Hide();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    string query = "SELECT Urun_Ad,Kategori,Birim,Miktar,Stok,Fiyat,EklenmeTarihi,EkleyenKullanici FROM Urunler WHERE Kategori = @kategori AND Urun_Ad LIKE '%' +  @urunad + '%'";
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@urunad", textBox1.Text);
                        cmd.Parameters.AddWithValue("@kategori", comboBox1.SelectedItem.ToString());
                        SqlDataReader reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            textBox1.Text = null;
            FillData();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                string query = "SELECT Urun_Ad,Kategori,Birim,Miktar,Stok,Fiyat,EklenmeTarihi,EkleyenKullanici FROM Urunler WHERE Urun_Ad LIKE '%' + @urunad + '%' ";
                if (comboBox1.SelectedIndex != -1)
                {
                    query += "AND Kategori = @kategori";
                }
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (comboBox1.SelectedIndex != -1)
                    {
                        cmd.Parameters.AddWithValue("@kategori", comboBox1?.SelectedItem?.ToString());
                    }

                    cmd.Parameters.AddWithValue("@urunad", textBox1.Text);
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void ürünEkleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddProduct addProduct = new AddProduct();
            addProduct.Show();
            this.Hide();
        }

        private void ürünDüzenleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModifyProduct modifyProduct = new ModifyProduct();
            modifyProduct.Show();
            this.Hide();
        }

        private void ürünSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveProduct removeProduct = new RemoveProduct();
            removeProduct.Show();
            this.Hide();
        }

        private void silmeGeçmişiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PastProducts pastProducts = new PastProducts();
            pastProducts.Show();
            this.Hide();
        }

        private void kullanıcıBilgileriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.Show();
            this.Hide();
        }

        private void ürünSatışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SellProduct sellProduct = new SellProduct();
            sellProduct.Show();
            this.Hide();
        }

        private void satışGeçmişiToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
