using System;
using System.Collections.Generic;

public class BankAccount
{
    private static readonly Random rng = new Random();

    public int Id { get; private set; }
    public string HolderName { get; set; }
    public double Funds { get; private set; }

    public const double StartingMinimum = 1000;

    public BankAccount(string name, double openingAmount)
    {
        if (openingAmount < StartingMinimum)
            throw new ArgumentException($"Initial deposit must be at least {StartingMinimum}.");

        Id = rng.Next(10000, 99999);
        HolderName = name;
        Funds = openingAmount;
    }

    public void AddFunds(double amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit must be a positive value.");
        Funds += amount;
    }

    public void RemoveFunds(double amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal must be a positive value.");
        if (amount > Funds)
            throw new InvalidOperationException("Insufficient balance.");
        Funds -= amount;
    }

    public override string ToString()
    {
        return $"Account ID: {Id} | Owner: {HolderName,-12} | Balance: ${Funds:0.00}";
    }
}

public class BankSystem
{
    private List<BankAccount> customers = new List<BankAccount>();

    public void ShowAllAccounts()
    {
        if (customers.Count == 0)
        {
            Console.WriteLine("There are currently no active accounts.");
            return;
        }

        foreach (var acc in customers)
        {
            Console.WriteLine(acc);
        }
    }

    public void RegisterNewAccount()
    {
        Console.Write("Enter customer name: ");
        string name = Console.ReadLine()?.Trim();

        while (string.IsNullOrWhiteSpace(name))
        {
            Console.Write("Name can't be empty. Please enter a valid name: ");
            name = Console.ReadLine()?.Trim();
        }

        double startingAmount = GetValidAmount($"Enter starting balance (minimum {BankAccount.StartingMinimum}): ", BankAccount.StartingMinimum);

        try
        {
            var account = new BankAccount(name, startingAmount);
            customers.Add(account);
            Console.WriteLine("Account successfully created.");
            Console.WriteLine(account);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to create account: " + ex.Message);
        }
    }

    public void ProcessDeposit()
    {
        var account = LocateAccount();

        if (account == null) return;

        double deposit = GetValidAmount("Enter amount to deposit: ");
        account.AddFunds(deposit);

        Console.WriteLine("Deposit complete.");
        Console.WriteLine(account);
    }

    public void ProcessWithdrawal()
    {
        var account = LocateAccount();

        if (account == null) return;

        double withdrawal = GetValidAmount("Enter amount to withdraw: ");

        try
        {
            account.RemoveFunds(withdrawal);
            Console.WriteLine("Withdrawal complete.");
            Console.WriteLine(account);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Transaction failed: " + ex.Message);
        }
    }

    private BankAccount LocateAccount()
    {
        Console.Write("Enter account ID: ");
        if (int.TryParse(Console.ReadLine(), out int accId))
        {
            var found = customers.Find(a => a.Id == accId);
            if (found == null)
            {
                Console.WriteLine("No account matches that ID.");
                return null;
            }
            return found;
        }
        else
        {
            Console.WriteLine("Invalid account ID format.");
            return null;
        }
    }

    private double GetValidAmount(string prompt, double minimum = 0.01)
    {
        double result;
        do
        {
            Console.Write(prompt);
            if (double.TryParse(Console.ReadLine(), out result) && result >= minimum)
                return result;

            Console.WriteLine($"Enter a value of at least {minimum}.");
        } while (true);
    }
}

class Program
{
    static void Main()
    {
        var bank = new BankSystem();
        bool active = true;

        while (active)
        {
            Console.WriteLine("\n--- Banking Menu ---");
            Console.WriteLine("1. Display all accounts");
            Console.WriteLine("2. Open new account");
            Console.WriteLine("3. Deposit funds");
            Console.WriteLine("4. Withdraw funds");
            Console.WriteLine("5. Quit");
            Console.Write("Choose an option (1-5): ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    bank.ShowAllAccounts();
                    break;
                case "2":
                    bank.RegisterNewAccount();
                    break;
                case "3":
                    bank.ProcessDeposit();
                    break;
                case "4":
                    bank.ProcessWithdrawal();
                    break;
                case "5":
                    Console.WriteLine("Exiting the system. Thank you.");
                    active = false;
                    break;
                default:
                    Console.WriteLine("Invalid selection. Choose between 1 and 5.");
                    break;
            }
        }
    }
}
