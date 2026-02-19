using System;

namespace GenesysCloudOAuthWebView.Core
{
    /// <summary>
    /// Delegate signature for when an exception occurs during the authentication process
    /// </summary>
    public delegate void ExceptionEncounteredDelegate(string source, Exception ex);

    /// <summary>
    /// Delegate signature for when an access token is successfully retrieved
    /// </summary>
    public delegate void AuthenticatedDelegate(OAuthResponse response);
}
