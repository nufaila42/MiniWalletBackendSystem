using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniWalletBackendSystem.DTOs;
using MiniWalletBackendSystem.Services;

namespace MiniWalletBackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet(CreateWalletRequest request)
        {
            try
            {
                var result = await _walletService.CreateWallet(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{walletId}/balance")]
        public async Task<IActionResult> GetBalance(Guid walletId)
        {
            try
            {
                var result = await _walletService.GetBalance(walletId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{walletId}/transactions")]
        public async Task<IActionResult> GetTransactions(
            Guid walletId,
            string? type,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var result = await _walletService.GetTransactions(
                    walletId,
                    type,
                    fromDate,
                    toDate,
                    pageNumber,
                    pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
