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
                if (!accounts.ContainsKey(msg.AccountId))
                {
                    accounts.Add(msg.AccountId, Context.ActorOf<BankAccountActor>());
                }

                accounts[msg.AccountId].Tell(msg);
            });

            Receive<Transfer>(msg =>
            {
                accounts[msg.AccountId].Tell(new Withdrawal {Amount = msg.Amount});
                accounts[msg.ToId].Tell(new Deposit {Amount = msg.Amount});
            });

            Receive<Deposit>(msg => accounts[msg.AccountId].Forward(msg));
            Receive<Withdrawal>(msg => accounts[msg.AccountId].Forward(msg));
            Receive<GetBalance>(msg => accounts[msg.AccountId].Forward(msg));
            Receive<Close>(msg => accounts[msg.AccountId].Forward(msg));
        }
    }
}