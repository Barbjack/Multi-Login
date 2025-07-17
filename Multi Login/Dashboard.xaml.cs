using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Multi_Login
{
    public partial class Dashboard : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        int selectedUserId = 0;

        public Dashboard()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Users", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    users.Add(new User
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Username = dr["Username"].ToString(),
                        Password = dr["Password"].ToString(),
                        Role = dr["Role"].ToString(),
                        Status = Convert.ToBoolean(dr["Status"])
                    });
                }
            }
            dgUsers.ItemsSource = users;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Users (Username, Password, Role,Status) VALUES (@Username, @Password,@Role, @Status)", con);
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                cmd.Parameters.AddWithValue("@Role", txtRole.Text);
                cmd.Parameters.AddWithValue("@Status", chkStatus.IsChecked == true);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            ClearForm();
            LoadUsers();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUserId == 0)
            {
                MessageBox.Show("Select a user to update.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE Users SET Username=@Username, Password=@Password, Role=@Role, Status=@Status WHERE Id=@Id", con);
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                cmd.Parameters.AddWithValue("@Status", chkStatus.IsChecked == true);
                cmd.Parameters.AddWithValue("@Role", txtRole.Text);
                cmd.Parameters.AddWithValue("@Id", selectedUserId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            ClearForm();
            LoadUsers();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUserId == 0)
            {
                MessageBox.Show("Select a user to delete.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE Id=@Id", con);
                cmd.Parameters.AddWithValue("@Id", selectedUserId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            ClearForm();
            LoadUsers();
        }

        private void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUsers.SelectedItem is User user)
            {
                selectedUserId = user.Id;
                txtUsername.Text = user.Username;
                txtPassword.Text = user.Password;
                txtRole.Text = user.Role;
                chkStatus.IsChecked = user.Status;
            }
        }

        private void ClearForm()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtRole.Clear();
            chkStatus.IsChecked = false;
            selectedUserId = 0;
        }
    }
}
