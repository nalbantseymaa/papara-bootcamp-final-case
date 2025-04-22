using ExpenseTracking.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracking.Api.Domain;

public class Phone : BaseEntity
{
    public long? EmployeeId { get; set; }
    public long? ManagerId { get; set; }
    public long? DepartmentId { get; set; }

    public string CountryCode { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsDefault { get; set; }

    public virtual Employee? Employee { get; set; }
    public virtual Manager? Manager { get; set; }
    public virtual Department? Department { get; set; }
}

public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
{
    public void Configure(EntityTypeBuilder<Phone> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.InsertedDate).IsRequired(true);
        builder.Property(x => x.UpdatedDate).IsRequired(false);
        builder.Property(x => x.InsertedUser).IsRequired(true).HasMaxLength(250);
        builder.Property(x => x.UpdatedUser).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.IsActive).IsRequired(true).HasDefaultValue(true);

        builder.Property(x => x.EmployeeId).IsRequired(false);
        builder.Property(x => x.DepartmentId).IsRequired(false);
        builder.Property(x => x.ManagerId).IsRequired(false);
        builder.Property(x => x.CountryCode).IsRequired().HasMaxLength(3);
        builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(12);
        builder.Property(x => x.IsDefault).IsRequired();

        builder.HasOne(x => x.Employee)
               .WithMany(e => e.Phones)
               .HasForeignKey(x => x.EmployeeId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Manager)
               .WithMany(m => m.Phones)
               .HasForeignKey(x => x.ManagerId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Department)
               .WithMany(d => d.Phones)
               .HasForeignKey(x => x.DepartmentId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.EmployeeId, x.IsDefault })
               .HasFilter("EmployeeId IS NOT NULL AND IsDefault = 1")
               .IsUnique();

        builder.HasIndex(x => new { x.ManagerId, x.IsDefault })
               .HasFilter("ManagerId IS NOT NULL AND IsDefault = 1")
               .IsUnique();

        builder.HasIndex(x => new { x.DepartmentId, x.IsDefault })
               .HasFilter("DepartmentId IS NOT NULL AND IsDefault = 1")
               .IsUnique();
    }
}