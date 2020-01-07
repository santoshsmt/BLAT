namespace LogCreator
{
    partial class frmSplitFile
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtChunks = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSplitFile = new System.Windows.Forms.Button();
            this.btnBrowseFil = new System.Windows.Forms.Button();
            this.txtBrowsFile = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // txtChunks
            // 
            this.txtChunks.Location = new System.Drawing.Point(189, 36);
            this.txtChunks.Name = "txtChunks";
            this.txtChunks.Size = new System.Drawing.Size(37, 20);
            this.txtChunks.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "How many chunks required ? ";
            // 
            // btnSplitFile
            // 
            this.btnSplitFile.Location = new System.Drawing.Point(446, 74);
            this.btnSplitFile.Name = "btnSplitFile";
            this.btnSplitFile.Size = new System.Drawing.Size(75, 23);
            this.btnSplitFile.TabIndex = 9;
            this.btnSplitFile.Text = "Split File";
            this.btnSplitFile.UseVisualStyleBackColor = true;
            this.btnSplitFile.Click += new System.EventHandler(this.btnSplit_Click);
            // 
            // btnBrowseFil
            // 
            this.btnBrowseFil.Location = new System.Drawing.Point(356, 74);
            this.btnBrowseFil.Name = "btnBrowseFil";
            this.btnBrowseFil.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFil.TabIndex = 8;
            this.btnBrowseFil.Text = "Browse File";
            this.btnBrowseFil.UseVisualStyleBackColor = true;
            // 
            // txtBrowsFile
            // 
            this.txtBrowsFile.Location = new System.Drawing.Point(41, 77);
            this.txtBrowsFile.Name = "txtBrowsFile";
            this.txtBrowsFile.Size = new System.Drawing.Size(299, 20);
            this.txtBrowsFile.TabIndex = 7;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // frmSplitFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 133);
            this.Controls.Add(this.txtChunks);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSplitFile);
            this.Controls.Add(this.btnBrowseFil);
            this.Controls.Add(this.txtBrowsFile);
            this.Name = "frmSplitFile";
            this.Text = "File Spiltter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtChunks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSplitFile;
        private System.Windows.Forms.Button btnBrowseFil;
        private System.Windows.Forms.TextBox txtBrowsFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}