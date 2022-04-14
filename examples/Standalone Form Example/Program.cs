using GenesysCloudOAuthWebView.WinForms;
using System;

namespace Standalone_Form_Example
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Create form
                var form = new OAuthWebViewForm();

                // Set settings
                form.oAuthWebView.Config = new GenesysCloudOAuthWebView.Core.OAuthConfig
                {
                    ClientId = "babbc081-0761-4f16-8f56-071aa402ebcb",
                    RedirectUri = "http://localhost:8080",
                    RedirectUriIsFake = true
                };

                // Open it
                var result = form.ShowDialog();

                Console.WriteLine($"Result: {result}");
                Console.WriteLine($"AccessToken: {form.oAuthWebView.AccessToken}");

                Console.WriteLine("Application complete.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
