using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Paystack.Contracts;
using Paystack.Model;
using sendsprint.ecommerce.Common.PaymentGateway.Model;
using sendsprint.ecommerce.Common.PaymentGateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paystack
{
    public static class Extension
    {
        public static IServiceCollection AddPaystack(this IServiceCollection services, IConfiguration configuration)
        {
            using IServiceScope scope = services.BuildServiceProvider().CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<PaystackTransferService>>();
            var gatewayManager = scope.ServiceProvider.GetRequiredService<IGatewayManager>();
            var sendSprintEcommerceApiHost = configuration.GetValue<string>("AppSettings:SendSprintEcommerceAPIHost");

            List<Gateway> paystackGateways;
            try
            {
                //TODO Move this configuration to DB
                paystackGateways = new List<Gateway>
                {
                    new Gateway
                    {
                        CredentialKey = "PaystackConfig",
                        GatewayType = GatewayType.Paystack,
                        Name = "Paystack",
                        CurrencyReceivePriority = 1,
                        CurrencySendPriority = 1,
                        AlertBalance = 9000000,
                        Status = GatewayStatus.Online,
                        MaxAmount = 500000,
                        Id = 1
                    }
                };
            }
            catch (Exception e)
            {
                return services;
            }

            foreach (var gw in paystackGateways)
            {
                var config = configuration.GetSection(gw.CredentialKey)?.Get<PaystackConfig>();
                if (string.IsNullOrEmpty(config?.SecretKey))
                {
                    //TODO send a notification mail to alert admin that flutterwave service is down
                }

                gatewayManager.AddTransactionProvider(
                    new PaystackTransferService(config, logger, sendSprintEcommerceApiHost, Convert.ToInt32(gw.Id), gw.CurrencyReceivePriority,
                        gw.CurrencySendPriority, Convert.ToInt32(gw.MaxAmount), gw.CredentialKey, gw.GatewayUrl, gw.Status, gw.AlertBalance)
                );
            }

            return services;
        }
    }
}
