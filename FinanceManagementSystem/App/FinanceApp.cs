using FinanceManagementSystem.Models;
using FinanceManagementSystem.Accounts;
using FinanceManagementSystem.Processors;

namespace FinanceManagementSystem.App
{
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            var account = new SavingsAccount("ACC123", 1000m);

            var t1 = new Transaction(1, DateTime.Now, 100m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 50m, "Entertainment");

            var mobileProcessor = new MobileMoneyProcessor();
            var bankProcessor = new BankTransferProcessor();
            var cryptoProcessor = new CryptoWalletProcessor();

            mobileProcessor.Process(t1);
            bankProcessor.Process(t2);
            cryptoProcessor.Process(t3);

            account.ApplyTransaction(t1);
            account.ApplyTransaction(t2);
            account.ApplyTransaction(t3);

            _transactions.AddRange(new[] { t1, t2, t3 });
        }
    }
}
