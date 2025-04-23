using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllAdressesQuery : IRequest<ApiResponse<List<AddressResponse>>>;
public record GetAdressesByIdQuery(int Id) : IRequest<ApiResponse<AddressResponse>>;
public record CreateAddressForEmployeeCommand(int EmployeeId, AddressRequest Address)
  : IRequest<ApiResponse<AddressResponse>>;

public record CreateAddressForDepartmentCommand(int DepartmentId, AddressRequest Address)
  : IRequest<ApiResponse<AddressResponse>>;

public record UpdateAddressCommand(int Id, AddressRequest Address) : IRequest<ApiResponse>;
public record DeleteAddressCommand(int Id) : IRequest<ApiResponse>;