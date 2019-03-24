
namespace My9GAG.Models
{
    public class RequestStatus
    {
        public RequestStatus()
        {
            IsSuccessful = false;
            Message = string.Empty;
        }

        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}
