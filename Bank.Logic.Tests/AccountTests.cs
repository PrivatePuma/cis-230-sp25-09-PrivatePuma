using System;
using System.Collections.Generic;
using Bank.Logic;
using Bank.Logic.Abstractions;
using Xunit;
using FluentAssertions;

namespace Bank.Logic.Tests
{
    public class AccountTests
    {
        private readonly IAccount account;

        public AccountTests()
        {
            account = new Account
            {
                Settings = new()
                {
                    OverdraftFee = 35.00,
                }
            };
        }

        public ITransaction CreateTransaction(TransactionType type, double amount, DateTime date)
        {
            return new Transaction
            {
                Type = type,
                Amount = amount,
                Date = date
            };
        }

        [Fact]
        public void GetBalance_WithDepositsAndWithdrawals_ShouldReturnCorrectBalance()
        {
            account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, 200, DateTime.UtcNow));
            account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 50, DateTime.UtcNow));

            double expectedBalance = 200 - 50;
            account.GetBalance().Should().Be(expectedBalance, nameof(IAccount.GetBalance));
        }

        [Fact]
        public void TryAddTransaction_WhenAddingInterest_ShouldReturnFalse()
        {
            bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Interest, 10, DateTime.UtcNow));

            result.Should().BeFalse($"{nameof(IAccount.TryAddTransaction)} should not allow direct interest transactions.");
        }

        [Fact]
        public void TryAddTransaction_WhenAddingOverdraftFeeDirectly_ShouldReturnFalse()
        {
            bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Fee_Overdraft, -35, DateTime.UtcNow));

            result.Should().BeFalse($"{nameof(IAccount.TryAddTransaction)} should not allow direct overdraft fee transactions.");
        }

        [Fact]
        public void TryAddTransaction_WhenWithdrawalExceedsBalance_ShouldReturnFalse()
        {
            bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 200, DateTime.UtcNow));

            result.Should().BeFalse(nameof(IAccount.TryAddTransaction));
        }

        [Fact]
        public void TryAddTransaction_WhenOverdraftOccurs_ShouldApplyOverdraftFee()
        {
            account.Settings.OverdraftFee = 35.00;

            bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 200, DateTime.UtcNow));

            result.Should().BeFalse(nameof(IAccount.TryAddTransaction));
            account.GetTransactions().Should().Contain(t =>
                t.Type == TransactionType.Fee_Overdraft &&
                t.Amount == -account.Settings.OverdraftFee, // Overdraft fee should be negative
                $"{nameof(IAccount.TryAddTransaction)} should apply overdraft fee.");
        }

        [Fact]
        public void TryAddTransaction_WhenDepositingNegativeAmount_ShouldReturnFalse()
        {
            bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, -100, DateTime.UtcNow));

            result.Should().BeFalse($"{nameof(IAccount.TryAddTransaction)} should not allow negative deposits.");
        }

        [Fact]
        public void TryAddTransaction_WhenWithdrawingZeroAmount_ShouldReturnFalse()
        {
            bool result = account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 0, DateTime.UtcNow));

            result.Should().BeFalse($"{nameof(IAccount.TryAddTransaction)} should not allow zero withdrawals.");
        }

        [Fact]
        public void GetBalance_WithMultipleDepositsAndWithdrawals_ShouldReturnCorrectTotal()
        {
            account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, 500, DateTime.UtcNow));
            account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 200, DateTime.UtcNow));
            account.TryAddTransaction(CreateTransaction(TransactionType.Deposit, 300, DateTime.UtcNow));
            account.TryAddTransaction(CreateTransaction(TransactionType.Withdraw, 100, DateTime.UtcNow));

            double expectedBalance = (500 - 200) + (300 - 100);
            account.GetBalance().Should().Be(expectedBalance, nameof(IAccount.GetBalance));
        }
    }
}
