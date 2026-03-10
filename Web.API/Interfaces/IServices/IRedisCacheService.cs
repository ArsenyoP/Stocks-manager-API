namespace Web.API.Interfaces.IServices
{
    public interface IRedisCacheService
    {
        Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default);
    }
}
