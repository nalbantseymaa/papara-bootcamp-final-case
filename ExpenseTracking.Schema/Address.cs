using ExpenseTracking.Base;

namespace ExpenseTracking.Schema;

public class AddressRequest
{
    public string? CountryCode { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Street { get; set; }
    public string? ZipCode { get; set; }
    public bool IsDefault { get; set; }
}

public class AddressResponse : BaseResponse
{
    public string? CountryCode { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Street { get; set; }
    public string? ZipCode { get; set; }
    public bool IsDefault { get; set; }
}