using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogCreator
{
    public partial class frmTestUI : Form
    {
        public frmTestUI()
        {
            InitializeComponent();
        }

        private void btnShowReport_Click(object sender, EventArgs e)
        {
            //dateTimePicker1.Value = DateTime.Now;
            string expression;
            expression = string.Empty;// string.Format("Time > #{0}# AND Time < #{1}#",
             //dateTimePicker1.Value.ToString("hh:mm:ss tt"), dateTimePicker2.Value.ToString("hh:mm:ss tt"));
            string path = Path.GetDirectoryName(Application.ExecutablePath) + @"\LogDataFiles\UpdatedLog_09_19_2019_23_36_41_117.log";
            DataTable dt = FileManager.ConvertToDataTable(path);
            

            DataRow[] foundRows;

            // Use the Select method to find all rows matching the filter.  
            foundRows = dt.Select(expression);

            DataTable dt2 = dt.Clone();
            //Import the Rows
            foreach (DataRow d in foundRows)
            {
                dt2.ImportRow(d);
            }
            //dgvReport.DataSource = FileManager.ConvertToDataTable(path);
            //BindingSource bs = new BindingSource();
            //bs.DataSource = dgvReport.DataSource;
            //bs.Filter = dgvReport.Columns[5].HeaderText.ToString() + " LIKE '%" + txtbxSearch.Text + "%'";
            advancedDataGridView1.DataSource = dt2;
            //dateTimePicker2.Value = DateTime.Now;
        }

        //private void advancedDataGridView1_FilterStringChanged(object sender, EventArgs e)
        //{
        //    this.BindingContext = this.advancedDataGridView1.FilterString.bin;
        //}

        private void advancedDataGridView1_FilterStringChanged(object sender, EventArgs e)
        {
            //var myDataGrid = sender as ADGV.AdvancedDataGridView;
            (advancedDataGridView1.DataSource as DataTable).DefaultView.RowFilter = advancedDataGridView1.FilterString;
        }
    }
}
