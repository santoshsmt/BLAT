using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace LogCreator
{
    public partial class frmSplitFile : Form
    {
        public frmSplitFile()
        {
            InitializeComponent();
            txtBrowsFile.Text = frmMain.FileName;
            btnBrowseFil.Visible = false;
        }


        public FileStream fs;
        string mergeFolder;
        int chunks = 10;

        List<string> Packets = new List<string>();

        //Split file is stored in drive
        string SaveFileFolder = @"c:\SplitMerge\";

        private void brows_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.ShowDialog();
                txtBrowsFile.Text = openFileDialog1.FileName;

                fs = new FileStream(txtBrowsFile.Text, FileMode.Open, FileAccess.Read);
                int FileLength = (int)fs.Length / 1024;
                string name = Path.GetFileName(txtBrowsFile.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("EXCEPTION:" + ex, "EXCEPTION!!", MessageBoxButtons.OK);
            }
        }

        private void txtChunks_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception)
            {
            }
        }

        private void createDirectory()
        {
            try
            {
                string rootDir = @"C:\SplitMerge";
                // If directory does not exist, create it. 
                if (!Directory.Exists(rootDir))
                {
                    Directory.CreateDirectory(rootDir);
                }
            }
            catch (Exception)
            {
            }
        }


        private void btnSplit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtChunks.Text))
                    SplitFile(txtBrowsFile.Text, Convert.ToInt32(txtChunks.Text));
                else
                {
                    if (MessageBox.Show("No Chunks Specified!!! \n\nDo you want to continue with Default(10) chunks?", "No Chunks!!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        SplitFile(txtBrowsFile.Text, Convert.ToInt32(chunks));
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public bool SplitFile(string SourceFile, int nNoofFiles)
        {
            bool Split = false;
            try
            {
                createDirectory();
                FileStream fs = new FileStream(SourceFile, FileMode.Open, FileAccess.Read);
                int SizeofEachFile = (int)Math.Ceiling((double)fs.Length / nNoofFiles);
                if (SizeofEachFile <= 807201549)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    btnSplitFile.Visible = false;
                    for (int i = 0; i < nNoofFiles; i++)
                    {
                        string baseFileName = Path.GetFileNameWithoutExtension(SourceFile);
                        string Extension = Path.GetExtension(SourceFile);

                        FileStream outputFile = new FileStream(SaveFileFolder + "\\" + baseFileName + "." + i.ToString().PadLeft(5, Convert.ToChar("0")) + Extension, FileMode.Create, FileAccess.Write);

                        mergeFolder = Path.GetDirectoryName(SourceFile);

                        int bytesRead = 0;
                        byte[] buffer = new byte[SizeofEachFile];

                        if ((bytesRead = fs.Read(buffer, 0, SizeofEachFile)) > 0)
                        {
                            outputFile.Write(buffer, 0, bytesRead);
                            //outp.Write(buffer, 0, BytesRead);

                            string packet = baseFileName + "." + i.ToString().PadLeft(3, Convert.ToChar("0")) + Extension.ToString();
                            Packets.Add(packet);
                        }

                        outputFile.Close();

                    }
                    fs.Close();
                    Cursor.Current = Cursors.Default;
                    btnSplitFile.Visible = true;
                    MessageBox.Show("Files have been splitted and saved at location C:\\SplitMerge\\", "Files Splitted", MessageBoxButtons.OK);
                    //Application.Exit();
                    var mainForm = new frmMain();
                    this.Hide();
                    mainForm.Show();
                    //openInExplorer("C:\\SplitMerge\\");
                }
                else
                {
                    if (string.IsNullOrEmpty(txtChunks.Text))
                        MessageBox.Show("Please specify number of Chunks", "Chunks Required!!", MessageBoxButtons.OK);
                    else
                        MessageBox.Show("Not enough chunks!!\n Please add some more.!!", "Chunks Required!!", MessageBoxButtons.OK);

                    fs.Close();
                }
            }
            catch (Exception Ex)
            {
                throw new ArgumentException(Ex.Message);
            }

            return Split;
        }

        static void openInExplorer(string path)
        {
            try
            {
                string cmd = "explorer.exe";
                string arg = "/select, " + path;
                Process.Start(cmd, arg);
            }
            catch (Exception)
            {
            }
        }
    }
}

