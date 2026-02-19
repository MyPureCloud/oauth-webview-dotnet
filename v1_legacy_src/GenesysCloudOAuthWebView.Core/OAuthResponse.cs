using System;
using System.Linq;
using System.Text;
using System.Web;

namespace GenesysCloudOAuthWebView.Core
{
    public class OAuthResponse
    {
        /// <summary>
        /// The access token provided by Genesys Cloud
        /// </summary>
        public string AccessToken { get; internal set; }

        /// <summary>
        /// The duration, in seconds, until the access token expires
        /// </summary>
        public int TokenExpiresInSeconds { get; internal set; }

        /// <summary>
        /// Parses the OAuth response from Genesys Cloud into an OAuthResponse object
        /// </summary>
        /// <param name="config">The desired OAuth configuration</param>
        /// <param name="responseUriString">The response URI from Genesys Cloud</param>
        /// <param name="exceptionEncounteredDelegate">Delegate method to perform exception handling</param>
        /// <returns></returns>
        public static OAuthResponse ParseOAuthResponse(OAuthConfig config, string responseUriString, ExceptionEncounteredDelegate exceptionEncounteredDelegate)
        {
            Uri responseUri;

            try
            {
                responseUri = new Uri(responseUriString);
            }
            catch (Exception e)
            {
                exceptionEncounteredDelegate?.Invoke("ParseOAuthResponse", new Exception(e.Message));
                return null;
            }

            // Check for errors
            if (responseUri.Fragment.StartsWith("#/error?"))
            {
                // Strip leading part of path and parse
                var errorFragment = HttpUtility.ParseQueryString(responseUri.Fragment.Substring(8));

                // Check for errorKey parameter
                if (errorFragment.AllKeys.Contains("errorKey"))
                {
                    exceptionEncounteredDelegate?.Invoke("ParseOAuthResponse", new Exception(errorFragment["errorKey"]));
                }
                else
                {
                    exceptionEncounteredDelegate?.Invoke("ParseOAuthResponse", new Exception($"Unknown error returned in fragment: {responseUri.Fragment}"));
                }

                return null;
            }

            // Redirect with error
            if (responseUri.Fragment.StartsWith("#error=") || responseUri.Fragment.Contains("&error="))
            {
                // Strip leading part of path and parse
                var errorFragment = HttpUtility.ParseQueryString(responseUri.Fragment.TrimStart('#'));
                StringBuilder sb = new StringBuilder("Error: ");

                if (errorFragment.AllKeys.Contains("error"))
                {
                    sb.Append(errorFragment["error"]);
                }

                if (errorFragment.AllKeys.Contains("description"))
                {
                    sb.Append(", ");
                    sb.Append(errorFragment["description"]);
                }

                exceptionEncounteredDelegate?.Invoke("ParseOAuthResponse", new Exception(sb.ToString()));

                return null;
            }

            OAuthResponse response = new OAuthResponse();

            // Process our redirect URL
            if (responseUri.ToString().ToLowerInvariant().StartsWith(config.RedirectUri.ToLowerInvariant()))
            {
                var fragment = HttpUtility.ParseQueryString(responseUri.Fragment.TrimStart('#'));

                // Get the token from the redirect URI (implicit grant)
                if (fragment.AllKeys.Contains("expires_in"))
                {
                    var i = 0;
                    if (int.TryParse(fragment["expires_in"], out i))
                        response.TokenExpiresInSeconds = i;
                }
                if (fragment.AllKeys.Contains("access_token"))
                {
                    response.AccessToken = fragment["access_token"];
                }
            }

            return response;
        }
    }
}
