using System;
using System.Text;

namespace GenesysCloudOAuthWebView.Core
{
    public class OAuthConfig
    {
        /// <summary>
        /// Use a static property from PureCloudRegionHosts to set the environment
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// The redirect URI for the OAuth client
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// [True] if the redirect URI does not resolve. Setting this to true will hide the control when the redirect URI is encountered.
        /// </summary>
        public bool RedirectUriIsFake { get; set; }

        /// <summary>
        /// The OAuth Client ID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The Genesys Cloud organization name to login to
        /// </summary>
        public string Org { get; set; }

        /// <summary>
        /// The login provider for single sign-on
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// An arbitrary string used to associate a login request with a login response
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// [True] to always prompt the user for credentials
        /// </summary>
        public bool ForceLoginPrompt { get; set; }

        public OAuthConfig()
        {
            RedirectUriIsFake = false;
            Environment = "mypurecloud.com";
            Org = "";
            State = "";
            Provider = "";
            ForceLoginPrompt = false;
        }

        /// <summary>
        /// Generates the Genesys Cloud login URL based on the configuration parameters set.
        /// </summary>
        /// <returns>Login URL</returns>
        public string BuildLoginUrl()
        {
            StringBuilder sb = new StringBuilder($"https://login.{Environment}/authorize?client_id={ClientId}&response_type=token&redirect_uri={RedirectUri}");

            if (!string.IsNullOrEmpty(Org) && !string.IsNullOrEmpty(Provider))
            {
                // Both Org and Provider must be defined for either to be used
                sb.Append($"&org={Org}&provider={Provider}");
            }
            if (!string.IsNullOrEmpty(State))
            {
                sb.Append($"&state={State}");
            }
            if (ForceLoginPrompt)
            {
                sb.Append($"&prompt=login");
            }

            return sb.ToString();
        }
    }
}
