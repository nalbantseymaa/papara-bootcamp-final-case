using AutoMapper;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Mapper;

public class MapperConfig : Profile
{
  public MapperConfig()
  {
    ConfigureEmployeeMappings();
    ConfigureManagerMappings();
    ConfigureUserMappings();
    ConfigureCategoryMappings();
    ConfigurePaymentMethodMappings();
    ConfigureDepartmentMappings();
    ConfigurePhoneMappings();
    ConfigureAddressMappings();
    ConfigureExpenseMappings();
    ConfigureExpenseFileMappings();
  }

  private void ConfigureEmployeeMappings()
  {
    CreateMap<EmployeeRequest, Employee>();
    CreateMap<Employee, EmployeeResponse>()
      .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
        $"{src.FirstName} {(string.IsNullOrWhiteSpace(src.MiddleName) ? "" : src.MiddleName + " ")}{src.LastName}"));
  }

  private void ConfigureManagerMappings()
  {
    CreateMap<ManagerRequest, Manager>();
    CreateMap<Manager, ManagerResponse>()
      .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src =>
        $"{src.FirstName} {(string.IsNullOrWhiteSpace(src.MiddleName) ? "" : src.MiddleName + " ")}{src.LastName}"));
    CreateMap<UserRequest, Manager>();
    CreateMap<Manager, UserResponse>();
  }

  private void ConfigureUserMappings()
  {
    CreateMap<UserRequest, Employee>()
      .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));
    CreateMap<UserRequest, User>();
    CreateMap<User, UserResponse>();
  }

  private void ConfigureCategoryMappings()
  {
    CreateMap<CategoryRequest, ExpenseCategory>();
    CreateMap<ExpenseCategory, CategoryResponse>();
  }

  private void ConfigurePaymentMethodMappings()
  {
    CreateMap<PaymentMethodRequest, PaymentMethod>();
    CreateMap<PaymentMethod, PaymentMethodResponse>();
  }

  private void ConfigureDepartmentMappings()
  {
    CreateMap<DepartmentRequest, Department>();
    CreateMap<Department, DepartmentResponse>();
  }

  private void ConfigurePhoneMappings()
  {
    CreateMap<PhoneRequest, Phone>();
    CreateMap<Phone, PhoneResponse>()
      .ForMember(d => d.EmployeeId, o => o.MapFrom(src =>
        (src.User as Employee) != null ? ((Employee)src.User).Id : (long?)null))
      .ForMember(d => d.ManagerId, o => o.MapFrom(src =>
        (src.User as Manager) != null ? ((Manager)src.User).Id : (long?)null))
      .ForMember(d => d.DepartmentId, o => o.MapFrom(src =>
        src.User == null ? src.DepartmentId : (long?)null));
  }

  private void ConfigureAddressMappings()
  {
    CreateMap<AddressRequest, Address>();
    CreateMap<Address, AddressResponse>()
      .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.EmployeeId.HasValue ? s.EmployeeId : null))
      .ForMember(d => d.DepartmentId, o => o.MapFrom(s => s.EmployeeId.HasValue ? null : s.DepartmentId));
  }

  private void ConfigureExpenseMappings()
  {
    CreateMap<ExpenseRequest, Expense>();
    CreateMap<Expense, ExpenseResponse>()
      .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
        $"{src.Employee.FirstName} {(string.IsNullOrWhiteSpace(src.Employee.MiddleName) ? "" : src.Employee.MiddleName + " ")}{src.Employee.LastName}"))
      .ForMember(d => d.RejectionReason, o => o.MapFrom(s => s.RejectionReason));
  }

  private void ConfigureExpenseFileMappings()
  {
    CreateMap<ExpenseFileRequest, ExpenseFile>()
      .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.File.FileName))
      .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.File.Length));
    CreateMap<ExpenseFile, ExpenseFileResponse>();
  }
}
