using MoneyTracking;

MoneyManager manager = new MoneyManager();
manager.Load(); //load the last saved data

bool isRunning = true;

while (isRunning)
{
    MoneyManager.Header("WELCOME TO TRACKMONEY");

    Console.WriteLine($"Current account balance: {manager.Balance:0.00} kr");
    Console.WriteLine("Pick an option:");
    Console.WriteLine("1. Show posts");
    Console.WriteLine("2. Add new expense or income");
    Console.WriteLine("3. Edit item");
    Console.WriteLine("4. Remove item");
    Console.WriteLine("5. Save and quit");
    Console.WriteLine();
    Console.Write("Select 1-5: ");

    if (!int.TryParse(Console.ReadLine(), out int menuChoice))
    {
        continue;
    }

    switch (menuChoice)   
    {
        case 1:
            manager.ShowTransactions();
            break;
        case 2:
            manager.CreateTransaction();
            break;
        case 3:
            manager.EditItem();
            break;
        case 4:
            manager.RemoveItem();
            break;
        case 5:
            manager.Save();
            Console.WriteLine("Items saved. Goodbye.");
            isRunning = false;
            break;
    }
}