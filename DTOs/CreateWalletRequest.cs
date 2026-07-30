using System.ComponentModel.DataAnnotations;

namespace MiniWalletBackendSystem.DTOs
{
    public class CreateWalletRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        [Range(0, double.MaxValue)]
        public decimal InitialBalance { get; set; }
    }
}
