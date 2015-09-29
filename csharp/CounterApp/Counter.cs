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
            Receive<Get>(get => Sender.Tell(n));
            Receive<Increment>(increment => BecomeStacked(() => Behaviour(n + 1)));
            Receive<Decrement>(increment => UnbecomeStacked());
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