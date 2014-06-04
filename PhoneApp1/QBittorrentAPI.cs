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
        public delegate void GenericSuccess();
        public delegate void GenericFailure();

        public QBittorrentAPI(AuthSettings authSettings)
        {
            this.authSettings = authSettings;
        }

        private RestClient createRestClient()
        {
            string url = "http://" + authSettings.Host + ":" + authSettings.Port;
            Debug.WriteLine("Making RestClient with URL: " + url);
            return new RestClient(url);
        }

        public void FetchAllTorrents(TorrentsReceived successResponse, TorrentsRecvError errorResponse)
        {
            var client = createRestClient();
            var request = new RestRequest();
            request.Resource = "json/torrents";
            request.AddHeader("Cache-Control", "max-age=0");
            request.AddHeader("Pragma", "no-cache");
            //For some reason, the Cache headers are being ignored. 
            //Adding this hack to make sure I don't receive stale data.
            request.AddParameter("now", DateTime.Now.Ticks.ToString());
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

        public void PauseTorrent(string torrentHash, GenericSuccess successResponse, GenericSuccess failureResponse)
        {
            var client = createRestClient();
            var request = new RestRequest("command/pause", Method.POST);
            request.AddParameter("hash", torrentHash);
            request.Credentials = new NetworkCredential(authSettings.Username, authSettings.Password);
            try
            {
                client.ExecuteAsync(request, (response) =>
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        failureResponse();
                    }
                    else
                    {
                        successResponse();
                    }
                });
            }
            catch (Exception e)
            {
                //TODO: better exception handling here
                Debug.WriteLine("Exception was caught while attempting to pause torrent: " + e.Message);
                failureResponse();
            }
        }

        public void ResumeTorrent(string torrentHash, GenericSuccess successResponse, GenericSuccess failureResponse)
        {
            var client = createRestClient();
            var request = new RestRequest("command/resume", Method.POST);
            request.AddParameter("hash", torrentHash);
            request.Credentials = new NetworkCredential(authSettings.Username, authSettings.Password);
            try
            {
                client.ExecuteAsync(request, (response) =>
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        failureResponse();
                    }
                    else
                    {
                        successResponse();
                    }
                });
            }
            catch (Exception e)
            {
                //TODO: better exception handling here
                Debug.WriteLine("Exception was caught while attempting to resume torrent: " + e.Message);
                failureResponse();
            }
        }

        public void PauseAll(GenericSuccess successResponse, GenericSuccess failureResponse)
        {
            var client = createRestClient();
            var request = new RestRequest("command/pauseall", Method.POST);
            request.Credentials = new NetworkCredential(authSettings.Username, authSettings.Password);
            try
            {
                client.ExecuteAsync(request, (response) =>
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        failureResponse();
                    }
                    else
                    {
                        successResponse();
                    }
                });
            }
            catch (Exception e)
            {
                //TODO: better exception handling here
                Debug.WriteLine("Exception was caught while attempting to pause all torrents: " + e.Message);
                failureResponse();
            }
        }

        public void ResumeAll(GenericSuccess successResponse, GenericSuccess failureResponse)
        {
            var client = createRestClient();
            var request = new RestRequest("command/resumeall", Method.POST);
            request.Credentials = new NetworkCredential(authSettings.Username, authSettings.Password);
            try
            {
                client.ExecuteAsync(request, (response) =>
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        failureResponse();
                    }
                    else
                    {
                        successResponse();
                    }
                });
            }
            catch (Exception e)
            {
                //TODO: better exception handling here
                Debug.WriteLine("Exception was caught while attempting to resume all torrents: " + e.Message);
                failureResponse();
            }
        }
    }
    
}
