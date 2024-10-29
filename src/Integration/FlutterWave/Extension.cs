using FlutterWave.Model;
using FlutterWave.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using sendsprint.ecommerce.Common.PaymentGateway.Model;
using sendsprint.ecommerce.Common.PaymentGateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace FlutterWave
{
    public static class Extension
    {
        public static IServiceCollection AddFlutterWave(this IServiceCollection services, IConfiguration Configuration)
        {
            using IServiceScope scope = services.BuildServiceProvider().CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<FlutterWaveService>>();
            var gatewayManager = scope.ServiceProvider.GetRequiredService<IGatewayManager>();
            var sendSprintApiGatewayHost = Configuration.GetValue<string>("SendSprintApiGatewayHost");
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

            List<Gateway> flutterwaveGateways;
            try
            {
                //TODO Move this configuration to DB
                flutterwaveGateways = new List<Gateway>
                {
                    new Gateway
                    {
                        CredentialKey = "FlutterWaveConfig",
                        GatewayType = GatewayType.FlutterWave,
                        Name = "FlutterWave",
                        CurrencyReceivePriority = 1,
                        CurrencySendPriority = 1,
                        AlertBalance = 9000000,
                        Status = GatewayStatus.Online,
                        MaxAmount = 500000
                    }
                };
            }
            catch (Exception e)
            {
                return services;
            }

            foreach (var gw in flutterwaveGateways)
            {
                var config = Configuration.GetSection(gw.CredentialKey)?.Get<FlutterWaveConfig>();
                if (string.IsNullOrEmpty(config?.SecretKey))
                {
                    //TODO send a notification mail to alert admin that flutterwave service is down
                }

                gatewayManager.AddTransactionProvider(
                    new FlutterWaveService(httpClientFactory.CreateClient("Gateway"), config, logger, sendSprintApiGatewayHost, Convert.ToInt32(gw.Id),
                         gw.CurrencyReceivePriority, gw.CurrencySendPriority, gw.MaxAmount, gw.CredentialKey, gw.GatewayUrl, gw.Status, gw.AlertBalance)
                );
            }

            return services;
        }
    }
}
