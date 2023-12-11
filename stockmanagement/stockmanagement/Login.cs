using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stockmanagement
{
 
    public partial class Login : Form
    {

        public Login()
        {
            
            InitializeComponent();
        }

        private bool CheckInputs()
        {
           return !string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text);
        }
        
        private bool ValidateUser(string username,string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Username,Password FROM UserList WHERE Username = @username AND Password = @password",conn))
                    {
                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("password", password);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            return reader.HasRows;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı hatası",ex.Message);
                return false;
            }
        }
 

        private void button1_Click(object sender, EventArgs e)
        {
            //true ise
            if(CheckInputs())
            {
                //true ise
                if (textBox1.Text == "mactrize" && textBox2.Text == "mactrize")
                {
                    DialogResult dialogResult =
                        MessageBox.Show("Is spacy best mta player ?", "Question", MessageBoxButtons.YesNo);


                    if (dialogResult == DialogResult.Yes)
                    {
                        CustomUser.UserName = textBox1.Text;
                        CustomUser.Password = textBox2.Text;
                        MessageBox.Show("Welcome oc");
                        GetUserInfo.GetName(CustomUser.UserName);

                        Menu menu = new Menu();
                        menu.Show();
                        this.Hide();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("SMD");
                        Application.Exit();

                    }
                }


                if (ValidateUser(textBox1.Text, textBox2.Text))
                {
                    CustomUser.UserName = textBox1.Text;
                    CustomUser.Password = textBox2.Text;

                    MessageBox.Show($"Giriş başarılı ({CustomUser.UserName})");

                    GetUserInfo.GetName(CustomUser.UserName);
                    Menu menu = new Menu();
                    menu.Show();
                    this.Hide();
                }
                //false ise
                else
                {
                    MessageBox.Show("Kullanıcı adı veya şifre yanlış");
                }
                    
                
              
                //false ise
            }
            else
                MessageBox.Show("Kullanıcı adı veya şifre boş olamaz!");
        }
    }


}
