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
            this.WindowState = FormWindowState.Maximized;
        }

        private void btnShowReport_Click(object sender, EventArgs e)
        {
            advancedDataGridView1.DataSource = null;
            
            string path = Path.GetDirectoryName(Application.ExecutablePath) + @"\LogDataFiles\UpdatedLog.log";
            DataTable dt = FileManager.ConvertToDataTable(path);

            DataTable dt2 = dt.Select().Where(p => ((Convert.ToDateTime(p["Time"]) >= Convert.ToDateTime(dateTimePicker1.Value.ToString("hh:mm:ss tt"))) 
            && (Convert.ToDateTime(p["Time"]) <= Convert.ToDateTime(dateTimePicker2.Value.ToString("hh:mm:ss tt"))))
            ||(p["URL"].ToString().Contains(textBox1.Text.Trim()))).CopyToDataTable();

            IEnumerable<DataRow> orderedRows = dt2.AsEnumerable().OrderBy(r => r.Field<DateTime>("Time"));
            dt2 = orderedRows.CopyToDataTable();
            //dt2.DefaultView.Sort = "Time";
            advancedDataGridView1.DataSource = dt2;
            advancedDataGridView1.Columns["Time"].DefaultCellStyle.Format = "hh:mm:ss tt";
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
