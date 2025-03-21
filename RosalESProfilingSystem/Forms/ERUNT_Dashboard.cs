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
using System.Windows.Forms.DataVisualization.Charting;

namespace RosalESProfilingSystem.Forms
{
    public partial class ERUNT_Dashboard: Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        public ERUNT_Dashboard()
        {
            InitializeComponent();
        }

        private void cbScienceLearnerEnrollment_DropDown(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    string query = "SELECT DISTINCT SchoolYear FROM LearnersProfileScience ORDER BY SchoolYear ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            cbScienceLearnerEnrollment.Items.Clear();

                            while (reader.Read())
                            {
                                cbScienceLearnerEnrollment.Items.Add(reader["SchoolYear"].ToString());
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

        private void btnLoadEnrollment_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbScienceLearnerEnrollment.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a school year.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

                    int grade4Count = GetLearnerCount(conn, 4);
                    int grade5Count = GetLearnerCount(conn, 5);
                    int grade6Count = GetLearnerCount(conn, 6);
                    int totalCount = grade4Count + grade5Count + grade6Count;

                    txtGrade4.Text = grade4Count.ToString();
                    txtGrade5.Text = grade5Count.ToString();
                    txtGrade6.Text = grade6Count.ToString();
                    txtTotalLearners.Text = totalCount.ToString();

                    LoadChart(grade4Count, grade5Count, grade6Count, totalCount);
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

            DataPoint dp1 = new DataPoint(0, grade1Count) { AxisLabel = "Grade 4", LegendText = "Grade 4" };
            DataPoint dp2 = new DataPoint(0, grade2Count) { AxisLabel = "Grade 5", LegendText = "Grade 5" };
            DataPoint dp3 = new DataPoint(0, grade3Count) { AxisLabel = "Grade 6", LegendText = "Grade 6" };

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
            string schoolYear = cbScienceLearnerEnrollment.SelectedItem.ToString();
            string query = "SELECT COUNT(DISTINCT LRN) FROM LearnersProfileScience WHERE GradeLevel = @GradeLevel AND SchoolYear = @SchoolYear";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                cmd.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                return (int)cmd.ExecuteScalar();
            }
        }

