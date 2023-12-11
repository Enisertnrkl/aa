using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Stockmanagement;
using static Stockmanagement.Category;
using Menu = Stockmanagement.Menu;

namespace stockmanagement
{
    public partial class SellProduct : Form
    {
        public SellProduct()
        {
            InitializeComponent();
        }
        decimal profit = 0;
        decimal totalprofit = 0;
        decimal percentprofit = 0;
        decimal totalcost = 0;

        private void GetProductName()
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Urun_Ad FROM Urunler", conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        comboBox1.Items.Add(dr[0].ToString());
                    }
                }
            }
        }

        private void InitializeView()
        {
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = ProductList.products;
            dataGridView1.DataSource = bindingSource;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            decimal sellPrice = decimal.Parse(textBox1.Text);
            decimal costPrice = decimal.Parse(label11.Text);
            decimal quantity = decimal.Parse(textBox2.Text);

            if (Int32.Parse(textBox2.Text) > Int32.Parse(label10.Text))
            {
                MessageBox.Show("Mevcut stoktan fazla giremezsiniz");
                return;
            }
            Product product = new Product()
            {

                ProductName = comboBox1.SelectedItem.ToString(),
                Category = comboBox2.SelectedItem.ToString(),
                Unit = comboBox3.SelectedItem.ToString(),
                Quantity = comboBox4?.SelectedItem?.ToString(),
                SellPrice = decimal.Parse(textBox1.Text),
                Stock = Int32.Parse(textBox2.Text),
                AddedByUser = GetUserInfo.GetName(CustomUser.UserName),
                OldStock = Int32.Parse(label10.Text),
                Profit = (sellPrice * quantity) - (costPrice * quantity),
                PercentProfit = (((sellPrice * quantity) - (costPrice * quantity)) / (costPrice * quantity)) * 100,
            };
            ProductList.products.Add(product);

            InitializeView();



            profit = (product.SellPrice * quantity) - (costPrice * quantity);
            totalcost += costPrice * quantity;
          

            totalprofit += profit;
            percentprofit = (totalprofit / totalcost) * 100;
            label13.Text = totalprofit.ToString();


            if (percentprofit > 0)
            {
                label15.ForeColor = Color.Green;

            }
            else
            {
                label15.ForeColor = Color.Red;
            }
            label15.Text = $"{percentprofit:F2}".ToString();

            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox4.SelectedIndex = -1;
            textBox1.Text = String.Empty;
            textBox2.Text = String.Empty;
            label10.Text = String.Empty;
        }



        private void SellProduct_Load(object sender, EventArgs e)
        {
            label10.Text = null;
            label11.Text = null;
            label13.Text = null;
            GetProductName();
            comboBox4.Enabled = false;

            InitializeView();

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3?.SelectedItem?.ToString() == "Gram")
            {
                comboBox4.Enabled = true;
                comboBox4.Items.Clear();
                comboBox4.Items.Add(Quantity.GetQuantity(comboBox1.SelectedItem.ToString()));
            }
            else
            {
                comboBox4.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                label11.Text = Price.GetPrice(comboBox1.SelectedItem.ToString()).ToString();
                label10.Text = Stock.GetStock(comboBox1.SelectedItem.ToString());
                comboBox2.Text = null;
                comboBox3.Text = null;
                comboBox4.Text = null;
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                comboBox2.Items.Add(GetCategory(comboBox1.SelectedItem.ToString()));

                comboBox3.Items.Add(Unit.GetUnit(comboBox1.SelectedItem.ToString()));
            }
        }

        private void SellProduct_FormClosing(object sender, FormClosingEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "log.txt");
            int affectedrows = 0;
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE Urunler SET Stok = @stok WHERE Urun_Ad = @urunad", conn))
                {
                    foreach (Product product in ProductList.products)
                    {
                        cmd.Parameters.AddWithValue("@stok", product.OldStock - product.Stock);
                        cmd.Parameters.AddWithValue("@urunad", product.ProductName);
                        using (StreamWriter writer = File.AppendText(LogFilePath))
                        {
                            writer.WriteLine("Satılan Ürün Bilgileri");
                            writer.WriteLine(new string('-', 50));
                            writer.WriteLine($"Ürün Adı : {product.ProductName}");
                            writer.WriteLine($"Ürün Kategorisi : {product.Category}");
                            writer.WriteLine($"Ürün Birimi : {product.Unit}");
                            writer.WriteLine($"Ürün Miktarı : {product?.Quantity ?? "-"}");
                            writer.WriteLine($"Ürün Satış Fiyatı : {product.SellPrice}");
                            writer.WriteLine($"Satılan Stok : {product.Stock}");
                            writer.WriteLine($"Ürünü Satan Kişi: {product.AddedByUser}");
                            if (product.Profit > 0)
                            {
                                writer.WriteLine($"Tahmini Kâr: + {product.Profit:F2} TL");
                            }
                            else
                            {
                                writer.WriteLine($"Tahmini Kâr: {product.Profit:F2} TL");
                            }

                            if (product.PercentProfit > 0)
                            {
                                writer.WriteLine($"Tahmini Yüzde Kâr: + {product.PercentProfit:F2} %");
                            }
                            else
                            {
                                writer.WriteLine($"Tahmini Yüzde Kâr: - {product.PercentProfit:F2} %");

                            }

                            writer.WriteLine($"Ürünü Satan Kişi: {product.AddedByUser}");
                            writer.WriteLine($"Ürünün Satış Tarihi: {DateTime.Now}");
                            writer.WriteLine(new string('-', 50));
                            writer.WriteLine(new string('-', 50));
                        }
                        affectedrows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    if (affectedrows > 0)
                    {
                        MessageBox.Show("Ürünler başarıyla satıldı");
                        ProductList.products.Clear();
                        InitializeView();
                    }
                    else
                    {
                        MessageBox.Show("Ürünler satılamadı");
                    }


                }
            }
        }
    }
}
