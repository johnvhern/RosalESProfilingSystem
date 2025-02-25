using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LicenseContext = OfficeOpenXml.LicenseContext;
using System.IO;
using System.Data.SqlClient;

namespace RosalESProfilingSystem.Forms
{
    public partial class Literacy_ProfOfLearners: Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        string column;
        public Literacy_ProfOfLearners()
        {
            InitializeComponent();
            metroComboBox1.SelectedIndex = 0;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    DataTable dt = ReadExcelFile(openFileDialog.FileName);
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private DataTable ReadExcelFile(string filePath)
        {
            DataTable dt = new DataTable();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var  worksheet = package.Workbook.Worksheets[0];
                int colCount = worksheet.Dimension.End.Column;
                int rowCount = worksheet.Dimension.End.Row;

                for (int col = 1; col <= colCount; col++)
                {
                    dt.Columns.Add(worksheet.Cells[1, col].Text);
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    DataRow dr = dt.NewRow();
                    for(int col = 1; col <= colCount; col++)
                    {
                        dr[col - 1] = worksheet.Cells[row, col].Text;
                    }
                    dt.Rows.Add(dr);    
                }
            }
            return dt;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.DataSource == null)
                {
                    MessageBox.Show("No data to save. Please import an Excel file first.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataTable dt = (DataTable)dataGridView1.DataSource;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No data to save. The imported file appears to be empty.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

                    foreach (DataRow row in dt.Rows)
                    {
                        string query = @"INSERT INTO Learners (GradeLevel, LRN, LastName, FirstName, MiddleName, Gender, Birthdate, MotherMaidenName, Guardian, Relationship, Father, EmergencyContact, CurrentResidence, Religion, MotherTongue, Dialects, Ethnicities)
                                    VALUES (@GradeLevel, @LRN, @LastName, @FirstName, @MiddleName, @Gender, @Birthdate, @MotherMaidenName, @Guardian, @Relationship, @Father, @EmergencyContact, @CurrentResidence, @Religion, @MotherTongue, @Dialects, @Ethnicities)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@GradeLevel", row["Grade Level"]);
                            cmd.Parameters.AddWithValue("@LRN", row["LRN Number"]);
                            cmd.Parameters.AddWithValue("@LastName", row["Last Name"]);
                            cmd.Parameters.AddWithValue("@FirstName", row["First Name"]);
                            cmd.Parameters.AddWithValue("@MiddleName", row["Middle Name"]);
                            cmd.Parameters.AddWithValue("@Gender", row["Gender"]);
                            cmd.Parameters.AddWithValue("@Birthdate", row["Birthdate"]);
                            cmd.Parameters.AddWithValue("@MotherMaidenName", row["Mother's Maiden Name"]);
                            cmd.Parameters.AddWithValue("@Guardian", row["Guardian"]);
                            cmd.Parameters.AddWithValue("@Relationship", row["Relationship"]);
                            cmd.Parameters.AddWithValue("@Father", row["Father"]);
                            cmd.Parameters.AddWithValue("@EmergencyContact", row["Emergency Contact Number"]);
                            cmd.Parameters.AddWithValue("@CurrentResidence", row["Current Residence"]);
                            cmd.Parameters.AddWithValue("@Religion", row["Religion"]);
                            cmd.Parameters.AddWithValue("@MotherTongue", row["Mother Tongue"]);
                            cmd.Parameters.AddWithValue("@Dialects", row["Dialects"]);
                            cmd.Parameters.AddWithValue("@Ethnicities", row["Ethnicities"]);


                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Data saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}");
            }
            

                
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {

                string column = metroComboBox1.SelectedItem.ToString();
                string searchValue = textBox1.Text.Trim();

                if (string.IsNullOrEmpty(searchValue))
                {
                    MessageBox.Show("Please enter a search term");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();
                    string query = $"SELECT * FROM Learners WHERE {column} LIKE @SearchValue";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SearchValue", $"%{searchValue}%");

                        DataTable dt = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}");
            }
        }
    }
    }
