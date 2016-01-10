using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Azert.HttpClient.Services.Interfaces
{
    public interface IHttpService
    {
        Task<TResponse> CallHttpMethod<TResponse, TRequest>(string baseAddress, string uri, TRequest request,
                                                                                  IDictionary<string, string> headers,
                                                                                  HttpMethods method);

        Task<TResponse> HandleResponse<TResponse, TRequest>(HttpResponseMessage response);

        Task<HttpResponseMessage> InvokeHttpCall<TRequest>(HttpMethods method, TRequest request, System.Net.Http.HttpClient client,
                                                                                 Uri endpointUri);
    }
}