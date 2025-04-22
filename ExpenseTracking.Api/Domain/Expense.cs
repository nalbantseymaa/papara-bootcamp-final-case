using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Enum;
using ExpenseTracking.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class Expense : BaseEntity
{
    public long EmployeeId { get; set; }
    public long CategoryId { get; set; }
    public long PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string Location { get; set; }
    public DateTime ExpenseDate { get; set; }
    public ExpenseStatus Status { get; set; } = ExpenseStatus.Pending;
    public string? RejectionReason { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public bool? IsPaid { get; set; }
    public DateTime? PaymentDate { get; set; }
    public virtual Employee Employee { get; set; }
    public virtual ExpenseCategory Category { get; set; }
    public virtual PaymentMethod PaymentMethod { get; set; }
    public virtual ICollection<ExpenseFile> File { get; set; }
}
public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.InsertedDate).IsRequired(true);
        builder.Property(x => x.UpdatedDate).IsRequired(false);
        builder.Property(x => x.InsertedUser).IsRequired(true).HasMaxLength(250);
        builder.Property(x => x.UpdatedUser).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.IsActive).IsRequired(true).HasDefaultValue(true);

        builder.Property(x => x.EmployeeId).IsRequired(true);
        builder.Property(x => x.CategoryId).IsRequired(true);
        builder.Property(x => x.PaymentMethodId).IsRequired(true);
        builder.Property(x => x.Amount).IsRequired(true).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Description).IsRequired(false).HasMaxLength(500);
        builder.Property(x => x.Location).IsRequired(true).HasMaxLength(250);
        builder.Property(x => x.ExpenseDate).IsRequired(true);
        builder.Property(x => x.Status).IsRequired(true).HasDefaultValue(ExpenseStatus.Pending);
        builder.Property(x => x.RejectionReason).IsRequired(false).HasMaxLength(500);
        builder.Property(x => x.ApprovedDate).IsRequired(false);
        builder.Property(x => x.IsPaid).IsRequired(false);
        builder.Property(x => x.PaymentDate).IsRequired(false);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Expenses)
            .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PaymentMethod)
            .WithMany(x => x.Expenses)
            .HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.File)
            .WithOne(x => x.Expense)
            .HasForeignKey(x => x.ExpenseId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.EmployeeId).IsUnique(false);

    }
}