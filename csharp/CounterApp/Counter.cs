using Akka.Actor;

namespace CounterApp
{
    public class Counter : ReceiveActor
    {
        private int _count;

        public Counter()
        {
            Receive<Increment>(msg => _count += 1);
            Receive<Decrement>(msg => _count -= 1);
            Receive<Get>(msg => Sender.Tell(_count));
        }
    }   

    public class Increment {}
    public class Decrement {}
    public class Get {}
}