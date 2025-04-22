using ExpenseTracking.Base;

namespace ExpenseTracking.Schema;

public class CategoryRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
}

public class CategoryResponse : BaseResponse
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<ExpenseResponse> Expenses { get; set; }

}

public class UpdateCategoryRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
