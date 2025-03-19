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
    public partial class CompetencyChartForm_SciCAT: Form
    {
        private string selectedYear;
        private string selectedQuarter;
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        public CompetencyChartForm_SciCAT(string year, string quarter)
        {
            InitializeComponent();
            selectedYear = year;
            selectedQuarter = quarter;
        }

        private void LoadCompetencyData()
        {
            try
            {
                if (string.IsNullOrEmpty(selectedYear)) return;

                // Get total learners for the selected year
                int totalLearners = 0;
                string queryTotal = "SELECT COUNT(DISTINCT LRN) FROM LearnersProfileScience WHERE SchoolYear = @year";

                using (SqlConnection conn = new SqlConnection(dbConnection))
                using (SqlCommand cmd = new SqlCommand(queryTotal, conn))
                {
                    cmd.Parameters.AddWithValue("@year", selectedYear);
                    conn.Open();
                    totalLearners = Convert.ToInt32(cmd.ExecuteScalar());
                }

                lblTotalLearners.Text = totalLearners.ToString();

                if (totalLearners == 0) return; // Prevent division by zero

                // ✅ Create DataTable dt
                DataTable dt = new DataTable();

                string query = @"
            SELECT c.CompetencyName, COUNT(r.LearnerId) AS MasteredCount 
            FROM SciCATLearnerCompetencyProgress r 
            INNER JOIN SciCATCompetencies c ON r.CompetencyId = c.CompetencyId
            WHERE r.Mastered = 1 AND c.Quarter = @quarter
            GROUP BY c.CompetencyName 
            ORDER BY MasteredCount DESC";

                using (SqlConnection conn = new SqlConnection(dbConnection))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@quarter", selectedQuarter);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt); // ✅ Fill the DataTable
                }

                // ✅ Ensure DataTable is being used here
                chart1.Series.Clear();
                chart1.ChartAreas[0].AxisX.Title = "Competencies";
                chart1.ChartAreas[0].AxisY.Title = "Number of Learners";
                chart1.ChartAreas[0].AxisX.Interval = 1;
                chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                chart1.ChartAreas[0].BackColor = Color.White;
                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

                chart1.Titles.Clear();
                chart1.Titles.Add("Competency Mastery Chart");
                chart1.Titles[0].Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);

                Series series = new Series("Mastered Competencies")
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = true,
                    Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold),
                };

                foreach (DataRow row in dt.Rows)
                {
                    string competencyName = row["CompetencyName"].ToString();
                    int masteredCount = Convert.ToInt32(row["MasteredCount"]);

                    // ✅ Calculate Percentage
                    double percentage = ((double)masteredCount / totalLearners) * 100;

                    // ✅ Add Data Point and Set Label
                    int index = series.Points.AddXY(competencyName, masteredCount);
                    series.Points[index].Label = $"{masteredCount}\n({percentage:F1}%)";
                }

                chart1.Series.Add(series);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void CompetencyChartForm_SciCAT_Load(object sender, EventArgs e)
        {
            LoadCompetencyData();
        }
    }
}
