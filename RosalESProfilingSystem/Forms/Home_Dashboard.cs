using MetroFramework;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
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
            typeof(Panel).InvokeMember("DoubleBuffered",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
            null, panel4, new object[] { true });
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED - enables smoother UI rendering
                return cp;
            }
        }


        private void dataOnLoad()
        {

            if (cbSchoolYear.Items.Count > 0)
            {
                btnLoadAllEnrollments_Click(this, EventArgs.Empty);
                cbRMAList.SelectedIndex = 0;
                btnLoadRMA_Click(this, EventArgs.Empty);
                cbCRLAList.SelectedIndex = 0;
                cbCRLALanguage.SelectedIndex = 0;
                loadRMA_Click(this, EventArgs.Empty);
                cbSciCATList.SelectedIndex = 0;
                loadSciCATData_Click(this, EventArgs.Empty);
                cbERUNT.SelectedIndex = 0;
                btnLoadERUNT_Click(this, EventArgs.Empty);
            }
            
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
                MessageBox.Show("Error: " + ex.Message);
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

                else if (cbRMAList.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select an assessment type.", "No Assessment Type Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    string selectedAssessment = cbRMAList.SelectedItem.ToString();
                    string Year = cbSchoolYear.SelectedItem.ToString();
                    TallyRMAData(selectedAssessment);
                    TallyRMALearner(Year);

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


        private int GetRMALearnerCount(SqlConnection conn, int gradeLevel)
        {
            string schoolYear = cbSchoolYear.SelectedItem.ToString();
            string selectedAssessment = cbRMAList.SelectedItem.ToString();
            string query = "SELECT COUNT(*) FROM LearnersProfile WHERE RMAClassification IS NOT NULL AND RMAClassification <> '' AND GradeLevel = @GradeLevel AND SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                cmd.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                cmd.Parameters.AddWithValue("@AssessmentType", selectedAssessment);
                return (int)cmd.ExecuteScalar();
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

                    txtTotalLearners.Text = totalCount.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
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
                        txttTransitioning.Text = classificationData["Transitioning"].ToString();
                        txttGradeReady.Text = classificationData["Grade Ready"].ToString();

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
                        RMAChart.Invalidate();

                    }


                }

            }
        }

        private void loadRMA_Click(object sender, EventArgs e)
        {
            if (cbSchoolYear.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a school year.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (cbCRLAList.SelectedIndex == -1)
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
                string languageColumn;

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

                string assessmentType = cbCRLAList.SelectedItem.ToString();
                string Year = cbSchoolYear.SelectedItem.ToString();

                TallyCRLAData(assessmentType, languageColumn);
                TallyCRLALearners(Year, languageColumn);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private int GetCRLALearnerCount(SqlConnection conn, int gradeLevel, string languageColumn)
        {
            string schoolYear = cbSchoolYear.SelectedItem.ToString();
            string selectedAssessment = cbCRLAList.SelectedItem.ToString();
            string query = $"SELECT COUNT(*) FROM LearnersProfile WHERE {languageColumn} IS NOT NULL AND {languageColumn} <> '' AND GradeLevel = @GradeLevel AND SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                cmd.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                cmd.Parameters.AddWithValue("@AssessmentType", selectedAssessment);
                return (int)cmd.ExecuteScalar();
            }
        }

        private void TallyCRLALearners(string year, string languageColumn)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();

                    int grade1Count = GetCRLALearnerCount(conn, 1, languageColumn);
                    int grade2Count = GetCRLALearnerCount(conn, 2, languageColumn);
                    int grade3Count = GetCRLALearnerCount(conn, 3, languageColumn);
                    int totalCount = grade1Count + grade2Count + grade3Count;

                    txtCRLATotalLearners.Text = totalCount.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void TallyCRLAData(string assessmentType, string languageColumn)
        {
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string language = cbCRLALanguage.SelectedItem.ToString();
                string schoolYear = cbSchoolYear.SelectedItem.ToString();
                string query = $@"SELECT {languageColumn} AS Classification, COUNT(*) AS Total 
                         FROM LearnersProfile 
                         WHERE SchoolYear = @SchoolYear 
                         AND AssessmentType = @AssessmentType 
                         AND GradeLevel IN ('1', '2', '3') 
                         GROUP BY {languageColumn}";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                    cmd.Parameters.AddWithValue("@AssessmentType", assessmentType);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Reset the textboxes before updating
                        txtCRLALowEmerging.Text = "0";
                        txtCRLAHighEmerging.Text = "0";
                        txtCRLADeveloping.Text = "0";
                        txtCRLATransitioning.Text = "0";
                        txtCRLAGradeReady.Text = "0";

                        // Reset the chart properly
                        CRLAChart.Series.Clear();
                        CRLAChart.Titles.Clear();

                        Title chartTitle = new Title($"CRLA Classification - {language}")
                        {
                            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold)
                        };
                        CRLAChart.Titles.Add(chartTitle);

                        Series series = new Series
                        {
                            Name = $"CRLA Classification {language}",
                            IsVisibleInLegend = true,
                            ChartType = SeriesChartType.Doughnut
                        };
                        CRLAChart.Series.Add(series);

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
                            string rmaClassification = reader["Classification"].ToString();
                            int total = Convert.ToInt32(reader["Total"]);

                            if (classificationData.ContainsKey(rmaClassification))
                            {
                                classificationData[rmaClassification] = total;
                            }

                            totalClassification += total;
                        }

                        // Update the textboxes
                        txtCRLALowEmerging.Text = classificationData["Low Emerging"].ToString();
                        txtCRLAHighEmerging.Text = classificationData["High Emerging"].ToString();
                        txtCRLADeveloping.Text = classificationData["Developing"].ToString();
                        txtCRLATransitioning.Text = classificationData["Transitioning"].ToString();
                        txtCRLAGradeReady.Text = classificationData["Grade Ready"].ToString();

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
                        CRLAChart.DataBind();
                        CRLAChart.Invalidate();
                    }


                }
            }
        }


        private void loadSciCATData_Click(object sender, EventArgs e)
        {
            if (cbSchoolYear.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a school year.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (cbSciCATList.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an assessment type.", "No Assessment Type Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string selectedAssessment = cbSciCATList.SelectedItem.ToString();
                string Year = cbSchoolYear.SelectedItem.ToString();

                TallyScienceProficiencyData(selectedAssessment);
                TallySciCATData(Year);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private int GetSciCATLearnerCount(SqlConnection conn, int gradeLevel)
        {
            string schoolYear = cbSchoolYear.SelectedItem.ToString();
            string selectedAssessment = cbSciCATList.SelectedItem.ToString();
            string query = "SELECT COUNT(*) FROM LearnersProfileScience WHERE ClassificationLevel IS NOT NULL AND ClassificationLevel <> '' AND GradeLevel = @GradeLevel AND SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                cmd.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                cmd.Parameters.AddWithValue("@AssessmentType", selectedAssessment);
                return (int)cmd.ExecuteScalar();
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

                    txtSciCATLearners.Text = totalCount.ToString();

                }
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
                string schoolYear = cbSchoolYear.SelectedItem.ToString();
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
                        txtSatisfactoryProf.Text = "0";
                        txtGoodProficiency.Text = "0";
                        txtVeryGoodProficiency.Text = "0";
                        txtExcepProficiency.Text = "0";

                        // Reset the chart properly
                        SciCATChart.Series.Clear();
                        SciCATChart.Titles.Clear();

                        Title chartTitle = new Title($"Science Proficiency Classification")
                        {
                            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold)
                        };
                        SciCATChart.Titles.Add(chartTitle);

                        Series series = new Series
                        {
                            Name = $"Science Proficiency Classification",
                            IsVisibleInLegend = true,
                            ChartType = SeriesChartType.Doughnut
                        };
                        SciCATChart.Series.Add(series);

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
                        txtSatisfactoryProf.Text = classificationData["Satisfactory Proficiency"].ToString();
                        txtGoodProficiency.Text = classificationData["Good Proficiency"].ToString();
                        txtVeryGoodProficiency.Text = classificationData["Very Good Proficiency"].ToString();
                        txtExcepProficiency.Text = classificationData["Exceptional Proficiency"].ToString();
                        

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
                        SciCATChart.DataBind();
                        SciCATChart.Invalidate();
                    }


                }
            }
        }

        private void cbSchoolYear_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            dataOnLoad();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabControl1.TabPages[1];
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabControl1.TabPages[0];
        }

        private void btnLoadERUNT_Click(object sender, EventArgs e)
        {
            if (cbSchoolYear.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a school year.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }else if (cbERUNT.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an assessment type.", "No Assessment Type Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

                try
                {
                    string selectedAssessment = cbERUNT.SelectedItem.ToString();
                    string Year = cbSchoolYear.SelectedItem.ToString();
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

                    txtERUNTLearnerAssessed.Text = totalCount.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private int GetERUNTLearnerCount(SqlConnection conn, int gradeLevel)
        {
            string schoolYear = cbSchoolYear.SelectedItem.ToString();
            string selectedAssessment = cbERUNT.SelectedItem.ToString();
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
                string schoolYear = cbSchoolYear.SelectedItem.ToString();
                string query = "SELECT ERUNTClassification, COUNT(*) AS Total FROM LearnersProfileScience WHERE SchoolYear = @SchoolYear AND AssessmentType = @AssessmentType AND GradeLevel IN ('4', '5', '6') GROUP BY ERUNTClassification";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                    cmd.Parameters.AddWithValue("@AssessmentType", selectedAssessment);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        txtNotProficient.Text = "0";
                        txtLowProficiency.Text = "0";
                        txtNearlyProficient.Text = "0";
                        txtProficient.Text = "0";
                        txtHighlyProficient.Text = "0";

                        chart1.Series.Clear();
                        chart1.Titles.Clear();

                        Title chartTitle = new Title("ERUNT Classification")
                        {
                            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold)
                        };

                        chart1.Titles.Add(chartTitle);
                        Series series = new Series
                        {
                            Name = "ERUNT Classification",
                            IsVisibleInLegend = true,
                            ChartType = SeriesChartType.Doughnut
                        };



                        chart1.Series.Add(series);

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
                        txtLowProficiency.Text = classificationData["Low Proficiency"].ToString();
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
                        chart1.Invalidate();

                    }


                }

            }
        }

        private void Home_Dashboard_Load(object sender, EventArgs e)
        {
           
        }

        private void RMAChart_Click(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel10_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
