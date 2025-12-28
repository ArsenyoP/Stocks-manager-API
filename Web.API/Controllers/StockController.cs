using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Web.API.Data;
using Web.API.Dtos.Stock;
using Web.API.Interfaces;
using Web.API.Mappers;
using Web.API.Models;

namespace Web.API.Controllers
{
    [Route("api/stock")]
    [ApiController]

    public class StockController : ControllerBase
    {
        //База даних
        private readonly ApplicationDBContext _context;
        private readonly IStockRepository _stockRepo;
        public StockController(ApplicationDBContext context, IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
            _context = context;
        }

        // <--------GETS-------->

        //повертається JSON 
        [HttpGet]
        //✅
        public async Task<IActionResult> GetAll()
        {
            var stocks = await _stockRepo.GetAllAsync();

            var stockDto = stocks.Select(s => s.ToStockDto());
            return Ok(stockDto);
        }

        [HttpGet("{id}")]
        //✅
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var stock = await _stockRepo.GetByIdAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        [HttpGet("top")]
        //✅
        public async Task<IActionResult> GetTop()
        {
            var topStock = await _stockRepo.GetTopAsync();
            if (topStock == null)
            {
                return NotFound();
            }
            return Ok(topStock.ToStockDto());
        }

        [HttpGet("symbol/{symbol}")]
        //✅
        public async Task<IActionResult> GetBySymbol([FromRoute] string symbol)
        {
            var stock = await _stockRepo.GetBySymbolAsync(symbol);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        [HttpGet("high-div")]
        //✅
        public async Task<IActionResult> GetThreeGighrstDivedents()
        {
            var threeHightDivs = await _stockRepo.GetThreeGighrstDivedentsAsync();

            if (!threeHightDivs.Any())
            {
                return NotFound("Акцій не знайдено");
            }

            var threeHightDivsDto = threeHightDivs.Select(s => s.ToStockDto());

            return Ok(threeHightDivsDto);
        }



        // <--------POSTS-------->

        [HttpPost]
        //✅
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            var stockModel = stockDto.ToStockFromCreateDTO();
            await _stockRepo.CreateAsync(stockModel);

            return CreatedAtAction(nameof(GetById), new { id = stockModel.ID }, stockModel.ToStockDto());
        }

        [HttpPost("light")]
        //✅
        public async Task<IActionResult> CreateLight([FromBody] CreateLightStockRequestDto stockDto)
        {
            var stockLight = stockDto.ToStockFromCreateDtoLight();
            await _stockRepo.CreateLightAsync(stockLight);

            return CreatedAtAction(nameof(GetById), new { id = stockLight.ID }, stockLight.ToStockDto());
        }



        // <--------UPDATE-------->
        [HttpPut("{id}")]
        //✅
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateStock)
        {
            var stockModel = await _stockRepo.UpdateAsync(id, updateStock);

            if (stockModel == null)
            {
                return NotFound();
            }

            return Ok(stockModel.ToStockDto());
        }


        [HttpPut("{id}/update-symbol/")]
        //✅
        public async Task<IActionResult> UpdateSymbol([FromRoute] int id, [FromBody] string symbol)
        {
            var stockModel = await _stockRepo.UpdateSymbolAsync(id, symbol);

            if (stockModel == null)
            {
                return NotFound();
            }
            return Ok();
        }

        //✅
        [HttpPut("{id}/boost-dividents/")]
        public async Task<IActionResult> BoostDividents([FromRoute] int id, [FromBody] decimal percent)
        {
            var stockModel = await _stockRepo.BoostDividentsAsync(id, percent);

            if (stockModel == null)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPut("{id}/secure-update")]
        //✅
        public async Task<IActionResult> SecureUpdate([FromRoute] int id, [FromBody] SecureUpdateDTO updateDTO)
        {
            if (updateDTO.Purchase <= 0)
            {
                return BadRequest("Price cant be less than 0");
            }

            if (string.IsNullOrWhiteSpace(updateDTO.CompanyName))
            {
                return BadRequest("Name can't be empty");
            }

            var stockModel = await _stockRepo.SecureUpdateAsync(id, updateDTO);

            if (stockModel == null)
            {
                return NotFound();
            }

            return Ok();
        }


        // <--------DELETE-------->
        [HttpDelete("{id}")]
        //✅
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var stockModel = await _stockRepo.DeleteAsync(id);

            if (stockModel == null)
            {
                return NotFound();
            }

            return NoContent();
        } //✅
    }
}
