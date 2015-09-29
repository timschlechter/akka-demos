using System.Collections.Generic;
using Akka.Actor;

namespace AkkaBank
{
    public class BankActor : ReceiveActor
    {
        private readonly IDictionary<string, IActorRef> accounts = new Dictionary<string, IActorRef>();

        public BankActor()
        {
            Receive<Open>(msg =>
            {
                accounts.Add(msg.AccountId, Context.ActorOf<BankAccountActor>());
            });

            Receive<Transfer>(msg =>
            {
                accounts[msg.AccountId].Tell(new Withdrawal {Amount = msg.Amount});
                accounts[msg.ToId].Tell(new Deposit {Amount = msg.Amount});
            });

            Receive<BankMessage>(msg => accounts[msg.AccountId].Forward(msg));
        }
    }
}