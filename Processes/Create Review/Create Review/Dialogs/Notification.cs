using System;
using System.Drawing;
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

            // Update the dialog box so it fits the text
            ResizeDialogAndLabelSize(description);

            // Set up the dialog
            this.Text = title;
            this.label_Description.Text = description;

            pictureBox_Cross.Visible = (icon == FormIcon.Cross);
            pictureBox_Tick.Visible = (icon == FormIcon.Tick);
        }

        //
        // Calculates the delta needed to fit the text
        //
        private void ResizeDialogAndLabelSize(string containerText)
        {
            // Pull out the graphics context to get the scale
            using (Graphics context = CreateGraphics())
            {
                // Get the size, and if it's bigger, scale up
                SizeF expectedSize = context.MeasureString(containerText, label_Description.Font, label_Description.Width);
                if (expectedSize.Height > label_Description.Height)
                {
                    // We need to add a few additional lines as it doesn't take into account additional length
                    SizeF additionalSize = context.MeasureString("\n\n", label_Description.Font, label_Description.Width);

                    // Get the delta of the increase and increase the dialog
                    float heightDelta = expectedSize.Height - label_Description.Height;
                    this.Height = (int)Math.Ceiling(this.Height + heightDelta + additionalSize.Height);
                }
            }
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            // Lose it
            this.Close();
        }
    }
}
