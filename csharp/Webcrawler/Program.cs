using System;
using System.Threading.Tasks;
using Akka.Actor;
using Get = Webcrawler.Actors.Webcrawler.Get;

namespace Webcrawler {
    internal class Program {
        private static void Main(string[] args) {
            using (var system = ActorSystem.Create("counterSystem")) {

                var crawler = system.ActorOf<Actors.Webcrawler>("webcrawler");

                crawler.Ask<object>(new Get { Url = "http://www.google.nl"}).ContinueWith(HandleResult);
                crawler.Ask<object>(new Get { Url = "http://www.google.nl/1" }).ContinueWith(HandleResult);
                crawler.Ask<object>(new Get { Url = "http://www.google.nl/2" }).ContinueWith(HandleResult);
                crawler.Ask<object>(new Get { Url = "http://www.google.nl/3" }).ContinueWith(HandleResult);
                crawler.Ask<object>(new Get { Url = "http://www.google.nl/4" }).ContinueWith(HandleResult);

                Console.ReadKey();
            }
        }

        private static void HandleResult(Task<object> task) {
            Console.WriteLine(task.Result);
        }
    }
}