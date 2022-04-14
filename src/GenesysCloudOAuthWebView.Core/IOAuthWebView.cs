using System;
using System.Collections.Generic;
using System.Text;

namespace GenesysCloudOAuthWebView.Core
{
    public interface IOAuthWebView
    {
        /// <summary>
        /// The access token returned after authenticating.
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// The configuration to use for the web view.
        /// </summary>
        OAuthConfig Config { get; set; }

        /// <summary>
        /// Raised when an exception occurs during the authentication process
        /// </summary>
        event ExceptionEncounteredDelegate ExceptionEncountered;

        /// <summary>
        /// Raised when an Access Token is successfully retrieved
        /// </summary>
        event AuthenticatedDelegate Authenticated;

        /// <summary>
        /// Initiates the Implicit Grant OAuth flow
        /// </summary>
        void BeginImplicitGrant();
    }
}
