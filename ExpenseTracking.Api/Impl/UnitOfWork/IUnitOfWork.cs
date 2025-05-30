using ExpenseTracking.Api.Impl.GenericRepository;

namespace ExpenseTracking.Api.Impl.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> CommitAsync();
        int Commit();
    }
}