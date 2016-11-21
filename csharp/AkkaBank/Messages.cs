namespace AkkaBank {
    public class BankMessage {
        public string AccountId { get; set; }
    }

    public class Close : BankMessage {}

    public class Deposit : BankMessage {
        public double Amount { get; set; }
    }

    public class GetBalance : BankMessage {}

    public class Open : BankMessage {}

    public class Transfer : BankMessage {
        public string ToId { get; set; }
        public double Amount { get; set; }
    }

    public class Withdrawal : BankMessage {
        public double Amount { get; set; }
    }
}