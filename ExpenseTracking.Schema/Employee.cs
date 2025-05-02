using ExpenseTracking.Base;

namespace ExpenseTracking.Schema;

public class EmployeeRequest
{
    public long DepartmentId { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string IdentityNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public decimal Salary { get; set; }

}

public class EmployeeDetailResponse : BaseResponse
{
    public long DepartmentId { get; set; }
    public string EmployeeName { get; set; }
    public string Email { get; set; }
    public string IdentityNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public decimal Salary { get; set; }
    public int EmployeeNumber { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? ExitDate { get; set; }
    public ICollection<AddressResponse> Addresses { get; set; }
    public ICollection<PhoneResponse> Phones { get; set; }
    public ICollection<ExpenseResponse> Expenses { get; set; }
}

public class EmployeeResponse : BaseResponse
{
    public long DepartmentId { get; set; }
    public string EmployeeName { get; set; }
    public string Email { get; set; }
    public string IdentityNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public decimal Salary { get; set; }
    public int EmployeeNumber { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? ExitDate { get; set; }
    public IList<ManagedDepartmentDto> ManagedDepartments { get; set; }
}

public class ManagedDepartmentDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
}

public class UpdateEmployeeRequest
{
    public long DepartmentId { get; set; }
    public decimal Salary { get; set; }
}
