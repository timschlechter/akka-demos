using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util.Internal;
using HtmlAgilityPack;
using RestSharp;

namespace Webcrawler.Actors {
    public class Getter : ReceiveActor {
        public Getter(string url, int depth) {
            //GetContentAsync(url).PipeTo(Self);
            var self = Self;
            GetContentAsync(url).ContinueWith(task => {
                if (task.IsFaulted) {
                    self.Tell(new Status.Failure(task.Exception));
                }
                else {
                    self.Tell(task.Result);
                }
            });

            Receive<string>(body => {
                try {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(body);

                    doc.DocumentNode
                        .SelectNodes("//a[@href]")?
                        .Select(node => node.Attributes["href"].Value)
                        .ForEach(link => Context.Parent.Tell(new Controller.Check {Url = link, Depth = depth}));
                }
                finally {
                    Stop();
                }
            });

            Receive<Abort>(msg => Stop());
            Receive<Status.Failure>(msg => Stop());
        }

        private void Stop() {
            Context.Parent.Tell(new Done());
            Context.Stop(Self);
        }

        private Task<string> GetContentAsync(string url) {
            var tcs = new TaskCompletionSource<string>();

            try {
                var client = new RestClient(url);
                client.GetAsync(new RestRequest(), (response, handle) => {
                    if ((int) response.StatusCode >= 400) {
                        tcs.SetException(response.ErrorException);
                    }

                    tcs.SetResult(response.Content);
                });
            }
            catch (Exception ex) {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        private string GetContent(string url) {
            var client = new RestClient();


            var response = client.Get(new RestRequest(url));

            return response.Content;
        }

        public class Done {}

        public class Abort {}
    }
}