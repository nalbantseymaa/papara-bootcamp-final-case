using System.Text.Json;

namespace ExpenseTracking.Base;

public class ApiResponse
{
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public ApiResponse(string message = null)
    {
        Success = string.IsNullOrWhiteSpace(message);
        Message = message;
    }

    public ApiResponse(bool isSuccess, string message)
    {
        Success = isSuccess;
        Message = message;
    }

    public bool Success { get; set; }
    public string Message { get; set; }
    public DateTime ServerDate { get; set; } = DateTime.UtcNow;
    public Guid ReferenceNo { get; set; } = Guid.NewGuid();
}

public class ApiResponse<T>
{
    public DateTime ServerDate { get; set; } = DateTime.UtcNow;
    public Guid ReferenceNo { get; set; } = Guid.NewGuid();
    public bool Success { get; set; }

    public string Message { get; set; }
    public T Response { get; set; }

    public ApiResponse(bool isSuccess)
    {
        Success = isSuccess;
        Response = default;
        Message = isSuccess ? "Success" : "Error";
    }
    public ApiResponse(T data)
    {
        Success = true;
        Response = data;
        Message = "Success";
    }
    public ApiResponse(string message)
    {
        Success = false;
        Response = default;
        Message = message;
    }
}


