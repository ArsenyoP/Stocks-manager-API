using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Web.API.Data;
using Web.API.Dtos.Stock;
using Web.API.Exceptions;
using Web.API.Helpers;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using Web.API.Mappers;
using Web.API.Models;
using Web.API.Services;

namespace Web.API.Controllers
{
    [Route("api/stock")]
    [ApiController]

    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly IFinancialService _financialService;

        public StockController(IStockService stockService, IFinancialService financialService)
        {
            _stockService = stockService;
            _financialService = financialService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query, CancellationToken ct)
        {
            var stocksDto = await _stockService.GetAllAsync(query, ct);

            return Ok(stocksDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var stockModel = await _stockService.GetById(id, ct);

            return Ok(stockModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto, CancellationToken ct)
        {
            var stockModel = await _stockService.Create(stockDto, ct);

            return CreatedAtAction(nameof(GetById), new { id = stockModel.ID }, stockModel);
        }

        [HttpPut("{symbol}")]
        public async Task<IActionResult> Update([FromRoute] string symbol, [FromBody] UpdateStockRequestDto updateStock
            , CancellationToken ct)
        {
            var stockModel = await _stockService.Update(symbol, updateStock, ct);

            return Ok(stockModel);
        }


        [HttpDelete("{symbol}")]
        public async Task<IActionResult> Delete([FromRoute] string symbol, CancellationToken ct)
        {
            await _stockService.Delete(symbol, ct);

            return NoContent();
        }

        [HttpGet("api/{symbol}")]
        public async Task<IActionResult> GetBySymbolFromApi([FromRoute] string symbol, CancellationToken ct)
        {
            var result = await _stockService.GetBySymbol(symbol, ct);

            return Ok(result);
        }





    }
}
