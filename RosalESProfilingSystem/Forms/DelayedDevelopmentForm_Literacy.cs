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
    public partial class DelayedDevelopmentForm_Literacy: Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";

        private string _schoolYear;
        private string _gradeLevel;
        private string _assessmentType;
        private string _selectedLanguage;
        private string _originalLanguage;
        public DelayedDevelopmentForm_Literacy(string schoolYear, string gradeLevel, string assessmentType, string selectedLanguage)
        {
            InitializeComponent();
            _schoolYear = schoolYear;
            _gradeLevel = gradeLevel;
            _assessmentType = assessmentType;
            _originalLanguage = selectedLanguage;
            selectedLanguageConversion(selectedLanguage);
        }

        private void selectedLanguageConversion(string selectedLanguage)
        {
            if (selectedLanguage == "Akeanon")
            {
                _selectedLanguage = "CRLAClassificationAkeanon";
            }else if (selectedLanguage == "Filipino")
            {
                _selectedLanguage = "CRLAClassificationFilipino";
            }else if (selectedLanguage == "English")
            {
                _selectedLanguage = "CRLAClassificationEnglish";
            }
        }

        private void DelayedDevelopmentForm_Literacy_Load(object sender, EventArgs e)
        {
            LoadDevelopmentData();
        }

        private void LoadDevelopmentData()
        {
            using (SqlConnection conn = new SqlConnection(dbConnection))
            {
                string query = $@"SELECT ln.LastName, ln.FirstName, ln.LRN, ln.GradeLevel FROM LearnersProfile ln WHERE ln.{_selectedLanguage} IN ('Low Emerging', 'High Emerging', 
                                'Developing', 'Transitioning') AND ln.SchoolYear = @SchoolYear AND ln.GradeLevel = @GradeLevel AND ln.AssessmentType = @AssessmentType";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SchoolYear", _schoolYear);
                    cmd.Parameters.AddWithValue("@GradeLevel", _gradeLevel);
                    cmd.Parameters.AddWithValue("@AssessmentType", _assessmentType);
                    cmd.Parameters.AddWithValue("@SelectedLanguage", _selectedLanguage);
                    conn.Open();

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {

                        adapter.Fill(dt);

                    }
                    dataGridView1.DataSource = dt;
                }
                this.Text = $"Delayed Development Learners in Literacy ({_originalLanguage}) - Grade {_gradeLevel} ({_assessmentType} - {_schoolYear})";
            }
        }
    }
}
