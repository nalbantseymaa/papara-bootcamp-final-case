using System;
using ExpenseTracking.Base;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpenseTracking.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityName = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ChangedValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalValues = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InsertedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InsertedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OpenDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InsertedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.CheckConstraint("CK_User_RoleCheck", "Role IN ('Employee', 'Manager')");
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Managers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    ManagerId = table.Column<long>(type: "bigint", nullable: true),
                    InsertedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.CheckConstraint("CK_Address_EmployeeOrDepartment", "((EmployeeId IS NOT NULL AND DepartmentId IS NULL) OR (EmployeeId IS NULL AND DepartmentId IS NOT NULL))");
                    table.ForeignKey(
                        name: "FK_Addresses_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ManagerId = table.Column<long>(type: "bigint", nullable: true),
                    InsertedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdentityNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IBAN = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExitDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    InsertedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phones", x => x.Id);
                    table.CheckConstraint("CK_Phone_UserOrDepartment", "((UserId IS NOT NULL AND DepartmentId IS NULL) OR (UserId IS NULL AND DepartmentId IS NOT NULL))");
                    table.ForeignKey(
                        name: "FK_Phones_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Phones_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentMethodId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InsertedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ExpenseCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseFiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpenseId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    InsertedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseFiles_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpenseId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IBAN = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InsertedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Description", "InsertedDate", "InsertedUser", "IsActive", "ManagerId", "Name", "UpdatedDate", "UpdatedUser" },
                values: new object[,]
                {
                    { 1L, "Human Resources", new DateTime(2025, 5, 7, 1, 56, 44, 456, DateTimeKind.Local).AddTicks(3926), "system", true, null, "HR", null, null },
                    { 2L, "Information Technology", new DateTime(2025, 5, 7, 1, 56, 44, 456, DateTimeKind.Local).AddTicks(3928), "system", true, null, "IT", null, null },
                    { 3L, "Finance Department", new DateTime(2025, 5, 7, 1, 56, 44, 456, DateTimeKind.Local).AddTicks(3929), "system", true, null, "Finance", null, null }
                });

            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "Id", "Description", "InsertedDate", "InsertedUser", "IsActive", "Name", "UpdatedDate", "UpdatedUser" },
                values: new object[,]
                {
                    { 1L, null, new DateTime(2025, 5, 7, 1, 56, 44, 456, DateTimeKind.Local).AddTicks(4016), "system", true, "Travel", null, null },
                    { 2L, null, new DateTime(2025, 5, 7, 1, 56, 44, 456, DateTimeKind.Local).AddTicks(4017), "system", true, "Food", null, null },
                    { 3L, null, new DateTime(2025, 5, 7, 1, 56, 44, 456, DateTimeKind.Local).AddTicks(4018), "system", true, "Office Supplies", null, null }
                });

            migrationBuilder.InsertData(
                table: "PaymentMethods",
                columns: new[] { "Id", "Description", "InsertedDate", "InsertedUser", "IsActive", "Name", "UpdatedDate", "UpdatedUser" },
                values: new object[,]
                {
                    { 1L, null, new DateTime(2025, 5, 7, 1, 56, 44, 456, DateTimeKind.Local).AddTicks(3990), "system", true, "Credit Card", null, null },
                    { 2L, null, new DateTime(2025, 5, 7, 1, 56, 44, 456, DateTimeKind.Local).AddTicks(3991), "system", true, "Bank Transfer", null, null },
                    { 3L, null, new DateTime(2025, 5, 7, 1, 56, 44, 456, DateTimeKind.Local).AddTicks(3993), "system", true, "Cash", null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "UserName", "Email", "Password", "Secret", "Role", "InsertedUser", "InsertedDate", "OpenDate" },
                values: new object[,]
                {
                    { 1L, "admin", "admin@example.com", PasswordGenerator.CreateSHA256("admin123", "secret"), "secret", "Manager", "system", DateTime.UtcNow, DateTime.UtcNow },
                    { 2L, "admin2", "admin2@example.com", PasswordGenerator.CreateSHA256("admin1234", "secret"), "secret", "Manager", "system", DateTime.UtcNow, DateTime.UtcNow },
                    { 3L, "employee1", "employee1@example.com", PasswordGenerator.CreateSHA256("employee123", "secret"), "secret", "Employee", "system", DateTime.UtcNow, DateTime.UtcNow },
                    { 4L, "employee2", "employee2@example.com", PasswordGenerator.CreateSHA256("employee1234", "secret"), "secret", "Employee", "system", DateTime.UtcNow, DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "Managers",
                columns: new[] { "Id", "FirstName", "LastName", "MiddleName" },
                values: new object[,]
                {
                    { 1L, "Admin", "User", null },
                    { 2L, "Admin2", "User2", null }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DepartmentId", "FirstName", "LastName", "IdentityNumber", "DateOfBirth", "Salary", "IBAN", "HireDate" },
                values: new object[,]
                {
                    { 3L, 1L, "Employee", "One", "12345678901", new DateTime(1990, 1, 1), 5000.00m, "TR000000000000000000000001", DateTime.UtcNow },
                    { 4L, 2L, "Employee", "Two", "12345678902", new DateTime(1992, 5, 15), 6000.00m, "TR000000000000000000000002", DateTime.UtcNow }
                });

            // Include stored procedures from the Schema/StoredProcedures directory
            var sqlFiles = new[]
            {
                "../ExpenseTracking.Schema/StoredProcedures/GetCompanyExpenseReport.sql",
                "../ExpenseTracking.Schema/StoredProcedures/GetCompanyStatusReport.sql",
                "../ExpenseTracking.Schema/StoredProcedures/GetEmployeeExpenseReport.sql",
                "../ExpenseTracking.Schema/StoredProcedures/GetEmployeeReport.sql"
            };

            foreach (var file in sqlFiles)
            {
                var sql = File.ReadAllText(file);
                migrationBuilder.Sql(sql);
            }

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_DepartmentId_IsDefault",
                table: "Addresses",
                columns: new[] { "DepartmentId", "IsDefault" },
                unique: true,
                filter: "DepartmentId IS NOT NULL AND IsDefault = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_EmployeeId_IsDefault",
                table: "Addresses",
                columns: new[] { "EmployeeId", "IsDefault" },
                unique: true,
                filter: "EmployeeId IS NOT NULL AND IsDefault = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ManagerId",
                table: "Addresses",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_Name",
                table: "ExpenseCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseFiles_ExpenseId",
                table: "ExpenseFiles",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CategoryId",
                table: "Expenses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_EmployeeId",
                table: "Expenses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaymentMethodId",
                table: "Expenses",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_Name",
                table: "PaymentMethods",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_EmployeeId",
                table: "Payments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ExpenseId",
                table: "Payments",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Phones_DepartmentId_IsDefault",
                table: "Phones",
                columns: new[] { "DepartmentId", "IsDefault" },
                unique: true,
                filter: "DepartmentId IS NOT NULL AND IsDefault = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Phones_PhoneNumber",
                table: "Phones",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Phones_UserId_IsDefault",
                table: "Phones",
                columns: new[] { "UserId", "IsDefault" },
                unique: true,
                filter: "UserId IS NOT NULL AND IsDefault = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Departments_DepartmentId",
                table: "Addresses",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Employees_EmployeeId",
                table: "Addresses",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Employees_ManagerId",
                table: "Departments",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ExpenseFiles");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Phones");

            migrationBuilder.DropTable(
                name: "Managers");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
