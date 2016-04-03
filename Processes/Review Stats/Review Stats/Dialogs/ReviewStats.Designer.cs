namespace Review_Stats.Dialogs
{
    partial class ReviewStats
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReviewStats));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.button_RefreshGroups = new System.Windows.Forms.Button();
            this.pictureBox_RefreshingGroups = new System.Windows.Forms.PictureBox();
            this.button_CreateReview = new System.Windows.Forms.Button();
            this.pictureBox_RaisingReview = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RefreshingGroups)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RaisingReview)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Start Revision";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "End Revision";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(88, 22);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(156, 21);
            this.comboBox1.TabIndex = 2;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(88, 49);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(156, 21);
            this.comboBox2.TabIndex = 3;
            // 
            // button_RefreshGroups
            // 
            this.button_RefreshGroups.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_RefreshGroups.BackgroundImage")));
            this.button_RefreshGroups.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_RefreshGroups.Location = new System.Drawing.Point(250, 22);
            this.button_RefreshGroups.Name = "button_RefreshGroups";
            this.button_RefreshGroups.Size = new System.Drawing.Size(48, 48);
            this.button_RefreshGroups.TabIndex = 13;
            this.button_RefreshGroups.UseVisualStyleBackColor = true;
            // 
            // pictureBox_RefreshingGroups
            // 
            this.pictureBox_RefreshingGroups.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_RefreshingGroups.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_RefreshingGroups.Image")));
            this.pictureBox_RefreshingGroups.InitialImage = null;
            this.pictureBox_RefreshingGroups.Location = new System.Drawing.Point(258, 30);
            this.pictureBox_RefreshingGroups.Name = "pictureBox_RefreshingGroups";
            this.pictureBox_RefreshingGroups.Size = new System.Drawing.Size(32, 32);
            this.pictureBox_RefreshingGroups.TabIndex = 16;
            this.pictureBox_RefreshingGroups.TabStop = false;
            // 
            // button_CreateReview
            // 
            this.button_CreateReview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_CreateReview.BackgroundImage")));
            this.button_CreateReview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_CreateReview.Enabled = false;
            this.button_CreateReview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_CreateReview.Location = new System.Drawing.Point(15, 76);
            this.button_CreateReview.Name = "button_CreateReview";
            this.button_CreateReview.Size = new System.Drawing.Size(283, 58);
            this.button_CreateReview.TabIndex = 17;
            this.button_CreateReview.UseVisualStyleBackColor = true;
            // 
            // pictureBox_RaisingReview
            // 
            this.pictureBox_RaisingReview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_RaisingReview.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_RaisingReview.Image")));
            this.pictureBox_RaisingReview.InitialImage = null;
            this.pictureBox_RaisingReview.Location = new System.Drawing.Point(139, 90);
            this.pictureBox_RaisingReview.Name = "pictureBox_RaisingReview";
            this.pictureBox_RaisingReview.Size = new System.Drawing.Size(32, 32);
            this.pictureBox_RaisingReview.TabIndex = 18;
            this.pictureBox_RaisingReview.TabStop = false;
            // 
            // ReviewStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 146);
            this.Controls.Add(this.pictureBox_RaisingReview);
            this.Controls.Add(this.button_CreateReview);
            this.Controls.Add(this.pictureBox_RefreshingGroups);
            this.Controls.Add(this.button_RefreshGroups);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReviewStats";
            this.Text = "Review Statistics";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RefreshingGroups)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RaisingReview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button button_RefreshGroups;
        private System.Windows.Forms.PictureBox pictureBox_RefreshingGroups;
        private System.Windows.Forms.Button button_CreateReview;
        private System.Windows.Forms.PictureBox pictureBox_RaisingReview;
    }
}

