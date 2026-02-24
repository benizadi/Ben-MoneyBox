using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;
        public const decimal LowFundLimit = 500m;
        public const decimal PayInNotificationThreshold = 500m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public bool HasLowFunds => Balance < LowFundLimit;
        public bool HasReachedPayInLimit => (PayInLimit - PaidIn) < PayInNotificationThreshold;

        public void Withdraw(decimal amount)
        {
            if (Balance - amount < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make a withdraw");
            }

            Balance = Balance - amount;
            Withdrawn = Withdrawn + amount;
        }

        public void PayIn(decimal amount)
        {
            if (PaidIn + amount > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }
            Balance = Balance + amount;
            PaidIn = PaidIn + amount;
        }
    }
}
