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




    }
}