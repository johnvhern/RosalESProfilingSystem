using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RosalESProfilingSystem.Forms
{
    public partial class CompetenciesExpandedView : Form
    {
        private DataGridView originalGrid;
        public event Action DataUpdated;
        public CompetenciesExpandedView(DataGridView sourceGrid)
        {
            InitializeComponent();
            originalGrid = sourceGrid;

            dataGridView1.DataSource = originalGrid.DataSource;
        }

        public void SetData(object dataSource)
        {
            dataGridView1.DataSource = dataSource;
            dataGridView1.Columns[0].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn && e.RowIndex >= 0)
            {
                dataGridView1.EndEdit();

                // Reflect the checkbox state back to the original grid
                originalGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value =
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                // Raise the event to signal that data was updated
                DataUpdated?.Invoke();
            }
        }

    }
}
