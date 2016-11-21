using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Util.Internal;

namespace Webcrawler.Actors {
    public class Controller : ReceiveActor {
        private readonly IList<string> _cache = new List<string>();
        private readonly IList<IActorRef> _children = new List<IActorRef>();

        public Controller() {

            #region 1. Receive<Check>

            Receive<Check>(msg => {
                var depth = msg.Depth;
                var url = msg.Url;

                if (_cache.Contains(url)) {
                    return;
                }

                Console.WriteLine($"Depth {depth} - checking {url}");
                _cache.Add(url);
                if (depth > 0) {
                    _children.Add(Context.ActorOf(Props.Create<Getter>(url, depth - 1)));
                }
            });

            #endregion

            #region 2. Receive<Getter.Done>

            Receive<Getter.Done>(msg => {
                _children.Remove(Sender);

                if (!_children.Any()) {
                    Context.Parent.Tell(new Result { Links = _cache.ToArray() });
                }
            });

            #endregion

            #region 3. Receive timeout

            Context.SetReceiveTimeout(TimeSpan.FromSeconds(10));

            Receive<ReceiveTimeout>(msg => {
                _children.ForEach(child => child.Tell(new Getter.Abort()));
            });

            #endregion

            #region 4. Overall timeout

            //Context.System.Scheduler.ScheduleOnce(
            //    TimeSpan.FromSeconds(10), 
            //    () => { _children.ForEach(child => child.Tell(new Getter.Abort())); });

            //Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(10), Self, new Timeout(), Self);

            //Receive<Timeout>(msg => { _children.ForEach(child => child.Tell(new Getter.Abort())); });

            #endregion
        }

        public class Check
        {
            public string Url { get; set; }
            public int Depth { get; set; }
        }

        public class Result {
            public string[] Links { get; set; }
        }

        private class Timeout {}
    }
}