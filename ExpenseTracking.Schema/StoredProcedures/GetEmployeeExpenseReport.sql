CREATE PROCEDURE GetEmployeeExpenseReport
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    -------------------------------------------------
    -- 1) Employee Information
    -------------------------------------------------
    SELECT
        E.FirstName
        + ISNULL(' ' + E.MiddleName, '')
        + ' ' + E.LastName AS EmployeeName,
        D.Name AS DepartmentName
    FROM Employees E
    INNER JOIN Departments D ON E.DepartmentId = D.Id
    WHERE E.Id = @EmployeeId;

    -------------------------------------------------
    -- 2) Summary Statistics
    -------------------------------------------------
    SELECT
        'All' AS PeriodLabel,
        COUNT(Ex.Id) AS TotalCount,
        SUM(Ex.Amount) AS TotalAmount,
        SUM(CASE WHEN Ex.Status = 2 THEN 1 ELSE 0 END) AS ApprovedCount,
        SUM(CASE WHEN Ex.Status = 2 THEN Ex.Amount ELSE 0 END) AS ApprovedAmount,
        SUM(CASE WHEN Ex.Status = 3 THEN 1 ELSE 0 END) AS RejectedCount,
        SUM(CASE WHEN Ex.Status = 3 THEN Ex.Amount ELSE 0 END) AS RejectedAmount
    FROM Expenses Ex
    WHERE Ex.EmployeeId = @EmployeeId;

    -------------------------------------------------
    -- 3) Expense Details
    -------------------------------------------------
    SELECT
        EC.Name AS Category,
        PM.Name AS PaymentMethod,
        Ex.Amount,
        Ex.Location,
        Ex.ExpenseDate,
        Ex.Status,
        Ex.Description,
        Ex.RejectionReason
    FROM Expenses Ex
    INNER JOIN ExpenseCategories EC ON Ex.CategoryId = EC.Id
    INNER JOIN PaymentMethods PM ON Ex.PaymentMethodId = PM.Id
    WHERE Ex.EmployeeId = @EmployeeId
    ORDER BY Ex.ExpenseDate DESC;
END;
