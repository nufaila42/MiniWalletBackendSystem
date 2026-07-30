using System.ComponentModel.DataAnnotations;

namespace MiniWalletBackendSystem.DTOs
{
    public class CreditRequest
    {
        [Required]
        public Guid WalletId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string ReferenceId { get; set; }
    }
}
