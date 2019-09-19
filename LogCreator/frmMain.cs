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
        List<string> FilePath = new List<string>();
        bool isMultipleFileSelected = false;
        public static List<string> result = new List<string>();
        public static string FileData = string.Empty;

        public frmMain()
        {
            InitializeComponent();
            btnMultipleFile.Visible = true;
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
                FilePath.Clear();
                isMultipleFileSelected = true;
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
                    }
                    txtFilePath.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        DataTable dt = null;
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text)
                || FilePath.Count <= 0)
            {
                MessageBox.Show("Please browse a file.");
                btnBrowse.Focus();
                return;
            }
            try
            {
                this.Cursor = Cursors.WaitCursor;
                string msg = string.Empty;
                foreach (string path in FilePath)
                {
                    //FileData = FileManager.ReadFile(path);
                    //dt = FileManager.ConvertToDataTable(path);
                    FileManager.ReadFile(path);
                    FileManager.ProcessData(Path.GetFileNameWithoutExtension(path));
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
                MessageBox.Show(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                FileManager.OutputFileName = string.Empty;
                txtFilePath.Text = string.Empty;
                isMultipleFileSelected = false;
                FilePath.Clear();
            }
        }
    }
}
