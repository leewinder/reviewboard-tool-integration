namespace Create_Review
{
    partial class CreateReview
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateReview));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_ReviewId = new System.Windows.Forms.TextBox();
            this.textBox_Summary = new System.Windows.Forms.TextBox();
            this.textBox_Description = new System.Windows.Forms.TextBox();
            this.textBox_Testing = new System.Windows.Forms.TextBox();
            this.textBox_JiraId = new System.Windows.Forms.TextBox();
            this.checkedListBox_ReviewGroups = new System.Windows.Forms.CheckedListBox();
            this.toolTipDefault = new System.Windows.Forms.ToolTip(this.components);
            this.button_RefreshGroups = new System.Windows.Forms.Button();
            this.button_CreateReview = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox_ReviewLevel = new System.Windows.Forms.ComboBox();
            this.checkBox_CopiesAsAdds = new System.Windows.Forms.CheckBox();
            this.checkBox_KeepArtifacts = new System.Windows.Forms.CheckBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reviewboardAuthenticationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reviewDiffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filesForReviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox_RefreshingGroups = new System.Windows.Forms.PictureBox();
            this.pictureBox_RaisingReview = new System.Windows.Forms.PictureBox();
            this.groupBox_Options = new System.Windows.Forms.GroupBox();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RefreshingGroups)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RaisingReview)).BeginInit();
            this.groupBox_Options.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Review ID";
            this.toolTipDefault.SetToolTip(this.label1, "The ID of the review this is updating or leave blank for a new review");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Summary";
            this.toolTipDefault.SetToolTip(this.label2, "Summary of this change to describe the review (required)");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Description";
            this.toolTipDefault.SetToolTip(this.label3, "More detailed description of this change");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 230);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Testing Performed";
            this.toolTipDefault.SetToolTip(this.label4, "Any testing done on this commit");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 383);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Review Groups";
            this.toolTipDefault.SetToolTip(this.label5, "Which review groups should this review be posted to");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 356);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Jira ID";
            this.toolTipDefault.SetToolTip(this.label6, "Comma seperated list of Jira IDs this change refers \r\nto.  Can be empty if no Jir" +
        "a is referenced.\r\n");
            // 
            // textBox_ReviewId
            // 
            this.textBox_ReviewId.Location = new System.Drawing.Point(75, 38);
            this.textBox_ReviewId.Name = "textBox_ReviewId";
            this.textBox_ReviewId.Size = new System.Drawing.Size(261, 20);
            this.textBox_ReviewId.TabIndex = 6;
            this.toolTipDefault.SetToolTip(this.textBox_ReviewId, "The ID of the review this is updating or leave blank for a new review");
            // 
            // textBox_Summary
            // 
            this.textBox_Summary.Location = new System.Drawing.Point(75, 71);
            this.textBox_Summary.Name = "textBox_Summary";
            this.textBox_Summary.Size = new System.Drawing.Size(261, 20);
            this.textBox_Summary.TabIndex = 7;
            this.toolTipDefault.SetToolTip(this.textBox_Summary, "Summary of this change to describe the review (required)");
            // 
            // textBox_Description
            // 
            this.textBox_Description.Location = new System.Drawing.Point(15, 120);
            this.textBox_Description.Multiline = true;
            this.textBox_Description.Name = "textBox_Description";
            this.textBox_Description.Size = new System.Drawing.Size(321, 98);
            this.textBox_Description.TabIndex = 8;
            this.toolTipDefault.SetToolTip(this.textBox_Description, "More detailed description of this change");
            // 
            // textBox_Testing
            // 
            this.textBox_Testing.Location = new System.Drawing.Point(15, 246);
            this.textBox_Testing.Multiline = true;
            this.textBox_Testing.Name = "textBox_Testing";
            this.textBox_Testing.Size = new System.Drawing.Size(321, 98);
            this.textBox_Testing.TabIndex = 9;
            this.toolTipDefault.SetToolTip(this.textBox_Testing, "Any testing done on this commit");
            // 
            // textBox_JiraId
            // 
            this.textBox_JiraId.Location = new System.Drawing.Point(75, 353);
            this.textBox_JiraId.Name = "textBox_JiraId";
            this.textBox_JiraId.Size = new System.Drawing.Size(261, 20);
            this.textBox_JiraId.TabIndex = 10;
            this.toolTipDefault.SetToolTip(this.textBox_JiraId, "Comma seperated list of Jira IDs this change refers \r\nto.  Can be empty if no Jir" +
        "a is referenced.");
            // 
            // checkedListBox_ReviewGroups
            // 
            this.checkedListBox_ReviewGroups.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkedListBox_ReviewGroups.CheckOnClick = true;
            this.checkedListBox_ReviewGroups.FormattingEnabled = true;
            this.checkedListBox_ReviewGroups.Location = new System.Drawing.Point(15, 403);
            this.checkedListBox_ReviewGroups.Name = "checkedListBox_ReviewGroups";
            this.checkedListBox_ReviewGroups.Size = new System.Drawing.Size(253, 62);
            this.checkedListBox_ReviewGroups.TabIndex = 11;
            this.toolTipDefault.SetToolTip(this.checkedListBox_ReviewGroups, "Which review groups should this review be posted to");
            // 
            // button_RefreshGroups
            // 
            this.button_RefreshGroups.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_RefreshGroups.BackgroundImage")));
            this.button_RefreshGroups.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_RefreshGroups.Location = new System.Drawing.Point(274, 403);
            this.button_RefreshGroups.Name = "button_RefreshGroups";
            this.button_RefreshGroups.Size = new System.Drawing.Size(62, 62);
            this.button_RefreshGroups.TabIndex = 12;
            this.toolTipDefault.SetToolTip(this.button_RefreshGroups, "Refresh the list of review groups");
            this.button_RefreshGroups.UseVisualStyleBackColor = true;
            this.button_RefreshGroups.Click += new System.EventHandler(this.button_RefreshGroups_Click);
            // 
            // button_CreateReview
            // 
            this.button_CreateReview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_CreateReview.BackgroundImage")));
            this.button_CreateReview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_CreateReview.Enabled = false;
            this.button_CreateReview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_CreateReview.Location = new System.Drawing.Point(17, 575);
            this.button_CreateReview.Name = "button_CreateReview";
            this.button_CreateReview.Size = new System.Drawing.Size(319, 59);
            this.button_CreateReview.TabIndex = 13;
            this.toolTipDefault.SetToolTip(this.button_CreateReview, "Generate the review and open the commit dialog");
            this.button_CreateReview.UseVisualStyleBackColor = true;
            this.button_CreateReview.Click += new System.EventHandler(this.button_CreateReview_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Request";
            this.toolTipDefault.SetToolTip(this.label7, "What level of code review is this commit going to raise");
            // 
            // comboBox_ReviewLevel
            // 
            this.comboBox_ReviewLevel.AllowDrop = true;
            this.comboBox_ReviewLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ReviewLevel.FormattingEnabled = true;
            this.comboBox_ReviewLevel.Location = new System.Drawing.Point(60, 18);
            this.comboBox_ReviewLevel.Name = "comboBox_ReviewLevel";
            this.comboBox_ReviewLevel.Size = new System.Drawing.Size(255, 21);
            this.comboBox_ReviewLevel.TabIndex = 1;
            this.toolTipDefault.SetToolTip(this.comboBox_ReviewLevel, "What level of code review is this commit going to raise");
            // 
            // checkBox_CopiesAsAdds
            // 
            this.checkBox_CopiesAsAdds.AutoSize = true;
            this.checkBox_CopiesAsAdds.Location = new System.Drawing.Point(9, 45);
            this.checkBox_CopiesAsAdds.Name = "checkBox_CopiesAsAdds";
            this.checkBox_CopiesAsAdds.Size = new System.Drawing.Size(129, 17);
            this.checkBox_CopiesAsAdds.TabIndex = 0;
            this.checkBox_CopiesAsAdds.Text = "Show Copies as Adds";
            this.toolTipDefault.SetToolTip(this.checkBox_CopiesAsAdds, "Posts the Review using the --svn-show-copies-as-adds=y option");
            this.checkBox_CopiesAsAdds.UseVisualStyleBackColor = true;
            // 
            // checkBox_KeepArtifacts
            // 
            this.checkBox_KeepArtifacts.AutoSize = true;
            this.checkBox_KeepArtifacts.Location = new System.Drawing.Point(9, 68);
            this.checkBox_KeepArtifacts.Name = "checkBox_KeepArtifacts";
            this.checkBox_KeepArtifacts.Size = new System.Drawing.Size(131, 17);
            this.checkBox_KeepArtifacts.TabIndex = 3;
            this.checkBox_KeepArtifacts.Text = "Keep Review Artifacts";
            this.toolTipDefault.SetToolTip(this.checkBox_KeepArtifacts, "Copies the review artifacts into \'...\\Documents\\Reviewboard\r\n Integration Tools\\R" +
        "eviews\' when the review is complete");
            this.checkBox_KeepArtifacts.UseVisualStyleBackColor = true;
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(348, 24);
            this.menuStrip.TabIndex = 14;
            this.menuStrip.Text = "menuStrip";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reviewboardAuthenticationToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // reviewboardAuthenticationToolStripMenuItem
            // 
            this.reviewboardAuthenticationToolStripMenuItem.Name = "reviewboardAuthenticationToolStripMenuItem";
            this.reviewboardAuthenticationToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.reviewboardAuthenticationToolStripMenuItem.Text = "Reviewboard Authentication";
            this.reviewboardAuthenticationToolStripMenuItem.Click += new System.EventHandler(this.reviewboardAuthenticationToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reviewDiffToolStripMenuItem,
            this.filesForReviewToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // reviewDiffToolStripMenuItem
            // 
            this.reviewDiffToolStripMenuItem.Name = "reviewDiffToolStripMenuItem";
            this.reviewDiffToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.reviewDiffToolStripMenuItem.Text = "Show Review Diff";
            this.reviewDiffToolStripMenuItem.Click += new System.EventHandler(this.reviewDiffToolStripMenuItem_Click);
            // 
            // filesForReviewToolStripMenuItem
            // 
            this.filesForReviewToolStripMenuItem.Name = "filesForReviewToolStripMenuItem";
            this.filesForReviewToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.filesForReviewToolStripMenuItem.Text = "Show Files In Review";
            this.filesForReviewToolStripMenuItem.Click += new System.EventHandler(this.filesForReviewToolStripMenuItem_Click);
            // 
            // pictureBox_RefreshingGroups
            // 
            this.pictureBox_RefreshingGroups.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_RefreshingGroups.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_RefreshingGroups.Image")));
            this.pictureBox_RefreshingGroups.InitialImage = null;
            this.pictureBox_RefreshingGroups.Location = new System.Drawing.Point(289, 419);
            this.pictureBox_RefreshingGroups.Name = "pictureBox_RefreshingGroups";
            this.pictureBox_RefreshingGroups.Size = new System.Drawing.Size(32, 32);
            this.pictureBox_RefreshingGroups.TabIndex = 15;
            this.pictureBox_RefreshingGroups.TabStop = false;
            // 
            // pictureBox_RaisingReview
            // 
            this.pictureBox_RaisingReview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_RaisingReview.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_RaisingReview.Image")));
            this.pictureBox_RaisingReview.InitialImage = null;
            this.pictureBox_RaisingReview.Location = new System.Drawing.Point(159, 588);
            this.pictureBox_RaisingReview.Name = "pictureBox_RaisingReview";
            this.pictureBox_RaisingReview.Size = new System.Drawing.Size(32, 32);
            this.pictureBox_RaisingReview.TabIndex = 16;
            this.pictureBox_RaisingReview.TabStop = false;
            // 
            // groupBox_Options
            // 
            this.groupBox_Options.Controls.Add(this.checkBox_KeepArtifacts);
            this.groupBox_Options.Controls.Add(this.label7);
            this.groupBox_Options.Controls.Add(this.comboBox_ReviewLevel);
            this.groupBox_Options.Controls.Add(this.checkBox_CopiesAsAdds);
            this.groupBox_Options.Location = new System.Drawing.Point(15, 474);
            this.groupBox_Options.Name = "groupBox_Options";
            this.groupBox_Options.Size = new System.Drawing.Size(321, 95);
            this.groupBox_Options.TabIndex = 17;
            this.groupBox_Options.TabStop = false;
            this.groupBox_Options.Text = "Review Options";
            // 
            // CreateReview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 641);
            this.Controls.Add(this.groupBox_Options);
            this.Controls.Add(this.pictureBox_RaisingReview);
            this.Controls.Add(this.pictureBox_RefreshingGroups);
            this.Controls.Add(this.button_CreateReview);
            this.Controls.Add(this.button_RefreshGroups);
            this.Controls.Add(this.checkedListBox_ReviewGroups);
            this.Controls.Add(this.textBox_JiraId);
            this.Controls.Add(this.textBox_Testing);
            this.Controls.Add(this.textBox_Description);
            this.Controls.Add(this.textBox_Summary);
            this.Controls.Add(this.textBox_ReviewId);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateReview";
            this.Text = "Create Review";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateReview_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RefreshingGroups)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RaisingReview)).EndInit();
            this.groupBox_Options.ResumeLayout(false);
            this.groupBox_Options.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_ReviewId;
        private System.Windows.Forms.TextBox textBox_Summary;
        private System.Windows.Forms.TextBox textBox_Description;
        private System.Windows.Forms.TextBox textBox_Testing;
        private System.Windows.Forms.TextBox textBox_JiraId;
        private System.Windows.Forms.CheckedListBox checkedListBox_ReviewGroups;
        private System.Windows.Forms.ToolTip toolTipDefault;
        private System.Windows.Forms.Button button_RefreshGroups;
        private System.Windows.Forms.Button button_CreateReview;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reviewboardAuthenticationToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox_RefreshingGroups;
        private System.Windows.Forms.PictureBox pictureBox_RaisingReview;
        private System.Windows.Forms.GroupBox groupBox_Options;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox_ReviewLevel;
        private System.Windows.Forms.CheckBox checkBox_CopiesAsAdds;
        private System.Windows.Forms.CheckBox checkBox_KeepArtifacts;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reviewDiffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filesForReviewToolStripMenuItem;
    }
}

