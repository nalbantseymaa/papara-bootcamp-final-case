namespace ExpenseTracking.Schema;

public class PeriodSummary
{
    public PeriodSummary() { }
    public string PeriodLabel { get; set; }
    public int TotalCount { get; set; }
    public decimal TotalAmount { get; set; }
    public int ApprovedCount { get; set; }
    public decimal ApprovedAmount { get; set; }
    public int RejectedCount { get; set; }
    public decimal RejectedAmount { get; set; }
}

public record CategorySummaryResponse(
    string PeriodLabel,
    string CategoryName,
    int ExpenseCount,
    decimal TotalAmount
);

public record PaymentMethodSummaryResponse(
    string PeriodLabel,
    string PaymentMethodName,
    int ExpenseCount,
    decimal TotalAmount
);

public record DepartmentSummaryResponse(
    string PeriodLabel,
    string DepartmentName,
    int ExpenseCount,
    decimal TotalAmount
);

public class CompanyReportResponse
{
    public CompanyReportResponse(IEnumerable<PeriodSummary> overall) { }
    public List<PeriodSummary> Overall { get; init; }
    public List<CategorySummaryResponse> CategorySummaries { get; init; }
    public List<PaymentMethodSummaryResponse> PaymentMethodSummaries { get; init; }
    public List<DepartmentSummaryResponse> DepartmentSummaries { get; init; }
}

public class EmployeeReportResponse
{
    public int? EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string DepartmentName { get; set; }
    public int? TotalCount { get; set; }
    public decimal? TotalAmount { get; set; }
    public int ApprovedCount { get; set; }
    public decimal ApprovedAmount { get; set; }
    public int RejectedCount { get; set; }
    public decimal RejectedAmount { get; set; }
    public decimal? MaxExpense { get; set; }
    public decimal? MinExpense { get; set; }
    public decimal? AverageExpense { get; set; }
    public string TopExpenseCategory { get; set; }
    public string TopPaymentMethod { get; set; }
    public List<string> ExpenseCategories { get; set; } = new();
    public List<string> PaymentMethods { get; set; } = new();
    public List<string> ExpenseLocations { get; set; } = new();
}

public class ExpenseSummary
{
    public int? Count { get; set; }
    public decimal? TotalAmount { get; set; }
}

public class EmployeeExpenseReportResponse
{
    public string EmployeeName { get; set; }
    public string DepartmentName { get; set; }
    public PeriodSummary Summary { get; set; } = new();
    public List<ExpenseDetail> ExpenseDetails { get; set; } = new();
}

public class ExpenseDetail
{
    public string Category { get; set; }
    public string PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string Location { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string Status { get; set; }
    public string? Description { get; set; }
    public string? RejectionReason { get; set; }
}