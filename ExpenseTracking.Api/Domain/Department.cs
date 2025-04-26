using ExpenseTracking.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracking.Api.Domain;

public class Department : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public long? ManagerId { get; set; }

    public virtual Employee Manager { get; set; }
    public virtual ICollection<Phone> Phones { get; set; }
    public virtual ICollection<Address> Addresses { get; set; }
    public virtual ICollection<Employee> Employees { get; set; }
}

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.InsertedDate).IsRequired(true);
        builder.Property(x => x.UpdatedDate).IsRequired(false);
        builder.Property(x => x.InsertedUser).IsRequired(true).HasMaxLength(250);
        builder.Property(x => x.UpdatedUser).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.IsActive).IsRequired(true).HasDefaultValue(true);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired(false).HasMaxLength(500);
        builder.Property(x => x.ManagerId).IsRequired(false);

        builder.HasOne(d => d.Manager)
               .WithMany(m => m.ManagedDepartments)
               .HasForeignKey(d => d.ManagerId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(d => d.Employees)
              .WithOne(e => e.Department)
              .HasForeignKey(e => e.DepartmentId)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Name).IsUnique(true);
    }
}