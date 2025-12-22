using Web.API.Dtos.Stock;
using Web.API.Models;

namespace Web.API.Mappers
{
    public static class StockMappers
    {
        public static StockDto ToStockDto(this Stock stockModel)
        {
            return new StockDto
            {
                ID = stockModel.ID,
                CompanyName = stockModel.CompanyName,
                Symbol = stockModel.Symbol,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industy = stockModel.Industy,
                MarketCap = stockModel.MarketCap
            };
        }

    }
}
