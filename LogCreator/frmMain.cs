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
    public partial class frmMain : Form
    {
        int _Height = 0;
        List<string> FilePath = new List<string>();
        bool isMultipleFileSelected = false;
        public static List<string> result = new List<string>();
        public static string FileData = string.Empty;
        ToolTip t1 = new ToolTip();

        public frmMain()
        {
            InitializeComponent();
            btnMultipleFile.Visible = true;
            btnMultipleFile.Focus();
            buttonAnalyze.Enabled = false;
            label3.Visible = txtURL.Visible = false;
            this.WindowState = FormWindowState.Maximized;
          //  _Height = this.Height;
            buttonAnalyze.Left = dtpEnd.Right + 10;
            dtpStart.Value = DateTime.Now.Date;
            dtpEnd.Value = DateTime.Now.Date.AddHours(24).AddSeconds(-1);
          //  this.Height = buttonAnalyze.Top + buttonAnalyze.Height + 50;

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {

                FilePath.Clear();
                isMultipleFileSelected = false;
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Browse a Log File",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    DefaultExt = "log",
                    Filter = "Log files (*.log)|*.log",
                    FilterIndex = 2,
                    RestoreDirectory = true,
                    ReadOnlyChecked = true,
                    ShowReadOnly = true
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FilePath.Add(openFileDialog.FileName);
                    txtFilePath.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnMultipleFile_Click(object sender, EventArgs e)
        {
            try
            {
                var files = Directory.GetFiles(Application.StartupPath + @"\\LogDataFiles").FirstOrDefault();
                if (files != null)
                {
                    File.Delete(files);
                }


               // this.Height = buttonAnalyze.Top + buttonAnalyze.Height + 50;
                advancedDataGridView1.DataSource = null;
                FilePath.Clear();
                txtFilePath.Clear();
                isMultipleFileSelected = true;
                btnSubmit.Enabled = true;
                buttonAnalyze.Enabled = false;
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Browse a Log File",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    DefaultExt = "log",
                    Filter = "Log files (*.log)|*.log",
                    FilterIndex = 2,
                    RestoreDirectory = true,
                    ReadOnlyChecked = true,
                    ShowReadOnly = true,
                    Multiselect = true
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in openFileDialog.FileNames)
                    {
                        FilePath.Add(file);
                        txtFilePath.Text += Path.GetFileName(file) + Environment.NewLine;
                    }
                    Size size = TextRenderer.MeasureText(txtFilePath.Text, txtFilePath.Font);
                    txtFilePath.Height = size.Height + 20;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text)
                || FilePath.Count <= 0)
            {
                MessageBox.Show("Please browse a file.");
                btnMultipleFile.Focus();
                return;
            }
            try
            {
                string directory_name = Path.GetDirectoryName(Application.ExecutablePath) + @"\LogDataFiles";
                string[] files = Directory.GetFiles(directory_name);
                foreach (string file in files)
                {
                    File.Delete(file);
                }

                this.Cursor = Cursors.WaitCursor;
                string msg = string.Empty;
                foreach (string path in FilePath)
                {
                    //FileData = FileManager.ReadFile(path);
                    //dt = FileManager.ConvertToDataTable(path);
                    FileManager.ReadFile(path);
                    bool result = await FileManager.ProcessData(Path.GetFileNameWithoutExtension(path));
                    if (FileManager.FileDataBytes != null)
                    {
                        //byte[] data = Encoding.UTF8.GetBytes(text);
                        if (FileManager.CreateLogFile(isMultipleFileSelected))
                        {
                            msg = "File Created...";
                        }
                        else
                        {
                            msg = "Something wrong please try again.";
                        }
                    }
                    else
                    {
                        msg = "Unable to read data from file.";
                    }
                }
                MessageBox.Show(msg, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                buttonAnalyze.Enabled = true;
                btnSubmit.Enabled = false;
                txtFilePath.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                InitializeComponent();
            }
            finally
            {
                Cursor = Cursors.Default;
                FileManager.OutputFileName = string.Empty;
                // txtFilePath.Text = string.Empty;
                isMultipleFileSelected = false;
                FilePath.Clear();
            }
        }

        private void buttonAnalyze_Click(object sender, EventArgs e)
        {
            //Form frm = new frmTestUI();
            //frm.Show();
            //   this.Height = _Height;
            this.Cursor = Cursors.WaitCursor;
            string directory_name = Path.GetDirectoryName(Application.ExecutablePath) + @"\LogDataFiles";
            string[] files = Directory.GetFiles(directory_name);
            string path = string.Empty;
            foreach (string file in files)
            {
                path = file;
                break;
            }

            DataTable dt = FileManager.ConvertToDataTable(path);
            DataTable dt2 = dt.Select().Where(p => ((Convert.ToDateTime(p["Time"]) >= Convert.ToDateTime(dtpStart.Value.ToString("hh:mm:ss tt")))
            && (Convert.ToDateTime(p["Time"]) <= Convert.ToDateTime(dtpEnd.Value.ToString("hh:mm:ss tt"))))).CopyToDataTable();

            IEnumerable<DataRow> orderedRows = dt2.AsEnumerable().OrderBy(r => r.Field<DateTime>("Time"));
            dt2 = orderedRows.CopyToDataTable();
            //dt2.DefaultView.Sort = "Time";
            advancedDataGridView1.DataSource = dt2;
            advancedDataGridView1.Columns["Time"].DefaultCellStyle.Format = "hh:mm:ss tt";
            advancedDataGridView1.Columns["Message"].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            advancedDataGridView1.Columns["Number"].Visible = false;
            this.Cursor = Cursors.Default;
            //dateTimePicker2.Value = DateTime.Now;
        }

        private void advancedDataGridView1_FilterStringChanged(object sender, EventArgs e)
        {
            (advancedDataGridView1.DataSource as DataTable).DefaultView.RowFilter = advancedDataGridView1.FilterString;
        }

        private void advancedDataGridView1_SortStringChanged(object sender, EventArgs e)
        {
            (advancedDataGridView1.DataSource as DataTable).DefaultView.Sort = advancedDataGridView1.SortString;
        }

        private void advancedDataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                string value = Convert.ToString(advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                if(!string.IsNullOrEmpty(value))
                    t1.Tag = value;
            }
            
        }

        private void advancedDataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string value = advancedDataGridView1.Rows[e.RowIndex].Cells["Message_type"].Value.ToString();
            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.ToLower();
                if (value.Contains("critical"))
                {
                    advancedDataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    return;
                }
                else if (value.Contains("warning"))
                {
                    advancedDataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow;
                    return;
                }
                else if (value.Contains("error"))
                {
                    advancedDataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Orange;
                    return;
                }
            }

            value = advancedDataGridView1.Rows[e.RowIndex].Cells["IsBlock"].Value.ToString();
            if (!string.IsNullOrWhiteSpace(value) && value.ToLower().Contains("true"))
            {
                advancedDataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                return;
            }
        }
    }
}
