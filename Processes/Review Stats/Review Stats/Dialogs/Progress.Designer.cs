namespace Review_Stats.Dialogs
{
    partial class Progress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Progress));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label_Progress = new System.Windows.Forms.Label();
            this.progressBar_Progress = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_ReportName = new System.Windows.Forms.TextBox();
            this.button_GenerateReport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(12, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(80, 80);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label_Progress
            // 
            this.label_Progress.AutoSize = true;
            this.label_Progress.Location = new System.Drawing.Point(98, 72);
            this.label_Progress.Name = "label_Progress";
            this.label_Progress.Size = new System.Drawing.Size(156, 13);
            this.label_Progress.TabIndex = 1;
            this.label_Progress.Text = "Identifying repository revisions...";
            this.label_Progress.Visible = false;
            // 
            // progressBar_Progress
            // 
            this.progressBar_Progress.Location = new System.Drawing.Point(101, 33);
            this.progressBar_Progress.MarqueeAnimationSpeed = 2;
            this.progressBar_Progress.Name = "progressBar_Progress";
            this.progressBar_Progress.Size = new System.Drawing.Size(375, 32);
            this.progressBar_Progress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar_Progress.TabIndex = 2;
            this.progressBar_Progress.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(98, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Report Name";
            // 
            // textBox_ReportName
            // 
            this.textBox_ReportName.Location = new System.Drawing.Point(174, 5);
            this.textBox_ReportName.Name = "textBox_ReportName";
            this.textBox_ReportName.Size = new System.Drawing.Size(302, 20);
            this.textBox_ReportName.TabIndex = 4;
            // 
            // button_GenerateReport
            // 
            this.button_GenerateReport.Location = new System.Drawing.Point(359, 33);
            this.button_GenerateReport.Name = "button_GenerateReport";
            this.button_GenerateReport.Size = new System.Drawing.Size(117, 52);
            this.button_GenerateReport.TabIndex = 5;
            this.button_GenerateReport.Text = "Generate Report";
            this.button_GenerateReport.UseVisualStyleBackColor = true;
            this.button_GenerateReport.Click += new System.EventHandler(this.button_GenerateReport_Click);
            // 
            // Progress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 91);
            this.Controls.Add(this.button_GenerateReport);
            this.Controls.Add(this.textBox_ReportName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar_Progress);
            this.Controls.Add(this.label_Progress);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Progress";
            this.Text = "Review Statistics";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label_Progress;
        private System.Windows.Forms.ProgressBar progressBar_Progress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_ReportName;
        private System.Windows.Forms.Button button_GenerateReport;
    }
}

