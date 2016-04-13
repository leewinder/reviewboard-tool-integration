
namespace Ingg.Stats_Runner.Shared.Extensions
{
    public static class ProgressBarExtensions
    {
        //
        // Sets the progress bar value, without using 'Windows Aero' animation.
        // Taken from http://stackoverflow.com/a/22941217/48651
        //
        public static void SetProgressNoAnimation(this System.Windows.Forms.ProgressBar pb, int value)
        {
            try
            {
                // To get around the progressive animation, we need to move the 
                // progress bar backwards.
                if (value == pb.Maximum)
                {
                    // Special case as value can't be set greater than Maximum.
                    pb.Maximum = value + 1;     // Temporarily Increase Maximum
                    pb.Value = value + 1;       // Move past
                    pb.Maximum = value;         // Reset maximum
                }
                else
                {
                    pb.Value = value + 1;       // Move past
                }
                pb.Value = value;               // Move to correct value
            }
            catch
            {
            }
        }
    }
}
