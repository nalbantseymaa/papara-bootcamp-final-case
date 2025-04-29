using System.Text.Json.Serialization;
using ExpenseTracking.Base;

namespace ExpenseTracking.Schema;

public class AddressRequest
{
    public string CountryCode { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public bool IsDefault { get; set; }
}

public class AddressResponse : BaseResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? EmployeeId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? DepartmentId { get; set; }
    public string? CountryCode { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Street { get; set; }
    public string? ZipCode { get; set; }
    public bool IsDefault { get; set; }
}
