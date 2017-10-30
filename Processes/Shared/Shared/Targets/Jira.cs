using RB_Tools.Shared.Authentication.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Targets
{
    //
    // Interface for working with Jira
    //
    public class Jira
    {
        private static Atlassian.Jira.Jira _jiraInstance = null;
        private static Atlassian.Jira.Jira GetJiraInstance(Simple credentials)
        {
            if (_jiraInstance != null)
            {
                return _jiraInstance;
            }

            try
            {
                _jiraInstance = Atlassian.Jira.Jira.CreateRestClient(credentials.Server, credentials.User, credentials.Password);
            }
            catch (Exception e)
            {
                string message = string.Format("Unable to generate Jira instance for user '{0}'\n\n{1}", credentials.User, e.InnerException.Message);
                throw new InvalidOperationException(message);
            }

            return _jiraInstance;
        }
        //
        // Validates if a Jira ticket exists
        //
        public static bool ValidateJiraTicker(Simple credentials, string ticketId)
        {
            // Create our rest client
            Atlassian.Jira.Jira jiraInstance = GetJiraInstance(credentials);
            try
            {
                var issueTask = jiraInstance.Issues.GetIssueAsync(ticketId);
                issueTask.Wait();
            }
            catch (Exception e)
            {
                string message = string.Format("Unable to find Jira ticket '{0}'\n\n{1}", ticketId, e.InnerException.Message);
                throw new InvalidOperationException(message);
            }
            
            // We found it just fine
            return true;
        }

        /// <summary>
        /// Posts a message to a given jira ticket as a comment
        /// </summary>
        /// <param name="credentials">Users jira credentials</param>
        /// <param name="message">Message to be posted as a comment</param>
        /// <param name="ticketId">Jira ticket ID to post comment on</param>
        /// <returns>bool indicating success of posting a comment.</returns>
        public static bool PostMessageToJiraTicket(Simple credentials, string message, string ticketId)
        {
            Atlassian.Jira.Jira jiraInstance = GetJiraInstance(credentials);
            try
            {
                var issueTask = jiraInstance.Issues.GetIssueAsync(ticketId);
                issueTask.Wait();
                jiraInstance.GetIssue(ticketId).AddComment(message);
            }
            catch (Exception e)
            {
                string exceptionMessage = string.Format("Unable to post log message to jira ticket '{0}'\n\n{1}", ticketId, e.InnerException.Message);
                throw new InvalidOperationException(exceptionMessage);
            }

            return true;
        }
    }
}
