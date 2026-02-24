using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;


namespace MoneyBox.App.Tests.Features
{
    public class WithdrawMoneyTests
    {
        private readonly WithdrawMoney _withdrawMoney;
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<INotificationService> _notificationServiceMock;

        public WithdrawMoneyTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _notificationServiceMock = new Mock<INotificationService>();
            _withdrawMoney = new WithdrawMoney(_accountRepositoryMock.Object, _notificationServiceMock.Object);
        }

        [Fact]
        public void Execute_WithSufficientFunds_ShouldWithdrawMoney()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new Account
            {
                Id = accountId,
                User = new User { Id = Guid.NewGuid(), Name = "Test User", Email = "test@test.com" },
                Balance = 1000m,
                Withdrawn = 0m,
                PaidIn = 0m
            };

            _accountRepositoryMock.Setup(r => r.GetAccountById(accountId)).Returns(account);

            // Act
            _withdrawMoney.Execute(accountId, 100m);

            // Assert
            Assert.Equal(900m, account.Balance);
            Assert.Equal(100m, account.Withdrawn);
            _accountRepositoryMock.Verify(r => r.Update(account), Times.Once);
        }

        [Fact]
        public void Execute_WithInsufficientFunds_ThrowsException()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new Account
            {
                Id = accountId,
                User = new User { Id = Guid.NewGuid(), Name = "Test User", Email = "test@test.com" },
                Balance = 50m,
                Withdrawn = 0m,
                PaidIn = 0m
            };

            _accountRepositoryMock.Setup(r => r.GetAccountById(accountId)).Returns(account);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _withdrawMoney.Execute(accountId, 100m));
            _accountRepositoryMock.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void Execute_WhenBalanceFallsBelow500_SendsLowFundsNotification()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new Account
            {
                Id = accountId,
                User = new User { Id = Guid.NewGuid(), Name = "Test User", Email = "test@test.com" },
                Balance = 600m,
                Withdrawn = 0m,
                PaidIn = 0m
            };

            _accountRepositoryMock.Setup(r => r.GetAccountById(accountId)).Returns(account);

            // Act
            _withdrawMoney.Execute(accountId, 200m);

            // Assert
            _notificationServiceMock.Verify(
                n => n.NotifyFundsLow("test@test.com"),
                Times.Once
            );
        }
    }
}