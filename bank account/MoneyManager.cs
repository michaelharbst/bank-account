using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Xml.Linq;

namespace MoneyTracking;


/// <summary>
/// Main class for the solution 
/// </summary>
public class MoneyManager
{
    private const string FileName = "money-tracking.json";
    private int nextId = 1; // For generating uniqiue IDs
    /// <summary>
    /// The object for all transactions
    /// </summary>
    public List<Transaction> Transactions { get; private set; } = new();

    public decimal Balance => Transactions.Sum(transaction => transaction.Amount);

    /// <summary>
    /// zdding a transaction to the list
    /// </summary>
    public void CreateTransaction()
    {
        Header("CREATE POST");

        Console.WriteLine("Description: ");
        string description = Console.ReadLine();
        decimal amount = ReadPositiveDecimal("Amount: ");
        Month month = ReadMonth();
        TransactionType type = ReadTransactionType();

        Transaction transaction = new Transaction(nextId++, description, amount, month, type);

        string errors = ValidateTransaction(transaction); // empty string = no validation errors

        if (string.IsNullOrEmpty(errors))
        {
            if (transaction.TransactionType == TransactionType.Expense)
            {
                transaction.Amount = -amount;
            }
            Transactions.Add(transaction);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n{transaction.TransactionType} '{transaction.Title}' was added with ID {transaction.Id}.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nThe post was not added:");
            Console.WriteLine(errors);
            Console.ResetColor();
        }

        Pause();
    }

    /// <summary>
    /// Show transactions in the Transaction list
    /// </summary>
    public void ShowTransactions()
    {
        Header("SHOW TRANSACTIONS");

        if (Transactions.Count == 0)
        {
            Console.WriteLine("There are no transactions.");
            Pause();
            return;    // If there are no transactions then no reason to continue
        }

        Console.WriteLine("Display:");
        Console.WriteLine("1. All transaction");
        Console.WriteLine("2. Expenses only");
        Console.WriteLine("3. Incomes only");
        int filterChoice = ReadInteger("Select 1-3: ", 1, 3);

        Console.WriteLine("\nSort by:");
        Console.WriteLine("1. Month");
        Console.WriteLine("2. Amount");
        Console.WriteLine("3. Title");
        SortField sortField = (SortField)ReadInteger("Select 1-3: ", 1, 3);

        Console.WriteLine("\nDirection:");
        Console.WriteLine("1. Ascending");
        Console.WriteLine("2. Descending");
        SortDirection direction = (SortDirection)ReadInteger("Select 1-2: ", 1, 2);

        List<Transaction> result = Transactions;

        result = filterChoice switch
        {
            2 => result.Where(transaction => transaction.TransactionType == TransactionType.Expense).ToList(),
            3 => result.Where(transaction => transaction.TransactionType == TransactionType.Income).ToList(),
            _ => result
        };

        result = SortItems(result, sortField, direction);

        List<Transaction> displayedItems = result.ToList();

        Console.WriteLine();
        if (displayedItems.Count == 0)
        {
            Console.WriteLine("No items match the selected filter.");
        }
        else
        {
            PrintTable(displayedItems);
        }

        Pause();
    }

    /// <summary>
    /// modifying the specified transaction in the transaction list
    /// </summary>
    public void EditItem()
    {
        Header("EDIT ITEM");
        
        Transaction transaction = SelectTransaction();
        if (transaction == null)
        {
            Pause();
            return;
        }

        Console.WriteLine($"\nEditing: {transaction.Title}");
        Console.WriteLine("1. Title");
        Console.WriteLine("2. Amount");
        Console.WriteLine("3. Month");
        Console.WriteLine("4. Type");
        int choice = ReadInteger("Select 1-4: ", 1, 4);

        switch (choice)
        {
            case 1:
                Console.WriteLine("New description: ");
                transaction.Title = Console.ReadLine();
                break;
            case 2:
                transaction.Amount = ReadPositiveDecimal("New amount: ");
                break;
            case 3:
                transaction.Month = ReadMonth();
                break;
            case 4:
                transaction.TransactionType = ReadTransactionType();
                break;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nItem updated.");
        Console.ResetColor();
        Pause();
    }

    /// <summary>
    /// Removing the specified transaction in the transaction list
    /// </summary>
    public void RemoveItem()
    {
        Header("REMOVE ITEM");

        Transaction? item = SelectTransaction();
        if (item == null)
        {
            Pause();
            return;
        }

        Console.Write($"\nRemove '{item.Title}' (Y/N)? ");
        string answer = Console.ReadLine()?.Trim().ToLower() ?? "";

        if (answer == "y")
        {
            Transactions.Remove(item);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Item removed.");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine("No item was removed.");
        }

        Pause();
    }

    /// <summary>
    /// serializing and saving the transaction list
    /// </summary>
    public void Save()
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(Transactions, options);
        File.WriteAllText(FileName, json);
    }

