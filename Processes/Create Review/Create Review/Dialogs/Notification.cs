using System;
using System.Windows.Forms;

namespace Create_Review
{
    public partial class Notification : Form
    {
        // Icons
        public enum FormIcon
        {
            Tick,
            Cross,
        }

        //
        // Shows a notification
        //
        public static void Show(Form owner, string title, string description, FormIcon icon)
        {
            Notification notification = new Notification(title, description, icon);
            notification.ShowDialog(owner);
        }

        //
        // Private constructor
        //
        private Notification(string title, string description, FormIcon icon)
        {
            InitializeComponent();

            // Set up the dialog
            this.Text = title;
            this.label_Description.Text = description;

            pictureBox_Cross.Visible = (icon == FormIcon.Cross);
            pictureBox_Tick.Visible = (icon == FormIcon.Tick);
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            // Lose it
            this.Close();
        }
    }
}
