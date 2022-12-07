using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using WalletTransaction.DAL;
using WalletTransaction.Model;
using WalletTransaction.Model.DTO;
using WalletTransaction.Services.Interfaces;

namespace WalletTransaction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletServices _services;
        private readonly IMapper _mapper;
        private readonly WalletDbContext _dbContext;
        private readonly IGetApi _getApi;

        public WalletsController(IWalletServices services, IMapper mapper, WalletDbContext dbContext, IGetApi getApi)
        {
            _services = services;
            _mapper = mapper;
            _dbContext = dbContext;
            this._getApi = getApi;
        }
        [HttpPost]
        [Route("Register_New_Wallet")]
        public IActionResult RegisterNewWalletAccount([FromBody] RegisterNewWalletModel newWallet)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(newWallet);
                var wallet = _mapper.Map<Wallet>(newWallet);
                return Ok(_services.Create(wallet, newWallet.WalletPassword, newWallet.ConfirmPassword));
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Wallet account registration failed");
            }
        }
        [HttpGet]
        [Route("Get_All_Wallet_Account")]
        public IActionResult GetAllWalletAccount()
        {
            var wallet = _services.GetAllWallets();
            var cleanAccount = _mapper.Map<GetwalletModel>(wallet);
            return Ok(cleanAccount);
        }
        [HttpGet]
        [Route("Get_wallet_By_Wallet_Number")]
        public IActionResult GetByWalletNumber(string WalletNumber)
        {
            try
            {
                if (Regex.IsMatch(WalletNumber, @"/^\d{10}$/"))
                {
                    var wallNumber = _services.GetWalletByWalletNumber(WalletNumber);
                    var cleanWallet = _mapper.Map<GetwalletModel>(wallNumber);
                    return Ok(cleanWallet);

                }

                return BadRequest("wallet Number must be 10-digits");

            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Error retrieving data from database");
            }

        }
        [HttpGet]
        [Route ("Get_wallet_By_Email")]
        public async Task < IActionResult> GetByEmail(string Email)
        {
            try
            {

                var walletEmail = await _services.GetByEmail(Email);
                var cleanWallet = _mapper.Map<GetwalletModel>(walletEmail);
                return Ok(cleanWallet);

            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Error retrieving data from database");
            }
            

           


        }
        [HttpPut ]
        [Route ("Update_wallet")]

        public async Task < IActionResult> UpdateWalletAccount([FromBody] UpdateWalletModel model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(model);
                var wallet = _mapper.Map<Wallet>(model);
                await _services.Update(wallet);
                return Ok(wallet);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Wallet update fails");
            }

        }
        [HttpGet]
        [Route("Get_All_Wallets")]
        public async Task<ActionResult<List<Wallet>>> GetWallets(int page)
        {
            try
            {
                if (_dbContext.Wallets == null)
                    return NotFound();
                var pageResult = 2f;
                var pageCount = Math.Ceiling(_dbContext.Wallets.Count() / pageResult);
                var wallet = await _dbContext.Wallets
                .Skip((page - 1) * (int)pageResult)
                    .Take((int)pageResult).ToListAsync();
                var responsonse = new paging<Wallet>
                {
                    Translist = wallet,
                    CurrentPage = page,
                    Pages = (int)pageCount
                };
                return Ok(responsonse);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Error retrieving data from database");
            }

        }
        [HttpGet]
        [Route ("Convert_Currency")]
        public async Task <IActionResult > ConvertCurrency(string Currency, double amount)
        {
            try
            {

                return Ok(await _getApi.GetRateAsync(Currency, amount));
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                     "Conversion Failed");
            }
        }
    }
}