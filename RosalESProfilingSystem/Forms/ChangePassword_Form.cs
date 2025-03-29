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

namespace RosalESProfilingSystem.Forms
{
    public partial class ChangePassword_Form : Form
    {
        private readonly string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        public ChangePassword_Form()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {
                return;
            }
        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {

            txtNewPass.UseSystemPasswordChar = false;
        }

        private void pictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            txtNewPass.UseSystemPasswordChar = true;

        }

        private void pictureBox3_MouseDown_1(object sender, MouseEventArgs e)
        {
            txtConfirmPass.UseSystemPasswordChar = false;
        }

        private void pictureBox3_MouseUp_1(object sender, MouseEventArgs e)
        {
            txtConfirmPass.UseSystemPasswordChar = true;
        }

        private void lblBackToLogin_Click(object sender, EventArgs e)
        {
            this.Hide();
            Forms.Login_Form login = new Forms.Login_Form();
            login.ShowDialog();
        }

        private void btnChangePass_Click(object sender, EventArgs e)
        {
            string userName = txtUsername.Text.Trim();
            string newPass = txtNewPass.Text;
            string confirmPass = txtConfirmPass.Text;

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(newPass) || string.IsNullOrWhiteSpace(confirmPass))
            {
                MessageBox.Show("Please fill out all fields.");
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Passwords do not match. Please try again.");
                return;
            }

            if (ChangePassword(userName, newPass))
            {
                MessageBox.Show("Password successfully changed.");
                this.Hide();
                Login_Form login = new Login_Form();
                login.Show();
            }
            else
            {
                MessageBox.Show("Failed to change password. Please check your credentials");
            }
        }

        private bool ChangePassword(string userName, string newPass)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

                    string query = "SELECT Username FROM Users WHERE Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", userName);
                        string username = cmd.ExecuteScalar()?.ToString();

                        if (username != userName)
                        {
                            return false;
                        }
                    }

                    string newHashedPass = BCrypt.Net.BCrypt.HashPassword(newPass);
                    string updateQuery = "UPDATE Users SET PasswordHash = @PasswordHash WHERE Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PasswordHash", newHashedPass);
                        cmd.Parameters.AddWithValue("@Username", userName);
                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
