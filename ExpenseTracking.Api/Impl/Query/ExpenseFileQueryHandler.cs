using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Base.Enum;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFileTracking.Api.Impl.Query
{
    public class ExpenseFileQueryHandler :
        IRequestHandler<GetAllExpenseFilesQuery, ApiResponse<List<ExpenseFileResponse>>>,
        IRequestHandler<GetExpenseFileByIdQuery, ApiResponse<ExpenseFileResponse>>
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IAppSession appSession;

        public ExpenseFileQueryHandler(AppDbContext dbContext, IMapper mapper, IAppSession appSession)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.appSession = appSession;
        }

        public async Task<ApiResponse<List<ExpenseFileResponse>>> Handle(
            GetAllExpenseFilesQuery request,
            CancellationToken cancellationToken)
        {
            var query = dbContext.ExpenseFiles
                .Include(f => f.Expense)
                .Where(f => f.IsActive);

            query = FilterExpenseFilesByRole(query);

            var entities = await query.ToListAsync(cancellationToken);

            var mapped = mapper.Map<List<ExpenseFileResponse>>(entities);
            return new ApiResponse<List<ExpenseFileResponse>>(mapped);
        }

        public async Task<ApiResponse<ExpenseFileResponse>> Handle(
            GetExpenseFileByIdQuery request,
            CancellationToken cancellationToken)
        {
            var query = dbContext.ExpenseFiles
                .Include(f => f.Expense)
                .Where(f => f.IsActive && f.Id == request.Id);

            query = FilterExpenseFilesByRole(query);

            var entity = await query.FirstOrDefaultAsync(cancellationToken);
            if (entity == null)
                return new ApiResponse<ExpenseFileResponse>("Dosya bulunamadı");

            var mapped = mapper.Map<ExpenseFileResponse>(entity);
            return new ApiResponse<ExpenseFileResponse>(mapped);
        }

        private IQueryable<ExpenseFile> FilterExpenseFilesByRole(IQueryable<ExpenseFile> query)
        {
            if (appSession.UserRole != UserRole.Manager.ToString())
            {
                var userId = long.Parse(appSession.UserId);
                query = query.Where(f => f.Expense.EmployeeId == userId);
            }
            return query;
        }
    }
}
