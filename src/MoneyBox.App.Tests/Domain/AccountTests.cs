using Moneybox.App;

namespace MoneyBox.App.Tests.Domain
{
    public class AccountTests
    {
        [Fact]
        public void Withdraw_WithSufficientFunds_ShouldDecreaseBalance()
        {
            // Arrange
            var account = new Account
            {
                Balance = 1000m,
                Withdrawn = 0m
            };
            // Act
            account.Withdraw(200m);
            // Assert
            Assert.Equal(800m, account.Balance);
            Assert.Equal(200m, account.Withdrawn);
        }

        [Fact]
        public void Withdraw_WithInsufficientFunds_ShouldThrowException()
        {
            // Arrange
            var account = new Account
            {
                Balance = 100m,
                Withdrawn = 0m
            };
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => account.Withdraw(200m));
            Assert.Equal("Insufficient funds to make a withdraw", exception.Message);
        }

        [Fact]
        public void PayIn_WithinLimit_UpdatesBalanceAndPaidIn()
        {
            // Arrange
            var account = new Account
            {
                Balance = 1000m,
                PaidIn = 0m
            };
            // Act
            account.PayIn(2000m);
            // Assert
            Assert.Equal(3000m, account.Balance);
            Assert.Equal(2000m, account.PaidIn);
        }

        [Fact]
        public void PayIn_ExceedingLimit_ShouldThrowException()
        {
            // Arrange
            var account = new Account
            {
                Balance = 1000m,
                PaidIn = 3500m
            };
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => account.PayIn(600m));
            Assert.Equal("Account pay in limit reached", exception.Message);
        }
    }
}