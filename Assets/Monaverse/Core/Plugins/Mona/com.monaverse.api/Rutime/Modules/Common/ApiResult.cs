namespace Monaverse.Api.Modules.Common
{
    public record ApiResult(bool IsSuccess, string Message)
    {
        public bool IsSuccess { get; protected set; } = IsSuccess;
        public string Message { get; protected set; } = Message;

        public static ApiResult Success()
            => new(true, string.Empty);

        public static ApiResult Failed(string message)
            => new(false, message);
    }

    public record ApiResult<T> : ApiResult
    {
        public T Data { get; set; }
        public string JsonData { get; set; }

        public ApiResult(bool isSuccess, string message, T data, string jsonData = null)
            : base(isSuccess, message)
        {
            Data = data;
            JsonData = jsonData;
        }

        public ApiResult(bool isSuccess, string message) : base(isSuccess, message)
            => Data = default;

        public static ApiResult<T> Success(T data, string jsonData = null)
            => new(true, string.Empty, data, jsonData);

        public static ApiResult<T> Failed(string message, T data = default)
            => new(false, message, data);
    }
}