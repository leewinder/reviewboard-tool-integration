namespace Create_Review
{
    partial class Authentication
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Authentication));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_ReviewboardServer = new System.Windows.Forms.TextBox();
            this.textBox_ReviewboardUser = new System.Windows.Forms.TextBox();
            this.textBox_ReviewboardPassword = new System.Windows.Forms.TextBox();
            this.button_ClearAuthenitcationReviewboard = new System.Windows.Forms.Button();
            this.button_AuthenticateReviewBoard = new System.Windows.Forms.Button();
            this.pictureBox_Authenticating = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Authenticating)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Reviewboard Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Username";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Password";
            // 
            // textBox_ReviewboardServer
            // 
            this.textBox_ReviewboardServer.Location = new System.Drawing.Point(122, 6);
            this.textBox_ReviewboardServer.Name = "textBox_ReviewboardServer";
            this.textBox_ReviewboardServer.ReadOnly = true;
            this.textBox_ReviewboardServer.Size = new System.Drawing.Size(278, 20);
            this.textBox_ReviewboardServer.TabIndex = 3;
            // 
            // textBox_ReviewboardUser
            // 
            this.textBox_ReviewboardUser.Location = new System.Drawing.Point(122, 44);
            this.textBox_ReviewboardUser.Name = "textBox_ReviewboardUser";
            this.textBox_ReviewboardUser.Size = new System.Drawing.Size(278, 20);
            this.textBox_ReviewboardUser.TabIndex = 4;
            this.textBox_ReviewboardUser.TextChanged += new System.EventHandler(this.textBox_ReviewboardUser_TextChanged);
            // 
            // textBox_ReviewboardPassword
            // 
            this.textBox_ReviewboardPassword.Location = new System.Drawing.Point(122, 70);
            this.textBox_ReviewboardPassword.Name = "textBox_ReviewboardPassword";
            this.textBox_ReviewboardPassword.PasswordChar = '*';
            this.textBox_ReviewboardPassword.Size = new System.Drawing.Size(278, 20);
            this.textBox_ReviewboardPassword.TabIndex = 5;
            this.textBox_ReviewboardPassword.TextChanged += new System.EventHandler(this.textBox_ReviewboardPassword_TextChanged);
            // 
            // button_ClearAuthenitcationReviewboard
            // 
            this.button_ClearAuthenitcationReviewboard.Location = new System.Drawing.Point(15, 109);
            this.button_ClearAuthenitcationReviewboard.Name = "button_ClearAuthenitcationReviewboard";
            this.button_ClearAuthenitcationReviewboard.Size = new System.Drawing.Size(190, 32);
            this.button_ClearAuthenitcationReviewboard.TabIndex = 7;
            this.button_ClearAuthenitcationReviewboard.Text = "Clear Authenitcation Data";
            this.button_ClearAuthenitcationReviewboard.UseVisualStyleBackColor = true;
            this.button_ClearAuthenitcationReviewboard.Click += new System.EventHandler(this.button_ClearAuthenitcationReviewboard_Click);
            // 
            // button_AuthenticateReviewBoard
            // 
            this.button_AuthenticateReviewBoard.Location = new System.Drawing.Point(210, 109);
            this.button_AuthenticateReviewBoard.Name = "button_AuthenticateReviewBoard";
            this.button_AuthenticateReviewBoard.Size = new System.Drawing.Size(190, 32);
            this.button_AuthenticateReviewBoard.TabIndex = 8;
            this.button_AuthenticateReviewBoard.Text = "Authenticate";
            this.button_AuthenticateReviewBoard.UseVisualStyleBackColor = true;
            this.button_AuthenticateReviewBoard.Click += new System.EventHandler(this.button_AuthenticateReviewBoard_Click);
            // 
            // pictureBox_Authenticating
            // 
            this.pictureBox_Authenticating.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_Authenticating.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_Authenticating.Image")));
            this.pictureBox_Authenticating.InitialImage = null;
            this.pictureBox_Authenticating.Location = new System.Drawing.Point(289, 109);
            this.pictureBox_Authenticating.Name = "pictureBox_Authenticating";
            this.pictureBox_Authenticating.Size = new System.Drawing.Size(32, 32);
            this.pictureBox_Authenticating.TabIndex = 9;
            this.pictureBox_Authenticating.TabStop = false;
            // 
            // Authentication
            // 
            this.AcceptButton = this.button_AuthenticateReviewBoard;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 156);
            this.Controls.Add(this.pictureBox_Authenticating);
            this.Controls.Add(this.button_AuthenticateReviewBoard);
            this.Controls.Add(this.button_ClearAuthenitcationReviewboard);
            this.Controls.Add(this.textBox_ReviewboardPassword);
            this.Controls.Add(this.textBox_ReviewboardUser);
            this.Controls.Add(this.textBox_ReviewboardServer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Authentication";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Authenticating)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_ReviewboardServer;
        private System.Windows.Forms.TextBox textBox_ReviewboardUser;
        private System.Windows.Forms.TextBox textBox_ReviewboardPassword;
        private System.Windows.Forms.Button button_ClearAuthenitcationReviewboard;
        private System.Windows.Forms.Button button_AuthenticateReviewBoard;
        private System.Windows.Forms.PictureBox pictureBox_Authenticating;
    }
}