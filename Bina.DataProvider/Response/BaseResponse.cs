namespace Bina.DataProvider.Response;

public class BaseResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    public BaseResponse(T data, bool success = true, string message = null)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public BaseResponse(string message)
    {
        Success = false;
        Message = message;
    }
}