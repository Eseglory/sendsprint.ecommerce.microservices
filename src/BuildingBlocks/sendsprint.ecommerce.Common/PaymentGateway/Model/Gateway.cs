using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sendsprint.ecommerce.Common.PaymentGateway.Model
{
    public class Gateway : BaseGateway
    {
        [Obsolete]
        public int Priority { get; set; }
        public string CredentialKey { get; set; }
        public string GatewayUrl { get; set; }
        public GatewayType GatewayType { get; set; }
        public decimal Balance { get; set; }
    }
}
