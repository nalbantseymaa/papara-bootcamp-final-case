using ExpenseTracking.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracking.Api.Domain;

public class Phone : BaseEntity
{
       public long? UserId { get; set; }
       public long? DepartmentId { get; set; }
       public string CountryCode { get; set; }
       public string PhoneNumber { get; set; }
       public bool IsDefault { get; set; }
       public virtual User? User { get; set; }
       public virtual Department? Department { get; set; }
}

public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
{
       public void Configure(EntityTypeBuilder<Phone> builder)
       {
              builder.HasKey(x => x.Id);
              builder.Property(x => x.Id).UseIdentityColumn();

              builder.Property(x => x.InsertedDate).IsRequired();
              builder.Property(x => x.UpdatedDate).IsRequired(false);
              builder.Property(x => x.InsertedUser).IsRequired().HasMaxLength(250);
              builder.Property(x => x.UpdatedUser).HasMaxLength(250);
              builder.Property(x => x.IsActive).HasDefaultValue(true);

              builder.Property(x => x.CountryCode).IsRequired().HasMaxLength(3);
              builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(12);
              builder.Property(x => x.IsDefault).IsRequired();

              builder.HasOne(p => p.User)
              .WithMany(u => u.Phones)
              .HasForeignKey(p => p.UserId)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Cascade);

              builder.HasOne(p => p.Department)
                     .WithMany(d => d.Phones)
                     .HasForeignKey(p => p.DepartmentId)
                     .IsRequired(false)
                     .OnDelete(DeleteBehavior.ClientCascade);


              builder.HasIndex(x => x.PhoneNumber).IsUnique();

              builder.HasIndex(p => new { p.UserId, p.IsDefault })
                     .HasFilter($"{nameof(Phone.UserId)} IS NOT NULL AND {nameof(Phone.IsDefault)} = 1")
                     .IsUnique();

              builder.HasIndex(p => new { p.DepartmentId, p.IsDefault })
                     .HasFilter($"{nameof(Phone.DepartmentId)} IS NOT NULL AND {nameof(Phone.IsDefault)} = 1")
                     .IsUnique();

              builder.ToTable(tb => tb.HasCheckConstraint(
                     "CK_Phone_UserOrDepartment",
                     "((UserId IS NOT NULL AND DepartmentId IS NULL) OR (UserId IS NULL AND DepartmentId IS NOT NULL))"
                 ));
       }
}