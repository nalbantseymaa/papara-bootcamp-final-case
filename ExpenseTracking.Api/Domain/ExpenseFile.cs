using ExpenseTracking.Base;
using ExpenseTracking.Base.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracking.Api.Domain;
public class ExpenseFile : BaseEntity
{
    public long ExpenseId { get; set; }
    public string FileName { get; set; }
    public FileType FileType { get; set; }
    public byte[] FileData { get; set; }
    public long FileSize { get; set; }
    public virtual Expense Expense { get; set; }

}

public class ExpenseFileConfiguration : IEntityTypeConfiguration<ExpenseFile>
{
    public void Configure(EntityTypeBuilder<ExpenseFile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.InsertedDate).IsRequired(true);
        builder.Property(x => x.UpdatedDate).IsRequired(false);
        builder.Property(x => x.InsertedUser).IsRequired(true).HasMaxLength(250);
        builder.Property(x => x.UpdatedUser).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.IsActive).IsRequired(true).HasDefaultValue(true);

        builder.Property(x => x.FileName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.FileType).IsRequired();
        builder.Property(x => x.FileSize).IsRequired();
        builder.Property(x => x.FileData).IsRequired().HasColumnType("varbinary(max)");
        builder.Property(x => x.ExpenseId).IsRequired(true);

    }
}