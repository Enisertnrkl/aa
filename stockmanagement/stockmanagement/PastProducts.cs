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

namespace Stockmanagement
{
    public partial class PastProducts : Form
    {
        public PastProducts()
        {
            InitializeComponent();
        }

        private void PastProducts_Load(object sender, EventArgs e)
        {
            using(SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString())) 
            {
                string query = "SELECT Urun_Ad,Kategori,Birim,Miktar,Stok,Fiyat,SilinmeTarihi,SilenKullanici FROM SilinenUrunler";
                using(SqlCommand cmd = new SqlCommand(query, conn))
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void PastProducts_FormClosing(object sender, FormClosingEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
        }
    }
}
