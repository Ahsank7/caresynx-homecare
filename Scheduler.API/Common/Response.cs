namespace Scheduler.API.Common
{
    public class Response<T>
    {
        private int _status;
        private bool _isSuccess;

        public int Status 
        { 
            get => _status; 
            set 
            { 
                _status = value;
                // Automatically set IsSuccess based on status code
                _isSuccess = value >= 200 && value < 300;
            } 
        }
        public string Message { get; set; }
        public T Data { get; set; }
        public bool IsSuccess 
        { 
            get => _isSuccess; 
            set => _isSuccess = value; 
        }
        public List<string> Errors { get; set; } = new List<string>();
        public string TraceId { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }

        public static Response<T> Success(T data, string message = "Operation completed successfully")
        {
            return new Response<T>
            {
                Status = StatusCodes.Status200OK,
                Message = message,
                Data = data,
                IsSuccess = true
            };
        }

        public static Response<T> Error(string message, int statusCode = StatusCodes.Status400BadRequest, List<string> errors = null)
        {
            return new Response<T>
            {
                Status = statusCode,
                Message = message,
                Data = default(T),
                IsSuccess = false,
                Errors = errors ?? new List<string>()
            };
        }

        public static Response<T> NotFound(string message = "Resource not found")
        {
            return Error(message, StatusCodes.Status404NotFound);
        }

        public static Response<T> BadRequest(string message, List<string> errors = null)
        {
            return Error(message, StatusCodes.Status400BadRequest, errors);
        }

        public static Response<T> InternalServerError(string message = "An internal server error occurred")
        {
            return Error(message, StatusCodes.Status500InternalServerError);
        }

        public static Response<T> InternalServerError(Exception ex, string message = "An internal server error occurred")
        {
            return new Response<T>
            {
                Status = StatusCodes.Status500InternalServerError,
                Message = message,
                Data = default(T),
                IsSuccess = false,
                Errors = new List<string>(),
                Exception = ex?.Message,
                StackTrace = ex?.StackTrace
            };
        }

        public static Response<T> Unauthorized(string message = "Unauthorized access")
        {
            return Error(message, StatusCodes.Status401Unauthorized);
        }

        public static Response<T> Forbidden(string message = "Access forbidden")
        {
            return Error(message, StatusCodes.Status403Forbidden);
        }
    }

    public class Response : Response<object>
    {
        public static Response Success(string message = "Operation completed successfully")
        {
            return new Response
            {
                Status = StatusCodes.Status200OK,
                Message = message,
                IsSuccess = true
            };
        }

        public static Response Error(string message, int statusCode = StatusCodes.Status400BadRequest, List<string> errors = null)
        {
            return new Response
            {
                Status = statusCode,
                Message = message,
                IsSuccess = false,
                Errors = errors ?? new List<string>()
            };
        }

        public static Response NotFound(string message = "Resource not found")
        {
            return Error(message, StatusCodes.Status404NotFound);
        }

        public static Response BadRequest(string message, List<string> errors = null)
        {
            return Error(message, StatusCodes.Status400BadRequest, errors);
        }

        public static Response InternalServerError(string message = "An internal server error occurred")
        {
            return Error(message, StatusCodes.Status500InternalServerError);
        }

        public static Response InternalServerError(Exception ex, string message = "An internal server error occurred")
        {
            return new Response
            {
                Status = StatusCodes.Status500InternalServerError,
                Message = message,
                IsSuccess = false,
                Errors = new List<string>(),
                Exception = ex?.Message,
                StackTrace = ex?.StackTrace
            };
        }

        public static Response Unauthorized(string message = "Unauthorized access")
        {
            return Error(message, StatusCodes.Status401Unauthorized);
        }

        public static Response Forbidden(string message = "Access forbidden")
        {
            return Error(message, StatusCodes.Status403Forbidden);
        }
    }
}
