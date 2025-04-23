using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllPhonesQuery : IRequest<ApiResponse<List<PhoneResponse>>>;
public record GetPhonesByIdQuery(int Id) : IRequest<ApiResponse<PhoneResponse>>;
public record CreatePhoneForEmployeeCommand(int EmployeeId, PhoneRequest Phone)
  : IRequest<ApiResponse<PhoneResponse>>;

public record CreatePhoneForDepartmentCommand(int DepartmentId, PhoneRequest Phone)
  : IRequest<ApiResponse<PhoneResponse>>;
public record CreatePhoneForManagerCommand(int ManagerId, PhoneRequest Phone)
: IRequest<ApiResponse<PhoneResponse>>;

public record UpdatePhoneCommand(int Id, PhoneRequest Phone) : IRequest<ApiResponse>;
public record DeletePhoneCommand(int Id) : IRequest<ApiResponse>;