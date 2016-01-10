using System.Net.Http;
using System.Threading.Tasks;

namespace Azert.HttpClient.Services
{
    public class HttpClientFactory
    {
        private System.Net.Http.HttpClient _httpClient;



        public async Task<HttpResponseMessage> Get(string uri)
        {


            return default(HttpResponseMessage);
        }
    }
}