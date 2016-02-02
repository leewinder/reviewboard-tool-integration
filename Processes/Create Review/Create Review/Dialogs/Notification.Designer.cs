namespace Create_Review
{
    partial class Notification
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Notification));
            this.button_Close = new System.Windows.Forms.Button();
            this.pictureBox_Tick = new System.Windows.Forms.PictureBox();
            this.pictureBox_Cross = new System.Windows.Forms.PictureBox();
            this.label_Description = new System.Windows.Forms.Label();
            this.pictureBox_Info = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Tick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Cross)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Info)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Close
            // 
            this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Close.Location = new System.Drawing.Point(366, 97);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(89, 32);
            this.button_Close.TabIndex = 0;
            this.button_Close.Text = "Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // pictureBox_Tick
            // 
            this.pictureBox_Tick.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_Tick.BackgroundImage")));
            this.pictureBox_Tick.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox_Tick.Location = new System.Drawing.Point(5, 5);
            this.pictureBox_Tick.Name = "pictureBox_Tick";
            this.pictureBox_Tick.Size = new System.Drawing.Size(122, 122);
            this.pictureBox_Tick.TabIndex = 1;
            this.pictureBox_Tick.TabStop = false;
            // 
            // pictureBox_Cross
            // 
            this.pictureBox_Cross.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_Cross.BackgroundImage")));
            this.pictureBox_Cross.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox_Cross.Location = new System.Drawing.Point(5, 5);
            this.pictureBox_Cross.Name = "pictureBox_Cross";
            this.pictureBox_Cross.Size = new System.Drawing.Size(122, 122);
            this.pictureBox_Cross.TabIndex = 2;
            this.pictureBox_Cross.TabStop = false;
            // 
            // label_Description
            // 
            this.label_Description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_Description.Location = new System.Drawing.Point(133, 5);
            this.label_Description.Name = "label_Description";
            this.label_Description.Size = new System.Drawing.Size(322, 88);
            this.label_Description.TabIndex = 3;
            this.label_Description.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox_Info
            // 
            this.pictureBox_Info.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_Info.BackgroundImage")));
            this.pictureBox_Info.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox_Info.Location = new System.Drawing.Point(5, 5);
            this.pictureBox_Info.Name = "pictureBox_Info";
            this.pictureBox_Info.Size = new System.Drawing.Size(122, 122);
            this.pictureBox_Info.TabIndex = 4;
            this.pictureBox_Info.TabStop = false;
            // 
            // Notification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 132);
            this.Controls.Add(this.pictureBox_Info);
            this.Controls.Add(this.label_Description);
            this.Controls.Add(this.pictureBox_Cross);
            this.Controls.Add(this.pictureBox_Tick);
            this.Controls.Add(this.button_Close);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Notification";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Notification";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Tick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Cross)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Info)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.PictureBox pictureBox_Tick;
        private System.Windows.Forms.PictureBox pictureBox_Cross;
        private System.Windows.Forms.Label label_Description;
        private System.Windows.Forms.PictureBox pictureBox_Info;
    }
}