using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Web.API.Data;
using Web.API.Dtos.Stock;
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

        private readonly IStockRepository _stockRepo;
        private readonly IStockService _stockService;
        public StockController(IStockRepository stockRepo, IStockService stockService)
        {
            _stockRepo = stockRepo;
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
            var stockModel = await _stockRepo.GetById(id, ct);

            if (stockModel == null)
            {
                return BadRequest("Can't find stock");
            }

            return Ok(stockModel.ToStockDto());
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stockModel = stockDto.ToStockFromCreateDTO();
            await _stockRepo.CreateAsync(stockModel, ct);

            return CreatedAtAction(nameof(GetById), new { id = stockModel.ID }, stockModel.ToStockDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateStock
            , CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stockModel = await _stockRepo.UpdateAsync(id, updateStock, ct);

            if (stockModel == null)
            {
                return NotFound();
            }

            return Ok(stockModel.ToStockDto());
        }

        [HttpPut("{id:int}/boost-dividents/")]
        public async Task<IActionResult> BoostDividents([FromRoute] int id, [FromBody] decimal percent,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stockModel = await _stockRepo.BoostDividentsAsync(id, percent, ct);

            if (stockModel == null)
            {
                throw new Exception("Can't find stock");
            }
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            var stockModel = await _stockRepo.DeleteAsync(id, ct);

            if (stockModel == null)
            {
                throw new Exception("Can't find stock");
            }

            return NoContent();
        }
    }
}
