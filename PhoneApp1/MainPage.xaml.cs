using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using RestSharp;

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {

        private StorageWrapper storage;
        private AuthSettings authSettings;
        public bool settingsCorrect { get; set; }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            storage = new StorageWrapper(IsolatedStorageSettings.ApplicationSettings);
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            authSettings = storage.LoadAuthSettings();
            settingsCorrect = checkSettings();
            if (settingsCorrect)
            {
                ErrorBlock.Visibility = System.Windows.Visibility.Collapsed;
                refreshTorrents();
            }
            else
            {
                ErrorBlock.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private bool checkSettings()
        {
            bool invalid = string.IsNullOrWhiteSpace(authSettings.Host)
                            || authSettings.Port <= 0
                            || string.IsNullOrWhiteSpace(authSettings.Username)
                            || string.IsNullOrWhiteSpace(authSettings.Password);
            return !invalid;
        }

        private void refreshTorrents()
        {
            var client = new RestClient("http://" + authSettings.Host + ":" + authSettings.Port);
            //client.Authenticator = new HttpBasicAuthenticator(authSettings.Username, authSettings.Password);
            var request = new RestRequest();
            request.Resource = "/json/torrents";
            request.Credentials = new NetworkCredential(authSettings.Username, authSettings.Password);
            client.ExecuteAsync(request, (response) =>
            {
                Console.WriteLine(response.StatusCode);
                Console.Write(response.RawBytes);
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    //success(response.Data);
                    Console.WriteLine(response.Content);
                }
                else
                {
                    //failure(response.ErrorMessage);
                    Console.WriteLine(response.ErrorMessage);
                }
            });

        }
    }
}