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

namespace Web.API.Controllers
{
    [Route("api/stock")]
    [ApiController]

    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        public StockController(IStockService stockService)
        {
            _stockService = stockService;
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

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateStock
            , CancellationToken ct)
        {
            var stockModel = await _stockService.Update(id, updateStock, ct);

            return Ok(stockModel);
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            await _stockService.Delete(id, ct);

            return NoContent();
        }
    }
}
