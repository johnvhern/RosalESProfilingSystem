﻿using RosalESProfilingSystem.Components;
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
    public partial class LiteracyProgressTracking: Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        private int selectedLearnerID;
        private int selectedLearnerGrade;
        public LiteracyProgressTracking()
        {
            InitializeComponent();
            cbSearchTerm.SelectedIndex = 0;
        }

        private void cbSchoolYear_DropDown(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    string query = "SELECT DISTINCT SchoolYear FROM LearnersProfile UNION SELECT DISTINCT SchoolYear FROM LearnersProfileScience ORDER BY SchoolYear ASC";

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
                MessageBox.Show(ex.Message);
            }
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
                string column = cbSearchTerm.SelectedItem.ToString();
                string searchValue = textBox1.Text.Trim();

                if (string.IsNullOrEmpty(searchValue))
                {
                    MessageBox.Show("Please enter a search term.");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();
                    string query = $"WITH UniqueLearners AS (SELECT *, ROW_NUMBER() OVER (PARTITION BY GradeLevel, LastName, FirstName, MiddleName, LRN, Sex, Age ORDER BY Id ASC) AS row_num FROM LearnersProfile WHERE {column} LIKE @SearchValue AND SchoolYear = @SchoolYear) SELECT Id, GradeLevel, LastName, FirstName, MiddleName, LRN, Sex, Age FROM UniqueLearners WHERE row_num = 1;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SearchValue", $"%{searchValue}%");
                        cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }

                        gridLearners.DataSource = dt;
                        gridLearners.Columns["Id"].Visible = false;
                        dataGridView2.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}");
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = gridLearners.Rows[e.RowIndex];

                selectedLearnerID = Convert.ToInt32(row.Cells["Id"].Value);
                selectedLearnerGrade = Convert.ToInt32(row.Cells["GradeLevel"].Value);

                string lastName = row.Cells["LastName"].Value.ToString();
                string firstName = row.Cells["FirstName"].Value.ToString();
                string middleName = row.Cells["MiddleName"].Value.ToString();
                string lrn = row.Cells["LRN"].Value.ToString();
                string gradeLevel = row.Cells["GradeLevel"].Value.ToString();

                txtNameofLearner.Text = $"{firstName} {middleName} {lastName}";
                txtLRN.Text = lrn;
                txtGradeLevel.Text = gradeLevel;

                metroComboBox1.SelectedIndex = 0; // Reset quarter selection

                LoadCompetencyType(); // Load competency types

                // Load first competency type if available
                if (cbCompetencyType.Items.Count > 0)
                {
                    string firstCompetencyType = cbCompetencyType.SelectedItem.ToString();
                    int selectedQuarter = Convert.ToInt32(metroComboBox1.SelectedItem);

                    LoadGrade1Competencies(selectedQuarter, firstCompetencyType);
                }

                UpdateCompetencyStats();
            }
        }

        private void LoadCompetencyType()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    string query = "SELECT DISTINCT CompetencyType FROM CRLACompetencies WHERE GradeLevel = @GradeLevel ORDER BY CompetencyType ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@GradeLevel", selectedLearnerGrade);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            cbCompetencyType.Items.Clear();
                            while (reader.Read())
                            {
                                cbCompetencyType.Items.Add(reader["CompetencyType"].ToString());
                            }
                        }
                    }
                }
                if (cbCompetencyType.Items.Count > 0)
                {
                    cbCompetencyType.SelectedIndex = 0; // Select first competency type
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading competency types: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadGrade1Competencies(int quarter, string competencyType)
        {
            if (cbCompetencyType.SelectedItem == null)
            {
                MessageBox.Show("Please select a competency type.");
                return;
            }

            string selectedCompetencyType = cbCompetencyType.SelectedItem.ToString();

            LoadingForm loadingForm = new LoadingForm();
            loadingForm.Show();
            loadingForm.Refresh();

            try
                {

                    DataTable dt = await Task.Run(() =>
                    {
                        using (SqlConnection conn = new SqlConnection(dbConnection))
                        {
                            string query = @"
                SELECT c.CompetencyId, c.Quarter, c.Category, c.CompetencyName, 
                       ISNULL(lp.Mastered, 0) AS Mastered
                FROM CRLACompetencies c
                LEFT JOIN CRLALearnerCompetencyProgress lp 
                ON c.CompetencyId = lp.CompetencyId AND lp.LearnerId = @LearnerId
                WHERE c.Quarter = @Quarter AND c.CompetencyType = @CompetencyType AND c.GradeLevel = @GradeLevel";

                            using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                            {
                                da.SelectCommand.Parameters.AddWithValue("@GradeLevel", selectedLearnerGrade);
                                da.SelectCommand.Parameters.AddWithValue("@LearnerId", selectedLearnerID);
                                da.SelectCommand.Parameters.AddWithValue("@Quarter", quarter);
                                da.SelectCommand.Parameters.AddWithValue("@CompetencyType", competencyType);

                                DataTable tempDt = new DataTable();
                                da.Fill(tempDt);
                                return tempDt;
                            }
                        }
                    });

                    Invoke(new Action(() =>
                    {
                        dataGridView2.DataSource = dt;
                        dataGridView2.Columns["CompetencyId"].Visible = false;
                        dataGridView2.Columns["Quarter"].HeaderText = "Quarter";
                        dataGridView2.Columns["Category"].HeaderText = "Category";
                        dataGridView2.Columns["CompetencyName"].HeaderText = "Competency";
                        dataGridView2.Columns["Mastered"].HeaderText = "Mastered";

                        foreach (DataGridViewColumn col in dataGridView2.Columns)
                        {
                            col.ReadOnly = col.Name != "Mastered";
                        }

                        UpdateCompetencyStats();
                    }));
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading competencies: {ex.Message}");
                }
                finally
                {
                    loadingForm.Close();
                }
                
            }


        private void UpdateCompetencyStats()
        {
            int mastered = 0, total = dataGridView2.Rows.Count;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Mastered"].Value)) mastered++;
            }

            int unmastered = total - mastered;

            txtMastered.Text = mastered.ToString();
            txtUnmastered.Text = unmastered.ToString();

            DisplayCompetencyChart(mastered, unmastered);
        }

        private void DisplayCompetencyChart(int mastered, int unmastered)
        {
            chart1.Series.Clear();

            Series series = new Series
            {
                ChartType = SeriesChartType.Doughnut
            };

            series.Points.AddXY("Mastered", mastered);
            series.Points.AddXY("Unmastered", unmastered);

            series.IsValueShownAsLabel = true;
            series.Label = "#VALX: #VALY (#PERCENT{P0})";
            series.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            chart1.Series.Add(series);

            chart1.Legends.Clear();
            Legend legend = new Legend
            {
                Name = "CompetencyLegend",
                Docking = Docking.Top,
                Alignment = StringAlignment.Center,
                LegendStyle = LegendStyle.Table
            };
            chart1.Legends.Add(legend);
            series.Legend = "CompetencyLegend";
        }

        private void CompetencySelectionChanged()
        {
            if (selectedLearnerID > 0 && metroComboBox1.SelectedItem != null && cbCompetencyType.SelectedItem != null)
            {
                int quarter = Convert.ToInt32(metroComboBox1.SelectedItem);
                string competencyType = cbCompetencyType.SelectedItem.ToString();
                LoadGrade1Competencies(quarter, competencyType);
            }
        }

        private void cbCompetencyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CompetencySelectionChanged();
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CompetencySelectionChanged();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.DataSource == null)
            {
                MessageBox.Show("Please select a learner first.", "No Learner Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (CompetenciesExpandedView view = new CompetenciesExpandedView(dataGridView2))
                {
                    view.DataUpdated += ExpandedForm_DataUpdated;
                    view.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        private void ExpandedForm_DataUpdated()
        {
            // Refresh DataGridView to reflect changes
            dataGridView2.Refresh();
        }

        private void btnUpdateProgress_Click(object sender, EventArgs e)
        {
            if (cbSchoolYear.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a school year.", "No school year selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrEmpty(txtNameofLearner.Text))
            {
                MessageBox.Show("Please select a learner.", "No learner selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                conn.Open();

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells["CompetencyId"].Value != null)
                    {
                        int competencyId = Convert.ToInt32(row.Cells["CompetencyId"].Value);
                        bool mastered = Convert.ToBoolean(row.Cells["Mastered"].Value);

                        string query = @"MERGE INTO CRLALearnerCompetencyProgress AS target
                                        USING (VALUES (@LearnerId, @CompetencyId)) AS source (LearnerId, CompetencyId)
                                        ON target.LearnerId = source.LearnerId AND target.CompetencyId = source.CompetencyId
                                        WHEN MATCHED THEN
                                            UPDATE SET Mastered = @Mastered
                                        WHEN NOT MATCHED THEN
                                            INSERT (LearnerId, CompetencyId, Mastered)
                                            VALUES (@LearnerId, @CompetencyId, @Mastered);";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@LearnerId", selectedLearnerID);
                            cmd.Parameters.AddWithValue("@CompetencyId", competencyId);
                            cmd.Parameters.AddWithValue("@Mastered", mastered);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            MessageBox.Show("Competency progress updated successfully!");
            UpdateCompetencyStats();
        }

        private void btnViewCompetencyChart_Click(object sender, EventArgs e)
        {
            if (cbSchoolYear.SelectedItem == null)
            {
                MessageBox.Show("Please select a year first.", "No School Year Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            } else if (metroComboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a quarter first.", "No Quarter Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }else if (cbCompetencyType.SelectedItem == null)
            {
                MessageBox.Show("Please select a competency type first.", "No Competency Type Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

                string selectedYear = cbSchoolYear.SelectedItem.ToString();
            int quarter = Convert.ToInt32(metroComboBox1.SelectedItem);
            string competencyType = cbCompetencyType.SelectedItem.ToString();

            // 🔹 Pass the selected year to the chart form
            CompetencyChartForm_CRLA chartForm = new CompetencyChartForm_CRLA(selectedYear, quarter, competencyType);
            chartForm.Show(); // Opens the chart form
        }
    }
}
