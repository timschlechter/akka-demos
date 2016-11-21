using System;
using System.Threading.Tasks;
using RestSharp;

namespace Webcrawler.Actors {
    public class Webclient {
        public string GetContent(string url) {
            // Perform a GET request to the given URL
            var client = new RestClient(url);
            var response = client.Get(new RestRequest());

            if ((int) response.StatusCode >= 400) {
                throw new Exception(response.StatusDescription);
            }

            return response.Content;
        }

        #region Stuff

        public Task<string> GetContentAsync(string url)
        {
            var tcs = new TaskCompletionSource<string>();

            try
            {
                var client = new RestClient(url);

                client.GetAsync(new RestRequest(), (response, handle) =>
                {
                    if ((int)response.StatusCode >= 400)
                    {
                        tcs.SetException(new Exception(response.StatusDescription));
                    }
                    else
                    {
                        tcs.SetResult(response.Content);
                    }
                });
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        #endregion
    }
}