using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.API.Extensions;
using Web.API.Interfaces;
using Web.API.Models;

namespace Web.API.Controllers
{
    [Route("api/ portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPorrfolioRepository _porrfolioRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;

        public PortfolioController(IPorrfolioRepository porrfolioRepo,
            UserManager<AppUser> userManager, IStockRepository stockRepo)
        {
            _porrfolioRepo = porrfolioRepo;
            _userManager = userManager;
            _stockRepo = stockRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio(CancellationToken ct)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _porrfolioRepo.GetUserPortfolio(appUser, ct);
            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol, CancellationToken ct)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            var stock = await _stockRepo.GetBySymbolAsync(symbol, ct);

            if (stock == null) return BadRequest("Stock not found");

            var userPortfolio = await _porrfolioRepo.GetUserPortfolio(user, ct);

            if (userPortfolio.Any(s => s.Symbol.ToLower() == symbol.ToLower())) return BadRequest("Can't add stock twice");


            var portfolioModel = new Portfolio
            {
                StockId = stock.ID,
                AppUserId = user.Id
            };

            await _porrfolioRepo.CreatePortfolioAsync(portfolioModel, ct);

            if (portfolioModel == null)
            {
                return StatusCode(500, "Can't create");
            }
            else
            {
                return Created();
            }

        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol, CancellationToken ct)
        {
            //get id from claims
            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            var userPortfolio = await _porrfolioRepo.GetUserPortfolio(appUser);

            var filtredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower());

            if (filtredStock.Count() == 1)
            {
                await _porrfolioRepo.DeletePortfolioAsync(appUser, symbol, ct);
            }
            else
            {
                throw new Exception("Can't find stock");
            }

            return Ok();
        }
    }
}
