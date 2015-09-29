using System;
using Akka.Actor;

namespace AkkaBank
{
    public class BankAccountActor : ReceiveActor
    {
        private double balance;

        public BankAccountActor()
        {
            Become(Opened);
        }

        private void Opened()
        {
            Receive<GetBalance>(msg => Sender.Tell(balance));

            Receive<Deposit>(msg => balance += msg.Amount);

            Receive<Withdrawal>(msg =>
            {
                if (balance < msg.Amount)
                {
                    throw new Exception("Insufficient funds");
                }
                balance -= msg.Amount;
            });

            Receive<Close>(msg => Become(Closed));
        }

        private void Closed()
        {
            Receive<GetBalance>(msg => Sender.Tell(balance));

            Receive<Open>(msg => Become(Opened));

            ReceiveAny(msg =>
            {
                throw new Exception("Account is closed");
            });
        }
    }
}