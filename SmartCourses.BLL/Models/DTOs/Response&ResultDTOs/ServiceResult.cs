namespace SmartCourses.BLL.Models.DTOs.Response_ResultDTOs
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ServiceResult<T> Success(T data, string? message = null)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        public static ServiceResult<T> Failure(string error)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Errors = new List<string> { error }
            };
        }

        public static ServiceResult<T> Failure(List<string> errors)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Errors = errors
            };
        }
    }

    // Non-generic version
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ServiceResult Success(string? message = null)
        {
            return new ServiceResult
            {
                IsSuccess = true,
                Message = message
            };
        }

        public static ServiceResult Failure(string error)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                Errors = new List<string> { error }
            };
        }

        public static ServiceResult Failure(List<string> errors)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                Errors = errors
            };
        }
    }
}
