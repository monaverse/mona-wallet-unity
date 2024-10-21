namespace Monaverse.Api.Modules.Common
{
    public record ApiResult(bool IsSuccess, string Message, int StatusCode)
    {
        public bool IsSuccess { get; protected set; } = IsSuccess;
        public string Message { get; protected set; } = Message;
        public int StatusCode { get; protected set; } = StatusCode;

        public static ApiResult Success()
            => new(true, string.Empty, 200);

        public static ApiResult Failed(string message, int statusCode = 400)
            => new(false, message, statusCode);
    }

    public record ApiResult<T> : ApiResult
    {
        public T Data { get; set; }
        public string JsonData { get; set; }

        public ApiResult(bool isSuccess, string message, int statusCode, T data, string jsonData = null)
            : base(isSuccess, message, statusCode)
        {
            Data = data;
            JsonData = jsonData;
        }

        public ApiResult(bool isSuccess, string message, int statusCode) : base(isSuccess, message, statusCode)
            => Data = default;

        public static ApiResult<T> Success(T data, string jsonData = null)
            => new(true, string.Empty, 200, data, jsonData);

        public static ApiResult<T> Failed(string message, T data = default, int statusCode = 400)
            => new(false, message, statusCode, data);
    }
}