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
    public class TorrentState
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Active { get; set; }
        public bool Downloading { get; set; }
        public bool Seeding { get; set; }
        public bool Paused { get; set; }

        public static TorrentState create(string name)
        {
            var torrentState = new TorrentState();
            torrentState.Name = name;
            torrentState.Paused = false;
            switch (name)
            {
                case "error":
                    torrentState.DisplayName = "Error";
                    break;
                case "pausedUP":
                    torrentState.DisplayName = "Paused";
                    torrentState.Seeding = true;
                    torrentState.Paused = true;
                    break;
                case "pausedDL":
                    torrentState.DisplayName = "Paused";
                    torrentState.Downloading = true;
                    torrentState.Paused = true;
                    break;
                case "queuedUP":
                    torrentState.DisplayName = "Queued";
                    break;
                case "queuedDL":
                    torrentState.DisplayName = "Queued";
                    break;
                case "uploading":
                    torrentState.DisplayName = "Uploading";
                    torrentState.Active = true;
                    torrentState.Seeding = true;
                    break;
                case "stalledUP":
                    torrentState.DisplayName = "Stalled";
                    torrentState.Seeding = true;
                    break;
                case "stalledDL":
                    torrentState.DisplayName = "Stalled";
                    torrentState.Downloading = true;
                    break;
                case "checkingUP":
                    torrentState.DisplayName = "Checking";
                    break;
                case "checkingDL":
                    torrentState.DisplayName = "Checking";
                    break;
                case "downloading":
                    torrentState.DisplayName = "Downloading";
                    torrentState.Active = true;
                    torrentState.Downloading = true;
                    break;
                default:
                    torrentState.DisplayName = name;
                    break;
            }
            return torrentState;
        }
    }
}
