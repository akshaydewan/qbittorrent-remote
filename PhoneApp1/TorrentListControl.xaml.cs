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
using System.Collections;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using PhoneApp1;

namespace QBittorrentRemote
{
    public partial class TorrentListControl : UserControl
    {
        public TorrentListControl()
        {
            InitializeComponent();
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return TorrentsListBox.ItemsSource;
            }
            set
            {
                TorrentsListBox.ItemsSource = value;
            }
        }

        private void TorrentsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TorrentsListBox.SelectedItem == null)
            {
                return;
            }
            Torrent selectedTorrent = (Torrent)TorrentsListBox.SelectedItem;
            Debug.WriteLine("Navigating to DetailPage with selectedTorrent=" + selectedTorrent.Hash);
            ((PhoneApplicationFrame)Application.Current.RootVisual).Navigate(new Uri("/DetailPage.xaml?selectedTorrent=" + selectedTorrent.Hash, UriKind.Relative));
            TorrentsListBox.SelectedIndex = -1;
        }
    }
}
