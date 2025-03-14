using Bank.Logic.Abstractions;

namespace Bank.Logic;

public class Transaction : ITransaction
{
    public TransactionType Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double Amount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime Date { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
