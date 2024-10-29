using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Paystack.Model;
using sendsprint.ecommerce.Common;
using sendsprint.ecommerce.Common.Http;
using sendsprint.ecommerce.Common.PaymentGateway;
using sendsprint.ecommerce.Common.PaymentGateway.Model;

namespace Paystack.Contracts
{
    public class PaystackTransferService : TransactionProvider, IBankTransactionProvider
    {
        private const string BaseUrl = "https://api.paystack.co";
        private const string PaymentLinkEndpoint = "/transaction/initialize";
        private const string TransferRecipientEndpoint = "/transferrecipient";
        private const string TransferEndpoint = "/transfer";
        private const string PaymentStatusEndpoint = "/transaction/verify";
        private const string CallBackUrl = "GatewayCallBacks/paystackcallback";
        private const string BalanceEndpoint = "/balance";
        private readonly PaystackConfig _config;
        private readonly ILogger<PaystackTransferService> _logger;
        private readonly string _utilityHubApiHost;

        public PaystackTransferService(PaystackConfig config, ILogger<PaystackTransferService> logger, string utilityHubApiHost, int gatewayId, int currencyReceivePriority, int currencySendPriority,
            int maxAmount, string credentialKey, string url, GatewayStatus status, int gatewayAlert)
        {
            _config = config;
            _logger = logger;
            _utilityHubApiHost = utilityHubApiHost;
            GatewayId = gatewayId;
            CurrencyReceivePriority = currencyReceivePriority;
            CurrencySendPriority = currencySendPriority;
            MaxAmount = maxAmount;
            CredentialKey = credentialKey;
            Url = url;
            Status = status;
            AlertBalance = gatewayAlert;
        }

        public override GatewayType GatewayType => GatewayType.Paystack;

        public async Task<BaseResponse<TransactionStatusResponse>> QueryTransaction(string transactionReference, string collectionReference)
        {
            try
            {
                var client = new RestClient<bool>(_logger);
                var headers = new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {_config.SecretKey}"}
            };

                var response = await client.GetAsync($"{BaseUrl}{PaymentStatusEndpoint}/{transactionReference}", headers);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Paystack QueryTransaction failed for ${transactionReference}: {response.Result}");
                    return new BaseResponse<TransactionStatusResponse>(ApiResponseCodes.OPERATION_FAILED, nameof(ApiResponseCodes.OPERATION_FAILED),
                        $"Error fetching transfer status.");
                }

                var result = JsonConvert.DeserializeObject<PaymentQueryResponse>(response.Result);

                _logger.LogDebug($"Paystack QueryTransaction success {response.Result}");

                return new BaseResponse<TransactionStatusResponse>(ApiResponseCodes.SUCCESSFUL, nameof(ApiResponseCodes.SUCCESSFUL), "Query successful",
                    new TransactionStatusResponse
                    {
                        TransactionStatus = MapTransactionStatus(result.Data.Status),
                        Status = result.Data.Status,
                        Amount = (double)(result.Data.Amount) / 100,
                        SettledAmount = (result.Data.Amount - result.Data.Fees) / 100
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Paystack QueryTransaction failed for ${ex.Message}");
                return new BaseResponse<TransactionStatusResponse>(ApiResponseCodes.OPERATION_FAILED, nameof(ApiResponseCodes.OPERATION_FAILED),
                    $"Error fetching transfer status.");
            }
        }

        public static TransactionStatus MapTransactionStatus(string statusStr)
        {
            return statusStr switch
            {
                null => TransactionStatus.UNKNOWN,
                "success" => TransactionStatus.SUCCESS,
                "abandoned" => TransactionStatus.CANCELLED,
                "cancelled" => TransactionStatus.CANCELLED,
                _ => TransactionStatus.UNKNOWN
            };
        }
        public async Task<PaymentProvider> ReceiveFundsDetails(ReceiveFundsRequest request)
        {
            var linkResp = await ReceiveFundsLink(request);

            return new PaymentProvider
            {
                Name = "Paystack",
                PaymentLink = linkResp.Status ? linkResp.Link : null,
                Amount = request.Amount
            };
        }

        public async Task<RecievePaymentResponseModel> ReceiveFundsLink(ReceiveFundsRequest request)
        {
            PaystackRequest paystack = new PaystackRequest
            {
                reference = request.Reference,
                callback_url = $"{_utilityHubApiHost}/{CallBackUrl}",
                amount = Convert.ToString(request.Amount * 100),
                email = request.Email,
            };

            var restClient = new RestClient<PaystackRequest>(_logger);
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {_config.SecretKey}");
            headers.Add("Cache-Control", "no-cache");

            var response = await restClient.PostAsync($"{BaseUrl}{PaymentLinkEndpoint}", headers, paystack);
            if (response.IsSuccessStatusCode)
            {
                var paymentResult = JsonConvert.DeserializeObject<PaymentResponse>(response.Result);

                return new RecievePaymentResponseModel
                {
                    Link = paymentResult.Data.authorization_url,
                    Message = paymentResult.Message,
                    Status = true,
                    PlatformType = GatewayType
                };
            }

            return new RecievePaymentResponseModel
            {
                Link = string.Empty,
                Message = "Not Successful",
                Status = false,
            };
        }
    }
}