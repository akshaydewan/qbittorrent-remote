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
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {

        private StorageWrapper storage;
        private AuthSettings authSettings;
        public bool settingsCorrect { get; set; }
        public Collection<Torrent> ActiveTorrents { get; set; }
        public Collection<Torrent> SeedingTorrents { get; set; }
        public Collection<Torrent> DownloadingTorrents { get; set; }
        public Collection<Torrent> AllTorrents { get; set; }


        // Constructor-----------------------------------------------------------------------------
        public MainPage()
        {
            InitializeComponent();
            storage = new StorageWrapper(IsolatedStorageSettings.ApplicationSettings);
            ActiveTorrents = new ObservableCollection<Torrent>();
            SeedingTorrents = new ObservableCollection<Torrent>();
            DownloadingTorrents = new ObservableCollection<Torrent>();
            AllTorrents = new ObservableCollection<Torrent>();

            AllTorrentsListBox.ItemsSource = AllTorrents;
            ActiveListBox.ItemsSource = ActiveTorrents;
            DownloadingListBox.ItemsSource = DownloadingTorrents;
            SeedingListBox.ItemsSource = SeedingTorrents;
        }

        // Event Handlers--------------------------------------------------------------------------
        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            authSettings = storage.LoadAuthSettings();
            settingsCorrect = areSettingsValid();
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
        private bool areSettingsValid()
        {
            bool invalid = string.IsNullOrWhiteSpace(authSettings.Host)
                            || authSettings.Port <= 0
                            || string.IsNullOrWhiteSpace(authSettings.Username)
                            || string.IsNullOrWhiteSpace(authSettings.Password);
            return !invalid;
        }

        private void showProgressBar(bool show)
        {
            ProgressBar.IsVisible = show;
        }

        private void refreshTorrents()
        {
            clearTorrents();
            showProgressBar(true);
            QBittorrentAPI api = new QBittorrentAPI(authSettings);
            api.FetchAllTorrents(TorrentsReceived, TorrentsRecvError);
        }

        private void clearTorrents()
        {
            ActiveTorrents.Clear();
            DownloadingTorrents.Clear();
            SeedingTorrents.Clear();
            AllTorrents.Clear();
        }

        public void TorrentsReceived(List<Torrent> torrents)
        {
            Debug.WriteLine("Torrents Received");
            if (torrents != null)
            {
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
                    AllTorrents.Add(t);
                });
            }
            showProgressBar(false);
        }

        public void TorrentsRecvError()
        {
            Debug.WriteLine("Error while receiving torrents");
            MessageBox.Show("Failed to fetch torrents. Please check your settings");
            showProgressBar(false);
        }
    }
}
