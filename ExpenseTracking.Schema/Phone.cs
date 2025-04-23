using ExpenseTracking.Base;

namespace ExpenseTracking.Schema;

public class PhoneRequest
{
    public string CountryCode { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsDefault { get; set; }
}
public class PhoneResponse : BaseResponse
{
    public string CountryCode { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsDefault { get; set; }

    public EmployeeResponse? Employee { get; set; }
    public DepartmentResponse? Department { get; set; }
    public ManagerResponse? Manager { get; set; }
}