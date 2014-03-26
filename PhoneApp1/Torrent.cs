using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneApp1
{
    public class Torrent
    {
        private string _state;
        public string Hash { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public float Progress { get; set; }
        public string DlSpeed { get; set; }
        public string UpSpeed { get; set; }
        public string Priority { get; set; }
        public int NumSeeds { get; set; }
        public int NumLeechs { get; set; }
        public string Ratio { get; set; }
        public string ETA { get; set; }
        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                this.TorrentState = TorrentState.create(value);

            }
        }
        public TorrentState TorrentState { get; set; }
        public string ProgressPercent
        {
            get
            {
                return Progress * 100 + "%";
            }
        }
    }
}
