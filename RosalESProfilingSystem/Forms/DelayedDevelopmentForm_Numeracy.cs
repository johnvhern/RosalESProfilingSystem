using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using RosalESProfilingSystem.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RosalESProfilingSystem.Forms
{
    public partial class DelayedDevelopmentForm_Numeracy: Form
    {
        private string dbConnection = "Data Source=localhost\\sqlexpress;Initial Catalog=RosalES;Integrated Security=True;";

        private string _schoolYear;
        private string _gradeLevel;
        private string _assessmentType;
        public DelayedDevelopmentForm_Numeracy(string schoolYear, string gradeLevel, string assessmentType)
        {
            InitializeComponent();
            _schoolYear = schoolYear;
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
                                'Developing', 'Transitioning') AND ln.SchoolYear = @SchoolYear AND ln.GradeLevel = @GradeLevel AND ln.AssessmentType = @AssessmentType";

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
            this.Text = $"Delayed Development Learners in Numeracy - Grade {_gradeLevel} ({_assessmentType} - {_schoolYear})";
        }

        private async void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Save Delayed Development in Numeracy Report",
                FileName = $"Learners with Delayed Development in Numeracy Report - Grade {_gradeLevel} ({_assessmentType} {_schoolYear}).pdf"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("No data available to export.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (Form loadingForm = new Form())
                {
                    loadingForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    loadingForm.StartPosition = FormStartPosition.CenterScreen;
                    loadingForm.Size = new System.Drawing.Size(300, 70);
                    loadingForm.ControlBox = false;
                    loadingForm.Text = "Exporting...";

                    ProgressBar progressBar = new ProgressBar()
                    {
                        Style = ProgressBarStyle.Marquee,
                        Dock = DockStyle.Fill
                    };

                    loadingForm.Controls.Add(progressBar);
                    loadingForm.Show();

                    await Task.Run(() => ExportToPDF(saveFileDialog.FileName));

                    loadingForm.Close();
                }

                MessageBox.Show("Report exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ExportToPDF(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                using (PdfWriter writer = new PdfWriter(fs))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document document = new Document(pdf))
                {
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                    document.Add(new Paragraph($"Delayed Development Learners Report - Grade {_gradeLevel} {_assessmentType} - {_schoolYear}")
                        .SetFont(boldFont)
                        .SetFontSize(18)
                        .SetTextAlignment(TextAlignment.CENTER));

                    Table table = new Table(4).UseAllAvailableWidth();
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Last Name").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("First Name").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("LRN").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Grade Level").SetFont(boldFont)));

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            table.AddCell(row.Cells["LastName"].Value?.ToString() ?? "");
                            table.AddCell(row.Cells["FirstName"].Value?.ToString() ?? "");
                            table.AddCell(row.Cells["LRN"].Value?.ToString() ?? "");
                            table.AddCell(row.Cells["GradeLevel"].Value?.ToString() ?? "");
                        }
                    }

                    document.Add(table);
                }
            }
            catch (IOException ioEx)
            {
                MessageBox.Show($"File access error: {ioEx.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException uaEx)
            {
                MessageBox.Show($"Access denied: {uaEx.Message}", "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unknown error occurred: {ex}", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    }
