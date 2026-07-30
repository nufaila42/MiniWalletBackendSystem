using Microsoft.EntityFrameworkCore;
using MiniWalletBackendSystem.DTOs;
using MiniWalletBackendSystem.Models;
using System.Data;

namespace MiniWalletBackendSystem.Services
{
    public class WalletService : IWalletService
    {
        private readonly MiniWalletDbContext _context;
        private readonly ILogger<WalletService> _logger;


        public WalletService(MiniWalletDbContext context,ILogger<WalletService> logger)
        {
            _context = context;
            _logger = logger;
        }

        //CREATE WALLET
        public async Task<object> CreateWallet(CreateWalletRequest request)
        {

            if (request.InitialBalance < 0)
            {
                throw new Exception("Initial balance cannot be negative");
            }


            bool emailExists = await _context.Wallets.AnyAsync(x => x.Email == request.Email);

            if (emailExists)
            {
                throw new Exception("Email already exists");
            }

            bool mobileExists = await _context.Wallets.AnyAsync(x => x.MobileNumber == request.MobileNumber);

            if (mobileExists)
            {
                throw new Exception("Mobile number already exists");
            }

            Wallet wallet = new Wallet
            {
                WalletId = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                MobileNumber = request.MobileNumber,
                Balance = request.InitialBalance,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Wallet created {WalletId}",wallet.WalletId);

            return new
            {
                wallet.WalletId,
                wallet.Name,
                wallet.Email,
                wallet.MobileNumber,
                wallet.Balance,
                wallet.CreatedAt
            };

        }

        // GET WALLET BALANCE
        public async Task<object> GetBalance(Guid walletId)
        {

            var wallet = await _context.Wallets.AsNoTracking().FirstOrDefaultAsync(x => x.WalletId == walletId);
            if (wallet == null)
            {
                throw new Exception("Wallet not found");
            }

            return new
            {
                walletId = wallet.WalletId,
                userName = wallet.Name,
                currentBalance = wallet.Balance,
                updatedTimestamp = wallet.UpdatedAt
            };
        }

        // CREDIT WALLET

        public async Task<object> Credit(CreditRequest request)
        {
            if (request.Amount <= 0)
            {
                throw new Exception("Amount must be greater than zero");
            }

            bool duplicate = await _context.WalletTransactions.AnyAsync(x => x.ReferenceId == request.ReferenceId);

            if (duplicate)
            {
                throw new Exception("Duplicate reference ID");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.WalletId == request.WalletId);
                if (wallet == null)
                {
                    throw new Exception("Wallet not found");
                }

                decimal balanceBefore = wallet.Balance;
                wallet.Balance += request.Amount;
                wallet.UpdatedAt = DateTime.UtcNow;

                WalletTransaction history = 
                new WalletTransaction
                {

                    TransactionId = Guid.NewGuid(),
                    WalletId = wallet.WalletId,
                    TransactionType = "Credit",
                    Amount = request.Amount,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.Balance,
                    ReferenceId = request.ReferenceId,
                    Status = "Success",
                    CreatedAt = DateTime.UtcNow
                };

                _context.WalletTransactions.Add(history);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new
                {
                    Message = "Credit successful",
                    WalletId = wallet.WalletId,
                    CurrentBalance = wallet.Balance
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // DEBIT WALLET
        public async Task<object> Debit(DebitRequest request)
        {
            if (request.Amount <= 0)
            {
                throw new Exception("Amount must be greater than zero");
            }
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                bool duplicate = await _context.WalletTransactions.AnyAsync(x => x.ReferenceId == request.ReferenceId);

                if (duplicate)
                {
                    throw new Exception("Duplicate reference ID");
                }

                var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.WalletId == request.WalletId);

                if (wallet == null)
                {
                    throw new Exception("Wallet not found");
                }

                if (wallet.Balance < request.Amount)
                {
                    throw new Exception("Insufficient balance");
                }

                decimal before = wallet.Balance;
                wallet.Balance -= request.Amount;
                wallet.UpdatedAt = DateTime.UtcNow;
                WalletTransaction history =
                new WalletTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    WalletId = wallet.WalletId,
                    TransactionType = "Debit",
                    Amount = request.Amount,
                    BalanceBefore = before,
                    BalanceAfter = wallet.Balance,
                    ReferenceId = request.ReferenceId,
                    Status = "Success",
                    CreatedAt = DateTime.UtcNow
                };
                _context.WalletTransactions.Add(history);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new
                {
                    Message = "Debit successful",
                    WalletId = wallet.WalletId,
                    CurrentBalance = wallet.Balance
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // WALLET TRANSFER
        public async Task<object> Transfer(TransferRequest request)
        {
            if (request.FromWalletId == request.ToWalletId)
            {
                throw new Exception("Sender and receiver cannot be same");
            }

            if (request.Amount <= 0)
            {
                throw new Exception("Amount must be greater than zero");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                bool duplicate = await _context.WalletTransactions.AnyAsync(x => x.ReferenceId == request.ReferenceId);

                if (duplicate)
                {
                    throw new Exception("Duplicate reference ID");
                }

                var sender = await _context.Wallets.FirstOrDefaultAsync(x => x.WalletId == request.FromWalletId);
                var receiver = await _context.Wallets.FirstOrDefaultAsync(x => x.WalletId == request.ToWalletId);

                if (sender == null || receiver == null)
                {
                    throw new Exception("Wallet not found");
                }

                if (sender.Balance < request.Amount)
                {
                    throw new Exception("Insufficient balance");
                }

                decimal senderBefore = sender.Balance;
                decimal receiverBefore = receiver.Balance;
                sender.Balance -= request.Amount;
                receiver.Balance += request.Amount;
                sender.UpdatedAt = DateTime.UtcNow;
                receiver.UpdatedAt = DateTime.UtcNow;

                var debit =
                new WalletTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    WalletId = sender.WalletId,
                    TransactionType = "TransferDebit",
                    Amount = request.Amount,
                    BalanceBefore = senderBefore,
                    BalanceAfter = sender.Balance,
                    ReferenceId = request.ReferenceId + "-DEBIT",
                    Status = "Success",
                    CreatedAt = DateTime.UtcNow
                };

                var credit =
                new WalletTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    WalletId = receiver.WalletId,
                    TransactionType = "TransferCredit",
                    Amount = request.Amount,
                    BalanceBefore = receiverBefore,
                    BalanceAfter = receiver.Balance,
                    ReferenceId = request.ReferenceId + "-CREDIT",
                    Status = "Success",
                    CreatedAt = DateTime.UtcNow
                };

                _context.WalletTransactions.AddRange(debit, credit);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new
                {
                    Message = "Transfer successful",
                    SenderBalance = sender.Balance,
                    ReceiverBalance = receiver.Balance
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // TRANSACTION HISTORY
        public async Task<object> GetTransactions(
            Guid walletId,
            string? type,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize)
        {
            var query = _context.WalletTransactions.AsNoTracking().Where(x => x.WalletId == walletId);
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x => x.TransactionType == type);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= fromDate);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= toDate);
            }

            var data = await query.OrderByDescending(x => x.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = data
            };
        }
    }
}