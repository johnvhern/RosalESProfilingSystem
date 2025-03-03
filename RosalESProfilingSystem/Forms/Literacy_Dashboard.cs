using RosalESProfilingSystem.Components;
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
    public partial class Literacy_Dashboard : Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        public Literacy_Dashboard()
        {
            InitializeComponent();



        }

        private void Literacy_Dashboard_Resize(object sender, EventArgs e)
        {

            int y = Screen.PrimaryScreen.Bounds.Height;
            int x = Screen.PrimaryScreen.Bounds.Width;
            this.Height = y - 40;
            this.Width = x;
            this.Left = 0;
            this.Top = 0;

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
                            cbLiteracyLearnerEnrollment.Items.Clear();
                            while (reader.Read())
                            {
                                cbLiteracyLearnerEnrollment.Items.Add(reader["SchoolYear"].ToString());

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoadEnrollment_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbLiteracyLearnerEnrollment.SelectedIndex == -1)
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
            string schoolYear = cbLiteracyLearnerEnrollment.SelectedItem.ToString();
            string query = "SELECT COUNT(DISTINCT LRN) FROM LearnersProfile WHERE GradeLevel = @GradeLevel AND SchoolYear = @SchoolYear";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                cmd.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                return (int)cmd.ExecuteScalar();
            }
        }

        private void btnLoadCRLA_Click(object sender, EventArgs e)
        {
            if (cbLiteracyLearnerEnrollment.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a school year.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (cbAssessmentType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an assessment type.", "No Assessment Type Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (cbCRLALanguage.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a language.", "No Language Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string languageColumn = "";

                if (cbCRLALanguage.SelectedItem.ToString() == "Akeanon")
                {
                    languageColumn = "CRLAClassificationAkeanon";
                }
                else if (cbCRLALanguage.SelectedItem.ToString() == "Filipino")
                {
                    languageColumn = "CRLAClassificationFilipino";
                }
                else if (cbCRLALanguage.SelectedItem.ToString() == "English")
                {
                    languageColumn = "CRLAClassificationEnglish";
                }
                else
                {
                    MessageBox.Show("Invalid language selected.");
                    return;
                }

                string assessmentType = cbAssessmentType.SelectedItem.ToString();

                TallyCRLAData(languageColumn, assessmentType);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void TallyCRLAData(string languageColumn, string assessmentType)
        {
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string language = cbCRLALanguage.SelectedItem.ToString();
                string schoolYear = cbLiteracyLearnerEnrollment.SelectedItem.ToString();
                string query = $@"SELECT {languageColumn}, COUNT(*) AS Total FROM LearnersProfile WHERE SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType AND GradeLevel IN ('1', '2', '3') GROUP BY {languageColumn}";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                    cmd.Parameters.AddWithValue("@AssessmentType", assessmentType);
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

                        Title chartTitle = new Title($"CRLA Classification {language}")
                        {
                            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold)
                        };

                        chartRMA.Titles.Add(chartTitle);

                        Series series = new Series
                        {
                            Name = $"CRLA Classification {language}" ,
                            IsVisibleInLegend = true,
                            ChartType = SeriesChartType.Doughnut
                        };

                        chartRMA.Series.Add(series);

                        int totalClassification = 0;
                        Dictionary<string, int> classificationData = new Dictionary<string, int>();


                        while (reader.Read())
                        {
                            string rmaClassification = reader[$"{languageColumn}"].ToString();
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
                                    txtTransitioning.Text = total.ToString();
                                    break;
                                case "Grade Ready":
                                    txtGradeReady.Text = total.ToString();
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
                        chartRMA.Invalidate();
                    }


                }
            }
        }
    }
    }
