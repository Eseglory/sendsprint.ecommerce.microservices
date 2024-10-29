using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paystack.Model
{

    public class PaymentResponseData
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }

    public class PaymentResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public PaymentResponseData Data { get; set; }
    }
}
