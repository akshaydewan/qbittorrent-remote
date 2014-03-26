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
        public List<Torrent> ActiveTorrents { get; set; }
        public List<Torrent> SeedingTorrents { get; set; }
        public List<Torrent> DownloadingTorrents { get; set; }
        public List<Torrent> AllTorrents { get; set; }


        // Constructor-----------------------------------------------------------------------------
        public MainPage()
        {
            InitializeComponent();
            storage = new StorageWrapper(IsolatedStorageSettings.ApplicationSettings);
            ActiveTorrents = new List<Torrent>();
            SeedingTorrents = new List<Torrent>();
            DownloadingTorrents = new List<Torrent>();
            AllTorrents = new List<Torrent>();

        }

        // Event Handlers--------------------------------------------------------------------------
        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            refreshTorrents();
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

        //-----------------------------------------------------------------------------------------
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
            QBittorrentAPI api = new QBittorrentAPI(authSettings);
            api.FetchAllTorrents(TorrentsReceived, TorrentsRecvError);
        }

        public void TorrentsReceived(List<Torrent> torrents)
        {
            ActiveTorrents.Clear();
            DownloadingTorrents.Clear();
            SeedingTorrents.Clear();
            torrents.ForEach(t =>
            {
                if (t.TorrentState.Active)
                {
                    ActiveTorrents.Add(t);
                }
                if (t.TorrentState.Downloading)
                {
                    DownloadingTorrents.Add(t);
                }
                if (t.TorrentState.Seeding)
                {
                    SeedingTorrents.Add(t);
                }
            });
            AllTorrents = torrents;
            AllTorrentsListBox.ItemsSource = torrents;
            ActiveListBox.ItemsSource = ActiveTorrents;
            DownloadingListBox.ItemsSource = DownloadingTorrents;
            SeedingListBox.ItemsSource = SeedingTorrents;
        }

        public void TorrentsRecvError()
        {
            MessageBox.Show("Failed to fetch torrents. Please check your settings");
        }
    }
}