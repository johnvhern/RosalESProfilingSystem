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
        OpenFileDialog openFileDialog;
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
            using (openFileDialog = new OpenFileDialog())
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
                    var schoolYear = worksheet.Cells[1, 2].Text;
                    var assessmentType = worksheet.Cells[2, 2].Text;
                    var gradeLevel = Convert.ToInt32(worksheet.Cells[3, 2].Value);

                    dataTable.Clear();
                    dataTable.Columns.Clear();

                    dataTable.Columns.Add("Last Name", typeof(string));
                    dataTable.Columns.Add("First Name", typeof(string));
                    dataTable.Columns.Add("Middle Name", typeof(string));
                    dataTable.Columns.Add("LRN", typeof(string));
                    dataTable.Columns.Add("Sex", typeof(string));
                    dataTable.Columns.Add("Age", typeof(int));

                    if (gradeLevel <= 3)
                    {
                        dataTable.Columns.Add("RMA Classification", typeof(string));
                        dataTable.Columns.Add("CRLA Classification (Akeanon)", typeof(string));
                        dataTable.Columns.Add("CRLA Classification (Filipino)", typeof(string));
                        dataTable.Columns.Add("CRLA Classification (English)", typeof(string));
                    }
                    else
                    {
                        dataTable.Columns.Add("Classification Level", typeof(string));
                    }

                    int startRow = 6;
                    for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var lrn = worksheet.Cells[row, 4].Text;

                        if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text) &&
                            string.IsNullOrWhiteSpace(worksheet.Cells[row, 2].Text) &&
                            string.IsNullOrWhiteSpace(lrn))
                        {
                            continue;
                        }

                        if (!IsDataAlreadySaved(schoolYear, assessmentType, gradeLevel, lrn))
                        {
                            if (gradeLevel <= 3)
                            {
                                dataTable.Rows.Add(
                                    worksheet.Cells[row, 1].Text,
                                    worksheet.Cells[row, 2].Text,
                                    worksheet.Cells[row, 3].Text,
                                    lrn,
                                    worksheet.Cells[row, 5].Text,
                                    Convert.ToInt32(worksheet.Cells[row, 6].Value),
                                    worksheet.Cells[row, 7].Text,
                                    worksheet.Cells[row, 8].Text,
                                    worksheet.Cells[row, 9].Text,
                                    worksheet.Cells[row, 10].Text
                                );
                            }
                            else
                            {
                                dataTable.Rows.Add(
                                    worksheet.Cells[row, 1].Text,
                                    worksheet.Cells[row, 2].Text,
                                    worksheet.Cells[row, 3].Text,
                                    lrn,
                                    worksheet.Cells[row, 5].Text,
                                    Convert.ToInt32(worksheet.Cells[row, 6].Value),
                                    worksheet.Cells[row, 7].Text
                                );
                            }
                        }
                    }

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("Data already saved. Please upload another excel file", "Already Saved Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtFilePath.Clear();
                        return;
                    }

                    dataGridView1.DataSource = dataTable;
                    dataGridView1.Tag = new { SchoolYear = schoolYear, AssessmentType = assessmentType, GradeLevel = gradeLevel };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private bool IsDataAlreadySaved(string schoolYear, string assessmentType, int gradeLevel, string lrn)
        {
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string query = "SELECT COUNT(*) FROM " + (gradeLevel <= 3 ? "LearnersProfile" : "LearnersProfileScience") + " WHERE SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType AND GradeLevel = @GradeLevel AND LRN = @LRN";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                    cmd.Parameters.AddWithValue("@AssessmentType", assessmentType);
                    cmd.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                    cmd.Parameters.AddWithValue("@LRN", lrn);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFilePath.Text))
                {
                    MessageBox.Show("Please import an excel file first.");
                    return;
                }

                if (dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("No data to save.", "Save Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveData(); // Call the existing SaveData method
                txtFilePath.Clear();
                dataGridView1.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void SaveData()
        {
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("No data to save.", "Save Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var tableName = ((dynamic)dataGridView1.Tag).GradeLevel <= 3 ? "LearnersProfile" : "UpperGradesProfile";

            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                conn.Open();
                foreach (DataRow row in dataTable.Rows)
                {
                    var query = tableName == "LearnersProfile" ?
                        "INSERT INTO LearnersProfile (SchoolYear, AssessmentType, GradeLevel, LastName, FirstName, MiddleName, LRN, Sex, Age, RMAClassification, CRLAClassificationAkeanon, CRLAClassificationFilipino, CRLAClassificationEnglish) " +
                        "VALUES (@SchoolYear, @AssessmentType, @GradeLevel, @LastName, @FirstName, @MiddleName, @LRN, @Sex, @Age, @RMAClassification, @CRLAClassificationAkeanon, @CRLAClassificationFilipino, @CRLAClassificationEnglish)" :
                        "INSERT INTO LearnersProfileScience (SchoolYear, AssessmentType, GradeLevel, LastName, FirstName, MiddleName, LRN, Sex, Age, ClassificationLevel) " +
                        "VALUES (@SchoolYear, @AssessmentType, @GradeLevel, @LastName, @FirstName, @MiddleName, @LRN, @Sex, @Age, @ClassificationLevel)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SchoolYear", ((dynamic)dataGridView1.Tag).SchoolYear);
                        cmd.Parameters.AddWithValue("@AssessmentType", ((dynamic)dataGridView1.Tag).AssessmentType);
                        cmd.Parameters.AddWithValue("@GradeLevel", ((dynamic)dataGridView1.Tag).GradeLevel);
                        cmd.Parameters.AddWithValue("@LastName", row["Last Name"]);
                        cmd.Parameters.AddWithValue("@FirstName", row["First Name"]);
                        cmd.Parameters.AddWithValue("@MiddleName", row["Middle Name"]);
                        cmd.Parameters.AddWithValue("@LRN", row["LRN"]);
                        cmd.Parameters.AddWithValue("@Sex", row["Sex"]);
                        cmd.Parameters.AddWithValue("@Age", row["Age"]);

                        if (tableName == "LearnersProfile")
                        {
                            cmd.Parameters.AddWithValue("@RMAClassification", row["RMA Classification"]);
                            cmd.Parameters.AddWithValue("@CRLAClassificationAkeanon", row["CRLA Classification (Akeanon)"]);
                            cmd.Parameters.AddWithValue("@CRLAClassificationFilipino", row["CRLA Classification (Filipino)"]);
                            cmd.Parameters.AddWithValue("@CRLAClassificationEnglish", row["CRLA Classification (English)"]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ClassificationLevel", row["Classification Level"]);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            MessageBox.Show("Data saved successfully.", "Save Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
            dataTable.Clear();
        }



        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbSchoolYear.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a school year, and enter a search term.");
                    return;
                }
                string schoolYear = cbSchoolYear.SelectedItem.ToString();
                string column = metroComboBox1.SelectedItem.ToString();
                string searchValue = textBox1.Text.Trim();

                if (string.IsNullOrEmpty(searchValue) || string.IsNullOrEmpty(schoolYear))
                {
                    MessageBox.Show("Please select a school year, and enter a search term.");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();
                    string query = $"SELECT DISTINCT GradeLevel, LastName, FirstName, MiddleName, LRN, Sex, Age FROM LearnersProfile WHERE {column} LIKE @SearchValue AND SchoolYear = @SchoolYear " +
                        $"UNION SELECT DISTINCT GradeLevel, LastName, FirstName, MiddleName, LRN, Sex, Age FROM LearnersProfileScience WHERE {column} LIKE @SearchValue AND SchoolYear = @SchoolYear";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SearchValue", $"%{searchValue}%");
                        cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
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

        private void cbSchoolYear_DropDown(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    string query = @"SELECT DISTINCT SchoolYear FROM LearnersProfile UNION SELECT DISTINCT SchoolYear FROM LearnersProfileScience ORDER BY SchoolYear DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            cbSchoolYear.Items.Clear();

                            while (reader.Read())
                            {
                                cbSchoolYear.Items.Add(reader["SchoolYear"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
    }
