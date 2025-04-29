using ExpenseTracking.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracking.Api.Domain;

public class Address : BaseEntity
{
       public long? EmployeeId { get; set; }
       public long? DepartmentId { get; set; }
       public string? CountryCode { get; set; }
       public string? City { get; set; }
       public string? District { get; set; }
       public string? Street { get; set; }
       public string? ZipCode { get; set; }
       public bool IsDefault { get; set; }
       public virtual Employee? Employee { get; set; }
       public virtual Department? Department { get; set; }
}

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
       public void Configure(EntityTypeBuilder<Address> builder)
       {
              builder.HasKey(ca => ca.Id);
              builder.Property(ca => ca.Id).UseIdentityColumn();

              builder.Property(x => x.InsertedDate).IsRequired(true);
              builder.Property(x => x.UpdatedDate).IsRequired(false);
              builder.Property(x => x.InsertedUser).IsRequired(true).HasMaxLength(250);
              builder.Property(x => x.UpdatedUser).IsRequired(false).HasMaxLength(250);
              builder.Property(x => x.IsActive).IsRequired(true).HasDefaultValue(true);

              builder.Property(ca => ca.EmployeeId).IsRequired(false);
              builder.Property(ca => ca.DepartmentId).IsRequired(false);
              builder.Property(ca => ca.CountryCode).IsRequired().HasMaxLength(3);
              builder.Property(ca => ca.City).IsRequired().HasMaxLength(100);
              builder.Property(ca => ca.District).IsRequired().HasMaxLength(100);
              builder.Property(ca => ca.Street).IsRequired().HasMaxLength(100);
              builder.Property(ca => ca.ZipCode).IsRequired().HasMaxLength(10);
              builder.Property(ca => ca.IsDefault).IsRequired();

              builder.HasOne(x => x.Employee)
                 .WithMany(e => e.Addresses)
                 .HasForeignKey(x => x.EmployeeId)
                 .OnDelete(DeleteBehavior.Cascade)
                 .IsRequired(false);

              builder.HasOne(x => x.Department)
                     .WithMany(d => d.Addresses)
                     .HasForeignKey(x => x.DepartmentId)
                     .OnDelete(DeleteBehavior.ClientCascade)
                     .IsRequired(false);

              builder.HasIndex(x => new { x.EmployeeId, x.IsDefault })
                     .HasFilter($"{nameof(Address.EmployeeId)} IS NOT NULL AND {nameof(Address.IsDefault)} = 1")
                     .IsUnique();

              builder.HasIndex(x => new { x.DepartmentId, x.IsDefault })
                     .HasFilter($"{nameof(Address.DepartmentId)} IS NOT NULL AND {nameof(Address.IsDefault)} = 1")
                     .IsUnique();

              builder.ToTable(tb => tb.HasCheckConstraint(
                  "CK_Address_EmployeeOrDepartment",
                  "((EmployeeId IS NOT NULL AND DepartmentId IS NULL) OR (EmployeeId IS NULL AND DepartmentId IS NOT NULL))"
              ));
       }
}