using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.API.Extensions;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using Web.API.Models;

namespace Web.API.Controllers
{
    [Route("api/ portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPorfolioService _porfolioService;

        public PortfolioController(UserManager<AppUser> userManager, IStockRepository stockRepo,
            IPorfolioService porfolioService)
        {
            _userManager = userManager;
            _stockRepo = stockRepo;
            _porfolioService = porfolioService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio(CancellationToken ct)
        {
            var userID = User.GetUserID();

            var userPortfolio = await _porfolioService.GetUserPortfolio(userID, ct);
            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol, CancellationToken ct)
        {
            var userID = User.GetUserID();

            await _porfolioService.AddToPortfolio(symbol, userID, ct);
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol, CancellationToken ct)
        {
            var userID = User.GetUserID();

            await _porfolioService.DeletePortfolioAsync(symbol, userID, ct);
            return Ok();
        }
    }
}
