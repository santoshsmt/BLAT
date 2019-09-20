namespace LogCreator
{
    partial class frmMain
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
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnMultipleFile = new System.Windows.Forms.Button();
            this.buttonAnalyze = new System.Windows.Forms.Button();
            this.advancedDataGridView1 = new ADGV.AdvancedDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.advancedDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtFilePath
            // 
            this.txtFilePath.BackColor = System.Drawing.Color.White;
            this.txtFilePath.Location = new System.Drawing.Point(150, 14);
            this.txtFilePath.MinimumSize = new System.Drawing.Size(0, 81);
            this.txtFilePath.Multiline = true;
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(299, 81);
            this.txtFilePath.TabIndex = 1;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(12, 43);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(132, 23);
            this.btnSubmit.TabIndex = 3;
            this.btnSubmit.Text = "Create log File";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnMultipleFile
            // 
            this.btnMultipleFile.Location = new System.Drawing.Point(12, 14);
            this.btnMultipleFile.Name = "btnMultipleFile";
            this.btnMultipleFile.Size = new System.Drawing.Size(132, 23);
            this.btnMultipleFile.TabIndex = 2;
            this.btnMultipleFile.Text = "Browse Multiple Files";
            this.btnMultipleFile.UseVisualStyleBackColor = true;
            this.btnMultipleFile.Click += new System.EventHandler(this.btnMultipleFile_Click);
            // 
            // buttonAnalyze
            // 
            this.buttonAnalyze.Location = new System.Drawing.Point(12, 72);
            this.buttonAnalyze.Name = "buttonAnalyze";
            this.buttonAnalyze.Size = new System.Drawing.Size(132, 23);
            this.buttonAnalyze.TabIndex = 4;
            this.buttonAnalyze.Text = "Analyze log file";
            this.buttonAnalyze.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonAnalyze.UseVisualStyleBackColor = true;
            this.buttonAnalyze.Click += new System.EventHandler(this.buttonAnalyze_Click);
            // 
            // advancedDataGridView1
            // 
            this.advancedDataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedDataGridView1.AutoGenerateContextFilters = true;
            this.advancedDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.advancedDataGridView1.DateWithTime = false;
            this.advancedDataGridView1.Location = new System.Drawing.Point(12, 117);
            this.advancedDataGridView1.Name = "advancedDataGridView1";
            this.advancedDataGridView1.Size = new System.Drawing.Size(923, 541);
            this.advancedDataGridView1.TabIndex = 5;
            this.advancedDataGridView1.TimeFilter = false;
            this.advancedDataGridView1.SortStringChanged += new System.EventHandler(this.advancedDataGridView1_SortStringChanged);
            this.advancedDataGridView1.FilterStringChanged += new System.EventHandler(this.advancedDataGridView1_FilterStringChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(947, 670);
            this.Controls.Add(this.advancedDataGridView1);
            this.Controls.Add(this.buttonAnalyze);
            this.Controls.Add(this.btnMultipleFile);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.txtFilePath);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Log Creator";
            ((System.ComponentModel.ISupportInitialize)(this.advancedDataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnMultipleFile;
        private System.Windows.Forms.Button buttonAnalyze;
        private ADGV.AdvancedDataGridView advancedDataGridView1;
    }
}

