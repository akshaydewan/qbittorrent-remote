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
using System.Collections.Generic;
using RestSharp;

namespace PhoneApp1
{
    public class QBittorrentAPI
    {
        private AuthSettings authSettings;
        public delegate void TorrentsReceived(List<Torrent> torrents);
        public delegate void TorrentsRecvError();

        public QBittorrentAPI(AuthSettings authSettings)
        {
            this.authSettings = authSettings;
        }

        public void FetchAllTorrents(TorrentsReceived successResponse, TorrentsRecvError errorResponse)
        {
            var client = new RestClient("http://" + authSettings.Host + ":" + authSettings.Port);
            var request = new RestRequest();
            request.Resource = "json/torrents";
            request.Credentials = new NetworkCredential(authSettings.Username, authSettings.Password);
            client.ExecuteAsync<List<Torrent>>(request, (response) =>
            {
                Console.WriteLine(response.StatusCode);
                Console.Write(response.RawBytes);
                if (response.ResponseStatus == ResponseStatus.Error)
                {
                    errorResponse();
                }
                else
                {
                    successResponse(response.Data);
                }
            });
        }
    }
}
