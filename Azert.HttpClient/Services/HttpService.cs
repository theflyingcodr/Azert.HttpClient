using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azert.HttpClient.Services.Interfaces;
using Newtonsoft.Json;

namespace Azert.HttpClient.Services {
    public class HttpService : IHttpService
    {
        public async Task<TResponse> CallHttpMethod<TResponse, TRequest>(string baseAddress, string uri, TRequest request,
                                                                         IDictionary<string, string> headers,
                                                                         HttpMethods method) {

            using(var client = new System.Net.Http.HttpClient()) {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(baseAddress);

                var endpointUri = new Uri(uri);

                headers.ToList().ForEach(x => client.DefaultRequestHeaders.Add(x.Key, x.Value));

                var response = await InvokeHttpCall(method, request, client, endpointUri);

                return await HandleResponse<TResponse, TRequest>(response);

            }
        }

        public async Task<TResponse> HandleResponse<TResponse, TRequest>(HttpResponseMessage response) {

            if(response == null)
                return default(TResponse);

            if(response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());

            if(response.StatusCode == HttpStatusCode.NotFound)
                return default(TResponse);

            throw new HttpRequestException(
                $"Reason : {response.ReasonPhrase} /n Response Code: {response.StatusCode}, /n Content: {await response.Content.ReadAsStringAsync()}");
        }

        public async Task<HttpResponseMessage> InvokeHttpCall<TRequest>(HttpMethods method, TRequest request, System.Net.Http.HttpClient client,
                                                               Uri endpointUri) {
            switch(method) {
                case HttpMethods.POST:
                    return await client.PostAsJsonAsync(endpointUri, request);
                case HttpMethods.GET:
                    return await client.GetAsync(endpointUri);
                case HttpMethods.DELETE:
                    return await client.DeleteAsync(endpointUri);
                case HttpMethods.PUT:
                    return await client.PutAsJsonAsync(endpointUri, request);
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }
    }
}