
namespace PublicRelationWeb.Models
{
    public class Message
    {
        public Message()
        {
            IsSuccess = false;
            IsError = false;
        }
        public bool IsSuccess { get; set; }
        public string SuccessMessage { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }

}
