using System;

namespace GenesysCloudOAuthWebView.WinForms
{
    public class OAuthSettingsValidationException : Exception
    {
        public OAuthSettingsValidationException()
        {

        }

        public OAuthSettingsValidationException(string property) : base($"Invalid value: {property}")
        {

        }
    }
}