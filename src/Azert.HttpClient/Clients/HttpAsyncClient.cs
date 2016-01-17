using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azert.HttpClient.Clients.Interfaces;
using Azert.HttpClient.Services.Interfaces;

namespace Azert.HttpClient.Clients
{
    public class HttpAsyncClient : IHttpAsyncClient
    {
        private readonly IHttpService _httpService;
        private readonly IHttpCachingService _httpCachingService;

        public HttpAsyncClient(IHttpService httpService,
            IHttpCachingService httpCachingService)
        {
            _httpService = httpService;
            _httpCachingService = httpCachingService;

        }

        /// <summary>
        /// Performs an Http GET call to the specified base address and endpoint.
        /// </summary>
        /// <typeparam name="TResponse">Expected response type to return</typeparam>
        /// <param name="baseAddress">Base address to api service being called</param>
        /// <param name="uri">Endpoint</param>
        /// <param name="headers">Headers to add to request</param>
        /// <param name="cacheCheck">Optional - async function to check cache</param>
        /// <param name="setCache">Optional - async function to add an item to cache</param>
        /// <returns>TResponse - null if not found</returns>
        /// <exception cref="HttpRequestException">Thrown if non 200 or 404 status code returned</exception>
        public async Task<TResponse> Get<TResponse>(string baseAddress, string uri,
                                                    IDictionary<string, string> headers,
                                                    Func<string, Task<TResponse>> cacheCheck = null, Func<TResponse, string, Task> setCache = null) where TResponse : class
        {
            var cache = await _httpCachingService.CheckCache(baseAddress, uri, null, cacheCheck);

            if(cache != null)
                return cache;

            var response =
                await _httpService.CallHttpMethod<TResponse, object>(baseAddress, uri, null, headers, HttpMethods.GET);

            await _httpCachingService.AddToCache(response, baseAddress, uri, null, setCache);

            return response;
        }

        /// <summary>
        /// Performs an Http POST call to the specified base address and endpoint with an expected result of adding a resource
        /// </summary>
        /// <typeparam name="TResponse">Expected response type to return</typeparam>
        /// <typeparam name="TRequest">Request body</typeparam>
        /// <param name="baseAddress">Base address to api service being called</param>
        /// <param name="uri">Endpoint</param>
        /// <param name="request">Request object</param>
        /// <param name="headers">Headers to add to request</param>
        /// <param name="cacheCheck">Optional - function to check cache</param>
        /// <param name="setCache">Optional - function to add response to cache</param>
        /// <returns>TResponse - null if not found</returns>
        /// <exception cref="HttpRequestException">Thrown if non 200 or 404 status code returned</exception>
        public async Task<TResponse> Post<TRequest, TResponse>(string baseAddress, string uri, TRequest request,
                                                               IDictionary<string, string> headers,
                                                               Func<string, Task<TResponse>> cacheCheck = null,
                                                               Func<TResponse, string, Task> setCache = null) where TResponse : class
        {
            var cache = await _httpCachingService.CheckCache(baseAddress, uri, headers, cacheCheck);

            if(cache != null)
                return cache;

            var response =
                await _httpService.CallHttpMethod<TResponse, TRequest>(baseAddress, uri, request, headers, HttpMethods.POST);

            await _httpCachingService.AddToCache(response, baseAddress, uri, headers, setCache);

            return response;
        }

        /// <summary>
        /// Performs an Http PUT call to the specified base address and endpoint with an expected result of updating the resource
        /// </summary>
        /// <typeparam name="TResponse">Expected response type to return</typeparam>
        /// <typeparam name="TRequest">Request Body</typeparam>
        /// <param name="baseAddress">Base address to api service being called</param>
        /// <param name="uri">Endpoint</param>
        /// <param name="request">Request object</param>
        /// <param name="headers">Headers to add to request</param>
        /// <param name="cacheCheck">Optional - function to check cache</param>
        /// <param name="setCache">Optional - function to add object to cache</param>
        /// <returns>TResponse - null if not found</returns>
        /// <exception cref="HttpRequestException">Thrown if non 200 or 404 status code returned</exception>
        public async Task<TResponse> Put<TRequest, TResponse>(string baseAddress, string uri, TRequest request,
                                                               IDictionary<string, string> headers,
                                                               Func<string, Task<TResponse>> cacheCheck = null,
                                                               Func<TResponse, string, Task> setCache = null) where TResponse : class
        {
            var cache = await _httpCachingService.CheckCache(baseAddress, uri, headers, cacheCheck);

            if(cache != null)
                return cache;

            var response =
                await _httpService.CallHttpMethod<TResponse, TRequest>(baseAddress, uri, request, headers, HttpMethods.PUT);

            await _httpCachingService.AddToCache(response, baseAddress, uri, headers, setCache);

            return response;
        }

        /// <summary>
        /// Performs an Http DELETE call to the specified base address and endpoint with a result of removing a resource
        /// </summary>
        /// <param name="baseAddress">Base address to api service being called</param>
        /// <param name="uri">Endpoint</param>
        /// <param name="headers">Headers to add to request</param>
        /// <param name="voidCache">Optional function to void the item from cache</param>
        /// <returns>TResponse - null if not found</returns>
        /// <exception cref="HttpRequestException">Thrown if non 200 or 404 status code returned</exception>
        public async Task Delete(string baseAddress, string uri,
                                                    IDictionary<string, string> headers,
                                                    Func<string, Task> voidCache = null)
        {
            await _httpService.CallHttpMethod<object, object>(baseAddress, uri, null, headers, HttpMethods.DELETE);

            await _httpCachingService.VoidCache(baseAddress, uri, voidCache);
        }

    }
}