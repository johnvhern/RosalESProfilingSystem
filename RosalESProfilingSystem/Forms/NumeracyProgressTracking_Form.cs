using MetroFramework.Controls;
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
    public partial class NumeracyProgressTracking_Form: Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";
        public NumeracyProgressTracking_Form()
        {
            InitializeComponent();
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

                if (string.IsNullOrEmpty(searchValue) || string.IsNullOrEmpty(schoolYear))
                {
                    MessageBox.Show("Please select a school year, and enter a search term.");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    conn.Open();
                    string query = $"SELECT DISTINCT GradeLevel, LastName, FirstName, MiddleName, LRN, Sex, Age FROM LearnersProfile WHERE {column} LIKE @SearchValue AND SchoolYear = @SchoolYear";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SearchValue", $"%{searchValue}%");
                        cmd.Parameters.AddWithValue("@SchoolYear", schoolYear);
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}");
            }
        }
    }
}
