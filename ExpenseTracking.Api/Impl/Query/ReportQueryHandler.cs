using System.Data;
using Dapper;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class GetCompanyReportHandler
    : IRequestHandler<GetCompanyReportQuery, CompanyReportResponse>,
        IRequestHandler<GetCompanyByStatusQuery, List<PeriodSummary>>,
        IRequestHandler<GetCompanyByEmployeeQuery, List<EmployeeReportResponse>>,
        IRequestHandler<GetEmployeeExpensesQuery, EmployeeExpenseReportResponse>
{
    private readonly string connectionString;
    private readonly IAppSession appSession;

    public GetCompanyReportHandler(IConfiguration configuration, IAppSession appSession)
    {
        connectionString = configuration.GetConnectionString("MsSqlConnection")!;
        this.appSession = appSession;
    }

    public async Task<CompanyReportResponse> Handle(
    GetCompanyReportQuery request,
    CancellationToken cancellationToken)
    {
        return await DatabaseHelper.ExecuteStoredProcAsync(
            connectionString: connectionString,
            procedureName: "GetCompanyExpenseReport",
            parameters: new { Period = request.Period.ToString() },
            mapFunc: async multi =>
            {
                var overall = (await multi.ReadAsync<PeriodSummary>()).ToList();
                var byCategory = (await multi.ReadAsync<CategorySummaryResponse>()).ToList();
                var byPaymentMethod = (await multi.ReadAsync<PaymentMethodSummaryResponse>()).ToList();
                var byDepartment = (await multi.ReadAsync<DepartmentSummaryResponse>()).ToList();

                return new CompanyReportResponse(overall)
                {
                    Overall = overall,
                    CategorySummaries = byCategory,
                    PaymentMethodSummaries = byPaymentMethod,
                    DepartmentSummaries = byDepartment
                };
            },
            cancellationToken: cancellationToken
        );
    }

    public async Task<List<PeriodSummary>> Handle(
    GetCompanyByStatusQuery request,
    CancellationToken cancellationToken)
    {
        return await DatabaseHelper.ExecuteStoredProcAsync(
            connectionString: connectionString,
            procedureName: "GetCompanyStatusReport",
            parameters: new { Period = request.Period.ToString() },
            mapFunc: async multi =>
            {
                var summaries = (await multi.ReadAsync<PeriodSummary>()).ToList();
                return summaries;
            },
            cancellationToken: cancellationToken
        );
    }

    public async Task<List<EmployeeReportResponse>> Handle(
     GetCompanyByEmployeeQuery request,
     CancellationToken cancellationToken)
    {
        return await DatabaseHelper.ExecuteStoredProcAsync(
            connectionString: connectionString,
            procedureName: "GetEmployeeReport",
            parameters: new { Period = request.Period.ToString() },
            mapFunc: async multi =>
            {
                var cores = (await multi.ReadAsync<EmployeeReportResponse>()).ToList();
                if (!cores.Any()) return cores;
                var cats = (await multi.ReadAsync<string>()).ToList();
                var pays = (await multi.ReadAsync<string>()).ToList();
                var locs = (await multi.ReadAsync<string>()).ToList();
                cores.ForEach(e =>
                {
                    e.ExpenseCategories = cats;
                    e.PaymentMethods = pays;
                    e.ExpenseLocations = locs;
                });
                return cores;
            },
            cancellationToken: cancellationToken
        );
    }

    public async Task<EmployeeExpenseReportResponse> Handle(
    GetEmployeeExpensesQuery request,
    CancellationToken cancellationToken)
    {
        var employeeId = Convert.ToInt32(appSession.UserId);

        return await DatabaseHelper.ExecuteStoredProcAsync(
            connectionString,
            "GetEmployeeExpenseReport",
            new { EmployeeId = employeeId },
            async multi =>
            {
                var info = await multi.ReadFirstOrDefaultAsync<(string EmployeeName, string DepartmentName)>();
                if (info.Equals(default)) return null;

                return new EmployeeExpenseReportResponse
                {
                    EmployeeName = info.EmployeeName,
                    DepartmentName = info.DepartmentName,
                    Summary = await multi.ReadFirstOrDefaultAsync<PeriodSummary>(),
                    ExpenseDetails = (await multi.ReadAsync<ExpenseDetail>()).ToList()
                };
            },
            cancellationToken
        );
    }

    /// <summary>
    /// Executes a stored procedure with the given <paramref name="procedureName"/>,
    /// <paramref name="parameters"/>, and <paramref name="mapFunc"/> to map the result.
    /// </summary>
    /// <param name="connectionString">The connection string to the database.</param>
    /// <param name="procedureName">The name of the stored procedure to execute.</param>
    /// <param name="parameters">The parameters to pass to the stored procedure.</param>
    /// <param name="mapFunc">A function to map the result of the stored procedure.</param>
    /// <param name="cancellationToken">The cancellation token to observe while awaiting the task.</param>
    /// <returns>The result of <paramref name="mapFunc"/>, which is the result of the stored procedure.</returns>
    public static class DatabaseHelper
    {
        public static async Task<T> ExecuteStoredProcAsync<T>(
            string connectionString,
            string procedureName,
            object parameters,
            Func<SqlMapper.GridReader, Task<T>> mapFunc,
            CancellationToken cancellationToken)
        {
            await using var conn = new SqlConnection(connectionString);
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            await using var multi = await conn.QueryMultipleAsync(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return await mapFunc(multi);
        }
    }
}



















// public class GetCompanyTotalsHandler
//     : IRequestHandler<GetCompanyTotalsQuery, IEnumerable<CompanyExpenseReportResponse>>
// {
//     private readonly IDbConnection _db;
//     public GetCompanyTotalsHandler(IConfiguration cfg)
//     {
//         _db = new SqlConnection(cfg.GetConnectionString("Default"));
//     }

//     public Task<IEnumerable<CompanyExpenseReportResponse>> Handle(
//         GetCompanyTotalsQuery request, CancellationToken cancellationToken)
//     {
//         const string spName = "sp_GetCompanyTotals";
//         return _db.QueryAsync<CompanyExpenseReportResponse>(
//             spName,
//             new { Period = request.Period.ToString() },
//             commandType: CommandType.StoredProcedure);
//     }
// }



