using Bank.Logic.Abstractions;
using FluentAssertions;

namespace Bank.Logic.Tests
{
    public class TransactionTests
    {
        public ITransaction CreateTransaction(TransactionType type, double amount, DateTime date)
        {
            return new Transaction()
            {
                Type = type,
                Amount = amount,
                Date = date
            };
        }

        [Fact]
        public void CreateTransaction_WithValidType_ShouldSetCorrectType()
        {
            var transaction = CreateTransaction(TransactionType.Deposit, 100, DateTime.UtcNow);

            transaction.Type.Should().Be(TransactionType.Deposit, nameof(ITransaction.Type));
        }

        [Fact]
        public void CreateTransaction_WithValidAmount_ShouldSetCorrectAmount()
        {
            var transaction = CreateTransaction(TransactionType.Deposit, 100, DateTime.UtcNow);

            transaction.Amount.Should().Be(100, nameof(ITransaction.Amount));
        }

        [Fact]
        public void CreateTransaction_WithValidDate_ShouldSetCorrectDate()
        {
            var date = DateTime.UtcNow;
            var transaction = CreateTransaction(TransactionType.Deposit, 100, date);

            transaction.Date.Should().BeCloseTo(date, TimeSpan.FromMilliseconds(10), nameof(ITransaction.Date));
        }

        [Theory]
        [InlineData(TransactionType.Deposit, 100, false)]
        [InlineData(TransactionType.Withdraw, -50, true)]
        [InlineData(TransactionType.Fee_Overdraft, -35, true)]
        [InlineData(TransactionType.Interest, 10, false)]
        [InlineData(TransactionType.Unknown, 0, false)]
        public void CreateTransaction_WithDifferentTypes_ShouldValidateAmountSign(TransactionType type, double amount, bool shouldBeNegative)
        {
            var transaction = CreateTransaction(type, amount, DateTime.UtcNow);

            if (shouldBeNegative)
            {
                transaction.Amount.Should().BeNegative($"{nameof(ITransaction.Amount)} should be negative for {type}");
            }
            else
            {
                transaction.Amount.Should().BePositive($"{nameof(ITransaction.Amount)} should be positive for {type}");
            }
        }

        [Theory]
        [InlineData(TransactionType.Deposit, -100)]
        [InlineData(TransactionType.Interest, -10)]
        public void CreateTransaction_WithNegativeAmountForPositiveTransaction_ShouldThrowException(TransactionType type, double amount)
        {
            Action act = () => CreateTransaction(type, amount, DateTime.UtcNow);

            act.Should().Throw<ArgumentOutOfRangeException>($"{nameof(ITransaction.Amount)} should be positive for {type}");
        }

        [Theory]
        [InlineData(TransactionType.Withdraw, 50)]
        [InlineData(TransactionType.Fee_Overdraft, 35)]
        public void CreateTransaction_WithPositiveAmountForNegativeTransaction_ShouldThrowException(TransactionType type, double amount)
        {
            Action act = () => CreateTransaction(type, amount, DateTime.UtcNow);

            act.Should().Throw<ArgumentOutOfRangeException>($"{nameof(ITransaction.Amount)} should be negative for {type}");
        }
    }
}
