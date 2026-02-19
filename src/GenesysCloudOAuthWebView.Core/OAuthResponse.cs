using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace GenesysCloudOAuthWebView.Core
{
    public class TokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }

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
            StringBuilder sb;

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

            // Redirect with error (hash)
            if (responseUri.Fragment.StartsWith("#error=") || responseUri.Fragment.Contains("&error="))
            {
                // Strip leading part of path and parse
                var errorFragment = HttpUtility.ParseQueryString(responseUri.Fragment.TrimStart('#'));
                sb = new StringBuilder("Error: ");

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

            // Redirect with error (query)
            if (responseUri.Query.StartsWith("?error=") || responseUri.Query.Contains("&error="))
            {
                // Strip leading part of path and parse
                var errorQuery = HttpUtility.ParseQueryString(responseUri.Query.TrimStart('?'));
                sb = new StringBuilder("Error: ");

                if (errorQuery.AllKeys.Contains("error"))
                {
                    sb.Append(errorQuery["error"]);
                }

                if (errorQuery.AllKeys.Contains("description"))
                {
                    sb.Append(", ");
                    sb.Append(errorQuery["description"]);
                }

                exceptionEncounteredDelegate?.Invoke("ParseOAuthResponse", new Exception(sb.ToString()));

                return null;
            }

            OAuthResponse response = new OAuthResponse();

            // Process our redirect URL
            if (responseUri.ToString().ToLowerInvariant().StartsWith(config.RedirectUri.ToLowerInvariant()))
            {
                if (config.IsPKCEGrant)
                {
                    // PKCE Grant Flow
                    var query = HttpUtility.ParseQueryString(responseUri.Query.TrimStart('?'));

                    // Get the code from the redirect URI (pkce grant)
                    if (query.AllKeys.Contains("code"))
                    {
                        var code = query["code"];

                        // Create an HttpClient instance
                        HttpClient httpClient;
                        if (config.Proxy != null) {
                            var handler = new HttpClientHandler
                            {
                                Proxy = config.Proxy
                            };

                            httpClient = new HttpClient(handler);
                        } else {
                            httpClient = new HttpClient();
                        }
                        // Timeout for the HTTP Request
                        httpClient.Timeout = TimeSpan.FromSeconds(10);

                        StringBuilder sbUrl = new StringBuilder($"https://login.{config.Environment}/oauth/token");

                        // Add the required headers
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                        // Prepare the request data
                        var formParams = new Dictionary<string, string>();
                        formParams.Add("grant_type", "authorization_code");
                        formParams.Add("code", code);
                        formParams.Add("code_verifier", config.PKCECodeVerifier);
                        formParams.Add("client_id", config.ClientId);
                        formParams.Add("redirect_uri", config.RedirectUri);

                        var content = new FormUrlEncodedContent(formParams);

                        // Send the POST request
                        HttpResponseMessage httpResponse;
                        try
                        {
                            httpResponse = httpClient
                                .PostAsync(sbUrl.ToString(), content)
                                .ConfigureAwait(false)
                                .GetAwaiter()
                                .GetResult();
                        }
                        catch (Exception ex)
                        {
                            exceptionEncounteredDelegate?.Invoke("ParseOAuthResponse", ex);
                            return null;
                        }

                        int statusCode = (int)httpResponse.StatusCode;

                        if (statusCode >= 400 || statusCode == 0)
                        {
                            sb = new StringBuilder("Error calling PostToken");
                            exceptionEncounteredDelegate?.Invoke("ParseOAuthResponse", new Exception(sb.ToString()));
                            return null;
                        }

                        // Read the response
                        string responseBodyJsonString;
                        try
                        {
                            responseBodyJsonString = httpResponse.Content
                                .ReadAsStringAsync()
                                .ConfigureAwait(false)
                                .GetAwaiter()
                                .GetResult();
                        }
                        catch (Exception ex)
                        {
                            exceptionEncounteredDelegate?.Invoke("ParseOAuthResponse", ex);
                            return null;
                        }

                        var oData = JsonConvert.DeserializeObject<TokenResponse>(responseBodyJsonString);


                        response.TokenExpiresInSeconds = oData.expires_in;
                        response.AccessToken = oData.access_token;
                        return response;
                    }
                }
                else
                {
                    // Implicit Grant Flow
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
                        return response;
                    }
                }
            }

            return null;
        }
    }
}
