using ExpenseTracking.Base.Enum;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetCompanyReportQuery(ReportPeriod Period) : IRequest<CompanyReportResponse>;
public record GetCompanyByStatusQuery(ReportPeriod Period) : IRequest<List<PeriodSummary>>;
public record GetCompanyByEmployeeQuery(ReportPeriod Period, long? EmployeeId) : IRequest<List<EmployeeReportResponse>>;
public record GetEmployeeExpensesQuery() : IRequest<EmployeeExpenseReportResponse>;



