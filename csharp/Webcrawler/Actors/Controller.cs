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
            //Context.SetReceiveTimeout(TimeSpan.FromSeconds(10));
            //Context.System.Scheduler.ScheduleOnce(TimeSpan.FromSeconds(10), () => { _children.ForEach(child => child.Tell(new Getter.Abort())); });
            //Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(10), Self, new Timeout(), Self);


            Receive<Check>(msg => {
                var depth = msg.Depth;
                var url = msg.Url;

                if (!_cache.Contains(url)) {
                    Console.WriteLine($"{depth} checking {url}");
                    _cache.Add(url);
                    if (depth > 0) {
                        _children.Add(Context.ActorOf(Props.Create<Getter>(url, depth - 1)));
                    }
                }
            });

            Receive<Getter.Done>(msg => {
                _children.Remove(Sender);
                    
                if (!_children.Any()) {
                    Context.Parent.Tell(new Result {Links = _cache.ToArray()});
                }
            });

            Receive<ReceiveTimeout>(msg => { _children.ForEach(child => child.Tell(new Getter.Abort())); });

            Receive<Timeout>(msg => { _children.ForEach(child => child.Tell(new Getter.Abort())); });
        }


        public class Result {
            public string[] Links { get; set; }
        }

        private class Timeout {}

        public class Check {
            public string Url { get; set; }
            public int Depth { get; set; }
        }
    }
}