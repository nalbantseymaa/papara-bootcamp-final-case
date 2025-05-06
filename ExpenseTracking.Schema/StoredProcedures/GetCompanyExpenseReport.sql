CREATE PROCEDURE GetCompanyExpenseReport
    @Period NVARCHAR(10)  -- 'Daily', 'Weekly', 'Monthly'
AS
BEGIN
    SET NOCOUNT ON;

    -------------------------------------------------
    -- 1) Main Report (only processed records = IsActive = 0)
    -------------------------------------------------
    SELECT 
        -- PeriodLabel: formatted as daily/weekly/monthly
        CASE 
            WHEN @Period = 'Daily'   THEN CONVERT(VARCHAR(10), e.ExpenseDate, 120)
            WHEN @Period = 'Weekly'  THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(DATEPART(WEEK, e.ExpenseDate) AS VARCHAR(2)), 2))
            WHEN @Period = 'Monthly' THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(MONTH(e.ExpenseDate) AS VARCHAR(2)), 2))
        END AS PeriodLabel,
        COUNT(*)                         AS TotalCount,
        SUM(e.Amount)                    AS TotalAmount,
        SUM(CASE WHEN e.Status = 2 THEN 1 ELSE 0 END)        AS ApprovedCount,
        SUM(CASE WHEN e.Status = 2 THEN e.Amount ELSE 0 END) AS ApprovedAmount,
        SUM(CASE WHEN e.Status = 3 THEN 1 ELSE 0 END)        AS RejectedCount,
        SUM(CASE WHEN e.Status = 3 THEN e.Amount ELSE 0 END) AS RejectedAmount

    FROM Expenses e
    WHERE e.IsActive = 0
    GROUP BY
        CASE 
            WHEN @Period = 'Daily'   THEN CONVERT(VARCHAR(10), e.ExpenseDate, 120)
            WHEN @Period = 'Weekly'  THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(DATEPART(WEEK, e.ExpenseDate) AS VARCHAR(2)), 2))
            WHEN @Period = 'Monthly' THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(MONTH(e.ExpenseDate) AS VARCHAR(2)), 2))
        END
    ORDER BY PeriodLabel;

    -------------------------------------------------
    -- 2) Category-Based Report
    -------------------------------------------------
    SELECT
        CASE 
            WHEN @Period = 'Daily'   THEN CONVERT(VARCHAR(10), e.ExpenseDate, 120)
            WHEN @Period = 'Weekly'  THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(DATEPART(WEEK, e.ExpenseDate) AS VARCHAR(2)), 2))
            WHEN @Period = 'Monthly' THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(MONTH(e.ExpenseDate) AS VARCHAR(2)), 2))
        END AS PeriodLabel,
        c.Name AS CategoryName,
        COUNT(*) AS ExpenseCount,
        SUM(e.Amount) AS TotalAmount
    FROM Expenses e
    INNER JOIN ExpenseCategories c ON e.CategoryId = c.Id
    WHERE e.IsActive = 0
    GROUP BY
        CASE 
            WHEN @Period = 'Daily'   THEN CONVERT(VARCHAR(10), e.ExpenseDate, 120)
            WHEN @Period = 'Weekly'  THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(DATEPART(WEEK, e.ExpenseDate) AS VARCHAR(2)), 2))
            WHEN @Period = 'Monthly' THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(MONTH(e.ExpenseDate) AS VARCHAR(2)), 2))
        END,
        c.Name
    ORDER BY PeriodLabel, c.Name;

    -------------------------------------------------
    -- 3) Payment Method-Based Report
    -------------------------------------------------
    SELECT
        CASE 
            WHEN @Period = 'Daily'   THEN CONVERT(VARCHAR(10), e.ExpenseDate, 120)
            WHEN @Period = 'Weekly'  THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(DATEPART(WEEK, e.ExpenseDate) AS VARCHAR(2)), 2))
            WHEN @Period = 'Monthly' THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(MONTH(e.ExpenseDate) AS VARCHAR(2)), 2))
        END AS PeriodLabel,
        p.Name AS PaymentMethodName,
        COUNT(*) AS ExpenseCount,
        SUM(e.Amount) AS TotalAmount
    FROM Expenses e
    INNER JOIN PaymentMethods p ON e.PaymentMethodId = p.Id
    WHERE e.IsActive = 0
    GROUP BY
        CASE 
            WHEN @Period = 'Daily'   THEN CONVERT(VARCHAR(10), e.ExpenseDate, 120)
            WHEN @Period = 'Weekly'  THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(DATEPART(WEEK, e.ExpenseDate) AS VARCHAR(2)), 2))
            WHEN @Period = 'Monthly' THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(MONTH(e.ExpenseDate) AS VARCHAR(2)), 2))
        END,
        p.Name
    ORDER BY PeriodLabel, p.Name;

    -------------------------------------------------
    -- 4) Department-Based Report
    -------------------------------------------------
    SELECT
        CASE 
            WHEN @Period = 'Daily'   THEN CONVERT(VARCHAR(10), e.ExpenseDate, 120)
            WHEN @Period = 'Weekly'  THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(DATEPART(WEEK, e.ExpenseDate) AS VARCHAR(2)), 2))
            WHEN @Period = 'Monthly' THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(MONTH(e.ExpenseDate) AS VARCHAR(2)), 2))
        END AS PeriodLabel,
        d.Name AS DepartmentName,
        COUNT(*) AS ExpenseCount,
        SUM(e.Amount) AS TotalAmount
    FROM Expenses e
    INNER JOIN Employees emp ON e.EmployeeId = emp.Id
    INNER JOIN Departments d ON emp.DepartmentId = d.Id
    WHERE e.IsActive = 0
    GROUP BY
        CASE 
            WHEN @Period = 'Daily'   THEN CONVERT(VARCHAR(10), e.ExpenseDate, 120)
            WHEN @Period = 'Weekly'  THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(DATEPART(WEEK, e.ExpenseDate) AS VARCHAR(2)), 2))
            WHEN @Period = 'Monthly' THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(MONTH(e.ExpenseDate) AS VARCHAR(2)), 2))
        END,
        d.Name
    ORDER BY PeriodLabel, d.Name;

END;
