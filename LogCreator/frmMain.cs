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

        public frmMain()
        {
            InitializeComponent();
            btnMultipleFile.Visible = true;
            btnMultipleFile.Focus();
            buttonAnalyze.Enabled = false;
            _Height = this.Height;
            this.Height = txtFilePath.Top + txtFilePath.Height + 50;
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


                this.Height = txtFilePath.Top + txtFilePath.Height + 50;
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
            this.Height = _Height;
            string directory_name = Path.GetDirectoryName(Application.ExecutablePath) + @"\LogDataFiles";
            string[] files = Directory.GetFiles(directory_name);
            string path = string.Empty;
            foreach (string file in files)
            {
                path = file;
                break;
            }

            advancedDataGridView1.DataSource = FileManager.ConvertToDataTable(path);
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
    }
}
