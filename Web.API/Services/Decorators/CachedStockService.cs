using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Web.API.Dtos.Stock;
using Web.API.Helpers;
using Web.API.Interfaces.IServices;
using Web.API.Models;

namespace Web.API.Services.Decorators
{
    public class CachedStockService : IStockService
    {
        private readonly IStockService _inner;
        private readonly IDistributedCache _cache;

        public CachedStockService(IStockService inner, IDistributedCache cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public bool CheackIsFresh(Stock stock)
        {
            throw new NotImplementedException();
        }

        public Task<StockDto> Create(CreateStockRequestDto stockDto, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<StockDto?> CreateFromApi(string symbol, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string symbol, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<List<StockDto>> GetAllAsync(QueryObject query, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<StockDto?> GetById(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<StockDto?> GetBySymbol(string symbol, CancellationToken ct)
        {
            string key = $"stock-{symbol.ToUpper()}";

            var cachedData = await _cache.GetStringAsync(key, ct);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<StockDto>(cachedData);
            }

            var stockDto = await _inner.GetBySymbol(symbol, ct);

            if (stockDto != null)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3)
                };
                await _cache.SetStringAsync(key, JsonSerializer.Serialize(stockDto), options, ct);
            }
            return stockDto;
        }


        public Task<StockDto> GetOrCreateStockAsync(string symbol, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<StockDto?> RefreshStock(Stock stock, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<StockDto> Update(string symbol, UpdateStockRequestDto updateStock, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
