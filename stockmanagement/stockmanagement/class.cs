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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace Stockmanagement
{
    public partial  class @class: Form
    {
        public @class()
        {
            InitializeComponent();
        }       

    }
    public class DatabaseConnection
    {
        public static string ConnectionString()
        {
            return
                "Data Source=mssql-157544-0.cloudclusters.net,10029;\r\nUser ID=Enis;\r\nPassword=Enis2002.;\r\nInitial Catalog=aaa;\r\nIntegrated Security=False;\r\nConnect Timeout=30;\r\nEncrypt=False;";
        }
    }
    public class GetUserInfo
    {
        public static string GetName(string username)
        {
            using(SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT * FROM UserList WHERE Username = @username",conn)) 
                { 
                    cmd.Parameters.AddWithValue("username", username);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                      return  CustomUser.Name = reader[1].ToString();
                    }
                }
            }
            return null;
        }
    }

    public class RoleInfo
    {
        public static string GetRole(string username)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Role FROM UserList WHERE Username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }

                }
            }
            return null;
        }
    }

    public  class Stock
    {
        public static string GetStock(string urunad)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Stok FROM Urunler WHERE Urun_Ad = @urunad", conn))
                {
                    cmd.Parameters.AddWithValue("@urunad", urunad);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        return dr[0].ToString();
                    }
                }
            }

            return null;
        }
    }

    public class Price
    {
        public static decimal GetPrice(string urunad)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT  Fiyat FROM Urunler WHERE Urun_Ad =@urunad", conn))
                {
                    cmd.Parameters.AddWithValue("@urunad", urunad);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            return decimal.Parse(dr[0].ToString());
                        }
                    }
                }
            }

            return 0;
        }
    }

    public  class Unit
    {
        public static string GetUnit(string urunad)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Birim FROM Urunler WHERE Urun_Ad = @urunad", conn))
                {
                    cmd.Parameters.AddWithValue("@urunad", urunad);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            return dr[0].ToString();
                        }
                    }
                }
            }

            return null;
        }
    }

    public class Quantity
    {
        public static string GetQuantity(string urunad)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Miktar FROM Urunler WHERE Urun_Ad = @urunad", conn))
                {
                    cmd.Parameters.AddWithValue("@urunad", urunad);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            {
                                return dr[0].ToString();
                            }
                        }
                    }
                }
            }

            return null;
        }
    }

    public class Category
    {
        public static string GetCategory(string urunad)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Kategori FROM Urunler WHERE Urun_Ad = @urunad", conn))
                {
                    cmd.Parameters.AddWithValue("@urunad", urunad);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            return dr[0].ToString();
                        }
                    }
                }
            }

            return null;
        }
    }


    public class CustomUser
    {
        public static string UserName { get; set; }
        public static string Password { get; set; }
        public static string Name { get; set;}
    }


    public class ProductList
    {
         public static List<Product> products = new List<Product>();
    }
    public class Product
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public string Quantity { get; set; }
        public int Stock { get; set; }
        [Browsable(false)]
        public int OldStock { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Profit { get; set; }
        public decimal PercentProfit { get; set; }
        public string AddedByUser { get; set; }
    }
}
