using System;
using System.Collections.Generic;

namespace MiniWalletBackendSystem.Models;

public partial class WalletTransaction
{
    public Guid TransactionId { get; set; }

    public Guid WalletId { get; set; }

    public string TransactionType { get; set; } = null!;

    public decimal Amount { get; set; }

    public decimal BalanceBefore { get; set; }

    public decimal BalanceAfter { get; set; }

    public string ReferenceId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}
