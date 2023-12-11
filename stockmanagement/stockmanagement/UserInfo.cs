using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Stockmanagement
{
    public partial class UserInfo : Form
    {
      
        public UserInfo()
        {
            InitializeComponent();
        }

        private void RefreshUserList()
        {
            string query = "SELECT Name FROM UserList";
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    listBox1.Items.Clear();
                    while (reader.Read())
                    {
                        if (reader[0].ToString() != "Enis")
                        {
                            listBox1.Items.Add(reader["Name"]);
                        }
                    }
                    reader.Close();
                }
            }
        }


        private void GetRoles()
        {
            using(SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT Role FROM Roles",conn))
                {
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader[0].ToString() != "Founder")
                            {
                                comboBox1.Items.Add(reader[0].ToString());
                            }
                        }
                    }
                }
            }
        }

        private bool DeleteUser()
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
            {
                conn.Open();
                string query = "DELETE FROM UserList WHERE Name = @name";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", listBox1.SelectedItem.ToString());
                    int affectedrows = cmd.ExecuteNonQuery();
                    if (affectedrows > 0)
                    {
                        MessageBox.Show("Kullanıcı silindi");
                        return true;
                    }
                }
            }
            return false;
        }

        private void UserInfo_Load(object sender, EventArgs e)
            {
            string Role = RoleInfo.GetRole(CustomUser.UserName);
            if(Role == "User" || Role == "Admin")
            { 
                button2.Visible = false;
                button1.Visible = false;
                button5.Visible = false;
            }
            GetRoles();
            RefreshUserList();
                textBox1.Enabled = false; textBox2.Enabled = false; textBox3.Enabled = false; textBox4.Enabled = false; textBox5.Enabled = false; comboBox1.Enabled = false; button3.Visible = false; button4.Visible = false;
            
            }
     
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM UserList WHERE Name = @name", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", listBox1.SelectedItem.ToString());
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            textBox1.Text = reader[1].ToString();
                            textBox2.Text = reader[2].ToString();
                            textBox3.Text = reader[3].ToString();
                            if (reader[4] != DBNull.Value)
                            {
                                textBox4.Text = Convert.ToDateTime(reader[4]).ToShortDateString();
                            }
                            else
                            {
                                textBox4.Text = null;
                            }
                            textBox5.Text = (DateTime.Now.Year - Convert.ToDateTime(reader[4]).Year).ToString();
                            comboBox1.Text = reader[6].ToString();
                        }
                    }
                }
            }
           
        }

        private void UserInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            comboBox1.Enabled = false;
            Menu menu = new Menu();
            menu.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(DeleteUser())
            {
                RefreshUserList();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            button3.Visible = true;
            button4.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == -1)
                {
                MessageBox.Show("Seçim yapılmadı");
                return;
                }
            if (listBox1.SelectedItem.ToString() != CustomUser.Name)
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE Userlist SET Role = @role WHERE Name = @name", conn))
                    {
                        cmd.Parameters.AddWithValue("@role", comboBox1.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@name", listBox1.SelectedItem.ToString());
                        int affectedrows = cmd.ExecuteNonQuery();
                        if (affectedrows > 0)
                        {
                            MessageBox.Show("Kullanıcı güncellendi");
                            comboBox1.Enabled = false;
                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı güncellenemedi");
                        }
                    }
                    button3.Visible = false;
                    button4.Visible = false;

                }
            }
            else
            {
                MessageBox.Show("Kendinizi güncelleyemezsiniz");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((listBox1?.SelectedItem?.ToString() == CustomUser.Name) && button3.Visible == true)
            {
                comboBox1.Enabled = false;
            }
            else if ((listBox1?.SelectedItem?.ToString() != CustomUser.Name) && button3.Visible == true)
            {
                comboBox1.Enabled = true;   
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button3.Visible = false;
            button4.Visible = false;
            comboBox1.Enabled=false;
        }
        private int buttonclick = 0;
        private void button5_Click(object sender, EventArgs e)
        {
            if (buttonclick == 0)
            {
                textBox1.Enabled = true; textBox2.Enabled = true; textBox3.Enabled = true; textBox4.Enabled = true; listBox1.SelectedIndex = -1; listBox1.Enabled = false; comboBox1.Enabled = true;
                textBox1.Text = String.Empty; textBox2.Text = String.Empty; textBox3.Text = String.Empty; textBox4.Text = String.Empty; textBox5.Text = String.Empty; comboBox1.Text = String.Empty;
                buttonclick++;
                button1.Enabled = false;
                button2.Enabled = false;
            }
            else if ( buttonclick == 1 )
            {
                using(SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO UserList (Name,Surname,Username,DateOfBirth,Password,Role) VALUES(@name,@surname,@username,@dateofbirth,@password,@role)", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", textBox1.Text);
                        cmd.Parameters.AddWithValue("@surname", textBox2.Text);
                        cmd.Parameters.AddWithValue("@username", textBox3.Text);
                        if ((DateTime.TryParse(textBox4.Text, out DateTime parsedTime)))
                        {
                            cmd.Parameters.AddWithValue("@dateofbirth", parsedTime);
                        }
                        else
                        {
                            MessageBox.Show("Yanlış tarih yazımı");
                            return;
                        }
                        cmd.Parameters.AddWithValue("@role", comboBox1.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@password", "123");
                        int affectedrows = cmd.ExecuteNonQuery();
                        if (affectedrows > 0)
                        {
                            MessageBox.Show("Kullanıcı eklendi");
                            textBox1.Text = String.Empty; textBox2.Text = String.Empty; textBox3.Text = String.Empty; textBox4.Text = String.Empty; textBox5.Text = String.Empty; comboBox1.Text = String.Empty;
                            listBox1.Enabled = true; textBox1.Enabled = false; textBox2.Enabled = false; textBox3.Enabled = false; textBox4.Enabled = false; textBox5.Enabled = false; comboBox1.Enabled = false; button3.Visible = false; button4.Visible = false; button1.Enabled = true; button2.Enabled = true;

                            RefreshUserList();
                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı eklenemedi");
                        }
                    }
                }
                buttonclick = 0;
            }
        }
    }
}
