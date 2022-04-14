using GenesysCloudOAuthWebView.Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WPFExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _clientId = "";
        private string _authToken = "";


        public string ClientId
        {
            get { return _clientId; }
            set
            {
                if (value == _clientId) return;
                _clientId = value;
                OnPropertyChanged();
            }
        }

        public string AuthToken
        {
            get { return _authToken; }
            set
            {
                if (value == _authToken) return;
                _authToken = value;
                OnPropertyChanged();
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            // Set up browser
            AuthBrowser.Config = new OAuthConfig
            {
                RedirectUri = "http://invaliduri/",
                RedirectUriIsFake = true
            };
            AuthBrowser.Authenticated += response =>
            {
                if (response == null || string.IsNullOrEmpty(response.AccessToken))
                {
                    Console.WriteLine($"[ERROR] Auth token missing in response");
                    return;
                }
                Console.WriteLine($"Token => {response.AccessToken}");
                AuthToken = response.AccessToken;
            };
            AuthBrowser.ExceptionEncountered += (source, exception) =>
            {
                Console.WriteLine($"[ERROR] {source} => {exception.Message}");
                Console.WriteLine(exception);
            };
        }

        private void StartImplicitGrant_Click(object sender, RoutedEventArgs e)
        {
            AuthBrowser.Config.ClientId = ClientId.Trim();
            AuthBrowser.BeginImplicitGrant();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
