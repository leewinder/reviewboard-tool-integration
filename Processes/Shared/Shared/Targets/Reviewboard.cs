using Newtonsoft.Json.Linq;
using RB_Tools.Shared.Authentication.Credentials;
using RB_Tools.Shared.Server;
using RB_Tools.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Targets
{
    // Shared information about the RB target
    public class Reviewboard
    {
        // API Codes
        public enum Result
        {
            Success = 200,
            NotFound = 100,

            Error = 500,
        };
        
        // API call result
        public class RequestApiResult
        {
            public Result   Code { get; private set; }

            public JObject  Result { get; private set; }
            public string   Error { get; private set; }

            // Constructor
            public RequestApiResult(Result code, JObject result, string error)
            {
                Code = code;

                Result = result;
                Error = error;
            }
        }
        //
        // Returns the path to the reviewboard executable
        //
        public static string Path()
        {
            // Do we already have it
            if (string.IsNullOrWhiteSpace(RbtPath) == false)
                return RbtPath;

            // Get the path
            Utilities.Process.Output output = Utilities.Process.Start(string.Empty, "where", "rbt");

            // Break up the paths given
            string[] pathsReturned = output.StdOut.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string thisPath in pathsReturned)
            {
                if (thisPath.ToLower().Contains("rbt.cmd") == true)
                    RbtPath = thisPath;

            }

            // Could we find it?
            if (string.IsNullOrWhiteSpace(RbtPath) == true)
                throw new System.ArgumentNullException("Unable to find the rbt.cmd path");

            // Done
            return RbtPath;
        }

        //
        // Runs an API request
        //
        public static RequestApiResult RequestApi(Simple credentials, string apiRequest, string workingDirectory)
        {
            // Build up the RBT request
            string rbtPath = Path();
            string rbtArgs = string.Format(@"api-get --username {0} --password {1} --server {2} {3}", credentials.User, credentials.Password, Names.Url[(int)Names.Type.Reviewboard], apiRequest);

            Process.Output output = Process.Start(workingDirectory, rbtPath, rbtArgs);
            if (string.IsNullOrEmpty(output.StdErr) == false)
                return new RequestApiResult(Result.Error, null, output.StdErr);

            // Parse the data
            try
            {
                JObject parsedOutput = JObject.Parse(output.StdOut);

                // Good or bad?
                string result = (string)parsedOutput["stat"];
                if (result.Equals("fail", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    // This was a bad response, so pull out the message
                    string errorCode = (string)parsedOutput["err"]["code"];
                    string errorMessage = (string)parsedOutput["err"]["msg"];
                    string returnMessage = string.Format("Error Code: {0}\n{1}", errorCode, errorMessage);

                    // Do we have a valid code?
                    int resultCode = int.Parse(errorCode);
                    bool knownResult = Enum.IsDefined(typeof(Result), resultCode);
                    if (knownResult == true)
                        return new RequestApiResult((Result)resultCode, null, returnMessage);

                    // Return a general error
                    return new RequestApiResult(Result.Error, null, returnMessage);
                }
                else
                {
                    // We had a valid response so return it
                    return new RequestApiResult(Result.Success, parsedOutput, string.Empty);
                }
            }
            catch
            {
                return new RequestApiResult(Result.Error, null, @"Unable to parse the Reviewboard API result");
            }
        }

        // Private properties
        private static string RbtPath { get; set; }
    }
}
