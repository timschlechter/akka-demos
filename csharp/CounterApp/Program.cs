using System;
using Akka.Actor;
using Akka.Configuration;

namespace CounterApp {
    internal class Program {
        private static void Main(string[] args) {
            var config = ConfigurationFactory.ParseString(@"akka { suppress-json-serializer-warning = true }");

            using (var system = ActorSystem.Create("counterSystem", config)) {
                var counter = system.ActorOf<Counter>("counter");

                counter.Tell(new Increment());
                counter.Tell(new Increment());
                counter.Tell(new Decrement());

                counter.Ask(new Decrement());

                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
        }
    }
}