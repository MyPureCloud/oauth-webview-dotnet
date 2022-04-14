using GenesysCloudOAuthWebView.Core;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;

namespace GenesysCloudOAuthWebView.WinForms
{
    public class OAuthWebView : WebView2, IOAuthWebView
    {
        #region Public members

        public string AccessToken { get; private set; }

        public OAuthConfig Config { get; set; }

        public event AuthenticatedDelegate Authenticated;

        public event ExceptionEncounteredDelegate ExceptionEncountered;

        #endregion

        public OAuthWebView()
        {
            Config = new OAuthConfig();
            NavigationStarting += OAuthWebView_NavigationStarting;
        }

        #region Private methods

        private void OAuthWebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            try
            {
                // Ignore the navigated event after we have an access token
                if (!string.IsNullOrEmpty(AccessToken)) return;

                OAuthResponse parsedResponse = OAuthResponse.ParseOAuthResponse(Config, e.Uri, RaiseExceptionEncountered);

                if (parsedResponse == null || string.IsNullOrEmpty(parsedResponse.AccessToken))
                {
                    return;
                }

                // Receiving an access token indicates authentication was successful
                RaiseAuthenticated(parsedResponse);

                if (Config.RedirectUriIsFake)
                {
                    Visible = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                RaiseExceptionEncountered("OAuthWebView.NavigationStarting", ex);
            }
        }

        private void RaiseExceptionEncountered(string source, Exception ex)
        {
            ExceptionEncountered?.Invoke(source, ex);
        }

        private void RaiseAuthenticated(OAuthResponse response)
        {
            AccessToken = response.AccessToken;
            Authenticated?.Invoke(response);
        }

        #endregion

        #region Public methods

        public void BeginImplicitGrant()
        {
            // Clear existing token
            AccessToken = "";

            // Navigate to the login URL
            Source = new Uri(Config.BuildLoginUrl());

            // Show control if we hid ourselves during a previous auth attempt
            if (Config.RedirectUriIsFake && !Visible)
            {
                Visible = true;
            }
        }

        #endregion
    }
}
