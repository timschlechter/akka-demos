using System;
using System.Linq;
using Akka.Actor;
using Akka.Util.Internal;

namespace Webcrawler.Actors {
    public class Webcrawler : ReceiveActor {

        private int _reqNr;

        public Webcrawler() {
            Waiting();
        }

        private void Waiting() {
            #region Upon Get start a traversal and become Running

            Receive<Get>(msg => { Become(() => RunNext(new[] {new Job {Client = Sender, Url = msg.Url}})); });

            #endregion
        }

        private void Running(Job[] queue) {
            #region Upon Get append a job to the queue and keep running

            Receive<Get>(msg => Become(() => EnqueueJob(queue, new Job {Client = Sender, Url = msg.Url})));

            #endregion

            #region Upon Controller.Result ship the result to the Client and run the next job (if any)

            Receive<Controller.Result>(msg => {
                var job = queue.First();
                job.Client.Tell(new Result {Url = job.Url, Links = msg.Links});
                Context.Stop(Sender);
                Become(() => RunNext(queue.Skip(1).ToArray()));
            });

            #endregion
        }

        #region RunNext and EnqueueJob helper method

        private class Job {
            public IActorRef Client { get; set; }
            public string Url { get; set; }
        }

        private void RunNext(Job[] queue) {
            if (!queue.Any()) {
                Waiting();
            }
            else {
                _reqNr += 1;
                var controller = Context.ActorOf(Props.Create<Controller>(), $"c{_reqNr}");
                controller.Tell(new Controller.Check {Url = queue.First().Url, Depth = 1});
                Running(queue);
            }
        }

        private void EnqueueJob(Job[] queue, Job job) {
            if (queue.Length > 3) {
                Sender.Tell(new Status.Failure(new Exception($"Too many jobs in queue: {job.Url}")));
                Running(queue);
            }
            else {
                Running(queue.Concat(job).ToArray());
            }
        }

        #endregion
        
        public class Get {
            public string Url { get; set; }
        }

        public class Result {
            public string Url { get; set; }
            public string[] Links { get; set; }
        }
    }
}