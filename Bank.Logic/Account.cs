using Bank.Logic.Abstractions;

namespace Bank.Logic;

public class Account : IAccount
{
    public AccountSettings Settings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double GetBalance() { throw new NotImplementedException(); }
    public IReadOnlyList<ITransaction> GetTransactions() { throw new NotImplementedException(); }
    public bool TryAddTransaction(ITransaction transaction) { throw new NotImplementedException(); }
}
