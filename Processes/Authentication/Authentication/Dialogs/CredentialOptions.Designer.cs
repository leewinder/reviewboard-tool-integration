namespace Authentication.Dialogs
{
    partial class CredentialOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CredentialOptions));
            this.button_ReviewboardAuthentication = new System.Windows.Forms.Button();
            this.button_ClearAllAuthentication = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_ReviewboardAuthentication
            // 
            this.button_ReviewboardAuthentication.Location = new System.Drawing.Point(12, 21);
            this.button_ReviewboardAuthentication.Name = "button_ReviewboardAuthentication";
            this.button_ReviewboardAuthentication.Size = new System.Drawing.Size(260, 36);
            this.button_ReviewboardAuthentication.TabIndex = 0;
            this.button_ReviewboardAuthentication.Text = "Reviewboard Authentication";
            this.button_ReviewboardAuthentication.UseVisualStyleBackColor = true;
            this.button_ReviewboardAuthentication.Click += new System.EventHandler(this.button_ReviewboardAuthentication_Click);
            // 
            // button_ClearAllAuthentication
            // 
            this.button_ClearAllAuthentication.Location = new System.Drawing.Point(12, 93);
            this.button_ClearAllAuthentication.Name = "button_ClearAllAuthentication";
            this.button_ClearAllAuthentication.Size = new System.Drawing.Size(260, 36);
            this.button_ClearAllAuthentication.TabIndex = 2;
            this.button_ClearAllAuthentication.Text = "Clear All Authentication Data";
            this.button_ClearAllAuthentication.UseVisualStyleBackColor = true;
            this.button_ClearAllAuthentication.Click += new System.EventHandler(this.button_ClearAllAuthentication_Click);
            // 
            // CredentialOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 139);
            this.Controls.Add(this.button_ClearAllAuthentication);
            this.Controls.Add(this.button_ReviewboardAuthentication);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CredentialOptions";
            this.Text = "Authentication";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_ReviewboardAuthentication;
        private System.Windows.Forms.Button button_ClearAllAuthentication;
    }
}

