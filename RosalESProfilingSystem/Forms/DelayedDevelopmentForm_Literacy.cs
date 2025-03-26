using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
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
using System.Windows.Forms.DataVisualization.Charting;

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
            this.ShowInTaskbar = false;
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
                string query = $@"
                                SELECT 
                                    ln.Sex,
                                    ln.LastName, 
                                    ln.FirstName, 
                                    ln.LRN, 
                                    ln.GradeLevel,       
                                    ln.{_selectedLanguage} AS '{_selectedLanguage}'
                                FROM LearnersProfile ln 
                                WHERE ln.{_selectedLanguage} IN (
                                    'Low Emerging', 
                                    'High Emerging', 
                                    'Developing', 
                                    'Transitioning'
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

        private List<(string LastName, string FirstName, string LRN, string Sex, string Classification)> GetGradeReadyLearners(string schoolYear, string gradeLevel, string assessmentType, string selectedLanguage)
        {
            List<(string LastName, string FirstName, string LRN, string Sex, string Classification)> gradeReadyLearners = new List<(string, string, string, string, string)>();

            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    connection.Open();
                    string query = $@"
                                    SELECT LastName, FirstName, LRN, Sex,  {selectedLanguage} AS {selectedLanguage}
                                    FROM LearnersProfile 
                                    WHERE {selectedLanguage} = 'Grade Ready' AND SchoolYear = @SchoolYear AND GradeLevel = @GradeLevel AND AssessmentType = @AssessmentType";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SchoolYear", schoolYear);
                        command.Parameters.AddWithValue("@GradeLevel", gradeLevel);
                        command.Parameters.AddWithValue("@AssessmentType", assessmentType);
                        command.Parameters.AddWithValue("@SelectedLanguage", selectedLanguage);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                gradeReadyLearners.Add((
                                    reader["LastName"].ToString(),
                                    reader["FirstName"].ToString(),
                                    reader["LRN"].ToString(),
                                    reader["Sex"].ToString(),
                                    reader[$"{selectedLanguage}"].ToString()
                                ));

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return gradeReadyLearners;
        }

        private async void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Save Delayed Development in Literacy Report",
                FileName = $"Learners with Delayed Development in Literacy ({_originalLanguage}) - Grade {_gradeLevel} ({_assessmentType} {_schoolYear}).pdf"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string schoolYear = _schoolYear;
                string gradeLevel = _gradeLevel;
                string assessmentType = _assessmentType;
                string selectedLanguage = _selectedLanguage;

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

                    await Task.Run(() => ExportToPDF(saveFileDialog.FileName, schoolYear, gradeLevel, assessmentType, selectedLanguage));

                    loadingForm.Close();
                }

                MessageBox.Show("Report exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ActivityLogger logger = new ActivityLogger();
                logger.LogActivity(LoggedUser.Username, $"Exported Learners with Delayed Development in Literacy ({_originalLanguage}) - Grade {_gradeLevel} ({_assessmentType} {_schoolYear})");
            }
        }

        private void ExportToPDF(string fileName, string schoolYear, string gradeLevel, string assessmentType, string selectedLanguage)
        {
            var gradeReadyLearners = GetGradeReadyLearners(schoolYear, gradeLevel, assessmentType, selectedLanguage);
            int lowEmergingCount = 0, highEmergingCount = 0, developingCount = 0, transitioningCount = 0, gradeReadyCount = 0, totalLearners = 0;
            try
            {

                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                    string query = $@"
                                SELECT 
                                    COUNT(CASE WHEN {_selectedLanguage} = 'Low Emerging' THEN 1 END) AS LowEmerging,
                                    COUNT(CASE WHEN {_selectedLanguage} = 'High Emerging' THEN 1 END) AS HighEmerging,
                                    COUNT(CASE WHEN {_selectedLanguage} = 'Developing' THEN 1 END) AS Developing,
                                    COUNT(CASE WHEN {_selectedLanguage} = 'Transitioning' THEN 1 END) AS Transitioning,
                                    COUNT(CASE WHEN {_selectedLanguage} = 'Grade Ready' THEN 1 END) AS GradeReady
                                FROM LearnersProfile WHERE SchoolYear = @SchoolYear AND GradeLevel = @GradeLevel AND AssessmentType = @AssessmentType";

                    using (SqlCommand command = new SqlCommand(query, conn))
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        command.Parameters.AddWithValue("@SchoolYear", _schoolYear);
                        command.Parameters.AddWithValue("@GradeLevel", _gradeLevel);
                        command.Parameters.AddWithValue("@AssessmentType", _assessmentType);
                        command.Parameters.AddWithValue("@SelectedLanguage", _selectedLanguage);
                        conn.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                lowEmergingCount = reader.GetInt32(0);
                                highEmergingCount = reader.GetInt32(1);
                                developingCount = reader.GetInt32(2);
                                transitioningCount = reader.GetInt32(3);
                                gradeReadyCount = reader.GetInt32(4);
                                totalLearners = lowEmergingCount + highEmergingCount + developingCount + transitioningCount + gradeReadyCount;
                            }
                        }


                    }
                }

                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                using (PdfWriter writer = new PdfWriter(fs))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document document = new Document(pdf))
                {
                    string currentDate = DateTime.Now.ToString("MMMM dd, yyyy - hh:mm tt");

                    Paragraph dateParagraph = new Paragraph($"Generated on: {currentDate}")
                                .SetFontSize(9)
                                .SetTextAlignment(TextAlignment.RIGHT);

                    document.Add(dateParagraph);

                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                    Table table = new Table(2).UseAllAvailableWidth();

                    Paragraph titleText = new Paragraph("Reading Literacy Assessment Report")
                        .SetFont(boldFont)
                        .SetFontSize(10)
                        .SetPaddingLeft(3)
                        .SetBackgroundColor(ColorConstants.GRAY)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetTextAlignment(TextAlignment.LEFT);

                    Cell textCell = new Cell().Add(titleText)
                            .SetBorder(Border.NO_BORDER)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE);

                    table.AddCell(textCell);

                    Paragraph assessmentTitle = new Paragraph("COMPREHENSIVE RAPID LITERACY ASSESSMENT (CRLA)")
                        .SetFont(boldFont)
                        .SetFontSize(10)
                        .SetTextAlignment(TextAlignment.RIGHT);

                    Cell assessmentText = new Cell().Add(assessmentTitle)
                                .SetBorder(Border.NO_BORDER)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE);

                    table.AddCell(assessmentText);

                    document.Add(table);

                    document.Add(new Paragraph($"Comprehensive Rapid Literacy Assessment Result ({_originalLanguage}) - Grade {_gradeLevel} {_assessmentType} {_schoolYear}")
                             .SetFont(boldFont)
                             .SetFontSize(11)
                             .SetPaddingTop(20)
                             .SetTextAlignment(TextAlignment.CENTER));

                    Table summaryTable = new Table(2).UseAllAvailableWidth().SetFontSize(10);

                    summaryTable.AddCell(new Cell().Add(new Paragraph("Low Emerging")).SetFont(boldFont));
                    summaryTable.AddCell(new Cell().Add(new Paragraph(lowEmergingCount.ToString())));

                    summaryTable.AddCell(new Cell().Add(new Paragraph("High Emerging")).SetFont(boldFont));
                    summaryTable.AddCell(new Cell().Add(new Paragraph(highEmergingCount.ToString())));

                    summaryTable.AddCell(new Cell().Add(new Paragraph("Developing")).SetFont(boldFont));
                    summaryTable.AddCell(new Cell().Add(new Paragraph(developingCount.ToString())));

                    summaryTable.AddCell(new Cell().Add(new Paragraph("Transitioning")).SetFont(boldFont));
                    summaryTable.AddCell(new Cell().Add(new Paragraph(transitioningCount.ToString())));

                    summaryTable.AddCell(new Cell().Add(new Paragraph("Grade Ready")).SetFont(boldFont));
                    summaryTable.AddCell(new Cell().Add(new Paragraph(gradeReadyCount.ToString())));

                    summaryTable.AddCell(new Cell().Add(new Paragraph("Total Learners Assessed")).SetFont(boldFont));
                    summaryTable.AddCell(new Cell().Add(new Paragraph(totalLearners.ToString())).SetFont(boldFont));

                    document.Add(summaryTable);

                    if (gradeReadyLearners.Count > 0)
                    {
                        document.Add(new Paragraph("Grade Ready Learners in Literacy").SetFont(boldFont).SetFontSize(11).SetTextAlignment(TextAlignment.CENTER).SetPaddingTop(5).SetPaddingBottom(5));

                        Table gradeReadyTable = new Table(5).UseAllAvailableWidth().SetFontSize(10);
                        gradeReadyTable.AddHeaderCell(new Cell().Add(new Paragraph("Last Name").SetFont(boldFont)));
                        gradeReadyTable.AddHeaderCell(new Cell().Add(new Paragraph("First Name").SetFont(boldFont)));
                        gradeReadyTable.AddHeaderCell(new Cell().Add(new Paragraph("LRN").SetFont(boldFont)));
                        gradeReadyTable.AddHeaderCell(new Cell().Add(new Paragraph("Sex").SetFont(boldFont)));
                        gradeReadyTable.AddHeaderCell(new Cell().Add(new Paragraph("Age").SetFont(boldFont)));

                        foreach (var learner in gradeReadyLearners)
                        {
                            gradeReadyTable.AddCell(learner.LastName);
                            gradeReadyTable.AddCell(learner.FirstName);
                            gradeReadyTable.AddCell(learner.LRN);
                            gradeReadyTable.AddCell(learner.Sex);
                            gradeReadyTable.AddCell(learner.Classification);
                        }
                        document.Add(gradeReadyTable);
                    }

                    document.Add(new Paragraph("Learners with Delayed Development in Literacy")
                             .SetFont(boldFont)
                             .SetFontSize(11)
                             .SetPaddingTop(20)
                             .SetTextAlignment(TextAlignment.CENTER));

                    // Group data by RMAClassification and Sex
                    var rmaData = ((DataTable)dataGridView1.DataSource)
                        .AsEnumerable()
                        .GroupBy(row => new
                        {
                            Classification = row[$"{_selectedLanguage}"].ToString(),
                            Sex = row["Sex"].ToString()
                        })
                        .Select(group => new
                        {
                            group.Key.Classification,
                            group.Key.Sex,
                            Count = group.Count()
                        })
                        .ToList();

                    int totalStudents = rmaData.Sum(x => x.Count);

                    // Create chart
                    Chart chart = new Chart
                    {
                        Width = 600,
                        Height = 400
                    };

                    ChartArea chartArea = new ChartArea();
                    chart.ChartAreas.Add(chartArea);

                    Series maleSeries = new Series("Male")
                    {
                        ChartType = SeriesChartType.Column,
                        IsValueShownAsLabel = true
                    };

                    Series femaleSeries = new Series("Female")
                    {
                        ChartType = SeriesChartType.Column,
                        IsValueShownAsLabel = true
                    };

                    // Get distinct RMA classifications
                    var classifications = rmaData.Select(x => x.Classification).Distinct().ToList();

                    foreach (var classification in classifications)
                    {
                        int maleCount = rmaData.FirstOrDefault(x => x.Classification == classification && x.Sex == "M")?.Count ?? 0;
                        int femaleCount = rmaData.FirstOrDefault(x => x.Classification == classification && x.Sex == "F")?.Count ?? 0;

                        double malePercentage = totalStudents > 0 ? (double)maleCount / totalStudents * 100 : 0;
                        double femalePercentage = totalStudents > 0 ? (double)femaleCount / totalStudents * 100 : 0;

                        maleSeries.Points.AddXY(classification, maleCount);
                        maleSeries.Points.Last().Label = $"{maleCount} ({malePercentage:F2}%)";

                        femaleSeries.Points.AddXY(classification, femaleCount);
                        femaleSeries.Points.Last().Label = $"{femaleCount} ({femalePercentage:F2}%)";
                    }

                    chart.Series.Add(maleSeries);
                    chart.Series.Add(femaleSeries);

                    chart.Legends.Add(new Legend("Legend")
                    {
                        Docking = Docking.Bottom
                    });

                    // Save chart image
                    string chartPath = Path.Combine(Path.GetTempPath(), "CRLAChart.png");
                    chart.SaveImage(chartPath, ChartImageFormat.Png);

                    // Add chart image to PDF
                    iText.Layout.Element.Image chartImage = new iText.Layout.Element.Image(iText.IO.Image.ImageDataFactory.Create(chartPath))
                        .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER)
                        .SetAutoScale(true);

                    document.Add(chartImage);

                    // Add Male Learners Table

                    Table maleTable = new Table(4).UseAllAvailableWidth().SetFontSize(11);
                    maleTable.AddHeaderCell(new Cell().Add(new Paragraph("Last Name").SetFont(boldFont)));
                    maleTable.AddHeaderCell(new Cell().Add(new Paragraph("First Name").SetFont(boldFont)));
                    maleTable.AddHeaderCell(new Cell().Add(new Paragraph("LRN").SetFont(boldFont)));
                    maleTable.AddHeaderCell(new Cell().Add(new Paragraph($"CRLA Classification ({_originalLanguage})").SetFont(boldFont)));

                    Table femaleTable = new Table(4).UseAllAvailableWidth().SetFontSize(11);
                    femaleTable.AddHeaderCell(new Cell().Add(new Paragraph("Last Name").SetFont(boldFont)));
                    femaleTable.AddHeaderCell(new Cell().Add(new Paragraph("First Name").SetFont(boldFont)));
                    femaleTable.AddHeaderCell(new Cell().Add(new Paragraph("LRN").SetFont(boldFont)));
                    femaleTable.AddHeaderCell(new Cell().Add(new Paragraph($"CRLA Classification ({_originalLanguage})").SetFont(boldFont)));

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            var targetTable = row.Cells["Sex"].Value?.ToString() == "M" ? maleTable : femaleTable;

                            targetTable.AddCell(row.Cells["LastName"].Value?.ToString() ?? "");
                            targetTable.AddCell(row.Cells["FirstName"].Value?.ToString() ?? "");
                            targetTable.AddCell(row.Cells["LRN"].Value?.ToString() ?? "");
                            targetTable.AddCell(row.Cells[$"{_selectedLanguage}"].Value?.ToString() ?? "");
                        }
                    }

                    if (maleTable.GetNumberOfRows() > 0)
                    {
                        document.Add(new Paragraph("Male Learners").SetFont(boldFont).SetFontSize(10));
                        document.Add(maleTable);
                    }

                    if (femaleTable.GetNumberOfRows() > 0)
                    {
                        // Add Female Learners Table
                        document.Add(new Paragraph("Female Learners").SetFont(boldFont).SetFontSize(10));
                        document.Add(femaleTable);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    }

