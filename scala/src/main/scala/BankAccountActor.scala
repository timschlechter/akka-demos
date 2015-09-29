import akka.actor.Actor

class BankAccountActor extends Actor {
  var balance : Double = 0

  def receive = opened

  val opened: Receive = {
    case GetBalance(_) => sender ! balance
    case Deposit(_, amount) => balance += amount
    case Withdrawal(_, amount) => {
      if (balance < amount) throw new Exception("Insufficient funds")
      balance -= amount
    }
    case Close => context.become(closed)
  }

  val closed: Receive = {
    case GetBalance => sender ! balance
    case Open => context.become(opened)
    case _ => throw new Exception("Account is closed")
  }
}
