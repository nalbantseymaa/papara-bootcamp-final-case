using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base.Enum;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IMediator mediator;
    public ReportsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Şirket genelinde toplam harcama tutarını getirir.
    /// </summary>
    /// <param name="period">daily, weekly, monthly</param>
    [HttpGet("company/total")]
    [Authorize(Roles = "Manager")]
    public async Task<CompanyReportResponse> GetCompanyReport([FromQuery] ReportPeriod period)
    {
        var operation = new GetCompanyReportQuery(period);
        var result = await mediator.Send(operation);
        return result;
    }

    /// <summary>
    /// Statü bazlı (onaylanan / reddedilen) harcama özetini getirir.
    /// </summary>
    /// <param name="period">daily, weekly, monthly</param>
    [HttpGet("company/by-status")]
    [Authorize(Roles = "Manager")]
    public async Task<List<PeriodSummary>> GetCompanyByStatus([FromQuery] ReportPeriod period)
    {
        var operation = new GetCompanyByStatusQuery(period);
        var result = await mediator.Send(operation);
        return result;
    }

    /// <summary>
    /// Personel bazlı harcama yoğunluğunu getirir.
    /// </summary>
    /// <param name="period">daily, weekly, monthly</param>
    [HttpGet("company/by-employee")]
    [Authorize(Roles = "Manager")]
    public async Task<List<EmployeeReportResponse>> GetCompanyByEmployee(
    [FromQuery] ReportPeriod period,
    [FromQuery] long? employeeId)
    {
        var operation = new GetCompanyByEmployeeQuery(period, employeeId);
        var result = await mediator.Send(operation);
        return result;
    }

    /// <summary>
    /// Personel bazlı harcama özetini getirir.
    /// </summary>
    [HttpGet("GetEmployeeExpenses")]
    [Authorize(Roles = "Employee")]
    public async Task<EmployeeExpenseReportResponse> GetEmployeeExpenses()
    {
        var operation = new GetEmployeeExpensesQuery();
        var result = await mediator.Send(operation);
        return result;

    }
}
