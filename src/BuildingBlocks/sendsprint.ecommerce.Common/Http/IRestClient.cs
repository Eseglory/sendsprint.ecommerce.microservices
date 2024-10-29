using System.Collections.Generic;
using System.Threading.Tasks;

namespace sendsprint.ecommerce.Common.Http
{
    public interface IRestClient<T>
    {
        Task<RestResponse> PostFormAsync(string endPointUrl, Dictionary<string, string> headers, T payLoad);
        Task<RestResponse> PostAsync(string endPointUrl, Dictionary<string, string> headers, T payLoad);
        Task<RestResponse> PatchAsync(string endPointUrl, Dictionary<string, string> headers, T payLoad);
        Task<RestResponse> GetAsync(string endPointUrl, Dictionary<string, string> headers);
    }

}