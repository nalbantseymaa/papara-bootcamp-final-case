using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Interfaces;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllPhonesQuery : IRequest<ApiResponse<List<PhoneResponse>>>;
public record GetPhonesByIdQuery(long Id) : IRequest<ApiResponse<PhoneResponse>>;
public record CreatePhoneForEmployeeCommand(long EmployeeId, PhoneRequest Phone)
    : IRequest<ApiResponse>, ICreatePhoneCommand
{
  public void ApplyOwner(Phone e) => e.UserId = EmployeeId;
}

public record CreatePhoneForDepartmentCommand(long DepartmentId, PhoneRequest Phone)
    : IRequest<ApiResponse>, ICreatePhoneCommand
{
  public void ApplyOwner(Phone e) => e.DepartmentId = DepartmentId;
}

public record CreatePhoneForManagerCommand(long ManagerId, PhoneRequest Phone)
    : IRequest<ApiResponse>, ICreatePhoneCommand
{
  public void ApplyOwner(Phone e) => e.UserId = ManagerId;
}
public record UpdatePhoneCommand(long Id, PhoneRequest Phone) : IRequest<ApiResponse>;
public record DeletePhoneCommand(long Id) : IRequest<ApiResponse>;