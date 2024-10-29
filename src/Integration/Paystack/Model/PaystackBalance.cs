using System;
using System.Collections.Generic;
using System.Text;

namespace Paystack.Model
{
    public class Data
    {
        public string currency { get; set; }
        public int balance { get; set; }
    }

    public class PaystackBalance
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<Data> data { get; set; }
    }

}