    /// <summary>
    /// Loading and deserializing the transaction list
    /// </summary>
    public void Load()
    {
        if (!File.Exists(FileName))
        {
            Transactions = new List<Transaction>();
            nextId = 1;
            return;
        }

        try
        {
            string json = File.ReadAllText(FileName);
            Transactions = JsonSerializer.Deserialize<List<Transaction>>(json)
                    ?? new List<Transaction>();

            nextId = Transactions.Count == 0
                ? 1
                : Transactions.Max(item => item.Id) + 1;
        }
        catch (JsonException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("The saved data file contains invalid JSON.");
            Console.ResetColor();
            Transactions = new List<Transaction>();
            nextId = 1;
            Pause();
        }
        catch (IOException exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"The saved data could not be loaded: {exception.Message}");
            Console.ResetColor();
            Transactions = new List<Transaction>();
            nextId = 1;
            Pause();
        }
    }

    private static List<Transaction> SortItems(List<Transaction> transactions, SortField field,
        SortDirection direction)
    {
        bool ascending = direction == SortDirection.Ascending;

        return field switch
        {
            SortField.Month => ascending
                ? transactions.OrderBy(transactions => transactions.Month).ToList()
                : transactions.OrderByDescending(transactions => transactions.Month).ToList(),

            SortField.Amount => ascending
                ? transactions.OrderBy(transactions => transactions.Amount).ToList()
                : transactions.OrderByDescending(transactions => transactions.Amount).ToList(),

            SortField.Title => ascending
                ? transactions.OrderBy(transactions => transactions.Title).ToList()
                : transactions.OrderByDescending(transactions => transactions.Title).ToList(),

            _ => transactions
        };
    }

    private Transaction? SelectTransaction()
    {
        if (Transactions.Count == 0)
        {
            Console.WriteLine("There are no transaction.");
            return null;
        }

        PrintTable(Transactions.OrderBy(transaction => transaction.Month).ToList());

        Console.Write("\nEnter item ID: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return null;
        }

        Transaction? transaction = Transactions.SingleOrDefault(transaction => transaction.Id == id);

        if (transaction == null)
        {
            Console.WriteLine($"No transaction with ID {id} was found.");
        }

        return transaction;
    }

    private static string ValidateTransaction(Transaction transaction)
    {
        string errors = "";

        if (string.IsNullOrWhiteSpace(transaction.Title))
        {
            errors += "The transaction must have a title.\n";
        }

        if (transaction.Amount <= 0)
        {
            errors += "The amount must be positive.\n";
        }

        if (!Enum.IsDefined(transaction.Month))
        {
            errors += "The transaction must have a valid month.\n";
        }

        if (!Enum.IsDefined(transaction.TransactionType))
        {
            errors += "The transaction must be an expense or an income.\n";
        }

        return errors;
    }

    private static decimal ReadPositiveDecimal(string message)
    {
        while (true)
        {
            Console.Write(message);

            if (decimal.TryParse(Console.ReadLine(), out decimal amount)
                && amount > 0)
            {
                return amount;
            }

            Console.WriteLine("Enter a positive number.");
        }
    }

    private static Month ReadMonth()
    {
        Console.WriteLine("\nMonth:");
        for (int month = 1; month <= 12; month++)
        {
            Console.WriteLine($"{month}. {(Month)month}");
        }

        return (Month)ReadInteger("Select month 1-12: ", 1, 12);
    }

    private static TransactionType ReadTransactionType()
    {
        Console.WriteLine("\nType:");
        Console.WriteLine("1. Expense");
        Console.WriteLine("2. Income");

        return (TransactionType)ReadInteger("Select 1-2: ", 1, 2);
    }

    private static int ReadInteger(string message, int minimum, int maximum)
    {
        while (true)
        {
            Console.Write(message);

            if (int.TryParse(Console.ReadLine(), out int number)
                && number >= minimum
                && number <= maximum) 
            {
                return number;
            }

            Console.WriteLine($"Enter a number from {minimum} to {maximum}.");
        }
    }

    private static void PrintTable(List<Transaction> transactions)
    {
        Console.WriteLine(
            "ID".PadRight(6) +
            "Type".PadRight(12) +
            "Month".PadRight(14) +
            "Title".PadRight(25) +
            "Amount".PadLeft(12));

        Console.WriteLine(new string('-', 69));

        foreach (Transaction transaction in transactions)
        {
            Console.ForegroundColor = transaction.TransactionType == TransactionType.Income
                ? ConsoleColor.Green
                : ConsoleColor.Yellow;

            Console.WriteLine(
                transaction.Id.ToString().PadRight(6) +
                transaction.TransactionType.ToString().PadRight(12) +
                transaction.Month.ToString().PadRight(14) +
                transaction.Title.PadRight(25) +
                $"{transaction.Amount:0.00}".PadLeft(12));

            Console.ResetColor();
        }

        Console.WriteLine(new string('-', 69));
        Console.WriteLine("");
        Console.WriteLine($"Balance is {transactions.Sum(trans => trans.Amount)}");
    }

    public static void Header(string title)
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine($"********   {title}   ********");
        Console.WriteLine();
    }

    private static void Pause()
    {
        Console.WriteLine("\nPress any key to continue.");
        Console.ReadKey();
    }
}
