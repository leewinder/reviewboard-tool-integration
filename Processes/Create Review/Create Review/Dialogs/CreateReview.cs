using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Create_Review
{
    public partial class CreateReview : Form
    {
        //
        // Constructor
        //
        public CreateReview(string originalRequest, Utilities.Review.Content reviewSource)
        {
            // Save our properties
            m_reviewSource = reviewSource;
            m_requestDirectory = ExtractSourceDirectory(m_reviewSource.Patch);
            m_originalRequest = originalRequest;

            InitializeComponent();

            InitialiseDialofElements();
            InitialiseReviewGroups();
            UpdateCreateReviewDialogState(State.Idle);

            // If we are not authenticated, force it
            if (Settings.ReviewAuth.Default.Authenticated == false)
            {
                Authentication auth = new Authentication(m_requestDirectory);
                auth.ShowDialog(this);
            }
        }

        // State of the dialog
        private enum State
        {
            Idle,
            RefreshGroups,
            PostReview,
            Terminate,
        }

        // Reasons for a review finishing
        private enum FinishReason
        {
            Success,
            Error,
            Closing,
            Interim,
        }

        // Review Request Properties
        private class ReviewRequestProperties
        {
            public readonly Utilities.Review.Properties         ReviewProperties;
            public readonly Reviewboard.ConnectionProperties    ConnectionProperties;

            // Constructor
            public ReviewRequestProperties(Utilities.Review.Properties reviewProperties, Reviewboard.ConnectionProperties connectionProperties)
            {
                ReviewProperties = reviewProperties;
                ConnectionProperties = connectionProperties;
            }
        };

        // Private properties
        private readonly Utilities.Review.Content   m_reviewSource;
        private readonly string                     m_requestDirectory;

        private readonly string                     m_originalRequest;

        private Reviewboard.ReviewGroup[]           m_reviewGroups = new Reviewboard.ReviewGroup[0];

        //
        // Initialises the elements of the dialog
        //
        private void InitialiseDialofElements()
        {
            // Set the first option by default
            comboBox_ReviewLevel.SelectedIndex = (int)Utilities.Review.Level.FullReview;
        }

        //
        // Initialises the review groups box
        //
        private void InitialiseReviewGroups()
        {
            // If we have nothing, bail
            if (string.IsNullOrWhiteSpace(Settings.ReviewGroups.Default.Groups) == true)
                return;

            // Read in what we've selected
            int[] selectedGroups = null;
            if (string.IsNullOrWhiteSpace(Settings.ReviewGroups.Default.Selected) == false)
                selectedGroups = JsonConvert.DeserializeObject<int[]>(Settings.ReviewGroups.Default.Selected);

            // Update the selection box
            m_reviewGroups = JsonConvert.DeserializeObject<Reviewboard.ReviewGroup[]>(Settings.ReviewGroups.Default.Groups);
            UpdateSelectedReviewGroups(selectedGroups);
        }

        //
        // Extracts the directory of the source
        //
        private string ExtractSourceDirectory(string requestSource)
        {
            // Get the properties of this source
            FileAttributes attr = File.GetAttributes(requestSource);
            if (attr.HasFlag(FileAttributes.Directory))
                return new FileInfo(requestSource + @"\").Directory.FullName;
            else
                return new FileInfo(requestSource).Directory.FullName;
        }

        //
        // Updates the display of the combo boxes
        //
        private void UpdateSelectedReviewGroups(int[] selectedGroups)
        {
            // Spin through and update the combo box
            for (int i = 0; i < m_reviewGroups.Length; ++i)
                checkedListBox_ReviewGroups.Items.Add(m_reviewGroups[i].DisplayName);

            // Update the checked items
            if (selectedGroups != null)
            {
                foreach (int index in selectedGroups)
                {
                    // Set the state of this entry
                    if (index < checkedListBox_ReviewGroups.Items.Count)
                        checkedListBox_ReviewGroups.SetItemCheckState(index, CheckState.Checked);
                }
            }
        }

        //
        // Saves which selected items have been selected
        //
        private void SaveSelectedReviewGroups()
        {
            // Spin through and pull out the indices that are selected
            List<int> selectedIndices = new List<int>();
            foreach (int thisIndex in checkedListBox_ReviewGroups.CheckedIndices)
                selectedIndices.Add(thisIndex);           

            // Serialise out the selected index and save
            string selectedIndexJson = string.Empty;
            if (selectedIndices.Count != 0)
                selectedIndexJson = JsonConvert.SerializeObject(selectedIndices.ToArray());

            // Save the data out
            Settings.ReviewGroups.Default.Selected = selectedIndexJson;
            Settings.ReviewGroups.Default.Save();
        }

        //
        // Updates the active state of the dialogs
        // 
        private void UpdateCreateReviewDialogState(State expectedState)
        {
            // If we need to terminate, just do it
            if (expectedState == State.Terminate)
            {
                this.Close();
                return;
            }

            // Set our default states
            bool reviewId = true, summary = true, description = true, testing = true, jira = true, groups = true, refreshGroups = true, raiseReview = true, reviewOptions = true;
            bool visibleRefresh = true, visibleCreate = true;
            bool visibleRefreshGroups = false, visibleRasingReview = false;
            bool requestTypeDropdown = true;
            bool requestSelectArtifacts = true;

            // Update the right states
            if (expectedState == State.RefreshGroups)
            {
                visibleRefresh = false;

                raiseReview = false;
                summary = false;
                groups = false;

                visibleRefreshGroups = true;
            }
            else if (expectedState == State.PostReview)
            {
                reviewId = false;
                summary = false;
                description = false;
                testing = false;
                jira = false;

                groups = false;

                reviewOptions = false;

                refreshGroups = false;

                visibleCreate = false;

                visibleRasingReview = true;
            }

            // If this is a patch file, we always have to raise a review and can't delete it
            if (m_reviewSource.Source == Utilities.Review.Source.Patch)
            {
                requestTypeDropdown = false;
                requestSelectArtifacts = false;
            }

            // Update the state
            textBox_ReviewId.Enabled = reviewId;
            textBox_Summary.Enabled = summary;
            textBox_Description.Enabled = description;
            textBox_Testing.Enabled = testing;
            textBox_JiraId.Enabled = jira;

            checkedListBox_ReviewGroups.Enabled = groups;

            button_RefreshGroups.Enabled = refreshGroups;
            button_CreateReview.Enabled = raiseReview;

            button_RefreshGroups.Visible = visibleRefresh;
            button_CreateReview.Visible = visibleCreate;

            pictureBox_RaisingReview.Visible = visibleRasingReview;
            pictureBox_RefreshingGroups.Visible = visibleRefreshGroups;

            groupBox_Options.Enabled = reviewOptions;
            comboBox_ReviewLevel.Enabled = requestTypeDropdown;
            checkBox_KeepArtifacts.Enabled = requestSelectArtifacts;
        }

        //
        // Runs the review request
        //
        private void TriggerReviewRequest(Utilities.Review.Properties reviewProperties)
        {
            // Build up the background work
            BackgroundWorker updateThread = new BackgroundWorker();

            // Called when we need to trigger the request
            updateThread.DoWork += (object objectSender, DoWorkEventArgs args) =>
            {
                // Pull out the properties of the request
                ReviewRequestProperties thisRequest = args.Argument as ReviewRequestProperties;
                Reviewboard.ReviewRequestResult result = Reviewboard.RequestReview(
                    thisRequest.ConnectionProperties.Directory,
                    thisRequest.ConnectionProperties.Server,
                    thisRequest.ConnectionProperties.User,
                    thisRequest.ConnectionProperties.Password,
                    thisRequest.ReviewProperties);

                // Save the result
                args.Result = result;
            };
            // Called when the thread is complete
            updateThread.RunWorkerCompleted += (object objectSender, RunWorkerCompletedEventArgs args) =>
            {
                // Check if we had an error
                if (args.Error != null)
                {
                    string body = string.Format("Exception thrown when trying to raise a new review\n\nException: {0}\n\nDescription: {1}", args.Error.GetType().Name, args.Error.Message);
                    Notification.Show(this, @"Unable to raise review", body, Notification.FormIcon.Cross);

                    OnReviewFinished(FinishReason.Error);
                }
                else
                {
                    // Pull out the results of the review
                    Reviewboard.ReviewRequestResult requestResult = args.Result as Reviewboard.ReviewRequestResult;

                    // If we don't have a review URL, we failed
                    if (string.IsNullOrWhiteSpace(requestResult.Error) == false)
                    {
                        // Raise the error and we're done
                        Notification.Show(this, @"Unable to raise review", requestResult.Error, Notification.FormIcon.Cross);
                        OnReviewFinished(FinishReason.Error);
                    }
                    else
                    {
                        // Open the browser, doing this manually so we can open on the diff
                        if (string.IsNullOrWhiteSpace(requestResult.Url) == false)
                            System.Diagnostics.Process.Start(requestResult.Url);

                        // If it's just a patch file, we don't need to do anything else
                        if (m_reviewSource.Source == Utilities.Review.Source.Patch)
                        {
                            OnReviewFinished(FinishReason.Success);
                        }
                        else
                        {
                            // Flag the actual review as done so we lose the path file 
                            // and then it doesn't show up in TortoiseSVN
                            OnReviewFinished(FinishReason.Interim);
                            TriggerTortoiseRequest(requestResult);
                        }
                    }
                }
            };

            // Kick off the request
            Reviewboard.ConnectionProperties connectionProperties = new Reviewboard.ConnectionProperties(m_requestDirectory, Settings.ReviewAuth.Default.Server, Settings.ReviewAuth.Default.User, Settings.ReviewAuth.Default.Password, true);
            ReviewRequestProperties requestProperties = new ReviewRequestProperties(reviewProperties, connectionProperties);
            updateThread.RunWorkerAsync(requestProperties);
        }

        //
        // Runs the review request
        //
        private void TriggerTortoiseRequest(Reviewboard.ReviewRequestResult requestResults)
        {
            // Build up the background work
            BackgroundWorker updateThread = new BackgroundWorker();

            // Called when we need to trigger the request
            updateThread.DoWork += (object objectSender, DoWorkEventArgs args) =>
            {
                // Pull out the properties of the request
                Reviewboard.ReviewRequestResult originalResults = args.Argument as Reviewboard.ReviewRequestResult;
                TortoiseSvn.OpenCommitDialog(originalResults.Properties, originalResults.Url);
            };
            // Called when the thread is complete
            updateThread.RunWorkerCompleted += (object objectSender, RunWorkerCompletedEventArgs args) =>
            {
                // Check if we had an error
                if (args.Error != null)
                {
                    string body = string.Format("Exception thrown when trying to commit to SVN\n\nException: {0}\n\nDescription: {1}", args.Error.GetType().Name, args.Error.Message);
                    Notification.Show(this, @"Unable to commit", body, Notification.FormIcon.Cross);

                    // Done
                    OnReviewFinished(FinishReason.Error);
                }
                else
                {
                    // Done
                    OnReviewFinished(FinishReason.Success);
                }
            };

            // Kick off the request
            updateThread.RunWorkerAsync(requestResults);
        }

        //
        // Called when the review has completed (for better or worse)
        //
        private void OnReviewFinished(FinishReason finishReason)
        {
            // Keep the patch file, delete the original if we created it
            Utilities.Storage.Keep(m_reviewSource.Patch, "Changes.patch", m_reviewSource.Source == Utilities.Review.Source.Files);

            // Go back to the final state
            if (finishReason == FinishReason.Success)
                UpdateCreateReviewDialogState(State.Terminate);
            else if (finishReason == FinishReason.Error)
                UpdateCreateReviewDialogState(State.Idle);
        }

        private void button_CreateReview_Click(object sender, EventArgs e)
        {
            // We need a sumary before we raise the review
            if (string.IsNullOrWhiteSpace(textBox_Summary.Text) == true)
            {
                Notification.Show(this, "Unable to post review", "You need to provide a summary before you can post a review", Notification.FormIcon.Cross);
                return;
            }

            // Before we do anything, we need to be authenticated
            if (Settings.ReviewAuth.Default.Authenticated == false)
            {
                Notification.Show(this, "Unable to generate review", "You must be authenticated with the Reviewboard server before generating a review.\n\nThe Authentication Request dialog will now open.", Notification.FormIcon.Cross);

                // Now open the authentication dialog
                Authentication authRequest = new Authentication(m_requestDirectory);
                authRequest.ShowDialog(this);

                // If we are still not authenticated, bail
                if (Settings.ReviewAuth.Default.Authenticated == false)
                    return;
            }

            // Save the state of our review groups
            SaveSelectedReviewGroups();

            // Update our state
            UpdateCreateReviewDialogState(State.PostReview);

            // Do we need to keep our artifacts?
            if (checkBox_KeepArtifacts.Checked == true)
                Utilities.Storage.KeepAssets();
            Utilities.Storage.Keep(m_originalRequest, "Original File List.txt", false);

            // Build up the list of review groups
            List<string> selectedReviewGroups = new List<string>();
            foreach (int thisIndex in checkedListBox_ReviewGroups.CheckedIndices)
            {
                Reviewboard.ReviewGroup thisGroup = m_reviewGroups[thisIndex];
                selectedReviewGroups.Add(thisGroup.InternalName);
            }

            // Build up the properties of this review
            Utilities.Review.Properties reviewProperties = new Utilities.Review.Properties(
                m_requestDirectory,

                m_reviewSource,

                textBox_ReviewId.Text,

                textBox_Summary.Text,
                textBox_Description.Text,
                textBox_Testing.Text,

                textBox_JiraId.Text.Trim().Replace(" ", ""),

                selectedReviewGroups,

                (Utilities.Review.Level)comboBox_ReviewLevel.SelectedIndex,
                checkBox_CopiesAsAdds.Checked
            );

            // Trigger the correct state depending on whether we are reviewing
            TriggerReviewRequest(reviewProperties);
        }

        private void reviewboardAuthenticationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show the authentication dialog
            Authentication authRequest = new Authentication(m_requestDirectory);
            authRequest.ShowDialog(this);
        }

        private void button_RefreshGroups_Click(object sender, EventArgs e)
        {
            // Before we do anything, we need to be authenticated
            if (Settings.ReviewAuth.Default.Authenticated == false)
            {
                Notification.Show(this, "Unable to refresh groups", "You must be authenticated with the Reviewboard server before refreshing the review groups.\n\nThe Authentication Request dialog will now open.", Notification.FormIcon.Cross);

                // Now open the authentication dialog
                Authentication authRequest = new Authentication(m_requestDirectory);
                authRequest.ShowDialog(this);

                // If we are still not authenticated, bail
                if (Settings.ReviewAuth.Default.Authenticated == false)
                    return;
            }

            // Lose everything in the box
            checkedListBox_ReviewGroups.Items.Clear();
            m_reviewGroups = new Reviewboard.ReviewGroup[0];

            Settings.ReviewGroups.Default.Reset();
            Settings.ReviewGroups.Default.Save();

            // Turn off the buttons and lock the settings
            UpdateCreateReviewDialogState(State.RefreshGroups);
            
            // Build up the background work
            BackgroundWorker updateThread = new BackgroundWorker();

            // Called when we need to trigger the request
            updateThread.DoWork += (object objectSender, DoWorkEventArgs args) =>
            {
                // Kick it off
                Reviewboard.ConnectionProperties userConnectionProperties = args.Argument as Reviewboard.ConnectionProperties;
                Reviewboard.ReviewGroup[] result = Reviewboard.GetReviewGroups(m_requestDirectory, userConnectionProperties.Server, userConnectionProperties.User, userConnectionProperties.Password);

                // Save the result
                args.Result = result;
            };
            // Called when the thread is complete
            updateThread.RunWorkerCompleted += (object objectSender, RunWorkerCompletedEventArgs args) =>
            {
                // Check if we had an error
                if (args.Error != null)
                {
                    string body = string.Format("Exception thrown when trying to retrive the review groups\n\nException: {0}\n\nDescription: {1}", args.Error.GetType().Name, args.Error.Message);
                    Notification.Show(this, @"Unable to update group list", body, Notification.FormIcon.Cross);
                }
                else
                {
                    // Update the list
                    m_reviewGroups = args.Result as Reviewboard.ReviewGroup[];
                    UpdateSelectedReviewGroups(null);

                    // Save the groups we have returned
                    string groupsJson = JsonConvert.SerializeObject(m_reviewGroups);
                    Settings.ReviewGroups.Default.Groups = groupsJson;
                    Settings.ReviewGroups.Default.Save();
                }

                // Set the button state back
                UpdateCreateReviewDialogState(State.Idle);
            };

            // Kick off the request
            Reviewboard.ConnectionProperties requestProperties = new Reviewboard.ConnectionProperties(m_requestDirectory, Settings.ReviewAuth.Default.Server, Settings.ReviewAuth.Default.User, Settings.ReviewAuth.Default.Password, true);
            updateThread.RunWorkerAsync(requestProperties);
        }

        private void CreateReview_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If we're closing, make sure we've cleaned up
            OnReviewFinished(FinishReason.Closing);
        }

        private void reviewDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(m_reviewSource.Patch);
        }

        private void filesForReviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Truncate the files and keep track of them
            StringBuilder filesToReview = new StringBuilder("The following files and folder have been included in this review\n\n");
            for (int i = 0; i < m_reviewSource.Files.Length; ++i)
            {
                string truncatedFile = Utilities.Paths.TruncateLongPath(m_reviewSource.Files[i]);
                filesToReview.Append("- " + truncatedFile + '\n');
            }

            // Just show the list
            Notification.Show(this, "Files in Review", filesToReview.ToString(), Notification.FormIcon.Info);
        }
    }
}