        private void btnLoadERUNTData_Click(object sender, EventArgs e)
        {
            if (cbScienceLearnerEnrollment.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a school year.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string selectedAssessment = cbAssessmentType.SelectedItem.ToString();
                string Year = cbScienceLearnerEnrollment.SelectedItem.ToString();
                TallyERUNTData(selectedAssessment);
                TallyERUNTLearner(Year);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void TallyERUNTLearner(string year)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

                    int grade4Count = GetERUNTLearnerCount(conn, 4);
                    int grade5Count = GetERUNTLearnerCount(conn, 5);
                    int grade6Count = GetERUNTLearnerCount(conn, 6);
                    int totalCount = grade4Count + grade5Count + grade6Count;

                    txtTotalLearnerAssessed.Text = totalCount.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private int GetERUNTLearnerCount(SqlConnection conn, int gradeLevel)
        {
            string schoolYear = cbScienceLearnerEnrollment.SelectedItem.ToString();
            string selectedAssessment = cbAssessmentType.SelectedItem.ToString();
            string query = "SELECT COUNT(*) FROM LearnersProfileScience WHERE ERUNTClassification IS NOT NULL AND ERUNTClassification <> '' AND GradeLevel = @GradeLevel AND SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                cmd.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                cmd.Parameters.AddWithValue("@AssessmentType", selectedAssessment);
                return (int)cmd.ExecuteScalar();
            }
        }

        private void TallyERUNTData(string selectedAssessment)
        {

            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string schoolYear = cbScienceLearnerEnrollment.SelectedItem.ToString();
                string query = "SELECT ERUNTClassification, COUNT(*) AS Total FROM LearnersProfileScience WHERE SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType AND GradeLevel IN ('4', '5', '6') GROUP BY ERUNTClassification";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                    cmd.Parameters.AddWithValue("@AssessmentType", selectedAssessment);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        txtNotProficient.Text = "0";
                        txtLowProficient.Text = "0";
                        txtNearlyProficient.Text = "0";
                        txtProficient.Text = "0";
                        txtHighlyProficient.Text = "0";

                        chartERUNT.Series.Clear();
                        chartERUNT.Titles.Clear();

                        Title chartTitle = new Title("ERUNT Classification")
                        {
                            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold)
                        };

                        chartERUNT.Titles.Add(chartTitle);
                        Series series = new Series
                        {
                            Name = "ERUNT Classification",
                            IsVisibleInLegend = true,
                            ChartType = SeriesChartType.Doughnut
                        };



                        chartERUNT.Series.Add(series);

                        int totalClassification = 0;
                        Dictionary<string, int> classificationData = new Dictionary<string, int>()
                            {
                                { "Not Proficient", 0 },
                                { "Low Proficiency", 0 },
                                { "Nearly Proficient", 0 },
                                { "Proficient", 0 },
                                { "Highly Proficient", 0 }
                            };


                        while (reader.Read())
                        {
                            string rmaClassification = reader["ERUNTClassification"].ToString();
                            int total = Convert.ToInt32(reader["Total"]);

                            if (classificationData.ContainsKey(rmaClassification))
                            {
                                classificationData[rmaClassification] = total;
                            }

                            totalClassification += total;
                        }

                        // Update the textboxes
                        txtNotProficient.Text = classificationData["Not Proficient"].ToString();
                        txtLowProficient.Text = classificationData["Low Proficiency"].ToString();
                        txtNearlyProficient.Text = classificationData["Nearly Proficient"].ToString();
                        txtProficient.Text = classificationData["Proficient"].ToString();
                        txtHighlyProficient.Text = classificationData["Highly Proficient"].ToString();

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
                        chartERUNT.Invalidate();

                    }


                }

            }
        }

        private void btnLoadPolling_Click(object sender, EventArgs e)
        {
            if (cbScienceLearnerEnrollment.SelectedIndex == -1)
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

        private void TallyPollingData(string selectedAssessmentPolling)
        {
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string schoolYear = cbScienceLearnerEnrollment.SelectedItem.ToString();
                string query = "SELECT GradeLevel, ERUNTClassification, COUNT(*) AS Total FROM LearnersProfileScience WHERE SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType AND GradeLevel IN ('4', '5', '6') AND ERUNTClassification IS NOT NULL AND ERUNTClassification <> '' GROUP BY GradeLevel, ERUNTClassification";

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
                            string rmaClassification = reader["ERUNTClassification"].ToString();
                            int total = Convert.ToInt32(reader["Total"]);
                            if (!gradeData.ContainsKey(gradeLevel))
                            {
                                gradeData[gradeLevel] = new Dictionary<string, int>
                                {
                                    { "Not Proficient", 0 },
                                    { "Low Proficiency", 0 },
                                    { "Nearly Proficient", 0 },
                                    { "Proficient", 0 },
                                    { "Highly Proficient", 0 }
                                };

                            }
                            gradeData[gradeLevel][rmaClassification] = total;
                        }

                        DisplayDelayed(gradeData, "4", txtDelayedNumbersG4, txtDelayedPercentG4);
                        DisplayDelayed(gradeData, "5", txtDelayedNumbersG5, txtDelayedPercentG5);
                        DisplayDelayed(gradeData, "6", txtDelayedNumbersG6, txtDelayedPercentG6);

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
                int delayedLearners = data["Not Proficient"] + data["Low Proficiency"] + data["Nearly Proficient"] + data["Proficient"];

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

        private void btnViewDelayedG4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDelayedNumbersG4.Text))
            {
                MessageBox.Show("No data to display for Grade 4. Please select school year and assessment type first.", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (txtDelayedNumbersG4.Text == "0")
            {
                MessageBox.Show("No delayed learners for Grade 4", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                OpenDelayedDevelopmentForm("4");
            }
        }

        private void btnViewDelayedG5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDelayedNumbersG5.Text))
            {
                MessageBox.Show("No data to display for Grade 5. Please select school year and assessment type first.", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (txtDelayedNumbersG5.Text == "0")
            {
                MessageBox.Show("No delayed learners for Grade 5", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                OpenDelayedDevelopmentForm("5");
            }
        }

        private void btnViewDelayedG6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDelayedNumbersG6.Text))
            {
                MessageBox.Show("No data to display for Grade 6. Please select school year and assessment type first.", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (txtDelayedNumbersG6.Text == "0")
            {
                MessageBox.Show("No delayed learners for Grade 6", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                OpenDelayedDevelopmentForm("6");
            }
        }

        private void OpenDelayedDevelopmentForm(string gradeLevel)
        {
            string schoolYear = cbScienceLearnerEnrollment.SelectedItem.ToString();
            string selectedAssessmentPolling = cbPollingAssessment.SelectedItem.ToString();
            DelayedDevelopmenForm_ERUNT form = new DelayedDevelopmenForm_ERUNT(schoolYear, gradeLevel, selectedAssessmentPolling);
            form.ShowDialog();
        }
    }
}
