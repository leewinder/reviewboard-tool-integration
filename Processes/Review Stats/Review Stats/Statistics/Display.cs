using Ingg.Stats_Runner.Shared.Extensions;
using RB_Tools.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Review_Stats.Statistics
{
    class Display
    {
        // State of the display
        public enum State
        {
            RequestingLogs,
            ExtractingLogs,
            ParsingLogs,
            QueryingReviewboard,
            CreatingResults,
        }
        //
        // Set our properties
        //
        public static void SetDisplayProperties(Form owner, Label progressLabel, ProgressBar progressBar, Button startButton, Logging logger)
        {
            // Save our properties
            s_owner = owner;

            s_progressLabel = progressLabel;
            s_progressBar = progressBar;

            // Show/hide
            startButton.Visible = false;

            progressBar.Visible = true;
            progressLabel.Visible = true;

            s_logger = logger;

            // Default state
            Start(State.RequestingLogs);
        }

        //
        // Starts a new state
        //
        public static void Start(State state)
        {
            // Start with default values
            Start(state, 0);
        }

        //
        // Starts a new state
        //
        public static void Start(State state, Int64 progressCountMax)
        {
            // Save our state
            s_state = state;
            s_logger.Log("Started state - {0} (assuming count of {1})", state.ToString(), progressCountMax);

            // Get our state
            string progressText = string.Format(StateMessages[(int)state], 0, progressCountMax);
            ProgressBarStyle progressStyle = ProgressStyle[(int)state];

            // Set up
            if (s_owner.InvokeRequired == false)
            {
                UpdateDialogDisplay(progressText, progressStyle, 0);
            }
            else
            {
                s_owner.Invoke(new MethodInvoker(() => 
                {
                    UpdateDialogDisplay(progressText, progressStyle, 0);
                }));
            }
        }

        //
        // Updates the new state
        //
        public static void Update(Int64 currentProgress, Int64 progressMax)
        {
            // Set up
            s_owner.Invoke((MethodInvoker)delegate
            {
                s_progressLabel.Text = string.Format(StateMessages[(int)s_state], currentProgress, progressMax);

                float sessionProgress = (currentProgress / (float)progressMax) * s_progressBar.Maximum;
                s_progressBar.SetProgressNoAnimation((int)sessionProgress);

                s_logger.Log("Progress so far - {0}", sessionProgress);
            });
        }

        // Properties
        private static Form         s_owner;

        private static Label        s_progressLabel;
        private static ProgressBar  s_progressBar;

        private static Logging      s_logger;

        private static State        s_state = State.RequestingLogs;

        // Display messages
        private static readonly string[] StateMessages = new string[]
        {
            @"Requesting SVN logs to review",
            @"Downloaded {0}/{1} logs",
            @"Parsing {0}/{1} logs",
            @"Processed {0}/{1} reviews",
            @"Creating review report",
        };

        private static readonly ProgressBarStyle[] ProgressStyle = new ProgressBarStyle[]
        {
            ProgressBarStyle.Marquee,
            ProgressBarStyle.Continuous,
            ProgressBarStyle.Continuous,
            ProgressBarStyle.Continuous,
            ProgressBarStyle.Marquee,
        };

        //
        // Updates the state of the dialog
        //
        private static void UpdateDialogDisplay(string text, ProgressBarStyle style, int progress)
        {
            s_progressLabel.Text = text;
            s_progressBar.Style = style;
            s_progressBar.SetProgressNoAnimation(progress);
        }
    }
}
