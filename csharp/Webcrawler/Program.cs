using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;

namespace Webcrawler {
    internal class Program {
        private static void Main(string[] args) {
            var config = ConfigurationFactory.ParseString(@"akka { suppress-json-serializer-warning = true }");

            using (var system = ActorSystem.Create("webcrawlerSystem", config)) {

                var crawler = system.ActorOf<Actors.Webcrawler>("webcrawler");

                crawler.Ask<object>(new Actors.Webcrawler.Get {Url = "http://www.google.nl/"}).ContinueWith(HandleResult);


                Console.ReadKey();
            }
        }

        #region HandleResult

        private static void HandleResult(Task<object> task) {

            var failure = task.Result as Status.Failure;
            if (failure != null) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(failure.Cause.Message);
                Console.ResetColor();
            }

            var result = task.Result as Actors.Webcrawler.Result;
            if (result != null) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Found {result.Links.Length} links for {result.Url}");
                Console.ResetColor();
            }
        }

        #endregion
    }
}