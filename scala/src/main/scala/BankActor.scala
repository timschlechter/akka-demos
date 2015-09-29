import akka.actor.{Props, ActorRef, Actor}

class BankActor extends Actor {
  var accounts = Map[String, ActorRef]()

  def receive = {
    case Open(accountId) => {
      accounts += (accountId -> context.actorOf(Props[BankAccountActor]))
    }

    case Transfer(fromId, toId, amount) => {
      accounts.get(fromId).get ! Withdrawal(fromId, amount)
      accounts.get(toId).get ! Deposit(toId, amount)
    }

    case msg: BankMessage => {
      accounts.get(msg.accountId).get forward msg
    }
  }
}
