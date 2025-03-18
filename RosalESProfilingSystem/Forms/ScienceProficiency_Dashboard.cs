using iText.Signatures;
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
    public partial class ScienceProficiency_Dashboard: Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        public ScienceProficiency_Dashboard()
        {
            InitializeComponent();
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

        private void LoadChart(int grade4Count, int grade5Count, int grade6Count, int totalCount)
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

            DataPoint dp1 = new DataPoint(0, grade4Count) { AxisLabel = "Grade 4", LegendText = "Grade 4" };
            DataPoint dp2 = new DataPoint(0, grade5Count) { AxisLabel = "Grade 5", LegendText = "Grade 5" };
            DataPoint dp3 = new DataPoint(0, grade6Count) { AxisLabel = "Grade 6", LegendText = "Grade 6" };

            dp1.Label = string.Format("{0} ({1:P1})", grade4Count, grade4Count / (double)totalCount);
            dp2.Label = string.Format("{0} ({1:P1})", grade5Count, grade5Count / (double)totalCount);
            dp3.Label = string.Format("{0} ({1:P1})", grade6Count, grade6Count / (double)totalCount);

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

        private void cbScienceLearnerEnrollment_DropDown(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    string query = $"SELECT DISTINCT SchoolYear FROM LearnersProfileScience ORDER BY SchoolYear ASC";

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
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnLoadScienceData_Click(object sender, EventArgs e)
        {
            if (cbScienceLearnerEnrollment.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a school year.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }else if (cbAssessmentType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an assessment type.", "No Assessment Type Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

                try
                {
                    string selectedAssessment = cbAssessmentType.SelectedItem.ToString();
                    TallyScienceProficiencyData(selectedAssessment);
                    string Year = cbScienceLearnerEnrollment.SelectedItem.ToString();
                    TallySciCATData(Year);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
        }

        private void TallySciCATData(string year)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

                    int grade4Count = GetSciCATLearnerCount(conn, 4);
                    int grade5Count = GetSciCATLearnerCount(conn, 5);
                    int grade6Count = GetSciCATLearnerCount(conn, 6);
                    int totalCount = grade4Count + grade5Count + grade6Count;

                    txtTotalLearnerAssessed.Text = totalCount.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private int GetSciCATLearnerCount(SqlConnection conn, int gradeLevel)
        {
            string schoolYear = cbScienceLearnerEnrollment.SelectedItem.ToString();
            string selectedAssessment = cbAssessmentType.SelectedItem.ToString();
            string query = "SELECT COUNT(*) FROM LearnersProfileScience WHERE ClassificationLevel IS NOT NULL AND ClassificationLevel <> '' AND GradeLevel = @GradeLevel AND SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                cmd.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                cmd.Parameters.AddWithValue("@AssessmentType", selectedAssessment);
                return (int)cmd.ExecuteScalar();
            }
        }

        private void TallyScienceProficiencyData(string selectedAssessment)
        {
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string schoolYear = cbScienceLearnerEnrollment.SelectedItem.ToString();
                string query = $@"SELECT ClassificationLevel, COUNT(*) AS Total 
                         FROM LearnersProfileScience 
                         WHERE SchoolYear = @SchoolYear 
                         AND AssessmentType = @AssessmentType 
                         AND GradeLevel IN ('4', '5', '6') 
                         GROUP BY ClassificationLevel";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                    cmd.Parameters.AddWithValue("@AssessmentType", selectedAssessment);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Reset the textboxes before updating
                        txtNoProficiency.Text = "0";
                        txtPoorProficiency.Text = "0";
                        txtWeakProficiency.Text = "0";
                        txtSatisfactoryProficiency.Text = "0";
                        txtGoodProficiency.Text = "0";
                        txtVeryGoodProficiency.Text = "0";
                        txtExceptionalProficiency.Text = "0";

                        // Reset the chart properly
                        chartScience.Series.Clear();
                        chartScience.Titles.Clear();

                        Title chartTitle = new Title($"Science Proficiency Classification")
                        {
                            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold)
                        };
                        chartScience.Titles.Add(chartTitle);

                        Series series = new Series
                        {
                            Name = $"Science Proficiency Classification",
                            IsVisibleInLegend = true,
                            ChartType = SeriesChartType.Doughnut
                        };
                        chartScience.Series.Add(series);

                        int totalClassification = 0;
                        Dictionary<string, int> classificationData = new Dictionary<string, int>()
                {
                    { "No Proficiency at All", 0 },
                    { "Poor Proficiency", 0 },
                    { "Weak Proficiency", 0 },
                    { "Satisfactory Proficiency", 0 },
                    { "Good Proficiency", 0 },
                    { "Very Good Proficiency", 0 },
                    { "Exceptional Proficiency", 0 }
                };

                        while (reader.Read())
                        {
                            string rmaClassification = reader["ClassificationLevel"].ToString();
                            int total = Convert.ToInt32(reader["Total"]);

                            if (classificationData.ContainsKey(rmaClassification))
                            {
                                classificationData[rmaClassification] = total;
                            }

                            totalClassification += total;
                        }

                        // Update the textboxes
                        txtNoProficiency.Text = classificationData["No Proficiency at All"].ToString();
                        txtPoorProficiency.Text = classificationData["Poor Proficiency"].ToString();
                        txtWeakProficiency.Text = classificationData["Weak Proficiency"].ToString();
                        txtSatisfactoryProficiency.Text = classificationData["Satisfactory Proficiency"].ToString();
                        txtGoodProficiency.Text = classificationData["Good Proficiency"].ToString();
                        txtVeryGoodProficiency.Text = classificationData["Very Good Proficiency"].ToString();
                        txtExceptionalProficiency.Text = classificationData["Exceptional Proficiency"].ToString();


                        // Calculate total learners correctly (instead of summing classificationData values)
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
                        chartScience.DataBind();
                        chartScience.Invalidate();
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
                string query = "SELECT GradeLevel, ClassificationLevel, COUNT(*) AS Total FROM LearnersProfileScience WHERE SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType AND GradeLevel IN ('4', '5', '6') GROUP BY GradeLevel, ClassificationLevel";

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
                            string rmaClassification = reader["ClassificationLevel"].ToString();
                            int total = Convert.ToInt32(reader["Total"]);
                            if (!gradeData.ContainsKey(gradeLevel))
                            {
                                gradeData[gradeLevel] = new Dictionary<string, int>
                                {
                                    { "No Proficiency at All", 0 },
                                    { "Poor Proficiency", 0 },
                                    { "Weak Proficiency", 0 },
                                    { "Satisfactory Proficiency", 0 },
                                    { "Good Proficiency", 0 },
                                    { "Very Good Proficiency", 0},
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

        private void DisplayDelayed(Dictionary<string, Dictionary<string, int>> data, string grade, TextBox txtDelayedNumber, TextBox txtDelayedPercent)
        {
            if (data.ContainsKey(grade))
            {
                var gradeData = data[grade];
                int totalLearners = gradeData.Values.Sum();
                int delayedLearners = gradeData["No Proficiency at All"] + gradeData["Poor Proficiency"] + gradeData["Weak Proficiency"] + gradeData["Satisfactory Proficiency"] + gradeData["Good Proficiency"] + gradeData["Very Good Proficiency"];
                ;
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
                OpenDelayedDevelopmentForm("4");
            }
        }

        private void OpenDelayedDevelopmentForm(string gradeLevel)
        {
            string schoolYear = cbScienceLearnerEnrollment.SelectedItem.ToString();
            string selectedAssessmentPolling = cbPollingAssessment.SelectedItem.ToString();
            DelayedDevelopmentForm_ScienceProficiency form = new DelayedDevelopmentForm_ScienceProficiency(schoolYear, gradeLevel, selectedAssessmentPolling);
            form.ShowDialog();
        }

    }
}
