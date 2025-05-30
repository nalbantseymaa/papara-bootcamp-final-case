namespace ExpenseTracking.Api.Impl.Service.Cache;

public interface ICacheService<TEntity> where TEntity : class
{
    Task SetAsync<TSource, TResponse>(string key, List<TSource> entities, TimeSpan? expiration = null)
       where TSource : class
       where TResponse : class;
    Task<List<TResponse>> GetAllAsync<TResponse>(string key) where TResponse : class; Task ClearCacheAsync(string key);

}