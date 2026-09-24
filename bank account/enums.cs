namespace MoneyTracking;

public enum TransactionType
{
    Expense = 1,
    Income = 2
}

public enum Month
{
    January = 1,
    February,
    March,
    April,
    May,
    June,
    July,
    August,
    September,
    October,
    November,
    December
}
/// <summary>
/// What should the data be sorted after
/// </summary>
public enum SortField
{
    Month = 1,
    Amount,
    Title
}

public enum SortDirection
{
    Ascending = 1,
    Descending
}
