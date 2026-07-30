using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniWalletBackendSystem.DTOs;
using MiniWalletBackendSystem.Models;
using MiniWalletBackendSystem.Services;

namespace MiniWalletBackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public TransactionController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        //CREDIT 
        [HttpPost("credit")]
        public async Task<IActionResult> Credit(CreditRequest request)
        {
            try
            {
                var result = await _walletService.Credit(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //DEBIT
        [HttpPost("debit")]
        public async Task<IActionResult> Debit(DebitRequest request)
        {
            try
            {
                var result = await _walletService.Debit(request);


                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //TRANSFER
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferRequest request)
        {
            try
            {
                var result = await _walletService.Transfer(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
