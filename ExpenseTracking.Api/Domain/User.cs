using ExpenseTracking.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracking.Api.Domain;

public class User : BaseEntity
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Secret { get; set; }
    public string Role { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public virtual ICollection<Phone> Phones { get; set; }
    public virtual ICollection<Address> Addresses { get; set; }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.InsertedDate).IsRequired(true);
        builder.Property(x => x.UpdatedDate).IsRequired(false);
        builder.Property(x => x.InsertedUser).IsRequired(true).HasMaxLength(250);
        builder.Property(x => x.UpdatedUser).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.IsActive).IsRequired(true).HasDefaultValue(true);

        builder.Property(x => x.UserName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Password).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Secret).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Role).IsRequired().HasMaxLength(50);
        builder.Property(x => x.OpenDate).IsRequired();
        builder.Property(x => x.LastLoginDate).IsRequired(false);

        builder.ToTable(t => t.HasCheckConstraint("CK_User_RoleCheck",
       "Role IN ('Employee', 'Manager')"));

        builder.HasIndex(x => x.UserName).IsUnique(true);
        builder.HasIndex(x => x.Email).IsUnique(true);
    }
}