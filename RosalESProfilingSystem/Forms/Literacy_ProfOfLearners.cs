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
        private DataTable dataTable = new DataTable();
        public Literacy_ProfOfLearners()
        {
            InitializeComponent();
            metroComboBox1.SelectedIndex = 0;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";

                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = openFileDialog.FileName;
                    PreviewExcelData(openFileDialog.FileName);
                }
            }
        }

        private void PreviewExcelData(string fileName)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo(fileName)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var assessmentType = worksheet.Cells[1, 2].Text;
                    var gradeLevel = Convert.ToInt32(worksheet.Cells[2, 2].Value);

                    dataTable.Clear();
                    dataTable.Columns.Clear();

                    dataTable.Columns.Add("Last Name", typeof(string));
                    dataTable.Columns.Add("First Name", typeof(string));
                    dataTable.Columns.Add("Middle Name", typeof(string));
                    dataTable.Columns.Add("LRN", typeof(string));
                    dataTable.Columns.Add("Sex", typeof(string));
                    dataTable.Columns.Add("Age", typeof(int));
                    dataTable.Columns.Add("RMA Classification", typeof(string));

                    int startRow = 5; // Data starts from row 5
                    for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                    {
                        dataTable.Rows.Add(
                            worksheet.Cells[row, 1].Text,
                            worksheet.Cells[row, 2].Text,
                            worksheet.Cells[row, 3].Text,
                            worksheet.Cells[row, 4].Text,
                            worksheet.Cells[row, 5].Text,
                            Convert.ToInt32(worksheet.Cells[row, 6].Value),
                            worksheet.Cells[row, 7].Text
                        );
                    }

                    dataGridView1.DataSource = dataTable;
                    dataGridView1.Tag = new { AssessmentType = assessmentType, GradeLevel = gradeLevel };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var metadata = (dynamic)dataGridView1.Tag;
                string assessmentType = metadata.AssessmentType;
                int gradeLevel = metadata.GradeLevel;

                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

                            foreach (DataRow row in dataTable.Rows)
                            {
                                string query = @"INSERT INTO LearnersProfile 
                                         (AssessmentType, GradeLevel, LastName, FirstName, MiddleName, LRN, Sex, Age, RMAClassification) 
                                         VALUES (@AssessmentType, @GradeLevel, @LastName, @FirstName, @MiddleName, @LRN, @Sex, @Age, @Classification)";

                                using (SqlCommand cmd = new SqlCommand(query, conn))
                                {
                                    cmd.Parameters.AddWithValue("@AssessmentType", assessmentType);
                                    cmd.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                                    cmd.Parameters.AddWithValue("@LastName", row["Last Name"]);
                                    cmd.Parameters.AddWithValue("@FirstName", row["First Name"]);
                                    cmd.Parameters.AddWithValue("@MiddleName", row["Middle Name"]);
                                    cmd.Parameters.AddWithValue("@LRN", row["LRN"]);
                                    cmd.Parameters.AddWithValue("@Sex", row["Sex"]);
                                    cmd.Parameters.AddWithValue("@Age", row["Age"]);
                                    cmd.Parameters.AddWithValue("@Classification", row["RMA Classification"]);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                }
                MessageBox.Show("Data successfully saved to the database!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
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
                    string query = $"SELECT * FROM LearnersProfile WHERE {column} LIKE @SearchValue";

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
