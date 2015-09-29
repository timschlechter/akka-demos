import java.util.concurrent.TimeUnit

import akka.actor.{Props, ActorSystem}
import akka.pattern.{ask}
import akka.util.Timeout

import scala.concurrent.Await

object BankApp extends App {

  implicit val timeout = Timeout(5, TimeUnit.SECONDS)

  val system = ActorSystem("bank")
  var bank = system.actorOf(Props[BankActor])

  bank ! Open("A")
  bank ! Open("B")

  bank ! Deposit("A", 100)
  bank ! Deposit("B", 100)

  bank ! Transfer("A", "B", 50)
  bank ! Transfer("B", "A", 25)

  printf("Balance account A: $%.2f\n", Await.result(bank ? GetBalance("A"), timeout.duration))
  printf("Balance account B: $%.2f\n", Await.result(bank ? GetBalance("B"), timeout.duration))

  system.shutdown()
}