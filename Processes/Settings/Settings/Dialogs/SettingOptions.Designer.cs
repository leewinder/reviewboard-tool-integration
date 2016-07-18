namespace Settings.Dialogs
{
    partial class SettingOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingOptions));
            this.button_ReviewboardAuthentication = new System.Windows.Forms.Button();
            this.button_ClearAllAuthentication = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_Logging = new System.Windows.Forms.CheckBox();
            this.button_Close = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.button_JiraAuthentication = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_ReviewboardAuthentication
            // 
            this.button_ReviewboardAuthentication.Location = new System.Drawing.Point(113, 19);
            this.button_ReviewboardAuthentication.Name = "button_ReviewboardAuthentication";
            this.button_ReviewboardAuthentication.Size = new System.Drawing.Size(119, 24);
            this.button_ReviewboardAuthentication.TabIndex = 0;
            this.button_ReviewboardAuthentication.Text = "Authenticate";
            this.toolTip.SetToolTip(this.button_ReviewboardAuthentication, "Authenticate against the Reviewboard server");
            this.button_ReviewboardAuthentication.UseVisualStyleBackColor = true;
            this.button_ReviewboardAuthentication.Click += new System.EventHandler(this.button_ReviewboardAuthentication_Click);
            // 
            // button_ClearAllAuthentication
            // 
            this.button_ClearAllAuthentication.Location = new System.Drawing.Point(113, 94);
            this.button_ClearAllAuthentication.Name = "button_ClearAllAuthentication";
            this.button_ClearAllAuthentication.Size = new System.Drawing.Size(119, 24);
            this.button_ClearAllAuthentication.TabIndex = 2;
            this.button_ClearAllAuthentication.Text = "Clear All";
            this.toolTip.SetToolTip(this.button_ClearAllAuthentication, "Clears the authentication data for all servers");
            this.button_ClearAllAuthentication.UseVisualStyleBackColor = true;
            this.button_ClearAllAuthentication.Click += new System.EventHandler(this.button_ClearAllAuthentication_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.button_JiraAuthentication);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button_ClearAllAuthentication);
            this.groupBox1.Controls.Add(this.button_ReviewboardAuthentication);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(246, 130);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server Authentication";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Authentication Data";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Reviewboard";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_Logging);
            this.groupBox2.Location = new System.Drawing.Point(12, 152);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(246, 52);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Debug Options";
            // 
            // checkBox_Logging
            // 
            this.checkBox_Logging.AutoSize = true;
            this.checkBox_Logging.Location = new System.Drawing.Point(9, 26);
            this.checkBox_Logging.Name = "checkBox_Logging";
            this.checkBox_Logging.Size = new System.Drawing.Size(100, 17);
            this.checkBox_Logging.TabIndex = 0;
            this.checkBox_Logging.Text = "Enable Logging";
            this.toolTip.SetToolTip(this.checkBox_Logging, "If enabled, logs will be created for all Reviewboard\r\nIntegration to help identif" +
        "y issues or problems");
            this.checkBox_Logging.UseVisualStyleBackColor = true;
            this.checkBox_Logging.CheckedChanged += new System.EventHandler(this.checkBox_Logging_CheckedChanged);
            // 
            // button_Close
            // 
            this.button_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Close.Location = new System.Drawing.Point(125, 212);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(133, 34);
            this.button_Close.TabIndex = 5;
            this.button_Close.Text = "Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Jira";
            // 
            // button_JiraAuthentication
            // 
            this.button_JiraAuthentication.Location = new System.Drawing.Point(113, 49);
            this.button_JiraAuthentication.Name = "button_JiraAuthentication";
            this.button_JiraAuthentication.Size = new System.Drawing.Size(119, 24);
            this.button_JiraAuthentication.TabIndex = 5;
            this.button_JiraAuthentication.Text = "Authenticate";
            this.toolTip.SetToolTip(this.button_JiraAuthentication, "Authenticate against the Reviewboard server");
            this.button_JiraAuthentication.UseVisualStyleBackColor = true;
            this.button_JiraAuthentication.Click += new System.EventHandler(this.button_JiraAuthentication_Click);
            // 
            // SettingOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 256);
            this.Controls.Add(this.button_Close);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingOptions";
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_ReviewboardAuthentication;
        private System.Windows.Forms.Button button_ClearAllAuthentication;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox_Logging;
        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_JiraAuthentication;
    }
}

