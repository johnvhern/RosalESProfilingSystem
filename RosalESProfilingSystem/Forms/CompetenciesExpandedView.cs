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
            dataGridView1.Columns["CompetencyId"].Visible = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn && e.RowIndex >= 0)
            {
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.ReadOnly = col.Name != "Mastered";
                }

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
