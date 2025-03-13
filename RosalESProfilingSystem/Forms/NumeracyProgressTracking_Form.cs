using MetroFramework.Controls;
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
    public partial class NumeracyProgressTracking_Form: Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        private int selectedLearnerID;
        private int selectedLearnerGrade;
        public NumeracyProgressTracking_Form()
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
                            
                            } }
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
                    string query = $"SELECT DISTINCT Id, GradeLevel, LastName, FirstName, MiddleName, LRN, Sex, Age FROM LearnersProfile WHERE {column} LIKE @SearchValue AND SchoolYear = @SchoolYear";

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
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}");
            }
        }

        private void gridLearners_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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

                metroComboBox1.SelectedIndex = 0;
                LoadCompetencies(1);

                UpdateCompetencyStats();
            }
        }

        private void UpdateCompetencyStats()
        {
            int mastered = dataGridView2.Rows.Cast<DataGridViewRow>().Count(r => Convert.ToBoolean(r.Cells["Mastered"].Value));
            int total = dataGridView2.Rows.Count;
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
            series.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
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

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedLearnerID > 0 && metroComboBox1.SelectedItem != null)
            {
                LoadCompetencies(Convert.ToInt32(metroComboBox1.SelectedItem));
            }
        }

        private void LoadCompetencies(int quarter)
        {
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string query = @"SELECT c.CompetencyId, c.Quarter, c.CompetencyNumber, c.CompetencyName, 
                                ISNULL(lp.Mastered, 0) AS Mastered
                                FROM RMACompetencies c
                                LEFT JOIN RMALearnerCompetencyProgress lp
                                ON c.CompetencyId = lp.CompetencyId AND lp.LearnerId = @LearnerId
                                WHERE c.Quarter = @Quarter AND GradeLevel = @GradeLevel";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@GradeLevel", selectedLearnerGrade);
                da.SelectCommand.Parameters.AddWithValue("@LearnerId", selectedLearnerID);
                da.SelectCommand.Parameters.AddWithValue("@Quarter", quarter);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView2.DataSource = dt;
                dataGridView2.Columns["CompetencyId"].Visible = false;
                dataGridView2.Columns["Quarter"].HeaderText = "Quarter";
                dataGridView2.Columns["CompetencyNumber"].HeaderText = "Competency No.";
                dataGridView2.Columns["CompetencyName"].HeaderText = "Competency";
                dataGridView2.Columns["Mastered"].HeaderText = "Mastered";

                foreach (DataGridViewColumn col in dataGridView2.Columns)
                {
                    col.ReadOnly = col.Name != "Mastered";
                }

                UpdateCompetencyStats();
            }
        }

        private void btnUpdateProgress_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                conn.Open();

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells["CompetencyId"].Value != null)
                    {
                        int competencyId = Convert.ToInt32(row.Cells["CompetencyId"].Value);
                        bool mastered = Convert.ToBoolean(row.Cells["Mastered"].Value);

                        string query = @"MERGE INTO RMALearnerCompetencyProgress AS target
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

        private void button1_Click(object sender, EventArgs e)
        {
            using (CompetenciesExpandedView view = new CompetenciesExpandedView(dataGridView2))
            {
                view.DataUpdated += ExpandedForm_DataUpdated;
                view.ShowDialog();
            }
        }

        private void ExpandedForm_DataUpdated()
        {
            // Refresh DataGridView to reflect changes
            dataGridView2.Refresh();
        }
    }
    }
