using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace AkkaBank
{
    internal class Program
    {
        private static void Main()
        {
            var system = ActorSystem.Create("bank");
            var bank = system.ActorOf<BankActor>();

            bank.Tell(new Open {AccountId = "A"});
            bank.Tell(new Open {AccountId = "B"});

            bank.Tell(new Deposit {AccountId = "A", Amount = 100});
            bank.Tell(new Deposit {AccountId = "B", Amount = 100});

            bank.Tell(new Transfer {AccountId = "A", ToId = "B", Amount = 50});
            bank.Tell(new Transfer {AccountId = "B", ToId = "A", Amount = 25});

            Console.WriteLine("Balance account A: {0:C}", AwaitResult(bank.Ask(new GetBalance {AccountId = "A"})));
            Console.WriteLine("Balance account B: {0:C}", AwaitResult(bank.Ask(new GetBalance {AccountId = "B"})));

            //bank.Tell(new Close {AccountId = "A"});
            //bank.Tell(new Deposit {AccountId = "A", Amount = 100});

            Console.ReadKey();
            system.Shutdown();
        }

        private static T AwaitResult<T>(Task<T> task)
        {
            task.Wait();
            return task.Result;
        }
    }
}