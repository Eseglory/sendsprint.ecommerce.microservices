using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace sendsprint.ecommerce.Common.Http
{
    public class RestClient<T> : IRestClient<T>
    {
        private ILogger _logger;

        public RestClient(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<RestResponse> PostFormAsync(string endPointUrl, Dictionary<string, string> headers, T payLoad)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _logger.LogDebug(endPointUrl);

            if (headers != null)
            {
                AddHeaders(client, headers);
            }
            var json = JsonConvert.SerializeObject(payLoad);
            var payLoadDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            using var form = new MultipartFormDataContent();
            foreach (var (key, value) in payLoadDict)
            {
                if (value != null)
                {
                    form.Add(new StringContent(value), key);
                }
            }

            HttpResponseMessage responsemessage = await client.PostAsync(endPointUrl, form);
            if (responsemessage.IsSuccessStatusCode)
            {
                using HttpContent content = responsemessage.Content;
                Task<string> result = content.ReadAsStringAsync();
                _logger.LogDebug(result.Result);
                return new RestResponse(responsemessage.IsSuccessStatusCode, responsemessage.StatusCode.ToString(), responsemessage.ReasonPhrase, result.Result);
            }

            var badresult = responsemessage.Content?.ReadAsStringAsync();
            _logger.LogWarning(badresult?.Result);
            return new RestResponse(responsemessage.IsSuccessStatusCode, responsemessage.StatusCode.ToString(), responsemessage.ReasonPhrase, badresult?.Result);
        }

        public async Task<RestResponse> PostAsync(string endPointUrl, Dictionary<string, string> headers, T payLoad)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (headers != null)
            {
                AddHeaders(client, headers);
            }
            String jsonInString = JsonConvert.SerializeObject(payLoad);

            _logger.LogDebug($"Post to {endPointUrl}: {jsonInString}");
            var ct = new StringContent(jsonInString, Encoding.UTF8, "application/json");
            HttpResponseMessage responsemessage = await client.PostAsync(endPointUrl, ct);
            RestResponse response = null;
            if (responsemessage.IsSuccessStatusCode)
            {
                using HttpContent content = responsemessage.Content;
                Task<string> result = content.ReadAsStringAsync();
                _logger.LogDebug($"Response from {endPointUrl}: {result.Result}");
                return new RestResponse(responsemessage.IsSuccessStatusCode, responsemessage.StatusCode.ToString(), responsemessage.ReasonPhrase, result.Result);
            }

            var badresult = responsemessage.Content?.ReadAsStringAsync();
            _logger.LogWarning($"Bad response from {endPointUrl}: {badresult?.Result}");
            return new RestResponse(responsemessage.IsSuccessStatusCode, responsemessage.StatusCode.ToString(), responsemessage.ReasonPhrase, badresult?.Result);
        }

        public async Task<RestResponse> PatchAsync(string endPointUrl, Dictionary<string, string> headers, T payLoad)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _logger.LogDebug(endPointUrl);

            if (headers != null)
            {
                AddHeaders(client, headers);
            }

            String jsonInString = JsonConvert.SerializeObject(payLoad);
            _logger.LogDebug(jsonInString);

            var ct = new StringContent(jsonInString, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), endPointUrl) { Content = ct };
            HttpResponseMessage responsemessage = await client.SendAsync(request);

            if (responsemessage.IsSuccessStatusCode)
            {
                using HttpContent content = responsemessage.Content;
                Task<string> result = content.ReadAsStringAsync();
                _logger.LogDebug(result.Result);
                return new RestResponse(responsemessage.IsSuccessStatusCode, responsemessage.StatusCode.ToString(), responsemessage.ReasonPhrase,
                    ExtractJSON(result.Result));
            }

            var badresult = responsemessage.Content?.ReadAsStringAsync();
            _logger.LogWarning(badresult?.Result);
            return new RestResponse(responsemessage.IsSuccessStatusCode, responsemessage.StatusCode.ToString(), responsemessage.ReasonPhrase,
                badresult?.Result);
        }

        public async Task<RestResponse> GetAsync(string endPointUrl, Dictionary<string, string> headers = null)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (headers != null)
            {
                AddHeaders(client, headers);
            }

            HttpResponseMessage responsemessage = await client.GetAsync(endPointUrl);

            if (responsemessage.IsSuccessStatusCode)
            {
                using HttpContent content = responsemessage.Content;
                Task<string> result = content.ReadAsStringAsync();
                _logger.LogDebug(result.Result);
                return new RestResponse(responsemessage.IsSuccessStatusCode, responsemessage.StatusCode.ToString(), responsemessage.ReasonPhrase, ExtractJSON(result.Result));
            }

            Task<string> badresult = responsemessage.Content?.ReadAsStringAsync();
            _logger.LogWarning(badresult?.Result);
            return new RestResponse(responsemessage.IsSuccessStatusCode, responsemessage.StatusCode.ToString(), responsemessage.ReasonPhrase, badresult?.Result);
        }

        private void AddHeaders(HttpClient client, Dictionary<string, string> headers)
        {
            foreach (var c in headers)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(c.Key, c.Value);
            }
        }

        private string ExtractJSON(string result)
        {

            if (result.StartsWith("{") || result.StartsWith("[")) return result;
            var match = Regex.Match(result, @"\{(.|\s)*\}");
            return match?.Value;
        }

    }
}
