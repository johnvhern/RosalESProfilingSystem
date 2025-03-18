using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    public partial class Numeracy_Dashboard : Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        public Numeracy_Dashboard()
        {
            InitializeComponent();
            cbAssessmentType.SelectedIndex = 0;
           
        }

        private void btnLoadEnrollment_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbNumeracyLearnerEnrollment.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a school year.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

                    int grade1Count = GetLearnerCount(conn, 1);
                    int grade2Count = GetLearnerCount(conn, 2);
                    int grade3Count = GetLearnerCount(conn, 3);
                    int totalCount = grade1Count + grade2Count + grade3Count;

                    txtGrade1.Text = grade1Count.ToString();
                    txtGrade2.Text = grade2Count.ToString();
                    txtGrade3.Text = grade3Count.ToString();
                    txtTotalLearners.Text = totalCount.ToString();

                    LoadChart(grade1Count, grade2Count, grade3Count, totalCount);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void LoadChart(int grade1Count, int grade2Count, int grade3Count, int total)
        {
            chartLearners.Series.Clear();
            chartLearners.Titles.Clear();
            Title chartTitle = new Title("Learner Distribution by Grade Level")
            {
                Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold)
            };
            chartLearners.Titles.Add(chartTitle);
            Series series = new Series
            {
                Name = "Learners",
                IsVisibleInLegend = true,
                ChartType = SeriesChartType.Doughnut
            };

            chartLearners.Series.Add(series);

            DataPoint dp1 = new DataPoint(0, grade1Count) { AxisLabel = "Grade 1", LegendText = "Grade 1" };
            DataPoint dp2 = new DataPoint(0, grade2Count) { AxisLabel = "Grade 2", LegendText = "Grade 2" };
            DataPoint dp3 = new DataPoint(0, grade3Count) { AxisLabel = "Grade 3", LegendText = "Grade 3" };

            dp1.Label = string.Format("{0} ({1:P1})", grade1Count, grade1Count / (double)total);
            dp2.Label = string.Format("{0} ({1:P1})", grade2Count, grade2Count / (double)total);
            dp3.Label = string.Format("{0} ({1:P1})", grade3Count, grade3Count / (double)total);

            series.Points.Add(dp1);
            series.Points.Add(dp2);
            series.Points.Add(dp3);

            series.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);

            chartLearners.Invalidate();
        }

        private int GetLearnerCount(SqlConnection conn, int gradeLevel)
        {
            string schoolYear = cbNumeracyLearnerEnrollment.SelectedItem.ToString();
            string query = "SELECT COUNT(DISTINCT LRN) FROM LearnersProfile WHERE GradeLevel = @GradeLevel AND SchoolYear = @SchoolYear";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                cmd.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                return (int)cmd.ExecuteScalar();
            }
        }

        private void btnLoadRMA_Click(object sender, EventArgs e)
        {
            if (cbNumeracyLearnerEnrollment.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a school year.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string selectedAssessment = cbAssessmentType.SelectedItem.ToString();
                string Year = cbNumeracyLearnerEnrollment.SelectedItem.ToString();
                TallyRMAData(selectedAssessment);
                TallyRMALearner(Year);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void TallyRMALearner(string year)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

                    int grade1Count = GetRMALearnerCount(conn, 1);
                    int grade2Count = GetRMALearnerCount(conn, 2);
                    int grade3Count = GetRMALearnerCount(conn, 3);
                    int totalCount = grade1Count + grade2Count + grade3Count;

                    txtTotalLearnerAssessed.Text = totalCount.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private int GetRMALearnerCount(SqlConnection conn, int gradeLevel)
        {
            string schoolYear = cbNumeracyLearnerEnrollment.SelectedItem.ToString();
            string selectedAssessment = cbAssessmentType.SelectedItem.ToString();
            string query = "SELECT COUNT(*) FROM LearnersProfile WHERE RMAClassification IS NOT NULL AND RMAClassification <> '' AND GradeLevel = @GradeLevel AND SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                cmd.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                cmd.Parameters.AddWithValue("@AssessmentType", selectedAssessment);
                return (int)cmd.ExecuteScalar();
            }
        }

        private void TallyRMAData(string selectedAssessment)
        {

            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string schoolYear = cbNumeracyLearnerEnrollment.SelectedItem.ToString();
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
                        txtTransitioning.Text = "0";
                        txtGradeReady.Text = "0";

                        chartRMA.Series.Clear();
                        chartRMA.Titles.Clear();

                        Title chartTitle = new Title("RMA Classification")
                        {
                            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold)
                        };

                        chartRMA.Titles.Add(chartTitle);
                        Series series = new Series
                        {
                            Name = "RMA Classification",
                            IsVisibleInLegend = true,
                            ChartType = SeriesChartType.Doughnut
                        };



                        chartRMA.Series.Add(series);

                        int totalClassification = 0;
                        Dictionary<string, int> classificationData = new Dictionary<string, int>()
                            {
                                { "Low Emerging", 0 },
                                { "High Emerging", 0 },
                                { "Developing", 0 },
                                { "Transitioning", 0 },
                                { "Grade Ready", 0 }
                            };


                        while (reader.Read())
                        {
                            string rmaClassification = reader["RMAClassification"].ToString();
                            int total = Convert.ToInt32(reader["Total"]);

                            if (classificationData.ContainsKey(rmaClassification))
                            {
                                classificationData[rmaClassification] = total;
                            }

                            totalClassification += total;
                        }

                        // Update the textboxes
                        txtLowEmerging.Text = classificationData["Low Emerging"].ToString();
                        txtHighEmerging.Text = classificationData["High Emerging"].ToString();
                        txtDeveloping.Text = classificationData["Developing"].ToString();
                        txtTransitioning.Text = classificationData["Transitioning"].ToString();
                        txtGradeReady.Text = classificationData["Grade Ready"].ToString();

                        int totalLearners = classificationData.Values.Sum();

                        foreach (var item in classificationData)
                        {
                            double percentage = totalLearners > 0 ? ((double)item.Value / totalLearners) * 100 : 0;
                            DataPoint dp = new DataPoint(0, item.Value)
                            {
                                AxisLabel = item.Key,
                                LegendText = item.Key,
                                Label = $"{item.Value} ({percentage:F1}%)"
                            };
                            series.Points.Add(dp);
                        }

                        series.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
                        chartRMA.Invalidate();

                    }


                }

            }
        }

        private void TallyPollingData(string selectedAssessmentPolling)
        {
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string schoolYear = cbNumeracyLearnerEnrollment.SelectedItem.ToString();
                string query = "SELECT GradeLevel, RMAClassification, COUNT(*) AS Total FROM LearnersProfile WHERE SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType AND GradeLevel IN ('1', '2', '3') AND RMAClassification IS NOT NULL AND RMAClassification <> '' GROUP BY GradeLevel, RMAClassification";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                    cmd.Parameters.AddWithValue("@AssessmentType", selectedAssessmentPolling);
                    conn.Open();

                    var gradeData = new Dictionary<string, Dictionary<string, int>>();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string gradeLevel = reader["GradeLevel"].ToString();
                            string rmaClassification = reader["RMAClassification"].ToString();
                            int total = Convert.ToInt32(reader["Total"]);
                            if (!gradeData.ContainsKey(gradeLevel))
                            {
                                gradeData[gradeLevel] = new Dictionary<string, int>
                                {
                                    { "Low Emerging", 0 },
                                    { "High Emerging", 0 },
                                    { "Developing", 0 },
                                    { "Transitioning", 0 },
                                    { "Grade Ready", 0 }
                                };
                               
                            }
                            gradeData[gradeLevel][rmaClassification] = total;
                        }

                        DisplayDelayed(gradeData, "1", txtDelayedNumbersG1, txtDelayedPercentG1);
                        DisplayDelayed(gradeData, "2", txtDelayedNumbersG2, txtDelayedPercentG2);
                        DisplayDelayed(gradeData, "3", txtDelayedNumbersG3, txtDelayedPercentG3);

                    }

                }
            }
        }

        private void DisplayDelayed(Dictionary<string, Dictionary<string, int>> gradeData, string grade, TextBox txtDelayedNumber, TextBox txtDelayedPercent)
        {
            if (gradeData.ContainsKey(grade))
            {
                var data = gradeData[grade];

                // Count only learners who have a classification (ignore NULL/empty)
                int totalLearners = data.Where(kv => !string.IsNullOrEmpty(kv.Key)).Sum(kv => kv.Value);
                int delayedLearners = data["Low Emerging"] + data["High Emerging"] + data["Developing"] + data["Transitioning"];

                double delayedPercentage = totalLearners > 0 ? ((double)delayedLearners / totalLearners) * 100 : 0;

                txtDelayedNumber.Text = delayedLearners.ToString();
                txtDelayedPercent.Text = $"{delayedPercentage:F2}%";
            }
            else
            {
                txtDelayedNumber.Text = "0";
                txtDelayedPercent.Text = "0%";
            }
        }

        private void btnViewDelayedG1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDelayedNumbersG1.Text))
            {
                MessageBox.Show("No data to display for Grade 1. Please select school year and assessment type first.", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (txtDelayedNumbersG1.Text == "0")
            {
                MessageBox.Show("No delayed learners for Grade 1", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                OpenDelayedDevelopmentForm("1");
            }
                
        }

        

        private void btnViewDelayedG2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDelayedNumbersG2.Text))
            {
                MessageBox.Show("No data to display for Grade 2. Please select school year and assessment type first.", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (txtDelayedNumbersG2.Text == "0")
            {
                MessageBox.Show("No delayed learners for Grade 2", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                OpenDelayedDevelopmentForm("2");
            }
                
        }

        private void btnViewDelayedG3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDelayedNumbersG3.Text))
            {
                MessageBox.Show("No data to display for Grade 3. Please select school year and assessment type first.", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (txtDelayedNumbersG3.Text == "0")
            {
                MessageBox.Show("No delayed learners for Grade 3", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                OpenDelayedDevelopmentForm("3");
            }
        }

        private void OpenDelayedDevelopmentForm(string gradeLevel)
        {
            string schoolYear = cbNumeracyLearnerEnrollment.SelectedItem.ToString();
            string selectedAssessmentPolling = cbPollingAssessment.SelectedItem.ToString();
            DelayedDevelopmentForm_Numeracy form = new DelayedDevelopmentForm_Numeracy(schoolYear, gradeLevel, selectedAssessmentPolling);
            form.ShowDialog();
        }

        private void cbNumeracyLearnerEnrollment_DropDown(object sender, EventArgs e)
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
                            cbNumeracyLearnerEnrollment.Items.Clear();

                            while (reader.Read())
                            {
                                cbNumeracyLearnerEnrollment.Items.Add(reader["SchoolYear"].ToString());
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

        private void btnLoadPolling_Click(object sender, EventArgs e)
        {
            if (cbNumeracyLearnerEnrollment.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a school year.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (cbPollingAssessment.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an assessment type.", "No Assessment Type Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

                try
                {
                    string selectedAssessmentPolling = cbPollingAssessment.SelectedItem.ToString();
                    TallyPollingData(selectedAssessmentPolling);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbAssessmentType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel10_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel12_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbPollingAssessment_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
