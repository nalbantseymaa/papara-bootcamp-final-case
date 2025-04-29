using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Interfaces;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllAdressesQuery : IRequest<ApiResponse<List<AddressResponse>>>;
public record GetAdressesByIdQuery(int Id) : IRequest<ApiResponse<AddressResponse>>;
public record GetAdressesByParametersQuery(string? City, string? ZipCode, bool? IsDefault) : IRequest<ApiResponse<List<AddressResponse>>>;

public record CreateAddressForEmployeeCommand(long EmployeeId, AddressRequest Address)
    : IRequest<ApiResponse>, ICreateAddressCommand
{
  public void ApplyOwner(Address e) => e.EmployeeId = EmployeeId;
}
public record CreateAddressForDepartmentCommand(long DepartmentId, AddressRequest Address)
    : IRequest<ApiResponse>, ICreateAddressCommand
{
  public void ApplyOwner(Address e) => e.DepartmentId = DepartmentId;
}

public record UpdateAddressCommand(int Id, AddressRequest Address) : IRequest<ApiResponse>;
public record DeleteAddressCommand(int Id) : IRequest<ApiResponse>;