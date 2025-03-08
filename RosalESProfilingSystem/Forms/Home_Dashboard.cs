using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RosalESProfilingSystem.Forms
{
    public partial class Home_Dashboard: Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        public Home_Dashboard()
        {
            InitializeComponent();
        }

        private void cbSchoolYear_DropDown(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    string query = "SELECT DISTINCT SchoolYear FROM LearnersProfile ORDER BY SchoolYear ASC";

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

        private void btnLoadAllEnrollments_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbSchoolYear.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a school year.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    string query = @"
        SELECT 
            COUNT(CASE WHEN GradeLevel = '1' THEN 1 END) AS Grade1Total,
            COUNT(CASE WHEN GradeLevel = '2' THEN 1 END) AS Grade2Total,
            COUNT(CASE WHEN GradeLevel = '3' THEN 1 END) AS Grade3Total,
            COUNT(CASE WHEN GradeLevel = '4' THEN 1 END) AS Grade4Total,
            COUNT(CASE WHEN GradeLevel = '5' THEN 1 END) AS Grade5Total,
            COUNT(CASE WHEN GradeLevel = '6' THEN 1 END) AS Grade6Total,
            COUNT(*) AS OverallTotal
        FROM (
            SELECT GradeLevel FROM LearnersProfile WHERE SchoolYear = @SchoolYear
            UNION ALL
            SELECT GradeLevel FROM LearnersProfileScience WHERE SchoolYear = @SchoolYear
        ) AS CombinedGrades;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SchoolYear", cbSchoolYear.SelectedItem.ToString());

                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            txtGrade1.Text = reader["Grade1Total"].ToString();
                            txtGrade2.Text = reader["Grade2Total"].ToString();
                            txtGrade3.Text = reader["Grade3Total"].ToString();
                            txtGrade4.Text = reader["Grade4Total"].ToString();
                            txtGrade5.Text = reader["Grade5Total"].ToString();
                            txtGrade6.Text = reader["Grade6Total"].ToString();
                            txtTotal.Text = reader["OverallTotal"].ToString();
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void btnLoadRMA_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbSchoolYear.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a school year.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    string selectedAssessment = metroComboBox1.SelectedItem.ToString();
                    string Year = metroComboBox1.SelectedItem.ToString();
                    TallyRMAData(selectedAssessment);
                    TallyTotalLearners(Year);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void TallyTotalLearners(string year)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

                    int grade1Count = GetLearnerCount(conn, 1);
                    int grade2Count = GetLearnerCount(conn, 2);
                    int grade3Count = GetLearnerCount(conn, 3);
                    int totalCount = grade1Count + grade2Count + grade3Count;

                    txtTotalLearners.Text = totalCount.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private int GetLearnerCount(SqlConnection conn, int gradeLevel)
        {
            string schoolYear = cbSchoolYear.SelectedItem.ToString();
            string query = "SELECT COUNT(DISTINCT LRN) FROM LearnersProfile WHERE GradeLevel = @GradeLevel AND SchoolYear = @SchoolYear";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                cmd.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                return (int)cmd.ExecuteScalar();
            }
        }

        private void TallyRMAData(string selectedAssessment)
        {

            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string schoolYear = cbSchoolYear.SelectedItem.ToString();
                string query = "SELECT RMAClassification, COUNT(*) AS Total FROM LearnersProfile WHERE SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType AND GradeLevel IN ('1', '2', '3') GROUP BY RMAClassification";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                    cmd.Parameters.AddWithValue("@AssessmentType", selectedAssessment);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        txtLowEmerging.Text = "0";
                        txtHighEmerging.Text = "0";
                        txtDeveloping.Text = "0";
                        txttTransitioning.Text = "0";
                        txttGradeReady.Text = "0";

                        RMAChart.Series.Clear();
                        RMAChart.Titles.Clear();

                        Title chartTitle = new Title("RMA Classification")
                        {
                            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold)
                        };

                        RMAChart.Titles.Add(chartTitle);

                        Series series = new Series
                        {
                            Name = "RMA Classification",
                            IsVisibleInLegend = true,
                            ChartType = SeriesChartType.Doughnut
                        };

                        RMAChart.Series.Add(series);

                        int totalClassification = 0;
                        Dictionary<string, int> classificationData = new Dictionary<string, int>();


                        while (reader.Read())
                        {
                            string rmaClassification = reader["RMAClassification"].ToString();
                            int total = Convert.ToInt32(reader["Total"]);
                            classificationData[rmaClassification] = total;
                            totalClassification += total;

                            switch (rmaClassification)
                            {
                                case "Low Emerging":
                                    txtLowEmerging.Text = total.ToString();
                                    break;
                                case "High Emerging":
                                    txtHighEmerging.Text = total.ToString();
                                    break;
                                case "Developing":
                                    txtDeveloping.Text = total.ToString();
                                    break;
                                case "Transitioning":
                                    txttTransitioning.Text = total.ToString();
                                    break;
                                case "Grade Ready":
                                    txttGradeReady.Text = total.ToString();
                                    break;
                            }
                        }

                        foreach (var item in classificationData)
                        {
                            double percentage = totalClassification > 0 ? (double)item.Value / totalClassification : 0;
                            DataPoint dp = new DataPoint(0, item.Value)
                            {
                                AxisLabel = item.Key,
                                LegendText = item.Key,
                                Label = string.Format("{0} ({1:P1})", item.Value, percentage)
                            };
                            series.Points.Add(dp);
                        }

                        series.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
                        RMAChart.Invalidate();
                    }


                }

            }
        }

        
    }
}
