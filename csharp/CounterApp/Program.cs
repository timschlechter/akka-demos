using System;
using Akka.Actor;

namespace CounterApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var system = ActorSystem.Create("counterSystem");
            var counter = system.ActorOf<Counter>();

            counter.Tell(new Increment());
            counter.Tell(new Increment());
            counter.Tell(new Decrement());

            counter.Tell(new Get());
            
            //counter.Ask<int>(new Get()).ContinueWith(t => Console.WriteLine(t.Result));
            Console.ReadKey();
            system.Shutdown();
        }
    }
}