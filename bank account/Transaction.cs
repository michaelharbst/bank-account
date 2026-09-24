namespace MoneyTracking;

public class Transaction
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public Month Month { get; set; }
    public TransactionType TransactionType { get; set; }

    public Transaction()
    {
        // Required when System.Text.Json creates an item while loading data.
    }

    public Transaction(int id, string title, decimal amount, Month month, TransactionType type)
    {
        Id = id;
        Title = title;
        Amount = amount;
        Month = month;
        TransactionType = type;
    }
}
