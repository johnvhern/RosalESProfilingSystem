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
    public partial class DelayedDevelopmentForm_ScienceProficiency: Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";

        private string _schoolYear;
        private string _gradeLevel;
        private string _assessmentType;
        public DelayedDevelopmentForm_ScienceProficiency(string schoolYear, string gradeLevel, string assessmentType)
        {
            InitializeComponent();
            _schoolYear = schoolYear;
            _gradeLevel = gradeLevel;
            _assessmentType = assessmentType;
        }

        private void DelayedDevelopmentForm_ScienceProficiency_Load(object sender, EventArgs e)
        {
            LoadDevelopmentData();
        }

        private void LoadDevelopmentData()
        {
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string query = @"
                                SELECT 
                                    ln.Sex,
                                    ln.LastName, 
                                    ln.FirstName, 
                                    ln.LRN, 
                                    ln.GradeLevel, 
                                    ln.ClassificationLevel 
                                FROM LearnersProfileScience ln 
                                WHERE ln.ClassificationLevel IN (
                                    'No Proficiency at All', 
                                    'Poor Proficiency', 
                                    'Weak Proficiency', 
                                    'Satisfactory Proficiency', 
                                    'Good Proficiency', 
                                    'Very Good Proficiency'
                                ) 
                                AND ln.SchoolYear = @SchoolYear 
                                AND ln.GradeLevel = @GradeLevel 
                                AND ln.AssessmentType = @AssessmentType
                                ORDER BY ln.Sex, ln.LastName, ln.FirstName";


                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolYear", _schoolYear);
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
            this.Text = $"Delayed Development Learners in Science Proficiency - Grade {_gradeLevel} ({_assessmentType} - {_schoolYear})";
        }
    }
}
