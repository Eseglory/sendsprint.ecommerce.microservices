namespace Paystack.Model
{
    public class PaystackRequest
    {
        public string amount { get; set; }
        public string reference { get; set;}
        public string callback_url { get; set; }
        public string email { get; set; }
    }
}