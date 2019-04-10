
namespace My9GAG.Models
{
    public class RequestStatus
    {
        #region Constructors

        public RequestStatus()
        {
            IsSuccessful = false;
            Message = string.Empty;
        }

        #endregion

        #region Properties

        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

        #endregion
    }
}
