namespace DocGenerator.Application.DTOs.Commons
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "OK")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Fail(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default
            };
        }

        public static ApiResponse<T> Fail(List<string> errors)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Errores de validación",
                Errors = errors,
                Data = default
            };
        }
    }
}