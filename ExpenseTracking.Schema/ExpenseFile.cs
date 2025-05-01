using ExpenseTracking.Base;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracking.Schema;

public class ExpenseFileRequest
{
    public long ExpenseId { get; set; }
    public IFormFile File { get; set; }
}

public class UpdateExpenseFileRequest
{
    public IFormFile File { get; set; }
}

public class ExpenseFileResponse : BaseResponse
{
    public long ExpenseId { get; set; }
    public string FileType { get; set; } = null!;
    public string FileName { get; set; } = null!;
}


