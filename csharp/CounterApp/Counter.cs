using Akka.Actor;

namespace CounterApp
{
    public class Counter : ReceiveActor
    {
        public Counter()
        {
            Become(() => Behaviour(0));
        }

        private void Behaviour(int n)
        {
            Receive<Get>(get =>
            {
                Sender.Tell(n);
            });

            Receive<Increment>(incr => {
                BecomeStacked(() => Behaviour(n + 1));
            });

            Receive<Decrement>(decr =>
            {
                UnbecomeStacked();
            });
        }
    }

    public class Increment
    {
    }

    public class Decrement
    {
    }

    public class Get
    {
    }
}