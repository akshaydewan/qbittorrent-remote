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
using PhoneApp1;
using System.IO.IsolatedStorage;
using System.Diagnostics;

namespace QBittorrentRemote
{
    public partial class DetailPage : PhoneApplicationPage
    {

        private StorageWrapper storage;
        private AuthSettings authSettings;
        private string torrentHash;

        public Torrent torrent { get; set; }

        private string err = "Unable to fetch torrent details. Please check your settings and connection";

        public DetailPage()
        {
            InitializeComponent();
            storage = new StorageWrapper(IsolatedStorageSettings.ApplicationSettings);
            authSettings = storage.LoadAuthSettings();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.TryGetValue("selectedTorrent", out torrentHash))
            {
                fetchTorrentDetails(torrentHash);
            }
            else
            {
                Debug.WriteLine("Hash value was not passed to page");
                MessageBox.Show("Oops! Couldn't get the selected torrent");
            }
        }

        private void fetchTorrentDetails(string torrentHash)
        {
            showProgressBar(true);
            QBittorrentAPI api = new QBittorrentAPI(authSettings);
            api.FetchAllTorrents(TorrentsReceived, TorrentsRecvError);
        }

        public void TorrentsReceived(List<Torrent> torrents)
        {
            Debug.WriteLine("Torrents Received");
            if (torrents == null)
            {
                Debug.WriteLine("Torrent list was null");
                MessageBox.Show(err);
                NavigationService.GoBack();
            }
            torrent = findTorrent(torrents, torrentHash);
            if (torrent == null)
            {
                Debug.WriteLine("Could not find torrent with the provided hash");
                MessageBox.Show(err);
                NavigationService.GoBack();
            }
            populateUI();
            showProgressBar(false);
        }

        private Torrent findTorrent(List<Torrent> torrents, string hash)
        {
            return torrents.FirstOrDefault(t => t.Hash == hash);
        }

        public void TorrentsRecvError()
        {
            Debug.WriteLine("Error while receiving torrents");
            MessageBox.Show(err);
            showProgressBar(false);
        }

        private void showProgressBar(bool show)
        {
            LoadingProgressBar.IsVisible = show;
        }

        private void populateUI()
        {
            TorrentNameValue.Text = torrent.Name;
            DoneValue.Text = torrent.ProgressPercent;
            ETAValue.Text = torrent.ETA;
            StatusValue.Text = torrent.TorrentState.DisplayName;
            RatioValue.Text = torrent.Ratio;
            PeersValue.Text = torrent.NumLeechs;
            SeedsValue.Text = torrent.NumSeeds;
            SizeValue.Text = torrent.Size;
            DLValue.Text = torrent.DlSpeed;
            ULValue.Text = torrent.UpSpeed;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            fetchTorrentDetails(torrentHash);
        }
    }
}