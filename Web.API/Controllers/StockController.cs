using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Web.API.Data;
using Web.API.Dtos.Stock;
using Web.API.Helpers;
using Web.API.Interfaces;
using Web.API.Mappers;
using Web.API.Models;

namespace Web.API.Controllers
{
    [Route("api/stock")]
    [ApiController]

    public class StockController : ControllerBase
    {

        private readonly IStockRepository _stockRepo;
        public StockController(IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var stocks = await _stockRepo.GetAllAsync(query);

            var stockDto = stocks.Select(s => s.ToStockDto()).ToList();
            return Ok(stockDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var stockModel = await _stockRepo.GetById(id);

            if (stockModel == null)
            {
                return BadRequest("Can't find stock");
            }

            return Ok(stockModel.ToStockDto());
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stockModel = stockDto.ToStockFromCreateDTO();
            await _stockRepo.CreateAsync(stockModel);

            return CreatedAtAction(nameof(GetById), new { id = stockModel.ID }, stockModel.ToStockDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateStock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stockModel = await _stockRepo.UpdateAsync(id, updateStock);

            if (stockModel == null)
            {
                return NotFound();
            }

            return Ok(stockModel.ToStockDto());
        }

        [HttpPut("{id:int}/boost-dividents/")]
        public async Task<IActionResult> BoostDividents([FromRoute] int id, [FromBody] decimal percent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stockModel = await _stockRepo.BoostDividentsAsync(id, percent);

            if (stockModel == null)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var stockModel = await _stockRepo.DeleteAsync(id);

            if (stockModel == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
