namespace MiniWalletBackendSystem.DTOs
{
    public class DebitRequest
    {
        public Guid WalletId { get; set; }

        public decimal Amount { get; set; }

        public string ReferenceId { get; set; }
    }
}
