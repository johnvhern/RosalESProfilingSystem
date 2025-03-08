using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.Kernel.Colors;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading.Tasks;

namespace RosalESProfilingSystem.Forms
{
    public partial class DelayedDevelopmentForm_Numeracy : Form
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
                string query = @"
                    SELECT 
                        ln.Sex,
                        ln.LastName, 
                        ln.FirstName, 
                        ln.LRN, 
                        ln.GradeLevel, 
                        ln.RMAClassification
                    FROM LearnersProfile ln 
                    WHERE ln.RMAClassification IN (
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
                FileName = $"Learners with Delayed Development in Numeracy - Grade {_gradeLevel} ({_assessmentType} {_schoolYear}).pdf"
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
                    string currentDate = DateTime.Now.ToString("MMMM dd, yyyy - hh:mm tt");

                    Paragraph dateParagraph = new Paragraph($"Generated on: {currentDate}")
                                .SetFontSize(9)
                                .SetTextAlignment(TextAlignment.RIGHT);

                    document.Add(dateParagraph);

                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    

                    Table table = new Table(2).UseAllAvailableWidth();

                    Paragraph titleText = new Paragraph("Rapid Mathematics Assessment Report")
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

                    Paragraph assessmentTitle = new Paragraph("RAPID MATHEMATICS ASSESSMENT (RMA)")
                        .SetFont(boldFont)
                        .SetFontSize(10)
                        .SetTextAlignment(TextAlignment.RIGHT);

                    Cell assessmentText = new Cell().Add(assessmentTitle)
                                .SetBorder(Border.NO_BORDER)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE);

                    table.AddCell(assessmentText);

                    document.Add(table);

                    document.Add(new Paragraph($"Delayed Development Learners in Numeracy - Grade {_gradeLevel} {_assessmentType} {_schoolYear}")
                        .SetFont(boldFont)
                        .SetFontSize(11)
                        .SetPaddingTop(20)
                        .SetTextAlignment(TextAlignment.CENTER));

                    // Group data by RMAClassification and Sex
                    var rmaData = ((DataTable)dataGridView1.DataSource)
                        .AsEnumerable()
                        .GroupBy(row => new
                        {
                            Classification = row["RMAClassification"].ToString(),
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
                    string chartPath = Path.Combine(Path.GetTempPath(), "RMAChart.png");
                    chart.SaveImage(chartPath, ChartImageFormat.Png);

                    // Add chart image to PDF
                    iText.Layout.Element.Image chartImage = new iText.Layout.Element.Image(iText.IO.Image.ImageDataFactory.Create(chartPath))
                        .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER)
                        .SetAutoScale(true);

                    document.Add(chartImage);

                    // Add Male Learners Table
                    document.Add(new Paragraph("Male Learners").SetFont(boldFont).SetFontSize(10));

                    Table maleTable = new Table(4).UseAllAvailableWidth().SetFontSize(11);
                    maleTable.AddHeaderCell(new Cell().Add(new Paragraph("Last Name").SetFont(boldFont)));
                    maleTable.AddHeaderCell(new Cell().Add(new Paragraph("First Name").SetFont(boldFont)));
                    maleTable.AddHeaderCell(new Cell().Add(new Paragraph("LRN").SetFont(boldFont)));
                    maleTable.AddHeaderCell(new Cell().Add(new Paragraph("RMA Classification").SetFont(boldFont)));

                    Table femaleTable = new Table(4).UseAllAvailableWidth().SetFontSize(11);
                    femaleTable.AddHeaderCell(new Cell().Add(new Paragraph("Last Name").SetFont(boldFont)));
                    femaleTable.AddHeaderCell(new Cell().Add(new Paragraph("First Name").SetFont(boldFont)));
                    femaleTable.AddHeaderCell(new Cell().Add(new Paragraph("LRN").SetFont(boldFont)));
                    femaleTable.AddHeaderCell(new Cell().Add(new Paragraph("RMA Classification").SetFont(boldFont)));

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            var targetTable = row.Cells["Sex"].Value?.ToString() == "M" ? maleTable : femaleTable;

                            targetTable.AddCell(row.Cells["LastName"].Value?.ToString() ?? "");
                            targetTable.AddCell(row.Cells["FirstName"].Value?.ToString() ?? "");
                            targetTable.AddCell(row.Cells["LRN"].Value?.ToString() ?? "");
                            targetTable.AddCell(row.Cells["RMAClassification"].Value?.ToString() ?? "");
                        }
                    }

                    document.Add(maleTable);

                    // Add Female Learners Table
                    document.Add(new Paragraph("Female Learners").SetFont(boldFont).SetFontSize(10));
                    document.Add(femaleTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
