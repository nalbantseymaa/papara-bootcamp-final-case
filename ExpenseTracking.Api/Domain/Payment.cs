using ExpenseTracking.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracking.Api.Domain;

public class Payment : BaseEntity
{
    public long ExpenseId { get; set; }
    public long EmployeeId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string IBAN { get; set; }
    public bool Success { get; set; }
    public string ReferenceNumber { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Message { get; set; }
    public virtual Expense Expense { get; set; }
    public virtual Employee Employee { get; set; }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.InsertedDate).IsRequired(true);
        builder.Property(x => x.UpdatedDate).IsRequired(false);
        builder.Property(x => x.InsertedUser).IsRequired(true).HasMaxLength(250);
        builder.Property(x => x.UpdatedUser).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.IsActive).IsRequired(true).HasDefaultValue(true);

        builder.Property(x => x.ExpenseId).IsRequired(true);
        builder.Property(x => x.EmployeeId).IsRequired(true);
        builder.Property(x => x.Amount).IsRequired(true).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Description).IsRequired(false).HasMaxLength(500);
        builder.Property(x => x.IBAN).IsRequired(true).HasMaxLength(26);
        builder.Property(x => x.Success).IsRequired(true).HasDefaultValue(false);
        builder.Property(x => x.ReferenceNumber).IsRequired(true).HasMaxLength(50);
        builder.Property(x => x.Timestamp).IsRequired(true).HasDefaultValueSql("GETDATE()");
        builder.Property(x => x.Message).IsRequired(false).HasMaxLength(500);

        builder.HasOne(x => x.Expense)
        .WithMany(x => x.Payments)
        .HasForeignKey(x => x.ExpenseId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Employee)
        .WithMany(x => x.Payments)
        .HasForeignKey(x => x.EmployeeId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}