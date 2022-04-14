using GenesysCloudOAuthWebView.Core;
using Xunit;

namespace GenesysCloudOAuthWebView.Tests
{
    public class OAuthResponseUnitTests
    {
        private OAuthConfig generateConfig()
        {
            return new OAuthConfig
            {
                Environment = "usw2.pure.cloud",
                ClientId = "00000000-0000-0000-0000-000000000000",
                RedirectUri = "http://localhost"
            };
        }

        [Fact]
        public void ParseResponse_NoFragment()
        {
            var response = OAuthResponse.ParseOAuthResponse(generateConfig(), "https://www.google.com", null);

            Assert.Null(response.AccessToken);
        }

        [Fact]
        public void ParseResponse_NonUrl()
        {
            var response = OAuthResponse.ParseOAuthResponse(generateConfig(), "1", null);

            Assert.Null(response);
        }

        [Fact]
        public void ParseResponse_Token()
        {
            var response = OAuthResponse.ParseOAuthResponse(generateConfig(), "http://localhost/#access_token=1234&expires_in=60", null);

            Assert.True(response.AccessToken == "1234" && response.TokenExpiresInSeconds == 60);
        }

        [Fact]
        public void ParseResponse_TokenWrongUrl()
        {
            var response = OAuthResponse.ParseOAuthResponse(generateConfig(), "http://google.com/#access_token=1234&expires_in=60", null);

            Assert.Null(response.AccessToken);
        }

        [Fact]
        public void ParseResponse_ErrorNoRedirect()
        {
            var response = OAuthResponse.ParseOAuthResponse(generateConfig(), "https://login.usw2.pure.cloud/#/error?errorKey=invalidClientId", (source, exception) =>
            {
                Assert.Equal("invalidClientId", exception.Message);
            });

            Assert.Null(response);
        }

        [Fact]
        public void ParseResponse_ErrorRedirect()
        {
            var response = OAuthResponse.ParseOAuthResponse(generateConfig(), "http://localhost/#error=unauthorized_client", (source, exception) =>
            {
                Assert.Equal("Error: unauthorized_client", exception.Message);
            });

            Assert.Null(response);
        }

        [Fact]
        public void ParseResponse_ErrorScope()
        {
            var response = OAuthResponse.ParseOAuthResponse(generateConfig(), "http://localhost/#description=one+or+more+scopes+do+not+exist+or+are+not+allowed&error=invalid_scope", (source, exception) =>
            {
                Assert.Equal("Error: invalid_scope, one or more scopes do not exist or are not allowed", exception.Message);
            });

            Assert.Null(response);
        }

        [Fact]
        public void ParseResponse_ErrorScopeNoDescription()
        {
            var response = OAuthResponse.ParseOAuthResponse(generateConfig(), "http://localhost/#error=oh_no", (source, exception) =>
            {
                Assert.Equal("Error: oh_no", exception.Message);
            });

            Assert.Null(response);
        }

        [Fact]
        public void ParseResponse_ErrorScopeReversed()
        {
            var response = OAuthResponse.ParseOAuthResponse(generateConfig(), "http://localhost/#error=invalid_scope&description=one+or+more+scopes+do+not+exist+or+are+not+allowed", (source, exception) =>
            {
                Assert.Equal("Error: invalid_scope, one or more scopes do not exist or are not allowed", exception.Message);
            });

            Assert.Null(response);
        }
    }
}
