
namespace sendsprint.ecommerce.Common.PaymentGateway.Model
{
    public class BaseResponse<T>
    {
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public Status EnumStatus { get; set; }

        public BaseResponse()
        {
        }

        public BaseResponse(string status, string statusDescription, string message)
        {
            Status = status;
            StatusDescription = statusDescription;
            Message = message;
        }

        public BaseResponse(string status, string statusDescription, string message, T data)
        {
            Status = status;
            StatusDescription = statusDescription;
            Message = message;
            Data = data;
        }
    }

    public class BaseResponse
    {
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public BaseResponse()
        {
        }

        public BaseResponse(string status, string message)
        {
            Status = status;
            Message = message;
        }

        public BaseResponse(string status, string statusDescription, string message)
        {
            Status = status;
            StatusDescription = statusDescription;
            Message = message;
        }

        public BaseResponse(string status, string statusDescription, string message, List<string> errors)
        {
            Status = status;
            StatusDescription = statusDescription;
            Message = message;
            Errors = errors;
        }
    }
    public enum Status
    {
        Succeess = 1,
        Failed = 2
    }
}
