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
            return _inner.CheackIsFresh(stock);
        }

        public async Task<StockDto> Create(CreateStockRequestDto stockDto, CancellationToken ct)
        {
            var createdStockDto = await _inner.Create(stockDto, ct);

            if (createdStockDto != null)
            {
                var symbolUpper = createdStockDto.Symbol.ToUpper();
                string key = $"stock-{symbolUpper}";

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3)
                };

                await _cache.SetStringAsync(key, JsonSerializer.Serialize(createdStockDto), options);
            }

            return createdStockDto;
        }

        public async Task<StockDto?> CreateFromApi(string symbol, CancellationToken ct)
        {
            return await _inner.CreateFromApi(symbol, ct);
        }

        public async Task Delete(string symbol, CancellationToken ct)
        {
            await _inner.Delete(symbol, ct);

            string key = $"stock-{symbol.ToUpper()}";
            await _cache.RemoveAsync(key, ct);
        }

        public async Task<List<StockDto>> GetAllAsync(QueryObject query, CancellationToken ct = default)
        {
            return await _inner.GetAllAsync(query, ct);
        }

        public async Task<StockDto?> GetById(int id, CancellationToken ct = default)
        {
            var stockDto = await _inner.GetById(id, ct);

            if (stockDto == null)
            {
                return null;
            }

            string key = $"stock-{stockDto.Symbol.ToUpper()}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(3)
            };

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(stockDto), options, ct);
            return stockDto;
        }

        public async Task<StockDto?> GetBySymbol(string symbol, CancellationToken ct)
        {
            string key = $"stock-{symbol.ToUpper()}";

            var cachedData = await _cache.GetStringAsync(key, ct);
            if (!string.IsNullOrEmpty(cachedData))
            {
                var deserializedData = JsonSerializer.Deserialize<StockDto>(cachedData);
                if (deserializedData != null)
                {
                    return deserializedData;
                }
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

        public async Task<StockDto> GetOrCreateStockAsync(string symbol, CancellationToken ct)
        {
            var stockDto = await _inner.GetOrCreateStockAsync(symbol, ct);

            string key = $"stock-{stockDto.Symbol.ToUpper()}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(3)
            };

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(stockDto), options, ct);
            return stockDto;
        }

        public async Task<StockDto?> RefreshStock(Stock stock, CancellationToken ct)
        {
            return await _inner.RefreshStock(stock, ct);
        }

        public async Task<StockDto> Update(string symbol, UpdateStockRequestDto updateStock, CancellationToken ct)
        {
            var stockDto = await _inner.Update(symbol, updateStock, ct);

            string oldKey = $"stock-{symbol.ToUpper()}";
            await _cache.RemoveAsync(oldKey, ct);

            string key = $"stock-{stockDto.Symbol.ToUpper()}";

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(3)
            };
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(stockDto), options, ct);
            return stockDto;
        }
    }
}
