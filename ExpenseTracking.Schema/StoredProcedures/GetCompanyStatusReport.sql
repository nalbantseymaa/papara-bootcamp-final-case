CREATE PROCEDURE GetCompanyStatusReport
    @Period NVARCHAR(10)  -- 'Daily', 'Weekly', 'Monthly'
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        -- PeriodLabel
        CASE 
            WHEN @Period = 'Daily'   THEN CONVERT(VARCHAR(10), e.ExpenseDate, 120)                                            -- yyyy-MM-dd
            WHEN @Period = 'Weekly'  THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(DATEPART(WEEK, e.ExpenseDate) AS VARCHAR(2)), 2))  
            WHEN @Period = 'Monthly' THEN CONCAT(YEAR(e.ExpenseDate), '-', RIGHT('0' + CAST(MONTH(e.ExpenseDate) AS VARCHAR(2)), 2))      
        END AS PeriodLabel,

        -- Total processed expense count and amount
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
END;
