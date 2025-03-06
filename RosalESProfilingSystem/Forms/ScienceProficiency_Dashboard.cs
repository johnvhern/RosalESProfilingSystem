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

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
        }

        private void TallyScienceProficiencyData(string selectedAssessment)
        {
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string schoolYear = cbScienceLearnerEnrollment.SelectedItem.ToString();
                string query = "SELECT ClassificationLevel, COUNT(*) AS Total FROM LearnersProfileScience WHERE SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType AND GradeLevel IN ('4', '5', '6') GROUP BY ClassificationLevel";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                    cmd.Parameters.AddWithValue("@AssessmentType", selectedAssessment);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        txtNoProficiency.Text = "0";
                        txtPoorProficiency.Text = "0";
                        txtWeakProficiency.Text = "0";
                        txtSatisfactoryProficiency.Text = "0";
                        txtGoodProficiency.Text = "0";
                        txtVeryGoodProficiency.Text = "0";
                        txtExceptionalProficiency.Text = "0";

                        chartScience.Series.Clear();
                        chartScience.Titles.Clear();

                        Title chartTitle = new Title("Science Proficiency Classification")
                        {
                            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold)
                        };

                        chartScience.Titles.Add(chartTitle);

                        Series series = new Series
                        {
                            Name = "Science Proficiency Classification",
                            IsVisibleInLegend = true,
                            ChartType = SeriesChartType.Doughnut
                        };

                        chartScience.Series.Add(series);

                        int totalClassification = 0;
                        Dictionary<string, int> classificationData = new Dictionary<string, int>();


                        while (reader.Read())
                        {
                            string scienceClassification = reader["ClassificationLevel"].ToString();
                            int total = Convert.ToInt32(reader["Total"]);
                            classificationData[scienceClassification] = total;
                            totalClassification += total;

                            switch (scienceClassification)
                            {
                                case "No Proficiency at All":
                                    txtNoProficiency.Text = total.ToString();
                                    break;
                                case "Poor Proficiency":
                                    txtPoorProficiency.Text = total.ToString();
                                    break;
                                case "Weak Proficiency":
                                    txtWeakProficiency.Text = total.ToString();
                                    break;
                                case "Satisfactory Proficiency":
                                    txtSatisfactoryProficiency.Text = total.ToString();
                                    break;
                                case "Good Proficiency":
                                    txtGoodProficiency.Text = total.ToString();
                                    break;
                                case "Very Good Proficiency":
                                    txtVeryGoodProficiency.Text = total.ToString();
                                    break;
                                case "Exceptional Proficiency":
                                    txtExceptionalProficiency.Text = total.ToString();
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
                        chartScience.Invalidate();
                    }


                }

            }
        }
    }
}
