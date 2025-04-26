using ExpenseTracking.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracking.Api.Domain;

public class Employee : User
{
    public long DepartmentId { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string IdentityNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public decimal Salary { get; set; }

    public int EmployeeNumber { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? ExitDate { get; set; }

    public Department Department { get; set; }
    public ICollection<Department> ManagedDepartments { get; set; }
    public virtual ICollection<Address> Addresses { get; set; }
    public virtual ICollection<Phone> Phones { get; set; }
    public virtual ICollection<Expense> Expenses { get; set; }
}

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {

        builder.Property(x => x.DepartmentId).IsRequired();
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.MiddleName).IsRequired(false).HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
        builder.Property(x => x.IdentityNumber).IsRequired().HasMaxLength(11);
        builder.Property(x => x.DateOfBirth).IsRequired(true);

        builder.Property(x => x.Salary).IsRequired().HasPrecision(18, 2);
        builder.Property(x => x.EmployeeNumber).IsRequired();
        builder.Property(x => x.HireDate).IsRequired(true);
        builder.Property(x => x.ExitDate).IsRequired(false);

        builder.HasMany(x => x.Expenses)
            .WithOne(x => x.Employee)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.EmployeeNumber).IsUnique(true);

        builder.ToTable("Employees");
    }
}