using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sendsprint.ecommerce.Common.PaymentGateway.Model
{
    public class BaseGateway
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int CurrencySendPriority { get; set; }
        public int CurrencyReceivePriority { get; set; }
        public int AlertBalance { get; set; }
        public GatewayStatus Status { get; set; }
        public int MaxAmount { get; set; }
    }
}
