using MiniWalletBackendSystem.DTOs;

namespace MiniWalletBackendSystem.Services
{
    public interface IWalletService
    {
        Task<object> CreateWallet(CreateWalletRequest request);

        Task<object> GetBalance(Guid walletId);

        Task<object> Credit(CreditRequest request);

        Task<object> Debit(DebitRequest request);

        Task<object> Transfer(TransferRequest request);

        Task<object> GetTransactions(
            Guid walletId,
            string? type,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize);
    }
}
