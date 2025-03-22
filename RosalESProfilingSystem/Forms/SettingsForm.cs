using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RosalESProfilingSystem.Forms
{
    public partial class SettingsForm: Form
    {
        private readonly string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        public SettingsForm()
        {
            InitializeComponent();
            tabPage1.ImageIndex = 5;
            tabPage2.ImageIndex = 1;
            tabPage5.ImageIndex = 2;
            tabPage6.ImageIndex = 4;



        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void btnChangeUsername_Click(object sender, EventArgs e)
        {
            string oldUsername = txtCurrUsername.Text.Trim();
            string newUsername = txtNewUsername.Text.Trim();
            string password = txtUsernamePass.Text;

            if (string.IsNullOrWhiteSpace(oldUsername) || string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("All fields are required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (oldUsername == newUsername)
            {
                MessageBox.Show("New username cannot be the same as the old username!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                conn.Open();

                // Check if the old username and password match
                string query = "SELECT PasswordHash FROM Users WHERE Username = @OldUsername";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OldUsername", oldUsername);
                    object result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        MessageBox.Show("Old username not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string storedHash = result.ToString();
                    if (!BCrypt.Net.BCrypt.Verify(password, storedHash))
                    {
                        MessageBox.Show("Incorrect password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Check if the new username is already taken
                string checkUsernameQuery = "SELECT COUNT(*) FROM Users WHERE Username = @NewUsername";
                using (SqlCommand checkCmd = new SqlCommand(checkUsernameQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@NewUsername", newUsername);
                    int usernameExists = (int)checkCmd.ExecuteScalar();

                    if (usernameExists > 0)
                    {
                        MessageBox.Show("New username is already taken!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Update the username
                string updateQuery = "UPDATE Users SET Username = @NewUsername WHERE Username = @OldUsername";
                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@NewUsername", newUsername);
                    updateCmd.Parameters.AddWithValue("@OldUsername", oldUsername);
                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Password changed successfully! You'll be logged out from this session. Please login again", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LogoutUser();
                    }
                    else
                    {
                        MessageBox.Show("Error updating username!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void btnChangePass_Click(object sender, EventArgs e)
        {
            string username = txtCurrrUsernamePass.Text.Trim(); // Get username from TextBox
            string oldPassword = txtCurrPass.Text;
            string newPassword = txtNewPass.Text;
            string confirmPassword = txtNewPass.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(oldPassword) ||
                string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("All fields are required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("New passwords do not match!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                conn.Open();

                // Fetch the current password hash from the database using the username
                string query = "SELECT PasswordHash FROM Users WHERE Username = @Username";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    object result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        MessageBox.Show("User not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string storedHash = result.ToString();
                    if (!BCrypt.Net.BCrypt.Verify(oldPassword, storedHash))
                    {
                        MessageBox.Show("Incorrect old password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Hash the new password
                string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword, BCrypt.Net.BCrypt.GenerateSalt(12));

                // Update password in database
                string updateQuery = "UPDATE Users SET PasswordHash = @NewPassword WHERE Username = @Username";
                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@NewPassword", newHashedPassword);
                    updateCmd.Parameters.AddWithValue("@Username", username);
                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Password changed successfully! You'll be logged out from this session. Please login again", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LogoutUser();
                    }
                    else
                    {
                        MessageBox.Show("Error updating password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LogoutUser()
        {
           Application.Restart();
        }

        private void currentPassCheck_CheckedChanged(object sender, EventArgs e)
        {
            txtCurrPass.UseSystemPasswordChar = !currentPassCheck.Checked;
        }

        private void newPassCheck_CheckedChanged(object sender, EventArgs e)
        {
            txtNewPass.UseSystemPasswordChar = !newPassCheck.Checked;
        }

        private void confirmPassCheck_CheckedChanged(object sender, EventArgs e)
        {
            txtConfirmPass.UseSystemPasswordChar = !confirmPassCheck.Checked;
        }

        private void usernamePassCheck_CheckedChanged(object sender, EventArgs e)
        {
            txtUsernamePass.UseSystemPasswordChar = !usernamePassCheck.Checked;
        }
    }
}
