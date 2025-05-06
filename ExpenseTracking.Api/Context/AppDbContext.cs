using ExpenseTracking.Api.Domain;
using ExpenseTracking.Base;
using ExpenseTracking.Base.Enum;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ExpenseTracking.Api.Context;
public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor? httpContextAccessor;

    private IAppSession? appSession;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public IAppSession? AppSession
    {
        get
        {
            if (appSession == null && httpContextAccessor?.HttpContext != null)
            {
                appSession = JwtManager.GetSession(httpContextAccessor.HttpContext);
            }
            return appSession;
        }
    }

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
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Payment> Payments { get; set; }

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUser = AppSession?.UserName ?? "anonymous";
        var currentTime = DateTime.Now;

        var entyList = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity
         && (e.State == EntityState.Deleted || e.State == EntityState.Added || e.State == EntityState.Modified));

        var auditLogs = new List<AuditLog>();

        foreach (var entry in entyList)
        {
            var baseEntity = (BaseEntity)entry.Entity;
            var properties = entry.Properties.ToList();
            var changedProperties = properties.Where(p => p.IsModified).ToList();
            var changedValues = changedProperties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
            var originalValues = properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
            var changedValuesString = JsonConvert.SerializeObject(changedValues.Select(kvp => new { Key = kvp.Key, Value = kvp.Value }));
            var originalValuesString = JsonConvert.SerializeObject(originalValues.Select(kvp => new { Key = kvp.Key, Value = kvp.Value }));

            var auditLog = new AuditLog
            {
                EntityName = entry.Entity.GetType().Name,
                EntityId = baseEntity.Id.ToString(),
                Action = entry.State.ToString(),
                Timestamp = currentTime,
                UserName = currentUser,
                ChangedValues = changedValuesString,
                OriginalValues = originalValuesString,
            };

            if (entry.State == EntityState.Added)
            {
                baseEntity.InsertedDate = currentTime;
                baseEntity.InsertedUser = currentUser;
                baseEntity.IsActive = true;
            }
            else if (entry.State == EntityState.Modified)
            {
                baseEntity.UpdatedDate = currentTime;
                baseEntity.UpdatedUser = currentUser;
            }
            else if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                baseEntity.IsActive = false;
                baseEntity.UpdatedDate = currentTime;
                baseEntity.UpdatedUser = currentUser;
            }

            auditLogs.Add(auditLog);
        }

        if (auditLogs.Any())
        {
            Set<AuditLog>().AddRange(auditLogs);
        }

        return base.SaveChangesAsync(cancellationToken);
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

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, UserName = "admin", Email = "admin@example.com", Password = PasswordGenerator.CreateSHA256("admin123", "secret"), Secret = PasswordGenerator.GeneratePassword(30), Role = UserRole.Manager.ToString(), InsertedUser = "system", InsertedDate = DateTime.Now },
            new User { Id = 2, UserName = "admin2", Email = "admin2@example.com", Password = PasswordGenerator.CreateSHA256("admin1234", "secret1"), Secret = PasswordGenerator.GeneratePassword(30), Role = UserRole.Manager.ToString(), InsertedUser = "system", InsertedDate = DateTime.Now },
            new User { Id = 3, UserName = "employee1", Email = "employee1@example.com", Password = PasswordGenerator.CreateSHA256("employee123", "secret1"), Secret = PasswordGenerator.GeneratePassword(30), Role = UserRole.Employee.ToString(), InsertedUser = "system", InsertedDate = DateTime.Now },
            new User { Id = 4, UserName = "employee2", Email = "employee2@example.com", Password = PasswordGenerator.CreateSHA256("employee1234", "secret2"), Secret = PasswordGenerator.GeneratePassword(30), Role = UserRole.Employee.ToString(), InsertedUser = "system", InsertedDate = DateTime.Now }
        );

        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "HR", Description = "Human Resources", InsertedUser = "system", InsertedDate = DateTime.Now, IsActive = true },
            new Department
            {
                Id = 2,
                Name = "IT",
                Description = "Information Technology",
                InsertedUser = "system",
                InsertedDate = DateTime.Now,
                IsActive = true
            },
            new Department
            {
                Id = 3,
                Name = "Finance",
                Description = "Finance Department",
                InsertedUser = "system",
                InsertedDate = DateTime.Now,
                IsActive = true
            }
        );

        modelBuilder.Entity<PaymentMethod>().HasData(
            new PaymentMethod { Id = 1, Name = "Credit Card", InsertedUser = "system", InsertedDate = DateTime.Now, IsActive = true },
            new PaymentMethod { Id = 2, Name = "Bank Transfer", InsertedUser = "system", InsertedDate = DateTime.Now, IsActive = true },
            new PaymentMethod { Id = 3, Name = "Cash", InsertedUser = "system", InsertedDate = DateTime.Now, IsActive = true }
        );

        modelBuilder.Entity<ExpenseCategory>().HasData(
            new ExpenseCategory { Id = 1, Name = "Travel", InsertedUser = "system", InsertedDate = DateTime.Now, IsActive = true },
            new ExpenseCategory { Id = 2, Name = "Food", InsertedUser = "system", InsertedDate = DateTime.Now, IsActive = true },
            new ExpenseCategory { Id = 3, Name = "Office Supplies", InsertedUser = "system", InsertedDate = DateTime.Now, IsActive = true }
        );

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}