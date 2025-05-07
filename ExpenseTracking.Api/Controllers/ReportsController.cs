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
    /// Retrieves a detailed company expense report for a specified reporting period.
    /// </summary>
    /// <param name="period">The reporting period for which the company expense report is requested. Can be 'Daily', 'Weekly', or 'Monthly'.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the company expense report.</returns>
    [HttpGet("company/total")]
    [Authorize(Roles = "Manager")]
    public async Task<CompanyReportResponse> GetCompanyReport([FromQuery] ReportPeriod period)
    {
        var operation = new GetCompanyReportQuery(period);
        var result = await mediator.Send(operation);
        return result;
    }

    /// <summary>
    /// Retrieves a detailed company expense report by status for a specified reporting period.
    /// </summary>
    /// <param name="period">The reporting period for which the company expense report by status is requested. Can be 'Daily', 'Weekly', or 'Monthly'.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the company expense report by status.</returns>
    [HttpGet("company/by-status")]
    [Authorize(Roles = "Manager")]
    public async Task<List<PeriodSummary>> GetCompanyByStatus([FromQuery] ReportPeriod period)
    {
        var operation = new GetCompanyByStatusQuery(period);
        var result = await mediator.Send(operation);
        return result;
    }

    /// <summary>
    /// Retrieves a detailed company expense report by employee for a specified reporting period.
    /// </summary>
    /// <param name="period">The reporting period for which the company expense report by employee is requested. Can be 'Daily', 'Weekly', or 'Monthly'.</param>
    /// <param name="employeeId">The ID of the employee for which the company expense report by employee is requested. If null, the report will be generated for all employees.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the company expense report by employee.</returns>
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
    /// Retrieves the expense report for the authenticated employee.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the employee's expense report.</returns>
    [HttpGet("GetEmployeeExpenses")]
    [Authorize(Roles = "Employee")]
    public async Task<EmployeeExpenseReportResponse> GetEmployeeExpenses()
    {
        var operation = new GetEmployeeExpensesQuery();
        var result = await mediator.Send(operation);
        return result;

    }
}
