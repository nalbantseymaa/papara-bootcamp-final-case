using ExpenseTracking.Base;

namespace ExpenseTracking.Schema;

public class DepartmentRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public long? ManagerId { get; set; }

}

public class DepartmentResponse : BaseResponse
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public long? ManagerId { get; set; }
    public EmployeeResponse Manager { get; set; }
    public List<EmployeeResponse> Employees { get; set; }
    public List<PhoneResponse> Phones { get; set; }
    public List<AddressResponse> Addresses { get; set; }

}