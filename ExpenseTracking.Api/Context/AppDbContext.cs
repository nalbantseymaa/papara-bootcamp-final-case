using ExpenseTracking.Api.Domain;
using ExpenseTracking.Base;
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}