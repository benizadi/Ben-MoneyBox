using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyBox.App.Tests.Features
{
    public class TransferMoneyTests
    {
        private readonly TransferMoney _transferMoney;
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<INotificationService> _notificationServiceMock;

        public TransferMoneyTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _notificationServiceMock = new Mock<INotificationService>();
            _transferMoney = new TransferMoney(_accountRepositoryMock.Object, _notificationServiceMock.Object);
        }

        [Fact]
        public void Execute_WithValidTransfer_UpdatesBothAccounts()
        {
            // Arrange
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            var fromAccount = new Account
            {
                Id = fromAccountId,
                User = new User { Id = Guid.NewGuid(), Name = "From User", Email = "from@test.com" },
                Balance = 1000m,
                Withdrawn = 0m,
                PaidIn = 0m
            };

            var toAccount = new Account
            {
                Id = toAccountId,
                User = new User { Id = Guid.NewGuid(), Name = "To User", Email = "to@test.com" },
                Balance = 500m,
                Withdrawn = 0m,
                PaidIn = 0m
            };

            _accountRepositoryMock.Setup(r => r.GetAccountById(fromAccountId)).Returns(fromAccount);
            _accountRepositoryMock.Setup(r => r.GetAccountById(toAccountId)).Returns(toAccount);

            // Act
            _transferMoney.Execute(fromAccountId, toAccountId, 100m);

            // Assert
            Assert.Equal(900m, fromAccount.Balance);
            Assert.Equal(100m, fromAccount.Withdrawn);
            Assert.Equal(600m, toAccount.Balance);
            Assert.Equal(100m, toAccount.PaidIn);
            _accountRepositoryMock.Verify(r => r.Update(fromAccount), Times.Once);
            _accountRepositoryMock.Verify(r => r.Update(toAccount), Times.Once);
        }

        [Fact]
        public void Execute_WithInsufficientFunds_ThrowsException()
        {
            // Arrange
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            var fromAccount = new Account
            {
                Id = fromAccountId,
                User = new User { Id = Guid.NewGuid(), Name = "From User", Email = "from@test.com" },
                Balance = 50m,
                Withdrawn = 0m,
                PaidIn = 0m
            };

            var toAccount = new Account
            {
                Id = toAccountId,
                User = new User { Id = Guid.NewGuid(), Name = "To User", Email = "to@test.com" },
                Balance = 500m,
                Withdrawn = 0m,
                PaidIn = 0m
            };

            _accountRepositoryMock.Setup(r => r.GetAccountById(fromAccountId)).Returns(fromAccount);
            _accountRepositoryMock.Setup(r => r.GetAccountById(toAccountId)).Returns(toAccount);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                _transferMoney.Execute(fromAccountId, toAccountId, 100m));

            _accountRepositoryMock.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void Execute_ExceedingPayInLimit_ThrowsException()
        {
            // Arrange
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            var fromAccount = new Account
            {
                Id = fromAccountId,
                User = new User { Id = Guid.NewGuid(), Name = "From User", Email = "from@test.com" },
                Balance = 1000m,
                Withdrawn = 0m,
                PaidIn = 0m
            };

            var toAccount = new Account
            {
                Id = toAccountId,
                User = new User { Id = Guid.NewGuid(), Name = "To User", Email = "to@test.com" },
                Balance = 500m,
                Withdrawn = 0m,
                PaidIn = 3900m
            };

            _accountRepositoryMock.Setup(r => r.GetAccountById(fromAccountId)).Returns(fromAccount);
            _accountRepositoryMock.Setup(r => r.GetAccountById(toAccountId)).Returns(toAccount);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                _transferMoney.Execute(fromAccountId, toAccountId, 200m));

            // Verify accounts were not updated
            _accountRepositoryMock.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void Execute_WhenFromAccountFallsBelow500_SendsLowFundsNotification()
        {
            // Arrange
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            var fromAccount = new Account
            {
                Id = fromAccountId,
                User = new User { Id = Guid.NewGuid(), Name = "From User", Email = "from@test.com" },
                Balance = 600m,
                Withdrawn = 0m,
                PaidIn = 0m
            };

            var toAccount = new Account
            {
                Id = toAccountId,
                User = new User { Id = Guid.NewGuid(), Name = "To User", Email = "to@test.com" },
                Balance = 500m,
                Withdrawn = 0m,
                PaidIn = 0m
            };

            _accountRepositoryMock.Setup(r => r.GetAccountById(fromAccountId)).Returns(fromAccount);
            _accountRepositoryMock.Setup(r => r.GetAccountById(toAccountId)).Returns(toAccount);

            // Act
            _transferMoney.Execute(fromAccountId, toAccountId, 200m);

            // Assert
            _notificationServiceMock.Verify(
                n => n.NotifyFundsLow("from@test.com"),
                Times.Once
            );
        }
    }
}
