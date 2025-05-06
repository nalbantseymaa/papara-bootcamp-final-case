CREATE PROCEDURE GetEmployeeReport
    @Period     NVARCHAR(20),
    @EmployeeId INT = NULL   -- if NULL, all employees will be included
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @StartDate DATETIME = CASE 
            WHEN @Period = 'daily'   THEN DATEADD(HOUR,  -24, GETDATE())
            WHEN @Period = 'weekly'  THEN DATEADD(DAY,   -7,  CAST(GETDATE() AS DATE))
            WHEN @Period = 'monthly' THEN DATEADD(MONTH, -1,  CAST(GETDATE() AS DATE))
            ELSE GETDATE()
        END,
        @EndDate   DATETIME = GETDATE();

    -------------------------------------------------
    -- 1)  Summary + Approved/Rejected + TopExpense/TopPayment
    -------------------------------------------------
    SELECT 
        emp.Id                                     AS EmployeeId,
        CONCAT_WS(' ', emp.FirstName, NULLIF(emp.MiddleName,''), emp.LastName)
                                                   AS EmployeeName,
        d.Name                                     AS DepartmentName,

        COUNT(e.Id)                                AS TotalCount,
        SUM(e.Amount)                              AS TotalAmount,
        MAX(e.Amount)                              AS MaxExpense,
        MIN(e.Amount)                              AS MinExpense,
        AVG(e.Amount)                              AS AverageExpense,

        SUM(CASE WHEN e.Status = 2 THEN 1 ELSE 0 END)        AS ApprovedCount,
        SUM(CASE WHEN e.Status = 2 THEN e.Amount ELSE 0 END) AS ApprovedAmount,
        SUM(CASE WHEN e.Status = 3 THEN 1 ELSE 0 END)        AS RejectedCount,
        SUM(CASE WHEN e.Status = 3 THEN e.Amount ELSE 0 END) AS RejectedAmount,

        (SELECT TOP 1 c.Name
         FROM Expenses ex
         JOIN ExpenseCategories c ON ex.CategoryId = c.Id
         WHERE ex.IsActive = 0
           AND ex.ExpenseDate BETWEEN @StartDate AND @EndDate
           AND (@EmployeeId IS NULL OR ex.EmployeeId = @EmployeeId)
         GROUP BY c.Name
         ORDER BY SUM(ex.Amount) DESC
        )                                                AS TopExpenseCategory,

        (SELECT TOP 1 pm.Name
         FROM Expenses ex
         JOIN PaymentMethods pm ON ex.PaymentMethodId = pm.Id
         WHERE ex.IsActive = 0
           AND ex.ExpenseDate BETWEEN @StartDate AND @EndDate
           AND (@EmployeeId IS NULL OR ex.EmployeeId = @EmployeeId)
         GROUP BY pm.Name
         ORDER BY COUNT(*) DESC
        )                                                AS TopPaymentMethod

    FROM Expenses e
    JOIN Employees emp     ON e.EmployeeId   = emp.Id
    JOIN Departments d     ON emp.DepartmentId = d.Id
    WHERE e.IsActive = 0
      AND e.ExpenseDate BETWEEN @StartDate AND @EndDate
      AND (@EmployeeId IS NULL OR emp.Id = @EmployeeId)
    GROUP BY
        emp.Id, emp.FirstName, emp.MiddleName, emp.LastName, d.Name;


    -------------------------------------------------
    -- 2) Categories  (List<string>)
    -------------------------------------------------
    SELECT DISTINCT
        c.Name AS CategoryName
    FROM Expenses e
    JOIN ExpenseCategories c ON e.CategoryId = c.Id
    WHERE e.IsActive = 0
      AND e.ExpenseDate BETWEEN @StartDate AND @EndDate
      AND (@EmployeeId IS NULL OR e.EmployeeId = @EmployeeId);


    -------------------------------------------------
    -- 3) Payment Methods (List<string>)
    -------------------------------------------------
    SELECT DISTINCT
        pm.Name AS PaymentMethodName
    FROM Expenses e
    JOIN PaymentMethods pm ON e.PaymentMethodId = pm.Id
    WHERE e.IsActive = 0
      AND e.ExpenseDate BETWEEN @StartDate AND @EndDate
      AND (@EmployeeId IS NULL OR e.EmployeeId = @EmployeeId);


    -------------------------------------------------
    -- 4) Expense Locations (List<string>)
    -------------------------------------------------
    SELECT DISTINCT
        e.Location AS LocationName
    FROM Expenses e
    WHERE e.IsActive = 0
      AND e.ExpenseDate BETWEEN @StartDate AND @EndDate
      AND (@EmployeeId IS NULL OR e.EmployeeId = @EmployeeId);

END;
