using GenesysCloudOAuthWebView.Core;
using Xunit;

namespace GenesysCloudOAuthWebView.Tests
{
    public class OAuthConfigUnitTests
    {
        [Fact]
        public void BuildUrl_Basic()
        {
            OAuthConfig config = new OAuthConfig
            {
                Environment = "usw2.pure.cloud",
                ClientId = "00000000-0000-0000-0000-000000000000",
                RedirectUri = "http://localhost"
            };

            Assert.Equal("https://login.usw2.pure.cloud/authorize?client_id=00000000-0000-0000-0000-000000000000&response_type=token&redirect_uri=http://localhost", config.BuildLoginUrl());
        }

        [Fact]
        public void BuildUrl_Sso()
        {
            OAuthConfig config = new OAuthConfig
            {
                Environment = "usw2.pure.cloud",
                ClientId = "00000000-0000-0000-0000-000000000000",
                RedirectUri = "http://localhost",
                Org = "OrgName",
                Provider = "adfs"
            };

            Assert.Equal("https://login.usw2.pure.cloud/authorize?client_id=00000000-0000-0000-0000-000000000000&response_type=token&redirect_uri=http://localhost&org=OrgName&provider=adfs", config.BuildLoginUrl());
        }

        [Fact]
        public void BuildUrl_Sso_InvalidProviderOnly()
        {
            OAuthConfig config = new OAuthConfig
            {
                Environment = "usw2.pure.cloud",
                ClientId = "00000000-0000-0000-0000-000000000000",
                RedirectUri = "http://localhost",
                Provider = "adfs"
            };

            Assert.Equal("https://login.usw2.pure.cloud/authorize?client_id=00000000-0000-0000-0000-000000000000&response_type=token&redirect_uri=http://localhost", config.BuildLoginUrl());
        }

        [Fact]
        public void BuildUrl_Sso_InvalidOrgOnly()
        {
            OAuthConfig config = new OAuthConfig
            {
                Environment = "usw2.pure.cloud",
                ClientId = "00000000-0000-0000-0000-000000000000",
                RedirectUri = "http://localhost",
                Org = "OrgName"
            };

            Assert.Equal("https://login.usw2.pure.cloud/authorize?client_id=00000000-0000-0000-0000-000000000000&response_type=token&redirect_uri=http://localhost", config.BuildLoginUrl());
        }

        [Fact]
        public void BuildUrl_State()
        {
            OAuthConfig config = new OAuthConfig
            {
                Environment = "usw2.pure.cloud",
                ClientId = "00000000-0000-0000-0000-000000000000",
                RedirectUri = "http://localhost",
                State = "TestState"
            };

            Assert.Equal("https://login.usw2.pure.cloud/authorize?client_id=00000000-0000-0000-0000-000000000000&response_type=token&redirect_uri=http://localhost&state=TestState", config.BuildLoginUrl());
        }

        [Fact]
        public void BuildUrl_ForceLogin()
        {
            OAuthConfig config = new OAuthConfig
            {
                Environment = "usw2.pure.cloud",
                ClientId = "00000000-0000-0000-0000-000000000000",
                RedirectUri = "http://localhost",
                ForceLoginPrompt = true
            };

            Assert.Equal("https://login.usw2.pure.cloud/authorize?client_id=00000000-0000-0000-0000-000000000000&response_type=token&redirect_uri=http://localhost&prompt=login", config.BuildLoginUrl());
        }
    }
}
