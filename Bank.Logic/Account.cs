using Bank.Logic.Abstractions;

namespace Bank.Logic;

public class Account : IAccount
{
    private AccountSettings _settings;
    public AccountSettings Settings
    {
        get => _settings;
        set => _settings = value;
    }

    private List<ITransaction> _transactions = new();

    public double GetBalance() { return _transactions.Sum(t => t.Amount); }
    public IReadOnlyList<ITransaction> GetTransactions() { return _transactions.AsReadOnly(); }
    public bool TryAddTransaction(ITransaction transaction) 
    { 
        // Always reject unknown transactions
        if (transaction.Type == TransactionType.Unknown) 
        { 
            return false; 
        }

        // Reject direct interest additions and overdraft fees
        if (
            transaction.Type == TransactionType.Interest || 
            transaction.Type == TransactionType.Fee_Overdraft
            ) 
        { 
            return false; 
        }

        // Reject negative amounts for deposits
        if (transaction.Type == TransactionType.Deposit) 
            if (transaction.Amount < 0) 
                return false;
        

        // Reject positive amounts for withdrawals and fees
        if (
            transaction.Type == TransactionType.Withdraw || 
            transaction.Type == TransactionType.Fee_Overdraft || 
            transaction.Type == TransactionType.Fee_Management
            ) 
        { 
            if (transaction.Amount > 0) 
            { 
                return false; 
            }
        }

        var balance = GetBalance();

        // Add automatic overdraft
        if (transaction.Type == TransactionType.Withdraw && balance < -transaction.Amount) 
        { 
            var feeOverdraft = new Transaction 
            { 
                Type = TransactionType.Fee_Overdraft, 
                Amount = -Settings.OverdraftFee, 
                Date = DateTime.Now 
            }; 
            _transactions.Add(feeOverdraft); 
            return false;
        }

        if (transaction.Type == TransactionType.Withdraw && transaction.Amount == 0)
        {
            return false;
        }

        _transactions.Add(transaction); 
        return true;
    }
}