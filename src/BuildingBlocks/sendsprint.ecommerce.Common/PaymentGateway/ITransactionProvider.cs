using sendsprint.ecommerce.Common.PaymentGateway.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sendsprint.ecommerce.Common.PaymentGateway
{
    public interface ITransactionProvider
    {
        int GatewayId { get; }
        int CurrencySendPriority { get; set; }
        int CurrencyReceivePriority { get; set; }
        int MaxAmount { get; set; }
        string CredentialKey { get; }
        string Url { get; }
        GatewayStatus Status { get; set; }
        GatewayType GatewayType { get; }
        int AlertBalance { get; set; }
    }
}
