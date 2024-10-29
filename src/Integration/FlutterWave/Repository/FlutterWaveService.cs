using FlutterWave.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using sendsprint.ecommerce.Common;
using sendsprint.ecommerce.Common.PaymentGateway;
using sendsprint.ecommerce.Common.PaymentGateway.Model;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FlutterWave.Repository
{
    public class FlutterWaveService : TransactionProvider, IBankTransactionProvider
    {
        private const string TransferUrl = "https://api.flutterwave.com/v3/transfers";
        private const string BalanceEndpoint = "https://api.flutterwave.com/v3/balances";
        private const string ReceiveUrl = "https://api.flutterwave.com/v3/payments";
        private const string GetTransactionUrl = "https://api.flutterwave.com/v3/transactions/verify_by_reference";
        private const string ConfirmTransactionUrl = "https://api.flutterwave.com/v3/transactions/[id]/verify/";
        private const string AirtimeDisburseUrl = "https://api.flutterwave.com/v3/bills";
        private const string BankVerificationUrl = "https://api.flutterwave.com/v3/accounts/resolve";
        private const string CallBackUrl = "gatewaycallbacks/flutterwavecallback";
        private readonly HttpClient _client;
        private readonly ILogger<FlutterWaveService> _logger;
        private readonly FlutterWaveConfig _config;
        private readonly string _UtilityHubAPIHost;

        public FlutterWaveService(HttpClient client, FlutterWaveConfig config, ILogger<FlutterWaveService> logger,
            string UtilityHubAPIHost, int gatewayId, int currencyReceivePriority, int currencySendPriority,
            int maxAmount, string credentialKey, string url, GatewayStatus status, int gatewayAlert)
        {
            _client = client;
            _logger = logger;
            _config = config;
            _UtilityHubAPIHost = UtilityHubAPIHost;
            GatewayId = gatewayId;
            CurrencyReceivePriority = currencyReceivePriority;
            CurrencySendPriority = currencySendPriority;
            MaxAmount = maxAmount;
            CredentialKey = credentialKey;
            Url = url;
            Status = status;
            AlertBalance = gatewayAlert;
        }

        public override GatewayType GatewayType => GatewayType.FlutterWave;

        public async Task<BaseResponse<TransactionStatusResponse>> QueryTransaction(string transactionReference, string collectionReference)
        {
            long transactionId = 0;
            bool isFlutterwaveId = long.TryParse(transactionReference, out transactionId); //i now = 108  
            if (!isFlutterwaveId)
            {
                return await QueryTransactionByReference(transactionReference);
            }

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.SecretKey);

            var url = $"{ConfirmTransactionUrl.Replace("[id]", transactionReference)}";
            var response = await _client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var responseObject = JsonConvert.DeserializeObject<FlutterBaseResponse>(responseString);
                _logger.LogDebug($"Flutterwave QueryTransaction success {responseString}");
                var result = new TransactionStatusResponse
                {
                    TransactionStatus = MapTransactionStatus(responseObject.data.status, responseObject.data.processor_response),
                    Status = responseObject.data.status,
                    Amount = responseObject.data.amount,
                    SettledAmount = responseObject.data.amount_settled,
                    StatusDescription = responseObject.data.narration,
                    Reference = responseObject.data.reference,
                    TransactionInfo = string.Concat(responseObject.data.narration, "/", responseObject.data.processor_response)
                };
                return new BaseResponse<TransactionStatusResponse>(ApiResponseCodes.SUCCESSFUL,
                    nameof(ApiResponseCodes.SUCCESSFUL), "Query successful", result);
            }

            _logger.LogWarning($"Flutterwave QueryTransaction failed for {transactionReference}: {responseString}");
            return new BaseResponse<TransactionStatusResponse>(ApiResponseCodes.OPERATION_FAILED, nameof(ApiResponseCodes.OPERATION_FAILED),
                $"Error fetching transfer status.");
        }


        private async Task<BaseResponse<TransactionStatusResponse>> QueryTransactionByReference(string transactionReference)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.SecretKey);

            var url = $"{GetTransactionUrl}?tx_ref={transactionReference}";
            var response = await _client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var responseObject = JsonConvert.DeserializeObject<FlutterwaveTransactionVerificationResponse>(responseString);
                if (responseObject?.data != null && responseObject?.data?.amount > 0)
                {
                    _logger.LogDebug($"Flutterwave QueryTransactionByReference success {responseString}");
                    var result = new TransactionStatusResponse
                    {
                        TransactionStatus = MapTransactionStatus(responseObject.data.status, responseObject.data.processor_response),
                        Status = responseObject.data.status,
                        Amount = responseObject.data.amount,
                        SettledAmount = responseObject.data.amount_settled,
                        StatusDescription = responseObject.data.narration,
                        Reference = responseObject.data.flw_ref,
                        TransactionInfo = string.Concat(responseObject.data.narration, "/", responseObject.data.processor_response)
                    };
                    return new BaseResponse<TransactionStatusResponse>(ApiResponseCodes.SUCCESSFUL,
                        nameof(ApiResponseCodes.SUCCESSFUL), "Query successful", result);
                }
            }

            _logger.LogWarning($"Flutterwave QueryTransactionByReference failed for {transactionReference}: {responseString}");
            return new BaseResponse<TransactionStatusResponse>(ApiResponseCodes.OPERATION_FAILED, nameof(ApiResponseCodes.OPERATION_FAILED),
                $"Error fetching transfer status.");
        }

        public static TransactionStatus MapTransactionStatus(string statusStr, string processorResponse)
        {
            return statusStr switch
            {
                null => TransactionStatus.UNKNOWN,
                "successful" => TransactionStatus.SUCCESS,
                "failed" => TransactionStatus.CANCELLED,
                _ when "Transaction not completed by user".Equals(processorResponse, StringComparison.OrdinalIgnoreCase) => TransactionStatus.CANCELLED,
                _ => TransactionStatus.UNKNOWN
            };
        }

        public async Task<PaymentProvider> ReceiveFundsDetails(ReceiveFundsRequest request)
        {
            var linkResp = await ReceiveFundsLink(request);
            return (new PaymentProvider
            {
                Name = "Flutterwave",
                Amount = request.Amount,
                PaymentLink = linkResp.Status ? linkResp.Link : null
            });
        }

        public async Task<RecievePaymentResponseModel> ReceiveFundsLink(ReceiveFundsRequest request)
        {
            var customer = new FlutterCustomer
            {
                email = request.Email,
                phonenumber = request.PhoneNumber,
                name = request.Name
            };
            var flutterwaveRequest = new FluterwaveReceivePaymentRequest
            {
                amount = request.Amount.ToString(),
                tx_ref = request.Reference,
                currency = "NGN",
                payment_options = "card",
                redirect_url = $"{_UtilityHubAPIHost}/{CallBackUrl}",
                customer = customer
            };

            FlutterBaseResponse flutterBaseResponse = null;
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.SecretKey);
            var jsonObject = JsonConvert.SerializeObject(flutterwaveRequest);
            _logger.LogDebug($"Flutterwave ReceiveFunds request {jsonObject}");
            var response = await _client.PostAsync(new Uri($"{ReceiveUrl}"), new StringContent(jsonObject, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                flutterBaseResponse = JsonConvert.DeserializeObject<FlutterBaseResponse>(responseString);
                _logger.LogDebug($"Flutterwave ReceiveFunds response {responseString}");

                return new RecievePaymentResponseModel
                {
                    Message = flutterBaseResponse.message,
                    Status = true,
                    Link = flutterBaseResponse.data.Link,
                    PlatformType = GatewayType
                };
            }

            _logger.LogError($"Bad Flutterwave ReceiveFunds response {response.StatusCode} {response.ReasonPhrase}");
            return new RecievePaymentResponseModel
            {
                Message = response.ReasonPhrase,
                Status = false,
                Link = null
            };
        }
    }
}