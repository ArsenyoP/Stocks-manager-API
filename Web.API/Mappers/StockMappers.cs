using System.Runtime.CompilerServices;
using Web.API.Dtos.Stock;
using Web.API.Models;

namespace Web.API.Mappers
{
    public static class StockMappers
    {
        //заповнення шаблону для відправлення "StockDto"
        public static StockDto ToStockDto(this Stock stockModel) //приклеюється метод stockModel до Stock
        {
            return new StockDto
            {
                ID = stockModel.ID,
                CompanyName = stockModel.CompanyName,
                Symbol = stockModel.Symbol,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industy = stockModel.Industy,
                MarketCap = stockModel.MarketCap,
                Comments = stockModel.Comments.Select(x => x.ToCommentDto()).ToList()
            };
        }

        public static StockPortfolioDto ToStockPortfolioDto(this Stock stockModel) //приклеюється метод stockModel до Stock
        {
            return new StockPortfolioDto
            {
                ID = stockModel.ID,
                CompanyName = stockModel.CompanyName,
                Symbol = stockModel.Symbol,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industy = stockModel.Industy,
                MarketCap = stockModel.MarketCap,
            };
        }


        public static Stock ToStockFromCreateDTO(this CreateStockRequestDto stockDto)
        {
            return new Stock
            {
                Symbol = stockDto.Symbol,
                CompanyName = stockDto.CompanyName,
                Purchase = stockDto.Purchase,
                LastDiv = stockDto.LastDiv,
                MarketCap = stockDto.MarketCap,
                Industy = stockDto.Industy
            };
        }
    }
}
