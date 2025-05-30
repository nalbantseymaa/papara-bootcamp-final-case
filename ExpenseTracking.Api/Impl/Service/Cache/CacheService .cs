using System.Text;
using AutoMapper;
using ExpenseTracking.Api.Impl.UnitOfWork;
using ExpenseTracking.Base;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ExpenseTracking.Api.Impl.Service.Cache;

public class CacheService<TEntity> : ICacheService<TEntity> where TEntity : BaseEntity
{
    protected readonly IUnitOfWork unitOfWork;
    protected readonly IDistributedCache distributedCache;
    private readonly IMapper mapper;

    public CacheService(IUnitOfWork unitOfWork, IDistributedCache distributedCache, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.distributedCache = distributedCache;
        this.mapper = mapper;
    }

    public async Task SetAsync<TEntity, TResponse>(string key, List<TEntity> entities, TimeSpan? expiration = null)
      where TEntity : class
      where TResponse : class
    {
        var mapped = mapper.Map<List<TResponse>>(entities);

        var options = new DistributedCacheEntryOptions
        {
            SlidingExpiration = expiration ?? TimeSpan.FromMinutes(60),
            AbsoluteExpiration = DateTime.UtcNow.AddHours(12)
        };

        string model = JsonConvert.SerializeObject(mapped, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        byte[] data = Encoding.UTF8.GetBytes(model);
        await distributedCache.SetAsync(key, data, options);
    }

    public async Task ClearCacheAsync(string key)
    {
        await distributedCache.RemoveAsync(key);
    }

    public async Task<List<TResponse>> GetAllAsync<TResponse>(string key)
        where TResponse : class
    {
        var data = await distributedCache.GetAsync(key);
        if (data != null)
        {
            var entities = JsonConvert.DeserializeObject<List<TResponse>>(Encoding.UTF8.GetString(data));
            return entities ?? new List<TResponse>();
        }

        var dbEntities = await unitOfWork.Repository<TEntity>().GetAllAsync();
        var list = dbEntities?.ToList() ?? new List<TEntity>();

        if (list.Any())
            await SetAsync<TEntity, TResponse>(key, list);

        return mapper.Map<List<TResponse>>(list);
    }
}




