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
using Microsoft.Phone.Shell;

namespace QBittorrentRemote
{
    public partial class DetailPage : PhoneApplicationPage
    {

        private StorageWrapper storage;
        private AuthSettings authSettings;
        private string torrentHash;

        public Torrent torrent { get; set; }

        private const string err = "Unable to fetch torrent details. Please check your settings and connection";
        private const string pauseIcon = "/Icons/appbar.transport.pause.rest.png";
        private const string resumeIcon = "/Icons/appbar.transport.play.rest.png";

        public DetailPage()
        {
            InitializeComponent();
            storage = new StorageWrapper(IsolatedStorageSettings.ApplicationSettings);
            authSettings = storage.LoadAuthSettings();
        }

        //Event Handlers --------------------------------------------------------------------------
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

        private void PauseResume_Click(object sender, EventArgs e)
        {
            enablePauseResumeButton(false);
            if (torrent.TorrentState.Paused)
            {
                resumeTorrent();
            }
            else
            {
                pauseTorrent();
            }
        }

        public void TorrentsReceived(List<Torrent> torrents)
        {
            Debug.WriteLine("Torrents Received");
            if (torrents == null)
            {
                Debug.WriteLine("Torrent list was null");
                showProgressBar(false);
                MessageBox.Show(err);
                NavigationService.GoBack();
                return;
            }
            torrent = findTorrent(torrents, torrentHash);
            if (torrent == null)
            {
                Debug.WriteLine("Could not find torrent with the provided hash");
                showProgressBar(false);
                MessageBox.Show(err);
                NavigationService.GoBack();
                return;
            }
            populateUI();
            showProgressBar(false);
        }

        public void TorrentsRecvError()
        {
            Debug.WriteLine("Error while receiving torrents");
            showProgressBar(false);
            MessageBox.Show(err);
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            fetchTorrentDetails(torrentHash);
        }

        private void pauseSuccess()
        {
            fetchTorrentDetails(torrent.Hash);
        }

        private void pauseFail()
        {
            showProgressBar(false);
            MessageBox.Show("There was an error while trying to pause the torrent. Check your internet connection and try again");
            setPauseButtonState(torrent.TorrentState.Paused);
        }

        private void resumeSuccess()
        {
            fetchTorrentDetails(torrent.Hash);
        }

        private void resumeFail()
        {
            showProgressBar(false);
            MessageBox.Show("There was an error while trying to resume the torrent. Check your internet connection and try again");
            setPauseButtonState(torrent.TorrentState.Paused);
        }

        //Private methods -------------------------------------------------------------------------

        private void fetchTorrentDetails(string torrentHash)
        {
            showProgressBar(true);
            QBittorrentAPI api = new QBittorrentAPI(authSettings);
            api.FetchAllTorrents(TorrentsReceived, TorrentsRecvError);
        }

        private Torrent findTorrent(List<Torrent> torrents, string hash)
        {
            return torrents.FirstOrDefault(t => t.Hash == hash);
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
            setPauseButtonState(torrent.TorrentState.Paused);
        }

        private void setPauseButtonState(bool paused)
        {
            ApplicationBarMenuItem btn = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            if (paused)
            {
                btn.Text = "Resume";
            }
            else
            {
                btn.Text = "Pause";
            }
            btn.IsEnabled = true;
        }

        private void enablePauseResumeButton(bool isEnabled)
        {
            ApplicationBarMenuItem btn = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            btn.IsEnabled = isEnabled;
        }

        private void pauseTorrent()
        {
            showProgressBar(true);
            QBittorrentAPI api = new QBittorrentAPI(authSettings);
            api.PauseTorrent(torrent.Hash, pauseSuccess, pauseFail);
        }

        private void resumeTorrent()
        {
            showProgressBar(true);
            QBittorrentAPI api = new QBittorrentAPI(authSettings);
            api.ResumeTorrent(torrent.Hash, resumeSuccess, resumeFail);
        }



    }
}