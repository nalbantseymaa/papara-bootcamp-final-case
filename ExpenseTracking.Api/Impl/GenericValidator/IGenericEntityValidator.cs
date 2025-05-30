using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ExpenseTracking.Api.Impl.GenericValidator
{
    public interface IGenericEntityValidator
    {
        Task<(bool IsValid, TEntity? Entity, string? ErrorMessage)> ValidateActiveAndExistsAsync<TEntity>(DbSet<TEntity> dbSet, object id, CancellationToken cancellationToken) where TEntity : class;
    }
}
