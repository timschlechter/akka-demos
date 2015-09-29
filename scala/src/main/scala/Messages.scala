trait BankMessage {
  def accountId: String
}

case class Open(accountId: String) extends BankMessage
case class Close(accountId: String) extends BankMessage
case class Deposit(accountId: String, amount: Double) extends BankMessage
case class Withdrawal(accountId: String, amount: Double) extends BankMessage
case class GetBalance(accountId: String) extends BankMessage
case class Transfer(accountId: String, toId: String, amount: Double) extends BankMessage
