using GenesysCloudOAuthWebView.Core;
using System;
using System.Windows.Forms;

namespace WinFormsExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            oAuthWebBrowser1.Config = new OAuthConfig
            {
                RedirectUri = "http://invaliduri/",
                RedirectUriIsFake = true
            };

            // Handle events
            oAuthWebBrowser1.ExceptionEncountered +=
                (source, exception) =>
                {
                    Console.WriteLine($"[ERROR] {source} => {exception.Message}");
                    Console.WriteLine(exception);
                };
            oAuthWebBrowser1.Authenticated += response =>
            {
                if (response == null || string.IsNullOrEmpty(response.AccessToken))
                {
                    Console.WriteLine($"[ERROR] Auth token missing in response");
                    return;
                }
                Console.WriteLine($"Token => {response.AccessToken}");
                txtToken.Text = response.AccessToken;
            };
        }

        private void btnImplicitGrant_Click(object sender, EventArgs e)
        {
            oAuthWebBrowser1.Config.ClientId = txtClientId.Text.Trim();
            oAuthWebBrowser1.BeginImplicitGrant();
        }
    }
}
