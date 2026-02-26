using System;
using System.Text;
using System.Web;
using System.Net;
using System.Security.Cryptography;

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
        /// [True] for OAuth PKCE Grant (default value), [False] for OAuth Implicit Grant
        /// </summary>
        public bool IsPKCEGrant { get; set; }

        /// <summary>
        /// Storage for PKCE Code Verifier and Code Challenge
        /// </summary>
        public string PKCECodeVerifier  { get; internal set; }
        public string PKCECodeChallenge { get; internal set; }

        /// <summary>
        /// Proxy used optionally in PKCE Token request
        /// </summary>
        public IWebProxy Proxy;

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
            IsPKCEGrant = true;
            PKCECodeVerifier = "";
            PKCECodeChallenge = "";
            Proxy = null;
            Org = "";
            State = "";
            Provider = "";
            ForceLoginPrompt = false;
        }

        /// <summary>
        /// Generate a random string used as PKCE Code Verifier - length = 43 to 128.
        /// </summary>
        /// <param name="length"></param>
        /// <returns>String</returns>
        public static string GeneratePKCECodeVerifier(int length)
        {
            if (length < 43 || length > 128)
                throw new ArgumentException("Error calling GeneratePKCECodeVerifier: PKCE Code Verifier (length) must be between 43 and 128 characters");

            // String that contain both alphabets and numbers
            String unreservedCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-._~";
            // Initializing the empty string
            Random res = new Random();
            String randomString = "";
            for (int i = 0; i < length; i++) {
                // Selecting an index randomly
                int x = res.Next(unreservedCharacters.Length);
                // Appending the character at the index to the random alphanumeric string.
                randomString = randomString + unreservedCharacters[x];
            }
            return randomString;
        }

        /// <summary>
        /// Compute Base64Url PKCE Code Challenge from Code Verifier.
        /// </summary>
        /// <param name="code"></param>
        /// <returns>String</returns>
        public static string ComputePKCECodeChallenge(string code)
        {
            if (code.Length < 43 || code.Length > 128)
                throw new ArgumentException("Error calling ComputePKCECodeChallenge: PKCE Code Verifier (length) must be between 43 and 128 characters");

            var hashBase64Url = "";
            using (var sha256hash = SHA256.Create()) {
                byte[] payloadBytes = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(code));
                hashBase64Url = Convert.ToBase64String(payloadBytes);
                hashBase64Url = hashBase64Url.Replace('+', '-').Replace('/', '_');
                hashBase64Url = hashBase64Url.Split('=')[0];
            }
            return hashBase64Url;
        }

        /// <summary>
        /// Generates the Genesys Cloud login URL based on the configuration parameters set.
        /// </summary>
        /// <returns>Login URL</returns>
        public string BuildLoginUrl()
        {
            StringBuilder sb = new StringBuilder($"https://login.{Environment}/oauth/authorize?client_id={ClientId}&redirect_uri={RedirectUri}");

            if (IsPKCEGrant)
            {
                // Login (oauth/authorize) Url for PKCE OAuth Grant
                PKCECodeVerifier = OAuthConfig.GeneratePKCECodeVerifier(128);
                PKCECodeChallenge = OAuthConfig.ComputePKCECodeChallenge(PKCECodeVerifier);
                sb.Append($"&response_type=code&code_challenge={HttpUtility.UrlEncode(PKCECodeChallenge)}&code_challenge_method=S256");
            }
            else
            {
                // Login (oauth/authorize) Url for Implicit OAuth Grant
                sb.Append($"&response_type=token");
            }
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
