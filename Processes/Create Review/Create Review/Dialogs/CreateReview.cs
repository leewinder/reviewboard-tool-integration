﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

using RB_Tools.Shared.Server;
using RB_Tools.Shared.Authentication.Credentials;
using RB_Tools.Shared.Utilities;
using RB_Tools.Shared.Extensions;
using RB_Tools.Shared.Logging;
using System.Drawing;

namespace Create_Review
{
    public partial class CreateReview : Form
    {
        //
        // Constructor
        //
        public CreateReview(string originalRequest, Review.Review.Content reviewSource, Logging logger)
        {
            // Save our log
            m_logger = logger;

            // Save our properties
            m_reviewSource = reviewSource;
            m_requestDirectory = ExtractSourceDirectory();
            m_originalRequest = originalRequest;

            InitializeComponent();

            InitialiseDialogElements();
            InitialiseReviewGroups();
            UpdateCreateReviewDialogState(State.Idle);
            
            // If this is a patch review, disable the list files option
            if (reviewSource.Source == Review.Review.Source.Patch)
            {
                filesForReviewToolStripMenuItem.Enabled = false;
                filesForReviewToolStripMenuItem.ToolTipText = "Unable to view the individual files of a review\nwhen reviewing a manually created patch file";
            }
            else if (reviewSource.Source == Review.Review.Source.None)
            {
                filesForReviewToolStripMenuItem.Enabled = false;
                filesForReviewToolStripMenuItem.ToolTipText = "Unable to review the individual files of a review\nwhen skipping the review process";

                reviewDiffToolStripMenuItem.Enabled = false;
                reviewDiffToolStripMenuItem.ToolTipText = "Unable to view the diff content\nwhen skipping the review process";

                // Disable a few other options
                button_RefreshGroups.Enabled = false;
                checkedListBox_ReviewGroups.Enabled = false;
                textBox_ReviewId.Enabled = false;

                checkBox_CopiesAsAdds.Enabled = false;
                checkBox_KeepArtifacts.Enabled = false;
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
            public readonly Review.Review.Properties ReviewProperties;
            public readonly string                      WorkingCopy;

            // Constructor
            public ReviewRequestProperties(Review.Review.Properties reviewProperties, string workingCopy)
            {
                ReviewProperties = reviewProperties;
                WorkingCopy = workingCopy;
            }
        };

        // Private properties
        private readonly Review.Review.Content      m_reviewSource;
        private readonly string                     m_requestDirectory;

        private readonly string                     m_originalRequest;

        private readonly Logging                    m_logger;

        private Reviewboard.ReviewGroup[]           m_reviewGroups = new Reviewboard.ReviewGroup[0];

        //
        // Initialises the elements of the dialog
        //
        private void InitialiseDialogElements()
        {
            m_logger.Log("Initialising dialog elements");

            // Start with a clean combo
            comboBox_ReviewLevel.Items.Clear();

            // Add the review level entries to the combo box
            int reviewTypeCount = Enum.GetNames(typeof(RB_Tools.Shared.Review.Properties.Level)).Length;
            for (int i = 0; i < reviewTypeCount; ++i)
            {
                // Get the level
                var thisLevel = (RB_Tools.Shared.Review.Properties.Level)i;
                comboBox_ReviewLevel.Items.Add(thisLevel.GetDescription());

                m_logger.Log("* Adding level {0}", thisLevel);
            }

            // Set the first option by default
            int defaultOption = (m_reviewSource.Source == Review.Review.Source.None ? 1 : 0);
            comboBox_ReviewLevel.SelectedIndex = defaultOption;
        }

        //
        // Initialises the review groups box
        //
        private void InitialiseReviewGroups()
        {
            m_logger.Log("Initialising review groups");

            // If we have nothing, bail
            if (string.IsNullOrWhiteSpace(Settings.Settings.Default.Groups) == true)
                return;

            // Read in what we've selected
            int[] selectedGroups = null;
            if (string.IsNullOrWhiteSpace(Settings.Settings.Default.Selected) == false)
                selectedGroups = JsonConvert.DeserializeObject<int[]>(Settings.Settings.Default.Selected);

            // Update the selection box
            m_reviewGroups = JsonConvert.DeserializeObject<Reviewboard.ReviewGroup[]>(Settings.Settings.Default.Groups);
            UpdateSelectedReviewGroups(selectedGroups);
        }

        //
        // Extracts the directory of the source
        //
        private string ExtractSourceDirectory()
        {
            // Do we have a source
            if (m_reviewSource.Source == Review.Review.Source.None)
                return null;

            // Get the properties of this source
            FileAttributes attr = File.GetAttributes(m_reviewSource.Patch);
            if (attr.HasFlag(FileAttributes.Directory))
                return new FileInfo(m_reviewSource.Patch + @"\").Directory.FullName;
            else
                return new FileInfo(m_reviewSource.Patch).Directory.FullName;
        }

        //
        // Updates the display of the combo boxes
        //
        private void UpdateSelectedReviewGroups(int[] selectedGroups)
        {
            // Spin through and update the combo box
            for (int i = 0; i < m_reviewGroups.Length; ++i)
            {
                checkedListBox_ReviewGroups.Items.Add(m_reviewGroups[i].DisplayName);
                m_logger.Log("* Added {0}", m_reviewGroups[i].DisplayName);
            }

            // Update the checked items
            if (selectedGroups != null)
            {
                foreach (int index in selectedGroups)
                {
                    // Set the state of this entry
                    if (index < checkedListBox_ReviewGroups.Items.Count)
                    {
                        checkedListBox_ReviewGroups.SetItemCheckState(index, CheckState.Checked);
                        m_logger.Log("* Selected group {0}", index);
                    }
                }
            }
        }

        //
        // Saves which selected items have been selected
        //
        private void SaveSelectedReviewGroups()
        {
            m_logger.Log("Saving the currently selected review groups");

            // Spin through and pull out the indices that are selected
            List <int> selectedIndices = new List<int>();
            foreach (int thisIndex in checkedListBox_ReviewGroups.CheckedIndices)
                selectedIndices.Add(thisIndex);           

            // Serialise out the selected index and save
            string selectedIndexJson = string.Empty;
            if (selectedIndices.Count != 0)
                selectedIndexJson = JsonConvert.SerializeObject(selectedIndices.ToArray());

            // Save the data out
            Settings.Settings.Default.Selected = selectedIndexJson;
            Settings.Settings.Default.Save();
        }

        //
        // Updates the active state of the dialogs
        // 
        private void UpdateCreateReviewDialogState(State expectedState)
        {
            m_logger.Log("Dialog state requested - {0}", expectedState);

            // If we need to terminate, just do it
            if (expectedState == State.Terminate)
            {
                m_logger.Log("Closing dialog as requested");
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
            if (m_reviewSource.Source == Review.Review.Source.Patch)
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
        private void TriggerReviewRequest(Review.Review.Properties reviewProperties)
        {
            m_logger.Log("Triggering review request");

            // Build up the background work
            BackgroundWorker updateThread = new BackgroundWorker();

            // Called when we need to trigger the request
            updateThread.DoWork += (object objectSender, DoWorkEventArgs args) =>
            {
                // Pull out the properties of the request
                ReviewRequestProperties thisRequest = args.Argument as ReviewRequestProperties;
                OutputRequestProperties(thisRequest);

                // Do we need to validate the Jira ticket?
                bool ticketValid = ValidateJiraTicket(thisRequest.ReviewProperties.JiraId);
                if (ticketValid == false)
                {
                    args.Result = new Reviewboard.ReviewRequestResult(null, "Unable to validate the given Jira ticket", thisRequest.ReviewProperties);
                    return;
                }

                // Carry out the review
                args.Result = RequestReview(thisRequest);
            };
            // Called when the thread is complete
            updateThread.RunWorkerCompleted += (object objectSender, RunWorkerCompletedEventArgs args) =>
            {
                // Check if we had an error
                if (args.Error != null)
                {
                    m_logger.Log("Error raised during review request - {0}", args.Error.Message);
                    string body = string.Format("Exception thrown when trying to raise a new review\n\nException: {0}\n\nDescription: {1}", args.Error.GetType().Name, args.Error.Message);

                    // Was it an authentication error?
                    bool authenticationRequired = false;
                    if (args.Error.Message.Contains("username or password was not correct") == true)
                    {
                        authenticationRequired = true;
                        body += "\n\nDo you want to attempt to reauthenticate with the server?";
                    }

                    // Show the options
                    MessageBoxButtons buttonTypes = (authenticationRequired == true ? MessageBoxButtons.YesNo : MessageBoxButtons.OK);
                    DialogResult result = MessageBox.Show(this, body, @"Unable to raise review", buttonTypes, MessageBoxIcon.Error);

                    // Continue?
                    if (result == DialogResult.Yes && authenticationRequired == true)
                        RB_Tools.Shared.Authentication.Targets.Reviewboard.Authenticate(m_logger);
                    
                    OnReviewFinished(FinishReason.Error);
                }
                else
                {
                    // Pull out the results of the review
                    Reviewboard.ReviewRequestResult requestResult = args.Result as Reviewboard.ReviewRequestResult;

                    // If we don't have a review URL, we failed
                    if (string.IsNullOrWhiteSpace(requestResult.Error) == false)
                    {
                        m_logger.Log("Error raised during review request - {0}", requestResult.Error);

                        // Raise the error and we're done
                        MessageBox.Show(this, requestResult.Error, @"Unable to raise review", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        OnReviewFinished(FinishReason.Error);
                    }
                    else
                    {
                        if (requestResult.Properties.ReviewLevel == RB_Tools.Shared.Review.Properties.Level.FullReview)
                            m_logger.Log("Review posted successful - {0}", requestResult.Url);

                        // Open the browser, doing this manually so we can open on the diff
                        if (string.IsNullOrWhiteSpace(requestResult.Url) == false)
                            System.Diagnostics.Process.Start(requestResult.Url);

                        // If it's just a patch file, we don't need to do anything else
                        if (m_reviewSource.Source == Review.Review.Source.Patch)
                        {
                            m_logger.Log("Patch review completed, process finished");
                            OnReviewFinished(FinishReason.Success);
                        }
                        else
                        {
                            m_logger.Log("File review finished, moving onto next stage");

                            // Flag the actual review as done so we lose the path file 
                            // and then it doesn't show up in TortoiseSVN
                            OnReviewFinished(FinishReason.Interim);
                            TriggerTortoiseRequest(requestResult);
                        }
                    }
                }
            };
            
            // Kick off the request
            ReviewRequestProperties requestProperties = new ReviewRequestProperties(reviewProperties, m_requestDirectory);
            updateThread.RunWorkerAsync(requestProperties);
        }

        //
        // Outputs what properties we are using
        //
        private void OutputRequestProperties(ReviewRequestProperties thisRequest)
        {
            if (thisRequest.ReviewProperties.ReviewLevel == RB_Tools.Shared.Review.Properties.Level.FullReview)
            {
                m_logger.Log("Starting review request on '{0}'", thisRequest.WorkingCopy);
                m_logger.Log(" * Path: {0}", thisRequest.ReviewProperties.Path);
                m_logger.Log(" * Review ID: {0}", thisRequest.ReviewProperties.ReviewId);
                m_logger.Log(" * Summary: {0}", thisRequest.ReviewProperties.Summary);
                m_logger.Log(" * Description:\n{0}", thisRequest.ReviewProperties.Description);
                m_logger.Log(" * Testing:\n{0}", thisRequest.ReviewProperties.Testing);
                m_logger.Log(" * Jira ID: {0}", thisRequest.ReviewProperties.JiraId);
                m_logger.Log(" * Review Level: {0}", thisRequest.ReviewProperties.ReviewLevel);
                m_logger.Log(" * Copies As Adds: {0}", thisRequest.ReviewProperties.CopiesAsAdds);
                m_logger.Log(" * Source Type: {0}", thisRequest.ReviewProperties.Contents.Source);

                // Groups
                if (thisRequest.ReviewProperties.Groups != null && thisRequest.ReviewProperties.Groups.Count > 0)
                {
                    m_logger.Log("Posting to groups:");
                    foreach (string thisGroup in thisRequest.ReviewProperties.Groups)
                        m_logger.Log(" * {0}", thisGroup);
                }

                // Content files
                if (thisRequest.ReviewProperties.Contents.Files != null && thisRequest.ReviewProperties.Contents.Files.Length > 0)
                {
                    m_logger.Log("Reviewing files:");
                    foreach (string thisFile in thisRequest.ReviewProperties.Contents.Files)
                        m_logger.Log(" * {0}", thisFile);
                }

                // Content patch
                if (string.IsNullOrEmpty(thisRequest.ReviewProperties.Contents.Patch) == false)
                {
                    m_logger.Log("Review patch:");
                    m_logger.Log(" * {0}:", thisRequest.ReviewProperties.Contents.Patch);
                }
            }
            else
            {
                m_logger.Log("Skipping review request");
            }
        }

        //
        // Validates the Jira ticket if needed
        //
        bool ValidateJiraTicket(string jiraId)
        {
            // If we have no ticker, we're fine
            if (string.IsNullOrEmpty(jiraId) == true)
                return true;

            // Get our credentials
            string serverName = Names.Url[(int)Names.Type.Jira];
            Simple credentials = Credentials.Create(serverName, m_logger) as Simple;
            if (credentials == null)
            {
                m_logger.Log("Unable to create credentials for '{0}'", serverName);
                throw new FileNotFoundException(@"Unable to find the credentials for " + serverName);
            }

            // Split out ticket IDs so we validate them all
            string[] jiras = jiraId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (jiras == null || jiras.Length == 0)
                jiras = new string[] { jiraId };

            // Validate the ticker
            foreach (string thisTicket in jiras)
            {
                string ticketToCheck = thisTicket.Trim();
                bool tickerValid = RB_Tools.Shared.Targets.Jira.ValidateJiraTicker(credentials, ticketToCheck).Result;
                if (tickerValid == false)
                {
                    string message = string.Format("Unable to find ticket '{0}' on {1}", jiraId, serverName);

                    m_logger.Log(message);
                    throw new InvalidOperationException(message);
                }
            }

            // We're done
            return true;
        }

        //
        // Requests a new review
        //
        object RequestReview(ReviewRequestProperties thisRequest)
        {
            if (thisRequest.ReviewProperties.ReviewLevel == RB_Tools.Shared.Review.Properties.Level.FullReview)
            {

                // Get our credentials
                string serverName = Names.Url[(int)Names.Type.Reviewboard];
                Simple credentials = Credentials.Create(serverName, m_logger) as Simple;
                if (credentials == null)
                {
                    m_logger.Log("Unable to create credentials for '{0}'", serverName);
                    throw new FileNotFoundException(@"Unable to find the credentials for " + serverName);
                }

                // Request the review
                m_logger.Log("Requesting review");
                Reviewboard.ReviewRequestResult result = Reviewboard.RequestReview(
                    thisRequest.WorkingCopy,
                    credentials.Server,
                    credentials.User,
                    credentials.Password,
                    thisRequest.ReviewProperties,
                    m_logger);

                // Save the result
                m_logger.Log("Review request finished");
                return result;
            }
            else
            {
                // Save the result
                m_logger.Log("Review request skipped as a review is not being carried out");
                return new Reviewboard.ReviewRequestResult(null, null, thisRequest.ReviewProperties);
            }
        }

        //
        // Runs the review request
        //
        private void TriggerTortoiseRequest(Reviewboard.ReviewRequestResult requestResults)
        {
            m_logger.Log("Triggering TortoiseSVN dialog");

            // Build up the background work
            BackgroundWorker updateThread = new BackgroundWorker();

            // Called when we need to trigger the request
            updateThread.DoWork += (object objectSender, DoWorkEventArgs args) =>
            {
                // Pull out the properties of the request
                Reviewboard.ReviewRequestResult originalResults = args.Argument as Reviewboard.ReviewRequestResult;
                TortoiseSvn.OpenCommitDialog(originalResults.Properties, originalResults.Url, m_logger);
            };
            // Called when the thread is complete
            updateThread.RunWorkerCompleted += (object objectSender, RunWorkerCompletedEventArgs args) =>
            {
                // Check if we had an error
                if (args.Error != null)
                {
                    m_logger.Log("Error using TortoiseSVN - {0}", args.Error.Message);

                    string body = string.Format("Exception thrown when trying to commit to SVN\n\nException: {0}\n\nDescription: {1}", args.Error.GetType().Name, args.Error.Message);
                    MessageBox.Show(this, body, @"Unable to commit", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
            m_logger.Log("Review state finished - {0}", finishReason);

            // Only delete the file in certain situations

            // Keep the patch file, delete the original if we created it
            if (finishReason == FinishReason.Success || finishReason == FinishReason.Closing)
                Utilities.Storage.Keep(m_reviewSource.Patch, "Changes.patch", m_reviewSource.Source == Review.Review.Source.Files, m_logger);

            // Go back to the final state
            if (finishReason == FinishReason.Success)
                UpdateCreateReviewDialogState(State.Terminate);
            else if (finishReason == FinishReason.Error)
                UpdateCreateReviewDialogState(State.Idle);
        }

        //
        // Checks if the review option should be selected
        //
        private bool IsReviewOptionValid(int index)
        {
            var reviewOption = (RB_Tools.Shared.Review.Properties.Level)index;

            if (m_reviewSource.Source == Review.Review.Source.None && reviewOption == RB_Tools.Shared.Review.Properties.Level.FullReview)
                return false;

            // It's fine
            return true;
        }

        //
        // Starts a review
        //
        private void button_CreateReview_Click(object sender, EventArgs e)
        {
            // We need a sumary before we raise the review
            if (string.IsNullOrWhiteSpace(textBox_Summary.Text) == true)
            {
                MessageBox.Show(this, "You need to provide a summary before you can post a review", "Unable to post review", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Do we need a Jira ticket?
            if (Settings.Settings.Default.JiraRequired == true)
            {
                if (string.IsNullOrWhiteSpace(textBox_JiraId.Text) == true)
                {
                    MessageBox.Show(this, "You need to provide a Jira ticket before you can continue", "Unable to post review", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Only bother checking against the reviewboard server if we are doing a review
            RB_Tools.Shared.Review.Properties.Level reviewLevel = (RB_Tools.Shared.Review.Properties.Level)comboBox_ReviewLevel.SelectedIndex;
            if (reviewLevel == RB_Tools.Shared.Review.Properties.Level.FullReview)
            {
                string reviewboardServer = Names.Url[(int)Names.Type.Reviewboard];
                if (Credentials.Available(reviewboardServer) == false)
                {
                    m_logger.Log("Requesting Reviewboard credentials");

                    DialogResult dialogResult = MessageBox.Show(this, "You must be authenticated with the Reviewboard server before generating a review.\n\nDo you want to authenticate now?", "Authentication Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                        RB_Tools.Shared.Authentication.Targets.Reviewboard.Authenticate(m_logger);

                    // Check if we're still unauthenticated
                    if (Credentials.Available(reviewboardServer) == false)
                    {
                        m_logger.Log("Credentials still unavailable");
                        return;
                    }
                }
            }

            // Check Jira authentication if needed
            if (string.IsNullOrWhiteSpace(textBox_JiraId.Text) == false)
            {
                string jiraServer = Names.Url[(int)Names.Type.Jira];
                if (Credentials.Available(jiraServer) == false)
                {
                    m_logger.Log("Requesting Jira credentials");

                    DialogResult dialogResult = MessageBox.Show(this, "You must be authenticated with the Jira server before continuing.\n\nDo you want to authenticate now?", "Authentication Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                        RB_Tools.Shared.Authentication.Targets.Jira.Authenticate(m_logger);

                    // Check if we're still unauthenticated
                    if (Credentials.Available(jiraServer) == false)
                    {
                        m_logger.Log("Credentials still unavailable");
                        return;
                    }
                }
            }

            // Save the state of our review groups
            SaveSelectedReviewGroups();

            // Update our state
            UpdateCreateReviewDialogState(State.PostReview);

            // Do we need to keep our artifacts?
            if (checkBox_KeepArtifacts.Checked == true)
                Utilities.Storage.KeepAssets(textBox_Summary.Text, m_logger);
            Utilities.Storage.Keep(m_originalRequest, "Original File List.txt", false, m_logger);

            // Build up the list of review groups
            List<string> selectedReviewGroups = new List<string>();
            foreach (int thisIndex in checkedListBox_ReviewGroups.CheckedIndices)
            {
                Reviewboard.ReviewGroup thisGroup = m_reviewGroups[thisIndex];
                selectedReviewGroups.Add(thisGroup.InternalName);
            }

            // Build up the properties of this review
            Review.Review.Properties reviewProperties = new Review.Review.Properties(
                m_requestDirectory,

                m_reviewSource,

                textBox_ReviewId.Text,

                textBox_Summary.Text,
                textBox_Description.Text,
                textBox_Testing.Text,

                textBox_JiraId.Text.Trim().Replace(" ", ""),

                selectedReviewGroups,

                reviewLevel,
                checkBox_CopiesAsAdds.Checked
            );

            // Trigger the correct state depending on whether we are reviewing
            TriggerReviewRequest(reviewProperties);
        }

        private void button_RefreshGroups_Click(object sender, EventArgs e)
        {
            // Before we do anything, we need to be authenticated
            string reviewboardServer = Names.Url[(int)Names.Type.Reviewboard];
            if (Credentials.Available(reviewboardServer) == false)
            {
                m_logger.Log("Requesting Reviewboard credentials");

                DialogResult dialogResult = MessageBox.Show(this, "You must be authenticated with the Reviewboard server before refreshing the review groups.\n\nDo you want to authenticate now?", "Authentication Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                    RB_Tools.Shared.Authentication.Targets.Reviewboard.Authenticate(m_logger);

                // Check if we're still unauthenticated
                if (Credentials.Available(reviewboardServer) == false)
                {
                    m_logger.Log("Credentials still unavailable");
                    return;
                }
            }

            // Turn off the buttons and lock the settings
            UpdateCreateReviewDialogState(State.RefreshGroups);

            // Lose everything in the box
            checkedListBox_ReviewGroups.Items.Clear();
            m_reviewGroups = new Reviewboard.ReviewGroup[0];

            Settings.Settings.Default.Selected = string.Empty;
            Settings.Settings.Default.Groups = string.Empty;

            Settings.Settings.Default.Save();

            // Build up the background work
            BackgroundWorker updateThread = new BackgroundWorker();

            // Called when we need to trigger the request
            updateThread.DoWork += (object objectSender, DoWorkEventArgs args) =>
            {
                m_logger.Log("Starting group refresh");

                // Get our credentials
                Simple credentials = Credentials.Create(reviewboardServer, m_logger) as Simple;
                if (credentials == null)
                    throw new FileNotFoundException(@"Unable to find the credentials for " + reviewboardServer);

                // Get the groups
                Reviewboard.ReviewGroup[] result = Reviewboard.GetReviewGroups(m_requestDirectory, credentials.Server, credentials.User, credentials.Password, m_logger);

                // Save the result
                args.Result = result;
            };
            // Called when the thread is complete
            updateThread.RunWorkerCompleted += (object objectSender, RunWorkerCompletedEventArgs args) =>
            {
                // Check if we had an error
                if (args.Error != null)
                {
                    m_logger.Log("Error raised when updating review groups - {0}", args.Error.Message);
                    string body = string.Format("Exception thrown when trying to retrive the review groups\n\nException: {0}\n\nDescription: {1}", args.Error.GetType().Name, args.Error.Message);

                    // Was it an authentication error?
                    bool authenticationRequired = false;
                    if (args.Error.Message.Contains("username or password was not correct") == true)
                    {
                        authenticationRequired = true;
                        body += "\n\nDo you want to attempt to reauthenticate with the server?";
                    }

                    // Show the options
                    MessageBoxButtons buttonTypes = (authenticationRequired == true ? MessageBoxButtons.YesNo : MessageBoxButtons.OK);
                    DialogResult result = MessageBox.Show(this, body, @"Unable to update group list", buttonTypes, MessageBoxIcon.Error);

                    // Continue?
                    if (result == DialogResult.Yes && authenticationRequired == true)
                        RB_Tools.Shared.Authentication.Targets.Reviewboard.Authenticate(m_logger);
                }
                else
                {
                    m_logger.Log("Review groups successfully updated");

                    // Update the list
                    m_reviewGroups = args.Result as Reviewboard.ReviewGroup[];
                    UpdateSelectedReviewGroups(null);

                    // Save the groups we have returned
                    string groupsJson = JsonConvert.SerializeObject(m_reviewGroups);
                    Settings.Settings.Default.Groups = groupsJson;
                    Settings.Settings.Default.Save();
                }

                // Set the button state back
                UpdateCreateReviewDialogState(State.Idle);
            };

            // Kick off the request
            updateThread.RunWorkerAsync();
        }

        private void CreateReview_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If we're closing, make sure we've cleaned up
            m_logger.Log("Review dialog closing");
            OnReviewFinished(FinishReason.Closing);
        }

        private void reviewDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_logger.Log("Displaying current patch - {0}", m_reviewSource.Patch);
            System.Diagnostics.Process.Start(m_reviewSource.Patch);
        }

        private void filesForReviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_logger.Log("Listing all files in review");

            // Truncate the files and keep track of them
            StringBuilder filesToReview = new StringBuilder("The following files and folder have been included in this review\n\n");
            for (int i = 0; i < m_reviewSource.Files.Length; ++i)
            {
                string truncatedFile = Paths.TruncateLongPath(m_reviewSource.Files[i]);
                filesToReview.Append("- " + truncatedFile + '\n');

                m_logger.Log(" * {0}", m_reviewSource.Files[i]);
            }

            // Just show the list
            MessageBox.Show(this, filesToReview.ToString(), "Files in Review", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //
        // Called to manually render the review level combo box so we can disable entries
        //
        private void comboBox_ReviewLevel_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (e.Index >= 0)
            {
                if (IsReviewOptionValid(e.Index) == false)
                {
                    e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
                    e.Graphics.DrawString(comboBox.Items[e.Index].ToString(), comboBox.Font, Brushes.LightSlateGray, e.Bounds);
                }
                else
                {
                    e.DrawBackground();

                    // Set the brush according to whether the item is selected or not
                    Brush br = ((e.State & DrawItemState.Selected) > 0) ? SystemBrushes.HighlightText : SystemBrushes.ControlText;

                    e.Graphics.DrawString(comboBox.Items[e.Index].ToString(), comboBox.Font, br, e.Bounds);
                    e.DrawFocusRectangle();
                }
            }
        }

        //
        // Checks we don't select a disabled option 
        //
        private void comboBox_ReviewLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsReviewOptionValid(comboBox_ReviewLevel.SelectedIndex) == false)
                comboBox_ReviewLevel.SelectedIndex = 1;
        }

        private void jiraAuthenticationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RB_Tools.Shared.Authentication.Targets.Jira.Authenticate(m_logger);
        }

        private void reviewboardAuthenticationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show the authentication dialog
            RB_Tools.Shared.Authentication.Targets.Reviewboard.Authenticate(m_logger);
        }
    }

}
