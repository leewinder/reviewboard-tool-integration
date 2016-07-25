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
        //
        // Validates if a Jira ticket exists
        //
        public static bool ValidateJiraTicker(Simple credentials, string ticketId)
        {
            // Create our rest client
            Atlassian.Jira.Jira jiraInstance = null;
            try
            {
                jiraInstance = Atlassian.Jira.Jira.CreateRestClient(credentials.Server, credentials.User, credentials.Password);
            }
            catch (Exception e)
            {
                string message = string.Format("Unable to generate Jira instance for user '{0}'\n\n{1}", credentials.User, e.InnerException.Message);
                throw new InvalidOperationException(message);
            }

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
    }
}
