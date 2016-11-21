using System.Linq;
using Akka.Actor;
using Akka.Util.Internal;
using HtmlAgilityPack;

namespace Webcrawler.Actors {
    public class Getter : ReceiveActor {
        public Getter(string url, int depth) {
            #region 1. Use the webclient to GET the body of the given url and send it as a message to self

            var webclient = new Webclient();
            webclient.GetContentAsync(url).PipeTo(Self);

            #endregion

            #region 2. Receive<string> to process body and extract the links

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

            Receive<Status.Failure>(msg => Stop());

            #endregion

            #region 4. Abort

            Receive<Abort>(msg => Stop());

            #endregion
        }

        #region 3. Stop()

        private void Stop() {
            Context.Parent.Tell(new Done());
            Context.Stop(Self);
        }

        #endregion

        public class Done {}

        public class Abort {}
    }

}