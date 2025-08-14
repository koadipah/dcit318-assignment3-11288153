using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Interfaces
{
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }
}
