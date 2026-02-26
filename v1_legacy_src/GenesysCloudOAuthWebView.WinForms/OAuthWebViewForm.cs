using System;
using System.Windows.Forms;

namespace GenesysCloudOAuthWebView.WinForms
{
    public partial class OAuthWebViewForm : Form
    {
        public OAuthWebViewForm()
        {
            InitializeComponent();

            oAuthWebView.Authenticated += OAuthWebViewOnAuthenticated;
        }

        private void OAuthWebViewOnAuthenticated(Core.OAuthResponse response)
        {
            try
            {
                // Close the browser
                if (!string.IsNullOrEmpty(response.AccessToken))
                    Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private DialogResult ShowImpl(bool dialog, IWin32Window owner = null)
        {
            try
            {
                // Validate settings have been set
                if (string.IsNullOrEmpty(oAuthWebView.Config.ClientId))
                    throw new OAuthSettingsValidationException("ClientId");
                if (string.IsNullOrEmpty(oAuthWebView.Config.RedirectUri))
                    throw new OAuthSettingsValidationException("RedirectUri");
                if (!string.IsNullOrEmpty(oAuthWebView.Config.Org) && string.IsNullOrEmpty(oAuthWebView.Config.Provider))
                    throw new OAuthSettingsValidationException("Provider must be set if Org is set");
                if (string.IsNullOrEmpty(oAuthWebView.Config.Org) && !string.IsNullOrEmpty(oAuthWebView.Config.Provider))
                    throw new OAuthSettingsValidationException("Org must be set if Provider is set");

                // Navigate the browser (can't do this after ShowDialog has been called)
                oAuthWebView.BeginImplicitGrant();

                // Open window
                if (dialog)
                    base.ShowDialog(owner);
                else
                    base.Show(owner);

                // Return result based on if we got an access token
                return string.IsNullOrEmpty(oAuthWebView.AccessToken) ? DialogResult.Cancel : DialogResult.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        public new DialogResult ShowDialog()
        {
            return ShowImpl(true);
        }

        public new DialogResult ShowDialog(IWin32Window owner)
        {
            return ShowImpl(true, owner);
        }

        public new DialogResult Show()
        {
            return ShowImpl(false);
        }

        public new DialogResult Show(IWin32Window owner)
        {
            return ShowImpl(false, owner);
        }
    }
}
