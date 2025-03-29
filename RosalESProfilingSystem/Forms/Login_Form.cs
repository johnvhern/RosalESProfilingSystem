using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace RosalESProfilingSystem.Forms
{
    public partial class Login_Form : Form
    {
        private readonly string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        private int loginAttempts = 0;
        private const int maxLoginAttempts = 5;
        private System.Timers.Timer timer;

        public Login_Form()
        {
            InitializeComponent();
            this.AcceptButton = btnLogin;
            txtPassword.WaterMark = "Password";
            txtUsername.WaterMark = "Username";
            this.ActiveControl = label1;

        }

        private bool verifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }


        private void pictureBox4_Click(object sender, EventArgs e)
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

        private void btnLogin_Click(object sender, EventArgs e)
        {

            if (loginAttempts >= maxLoginAttempts)
            {
                MessageBox.Show("Maximum login attempts reached. Please try again later.");
                return; 
            }

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Login successful!");
                LoggedUser.Username = username;
                loginAttempts = 0;
                this.Hide();
                Forms.MainForm mainForm = new Forms.MainForm();
                mainForm.Show();
            }
            else
            {
                loginAttempts++;
                MessageBox.Show($"Invalid username or password. Attempt {loginAttempts} of {maxLoginAttempts}");

                if (loginAttempts >= maxLoginAttempts)
                {
                    btnLogin.Enabled = false;
                    lblForgotPassword.Visible = true;
                    timer = new System.Timers.Timer(60000);
                    timer.Elapsed += UnlockLoginButton;
                    timer.AutoReset = false;
                    timer.Start();
                }
            }

            //private void AddNewUser(object sender, EventArgs e)
            //{
            //    string username = txtUsername.Text.Trim();
            //    string password = txtPassword.Text;

            //    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            //    {
            //        MessageBox.Show("Username and password cannot be empty.");
            //        return;
            //    }

            //    if (InsertUser(username, password))
            //    {
            //        MessageBox.Show("User added successfully!");
            //    }
            //    else
            //    {
            //        MessageBox.Show("Failed to add user.");
            //    }
            //}

            //private bool InsertUser(string username, string password)
            //{
            //    try
            //    {
            //        using (SqlConnection conn = new SqlConnection(dbConnection))
            //        {
            //            conn.Open();

            //            string hashedPassword = HashPassword(password);
            //            string query = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)";

            //            using (SqlCommand cmd = new SqlCommand(query, conn))
            //            {
            //                cmd.Parameters.AddWithValue("@Username", username);
            //                cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
            //                int result = cmd.ExecuteNonQuery();
            //                return result > 0;
            //            }

            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return false;
            //    }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private void UnlockLoginButton(object sender, ElapsedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => btnLogin.Enabled = true));
            }
            else
            {
                btnLogin.Enabled = true;
            }
            loginAttempts = 0;
            timer.Dispose();
        }

        private bool AuthenticateUser(string username, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();
                    string query = "SELECT PasswordHash FROM Users WHERE Username = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        string hash = cmd.ExecuteScalar()?.ToString();
                        if (hash == null)
                        {
                            return false;
                        }
                        return verifyPassword(password, hash);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
            return false;
        }

        private void lblForgotPassword_Click(object sender, EventArgs e)
        {
            this.Hide();
            Forms.ChangePassword_Form changePasswordForm = new Forms.ChangePassword_Form();
            changePasswordForm.ShowDialog();
            this.Close();

        }

        private void cbShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowPass.Checked)
            {
                txtPassword.UseSystemPasswordChar = false; // Show password
                txtPassword.PasswordChar = '\0'; // Ensure password is visible
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true; // Hide password
                txtPassword.PasswordChar = '●'; // Mask password
            }
        }

        private void Login_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
