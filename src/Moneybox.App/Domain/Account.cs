using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;
        public const decimal LowFundThreshold = 500m;
        public const decimal PayInNotificationThreshold = 500m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public bool HasLowFunds => Balance < LowFundThreshold;

        public bool HasReachedPayInLimit => (PayInLimit - PaidIn) < PayInNotificationThreshold;

        public void Withdraw(decimal amount)
        {
            if(amount <= 0m)
            {
                throw new ArgumentException("Withdraw amount must be positive");
            }

            if (Balance - amount < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make a withdraw");
            }

            Balance -= amount;
            Withdrawn += amount;
        }

        public void PayIn(decimal amount)
        {
            if (amount <= 0m)
            {
                throw new ArgumentException("Pay in amount must be positive");
            }

            if (PaidIn + amount > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }
            Balance += amount;
            PaidIn += amount;
        }
    }
}
