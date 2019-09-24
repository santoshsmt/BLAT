using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        //int _Height = 0;
        List<string> FilePath = new List<string>();
        bool isMultipleFileSelected = false;
        public static List<string> result = new List<string>();
        public static string FileData = string.Empty;
        ToolTip t1 = new ToolTip();
        DataTable dt = null;
        DataTable dt2 = null;
        IEnumerable<DataRow> orderedRows = null;

        public frmMain()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            // backgroundWorker1.WorkerSupportsCancellation = true;
            InitializeBackgroundWorker();
            btnMultipleFile.Visible = true;
            btnMultipleFile.Focus();
            buttonAnalyze.Enabled = false;
            label3.Visible = txtURL.Visible = false;
            progressBar1.Visible = false;
            this.WindowState = FormWindowState.Maximized;
            //  _Height = this.Height;
            buttonAnalyze.Left = dtpEnd.Right + 10;
            dtpStart.Value = DateTime.Now.Date;
            dtpEnd.Value = DateTime.Now.Date.AddHours(24).AddSeconds(-1);
            //  this.Height = buttonAnalyze.Top + buttonAnalyze.Height + 50;

        }


        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork +=
                new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker1_RunWorkerCompleted);
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
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.Visible = true;//Show progress bar
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
                progressBar1.Visible = false;
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
                FileManager.FileLines = null;
                FileManager.FileDataBytes = null;
                GC.Collect(0);
                GC.Collect(1);
                GC.Collect(2);

            }
        }

        private void buttonAnalyze_Click(object sender, EventArgs e)
        {

            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.Visible = true;//Show progress bar

            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }


           

            //dateTimePicker2.Value = DateTime.Now;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //Application.DoEvents();
                //Form frm = new frmTestUI();
                //frm.Show();
                //   this.Height = _Height;
                //this.Cursor = Cursors.WaitCursor;
                string directory_name = Path.GetDirectoryName(Application.ExecutablePath) + @"\LogDataFiles";
                string[] files = Directory.GetFiles(directory_name);
                string path = string.Empty;
                foreach (string file in files)
                {
                    path = file;
                    break;
                }
                files = null;

                dt = FileManager.ConvertToDataTable(path);
                dt2 = dt.Select().Where(p => ((Convert.ToDateTime(p["Time"]) >= Convert.ToDateTime(dtpStart.Value.ToString("hh:mm:ss tt")))
                && (Convert.ToDateTime(p["Time"]) <= Convert.ToDateTime(dtpEnd.Value.ToString("hh:mm:ss tt"))))).CopyToDataTable();

                orderedRows = dt2.AsEnumerable().OrderBy(r => r.Field<DateTime>("Time"));
                dt2 = orderedRows.CopyToDataTable();
                //dt2.DefaultView.Sort = "Time";

            }
            catch (Exception ex) { MessageBox.Show("Error occured during operation: ", ex.Message); }
            //finally
            //{
            //    dt.Clear();
            //    dt.Dispose();
            //    //dt2.Clear();
            //    //dt2.Dispose();
            //    orderedRows = null;

            //    FileManager.tbl = null;
            //    GC.Collect(0);
            //    GC.Collect(1);
            //    GC.Collect(2);
            //    MessageBox.Show("Total Memory: " + GC.GetTotalMemory(false));
            //}
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                
            }
            else if (e.Error != null)
            {
                
            }
            else
            {
                advancedDataGridView1.DataSource = dt2;

                advancedDataGridView1.Columns["Time"].DefaultCellStyle.Format = "hh:mm:ss tt";
                advancedDataGridView1.Columns["Message"].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
                advancedDataGridView1.Columns["Number"].Visible = false;
                //this.Cursor = Cursors.Default;
                progressBar1.Visible = false;//Show progress bar
                dt2 = null;
            }
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
                if (!string.IsNullOrEmpty(value))
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

        private void advancedDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void advancedDataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] != null && advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().ToLower().StartsWith("http://"))
                {
                    string value = advancedDataGridView1.Rows[e.RowIndex].Cells["URL"].Value.ToString();
                    Process.Start(value);
                }
            }
        }
    }
}
