using AutoMapper;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Schema;


namespace ExpenseTracking.Api.Mapper;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<EmployeeRequest, Employee>();
        CreateMap<Employee, EmployeeResponse>()
          .ForMember(dest => dest.EmployeeNumber, opt => opt.MapFrom(src =>
              $"{src.FirstName} {(string.IsNullOrWhiteSpace(src.MiddleName) ? "" : src.MiddleName + " ")}{src.LastName}"));

        CreateMap<CategoryRequest, ExpenseCategory>();
        CreateMap<ExpenseCategory, CategoryResponse>();

        CreateMap<PaymentMethodRequest, PaymentMethod>();
        CreateMap<PaymentMethod, PaymentMethodResponse>();

        CreateMap<DepartmentRequest, Department>();
        CreateMap<Department, DepartmentResponse>();

        CreateMap<ManagerRequest, Manager>();
        CreateMap<Manager, ManagerResponse>().ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src =>
            $"{src.FirstName} {(string.IsNullOrWhiteSpace(src.MiddleName) ? "" : src.MiddleName + " ")}{src.LastName}"));




    }
}