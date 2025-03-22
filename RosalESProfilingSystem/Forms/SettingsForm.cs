using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
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
        private string backupPath = @"C:\RosalESBackup"; // Default backup path
        public SettingsForm()
        {
            InitializeComponent();
            tabPage1.ImageIndex = 5;
            tabPage2.ImageIndex = 1;
            tabPage5.ImageIndex = 2;
            tabPage6.ImageIndex = 4;
            txtBackupPath.Text = backupPath; // Set default backup path


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

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    backupPath = folderDialog.SelectedPath;
                    txtBackupPath.Text = backupPath;
                }
            }
        }

        private void btnRunBackupNow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(backupPath))
            {
                MessageBox.Show("Please select a backup path first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ensure the main backup path exists
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }

            // Generate a timestamped subfolder inside the backup path
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupFolder = Path.Combine(backupPath, $"Backup_{timestamp}");

            // Ensure the timestamped subfolder exists
            Directory.CreateDirectory(backupFolder);

            // Define batch file path inside the newly created folder
            string batchFilePath = Path.Combine(backupFolder, "backup.bat");

            // Generate the correct backup file path inside the same subfolder
            string backupType = rbFullBackup.Checked ? "full" : "diff";
            string backupFileName = backupType == "full" ? "RosalES_Full.bak" : "RosalES_Diff.bak";
            string backupFilePath = Path.Combine(backupFolder, backupFileName);

            // Escape backslashes for SQLCMD command
            string sqlCommand = $"sqlcmd -S localhost\\sqlexpress -E -Q \"BACKUP DATABASE RosalES TO DISK = '{backupFilePath}' WITH {(backupType == "full" ? "FORMAT, INIT" : "DIFFERENTIAL, INIT")}, NAME = '{backupType.ToUpper()} Backup';\"";

            // Write the batch script inside the backup folder
            File.WriteAllText(batchFilePath, sqlCommand);

            // Run the batch file
            try
            {
                System.Diagnostics.Process.Start(batchFilePath);
                AppendToLog($"[{DateTime.Now}] Manual {backupType} backup started in {backupFolder}.");
                MessageBox.Show("Backup process started manually!", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting backup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEnableScheduling_Click(object sender, EventArgs e)
        {
            string backupType = rbFullBackup.Checked ? "full" : "diff";
            string selectedTime = dateTimePickerSchedule.Value.ToString("HH:mm");

            CreateBackupBatchFile(backupType);
            ScheduleBackupTask(selectedTime, backupType);
        }

        private void btnDisableScheduling_Click(object sender, EventArgs e)
        {
            string backupType = rbFullBackup.Checked ? "full" : "diff";
            RemoveBackupTask(backupType);
        }

        private void CreateBackupBatchFile(string backupType)
        {
            if (string.IsNullOrEmpty(backupPath))
            {
                MessageBox.Show("Please select a backup path first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Generate a timestamped folder inside the backup path
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupFolder = Path.Combine(backupPath, $"Backup_{timestamp}");

            // Ensure the backup directory exists
            Directory.CreateDirectory(backupFolder);

            // Ensure backup path has a trailing slash
            if (!backupFolder.EndsWith("\\"))
            {
                backupFolder += "\\";
            }

            string filePath = Path.Combine(backupFolder, "backup.bat");

            // Dynamically use the selected backup path instead of hardcoding C:\RosalESBackup
            string backupFileName = backupType == "full" ? "RosalES_Full.bak" : "RosalES_Diff.bak";
            string backupFilePath = Path.Combine(backupFolder, backupFileName).Replace("\\", "\\\\");

            // Construct SQL command
            string sqlCommand = $"sqlcmd -S localhost\\sqlexpress -E -Q \"BACKUP DATABASE RosalES TO DISK = '{backupFilePath}' WITH {(backupType == "full" ? "FORMAT, INIT" : "DIFFERENTIAL, INIT")}, NAME = '{backupType.ToUpper()} Backup';\"";

            // Write the command to the batch file
            File.WriteAllText(filePath, sqlCommand);

            // Log the backup creation
            AppendToLog($"[{DateTime.Now}] Created {backupType} backup script in {backupFolder}.");
        }

        private void ScheduleBackupTask(string scheduleTime, string backupType)
        {
            try
            {
                using (TaskService ts = new TaskService())
                {
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = $"Automated {backupType} Backup for RosalES";

                    // Set Daily Trigger
                    DailyTrigger dailyTrigger = new DailyTrigger { DaysInterval = 1 };
                    dailyTrigger.StartBoundary = DateTime.Today.Add(TimeSpan.Parse(scheduleTime));
                    td.Triggers.Add(dailyTrigger);

                    // Set Action (Run batch file)
                    string backupFilePath = Path.Combine(backupPath, "backup.bat");
                    td.Actions.Add(new ExecAction(backupFilePath, null, null));

                    // Register Task
                    ts.RootFolder.RegisterTaskDefinition($"RosalES_{backupType}_Backup", td);
                    AppendToLog($"[{DateTime.Now}] Scheduled {backupType} backup at {scheduleTime}.");
                    MessageBox.Show($"{backupType} Backup Scheduled Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to schedule backup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveBackupTask(string backupType)
        {
            try
            {
                using (TaskService ts = new TaskService())
                {
                    ts.RootFolder.DeleteTask($"RosalES_{backupType}_Backup", false);
                    AppendToLog($"[{DateTime.Now}] Disabled scheduled {backupType} backup.");
                    MessageBox.Show($"{backupType} Backup Task Removed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to remove scheduled task: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppendToLog(string logText)
        {
            string logFilePath = Path.Combine(backupPath, "backup_logs.txt");
            File.AppendAllText(logFilePath, logText + Environment.NewLine);
            LoadBackupLogs(); // Refresh log list
        }

        private void LoadBackupLogs()
        {
            listBoxLogs.Items.Clear();
            string logFilePath = Path.Combine(backupPath, "backup_logs.txt");

            if (File.Exists(logFilePath))
            {
                string[] logs = File.ReadAllLines(logFilePath);
                listBoxLogs.Items.AddRange(logs);
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            LoadBackupLogs();
        }
    }
}
