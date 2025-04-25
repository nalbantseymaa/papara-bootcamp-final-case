using ExpenseTracking.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Context;
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Phone> Phones { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
    public DbSet<ExpenseFile> ExpenseFiles { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
        .ToTable("Users")
        .HasKey(u => u.Id);

        modelBuilder.Entity<Employee>()
            .ToTable("EmployeeUsers");

        modelBuilder.Entity<Manager>()
            .ToTable("ManagerUsers");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}