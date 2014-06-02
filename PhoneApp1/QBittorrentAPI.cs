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
using System.Text;
using System.Diagnostics;

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
            string url = "http://" + authSettings.Host + ":" + authSettings.Port;
            Debug.WriteLine("Making RestClient with URL: " + url);
            var client = new RestClient(url);
            var request = new RestRequest();
            request.Resource = "json/torrents";
            request.Credentials = new NetworkCredential(authSettings.Username, authSettings.Password);
            try
            {
                client.ExecuteAsync<List<Torrent>>(request, (response) =>
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        errorResponse();
                    }
                    else
                    {
                        string rawData = UTF8Encoding.UTF8.GetString(response.RawBytes, 0, response.RawBytes.Length);
                        System.Diagnostics.Debug.WriteLine(rawData);
                        successResponse(response.Data);
                    }
                });
            }
            catch (Exception e)
            {
                //TODO: better exception handling here
                Debug.WriteLine("Exception was caught while attempting to fetch torrents: " + e.Message);
                errorResponse();
            }
        }
    }
    
}
