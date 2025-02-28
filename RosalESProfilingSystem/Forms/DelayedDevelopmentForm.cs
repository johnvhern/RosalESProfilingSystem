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

namespace RosalESProfilingSystem.Forms
{
    public partial class DelayedDevelopmentForm: Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";

        private string _gradeLevel;
        private string _assessmentType;
        public DelayedDevelopmentForm(string gradeLevel, string assessmentType)
        {
            InitializeComponent();
            _gradeLevel = gradeLevel;
            _assessmentType = assessmentType;
        }

        private void DelayedDevelopmentForm_Load(object sender, EventArgs e)
        {
            LoadDevelopmentData();
        }

        private void LoadDevelopmentData()
        {
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string query = @"SELECT ln.LastName, ln.FirstName, ln.LRN, ln.GradeLevel FROM LearnersProfile ln WHERE ln.RMAClassification IN ('Low Emerging', 'High Emerging', 
                                'Developing', 'Transitioning') AND ln.GradeLevel = @GradeLevel AND ln.AssessmentType = @AssessmentType";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GradeLevel", _gradeLevel);
                    cmd.Parameters.AddWithValue("@AssessmentType", _assessmentType);
                    conn.Open();

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                       
                        adapter.Fill(dt);
                        
                    }
                    dataGridView1.DataSource = dt;
                }
            }
            this.Text = $"Delayed Development Learners - Grade {_gradeLevel} ({_assessmentType})";
        }
    }
}
