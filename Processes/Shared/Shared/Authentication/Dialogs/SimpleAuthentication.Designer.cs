using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RB_Tools.Shared.Authentication.Dialogs
{
    partial class SimpleAuthentication
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleAuthentication));
            this.pictureBox_Authenticating = new System.Windows.Forms.PictureBox();
            this.button_Authenticate = new System.Windows.Forms.Button();
            this.button_ClearAuthenitcation = new System.Windows.Forms.Button();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.textBox_User = new System.Windows.Forms.TextBox();
            this.textBox_Server = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Authenticating)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_Authenticating
            // 
            this.pictureBox_Authenticating.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_Authenticating.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_Authenticating.Image")));
            this.pictureBox_Authenticating.InitialImage = null;
            this.pictureBox_Authenticating.Location = new System.Drawing.Point(285, 115);
            this.pictureBox_Authenticating.Name = "pictureBox_Authenticating";
            this.pictureBox_Authenticating.Size = new System.Drawing.Size(32, 32);
            this.pictureBox_Authenticating.TabIndex = 18;
            this.pictureBox_Authenticating.TabStop = false;
            // 
            // button_Authenticate
            // 
            this.button_Authenticate.Location = new System.Drawing.Point(206, 115);
            this.button_Authenticate.Name = "button_Authenticate";
            this.button_Authenticate.Size = new System.Drawing.Size(190, 32);
            this.button_Authenticate.TabIndex = 17;
            this.button_Authenticate.Text = "Authenticate";
            this.button_Authenticate.UseVisualStyleBackColor = true;
            this.button_Authenticate.Click += new System.EventHandler(this.button_Authenticate_Click);
            // 
            // button_ClearAuthenitcation
            // 
            this.button_ClearAuthenitcation.Location = new System.Drawing.Point(11, 115);
            this.button_ClearAuthenitcation.Name = "button_ClearAuthenitcation";
            this.button_ClearAuthenitcation.Size = new System.Drawing.Size(190, 32);
            this.button_ClearAuthenitcation.TabIndex = 16;
            this.button_ClearAuthenitcation.Text = "Clear Authenitcation Data";
            this.button_ClearAuthenitcation.UseVisualStyleBackColor = true;
            this.button_ClearAuthenitcation.Click += new System.EventHandler(this.button_ClearAuthenitcation_Click);
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(79, 76);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.PasswordChar = '*';
            this.textBox_Password.Size = new System.Drawing.Size(317, 20);
            this.textBox_Password.TabIndex = 15;
            this.textBox_Password.TextChanged += new System.EventHandler(this.textBox_Password_TextChanged);
            // 
            // textBox_User
            // 
            this.textBox_User.Location = new System.Drawing.Point(79, 50);
            this.textBox_User.Name = "textBox_User";
            this.textBox_User.Size = new System.Drawing.Size(317, 20);
            this.textBox_User.TabIndex = 14;
            this.textBox_User.TextChanged += new System.EventHandler(this.textBox_User_TextChanged);
            // 
            // textBox_Server
            // 
            this.textBox_Server.Location = new System.Drawing.Point(79, 12);
            this.textBox_Server.Name = "textBox_Server";
            this.textBox_Server.ReadOnly = true;
            this.textBox_Server.Size = new System.Drawing.Size(317, 20);
            this.textBox_Server.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Username";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Server";
            // 
            // SimpleAuthentication
            // 
            this.AcceptButton = this.button_Authenticate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 160);
            this.Controls.Add(this.pictureBox_Authenticating);
            this.Controls.Add(this.button_Authenticate);
            this.Controls.Add(this.button_ClearAuthenitcation);
            this.Controls.Add(this.textBox_Password);
            this.Controls.Add(this.textBox_User);
            this.Controls.Add(this.textBox_Server);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SimpleAuthentication";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Simple Authentication";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Authenticating)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_Authenticating;
        private System.Windows.Forms.Button button_Authenticate;
        private System.Windows.Forms.Button button_ClearAuthenitcation;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.TextBox textBox_User;
        private System.Windows.Forms.TextBox textBox_Server;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}