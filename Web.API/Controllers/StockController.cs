using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Web.API.Data;
using Web.API.Dtos.Stock;
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
        public StockController(ApplicationDBContext context)
        {
            _context = context;
        }

        // <--------GETS-------->

        //повертається JSON 
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = await _context.Stocks.ToListAsync();

            var stockDto = stocks.Select(s => s.ToStockDto());
            return Ok(stockDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var stock = await _context.Stocks.FindAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        [HttpGet("top")]

        public async Task<IActionResult> GetTop()
        {
            var topStock = await _context.Stocks.OrderByDescending(s => -s.Purchase)
                .FirstOrDefaultAsync();
            if (topStock == null)
            {
                return NotFound();
            }
            return Ok(topStock.ToStockDto());
        }

        [HttpGet("symbol/{symbol}")]
        public async Task<IActionResult> GetBySymbol([FromRoute] string symbol)
        {
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        [HttpGet("high-div")]
        public async Task<IActionResult> GetThreeGighrstDivedents()
        {
            var threeHightDivs = await _context.Stocks
                .OrderByDescending(s => s.LastDiv).Take(3).ToListAsync();

            if (!threeHightDivs.Any())
            {
                return NotFound("Акцій не знайдено");
            }

            var threeHightDivsDto = threeHightDivs.Select(s => s.ToStockDto());

            return Ok(threeHightDivsDto);
        }



        // <--------POSTS-------->

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            var stockModel = stockDto.ToStockFromCreateDTO();
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = stockModel.ID }, stockModel.ToStockDto());
        }

        [HttpPost("light")]
        public async Task<IActionResult> CreateLight([FromBody] CreateLightStockRequestDto stockDto)
        {
            var stockLight = stockDto.ToStockFromCreateDtoLight();
            await _context.AddAsync(stockLight);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = stockLight.ID }, stockLight.ToStockDto());
        }



        // <--------UPDATE-------->
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateStock)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.ID == id);

            if (stockModel == null)
            {
                return NotFound();
            }

            stockModel.Symbol = updateStock.Symbol;
            stockModel.CompanyName = updateStock.CompanyName;
            stockModel.MarketCap = updateStock.MarketCap;
            stockModel.Purchase = updateStock.Purchase;
            stockModel.Industy = updateStock.Industy;
            stockModel.LastDiv = updateStock.LastDiv;
            await _context.SaveChangesAsync();

            return Ok(stockModel.ToStockDto());
        }


        [HttpPut("{id}/update-symbol/")]
        public async Task<IActionResult> UpdateSymbol([FromRoute] int id, [FromBody] string symbol)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.ID == id);

            if (stockModel == null)
            {
                return NotFound();
            }

            stockModel.Symbol = symbol;
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpPut("{id}/boost-dividents/")]
        public async Task<IActionResult> BoostDividents([FromRoute] int id, [FromBody] decimal percent)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.ID == id);

            if (stockModel == null)
            {
                return NotFound();
            }

            decimal updatedDevidents = stockModel.LastDiv * (percent / 100);
            stockModel.LastDiv = stockModel.LastDiv += updatedDevidents;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}/secure-update")]
        public async Task<IActionResult> SecureUpdate([FromRoute] int id, [FromBody] SecureUpdateDTO updateDTO)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.ID == id);

            if (stockModel == null)
            {
                return NotFound();
            }

            if (updateDTO.Purchase <= 0)
            {
                return BadRequest("Price cant be less than 0");
            }

            if (string.IsNullOrWhiteSpace(updateDTO.CompanyName))
            {
                return BadRequest("Name can't be empty");
            }

            stockModel.Purchase = updateDTO.Purchase;
            stockModel.CompanyName = updateDTO.CompanyName;
            await _context.SaveChangesAsync();

            return Ok();
        }


        // <--------DELETE-------->
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.ID == id);

            if (stockModel == null)
            {
                return NotFound();
            }

            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
