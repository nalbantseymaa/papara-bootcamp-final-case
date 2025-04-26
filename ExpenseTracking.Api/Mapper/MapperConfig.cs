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
      .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
          $"{src.FirstName} {(string.IsNullOrWhiteSpace(src.MiddleName) ? "" : src.MiddleName + " ")}{src.LastName}"));

        // .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));



        CreateMap<UserRequest, Employee>()
            .ForMember(dest => dest.UserName,
                       opt => opt.MapFrom(src => src.UserName));


        CreateMap<ManagerRequest, Manager>();
        CreateMap<Manager, ManagerResponse>().ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src =>
            $"{src.FirstName} {(string.IsNullOrWhiteSpace(src.MiddleName) ? "" : src.MiddleName + " ")}{src.LastName}"));

        CreateMap<UserRequest, Manager>();
        CreateMap<Manager, UserResponse>();

        CreateMap<UserRequest, User>();
        CreateMap<User, UserResponse>();

        CreateMap<CategoryRequest, ExpenseCategory>();
        CreateMap<ExpenseCategory, CategoryResponse>();

        CreateMap<PaymentMethodRequest, PaymentMethod>();
        CreateMap<PaymentMethod, PaymentMethodResponse>();

        CreateMap<DepartmentRequest, Department>();
        CreateMap<Department, DepartmentResponse>();




        CreateMap<PhoneRequest, Phone>();
        CreateMap<Phone, PhoneResponse>();

        CreateMap<AddressRequest, Address>();
        CreateMap<Address, AddressResponse>();

        CreateMap<ExpenseRequest, Expense>();
        CreateMap<Expense, ExpenseResponse>();

    }
}
