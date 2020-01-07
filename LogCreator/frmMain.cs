using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        public static string FileName = string.Empty;
        ToolTip t1 = new ToolTip();
        DataTable dt = null;
        DataTable dt2 = null;
        IEnumerable<DataRow> orderedRows = null;

        public frmMain()
        {
            InitializeComponent();           
            backgroundWorker1.WorkerReportsProgress = true;
            InitializeBackgroundWorker();
            btnMultipleFile.Visible = true;
            btnMultipleFile.Focus();
            buttonAnalyze.Enabled = false;
            label3.Visible = txtURL.Visible = false;
            buttonAnalyze.Left = dtpEnd.Right + 10;
            dtpStart.Value = DateTime.Now.Date;
            dtpEnd.Value = DateTime.Now.Date.AddHours(24).AddSeconds(-1);

            ThreadSafe(() => lblPer.Text = "0 %");
            ThreadSafe(() => toolStripProgressBar1.Value = 0);

            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            int w = screen.Width;
            int h = screen.Height;
            this.Location = new Point((screen.Width - w) / 2, (screen.Height - h) / 2);
            this.Size = new Size(w, h);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
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
                    FileManager.TotalNoofFiles = openFileDialog.FileNames.Count();
                    foreach (string file in openFileDialog.FileNames)
                    {
                        FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                       // int SizeofFile = (int)Math.Ceiling((double)fs.Length);
                        if (FileManager.GetFileSize(file)>300)
                        {
                            if (MessageBox.Show("Selected file is more than 300 mb. Plesae split it into Chunks", "Chunks Required!!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                FileName = file;
                                var splitForm = new frmSplitFile();
                                this.Hide();
                                splitForm.Show();
                            }
                            else
                                return;
                        }
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
                toolStripProgressBar1.Value = 0;
                //progressBar1.Visible = true;//Show progress bar
                string directory_name = Path.GetDirectoryName(Application.ExecutablePath) + @"\LogDataFiles";
                string[] files = Directory.GetFiles(directory_name);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                Thread thr = new Thread(UpdatePrograssbar);
                thr.Start();
                this.Cursor = Cursors.WaitCursor;
                string msg = string.Empty;
                foreach (string path in FilePath)
                {
                    FileManager.NoofFilesCounter++;
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
                ThreadSafe(() => lblPer.Text = "0 %");
                ThreadSafe(() => toolStripProgressBar1.Value = 0);
                toolStripProgressBar1.Value = 0;
                buttonAnalyze.Enabled = true;
                btnSubmit.Enabled = false;
                txtFilePath.ReadOnly = true;
                FileManager.NoofFilesCounter = 0;
                FileManager.TotalNoofFiles = 0;
                FileManager.FileLineCounter = 0;
                FileManager.FileLines = null;
                FileManager.FileDataBytes = null;
                GC.Collect(0);
                GC.Collect(1);
                GC.Collect(2);

            }
        }

        private void buttonAnalyze_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ThreadSafe(() => this.Cursor = Cursors.WaitCursor);
                string directory_name = Path.GetDirectoryName(Application.ExecutablePath) + @"\LogDataFiles";
                string[] files = Directory.GetFiles(directory_name);
                string path = string.Empty;
                foreach (string file in files)
                {
                    path = file;
                    break;
                }
                files = null;
                Thread thr = new Thread(UpdatePrograssbar);
                thr.Start();
                dt = FileManager.ConvertToDataTable(path);
                dt2 = dt.Select().Where(p => ((Convert.ToDateTime(p["Time"]) >= Convert.ToDateTime(dtpStart.Value.ToString("hh:mm:ss tt")))
                && (Convert.ToDateTime(p["Time"]) <= Convert.ToDateTime(dtpEnd.Value.ToString("hh:mm:ss tt"))))).CopyToDataTable();

                orderedRows = dt2.AsEnumerable().OrderBy(r => r.Field<DateTime>("Time"));
                dt2 = orderedRows.CopyToDataTable();
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
                ThreadSafe(() => this.Cursor = Cursors.Default);
                ThreadSafe(() => lblPer.Text = "0 %");
                ThreadSafe(() => toolStripProgressBar1.Value = 0);
                dt2 = null;
            }
        }

        private void ThreadSafe(Action action)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    Invoke(action);
                }
                else
                {
                    action();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void UpdatePrograssbar()
        {
            try
            {
                int per = 0;
                int lastper = 0;
                int NumofChunk = 1;
                double delta = 0;
                if (FileManager.TotalNoofFiles > 1)
                {
                    NumofChunk = (int)Math.Ceiling(100.0 / FileManager.TotalNoofFiles);
                    delta = NumofChunk / 100.0;
                }
                else
                {
                    delta = NumofChunk;
                }
                while (per >= 0 && per < 100)
                {
                    double tmp_per = FileManager.FileLines != null && FileManager.FileLineCounter > 0
                        ? Math.Round(((double)FileManager.FileLineCounter / (double)FileManager.FileLines.Count), 4) * 100.0 : 0;
                    per = (int)(NumofChunk * (FileManager.NoofFilesCounter - 1)) + (int)(tmp_per * delta);
                    if (per > 100)
                    {
                        per = 100;
                    }
                    else if (per < 0)
                    {
                        per = 0;
                    }
                    if (lastper <= per)
                    {
                        ThreadSafe(() => lblPer.Text = per + " %");
                        ThreadSafe(() => toolStripProgressBar1.Value = per);
                        lastper = per;
                    }
                    Thread.Sleep(25);
                }
            }
            catch (Exception ex)
            {
                //
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

        private void btnFileSplitter_Click(object sender, EventArgs e)
        {
            var splitForm = new frmSplitFile();
            splitForm.Show();
        }
    }
}
