using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ExpenseTracking.Api.Impl.GenericValidator
{
    public class GenericEntityValidator : IGenericEntityValidator
    {
        public async Task<(bool IsValid, TEntity? Entity, string? ErrorMessage)> ValidateActiveAndExistsAsync<TEntity>(DbSet<TEntity> dbSet, object id, CancellationToken cancellationToken) where TEntity : class
        {
            var entity = await dbSet.FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                return (false, null, $"{typeof(TEntity).Name} not found");

            var isActiveProp = typeof(TEntity).GetProperty("IsActive");
            if (isActiveProp != null)
            {
                var isActive = isActiveProp.GetValue(entity) as bool?;
                if (isActive.HasValue && !isActive.Value)
                    return (false, null, $"{typeof(TEntity).Name} is inactive");
            }
            return (true, entity, null);
        }
    }
}
//hespinde acitve prop var 