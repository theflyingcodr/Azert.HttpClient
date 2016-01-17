using System;
using System.Threading.Tasks;

namespace Azert.HttpClient.Services.Interfaces
{
    public interface IHttpCachingService
    {
        /// <summary>
        /// Checks the cache, if found returns the object located by the cacheCheck function.
        /// If no cached item returns null.
        /// 
        /// This is optional, if no cacheCheck func is defined nothing is checked
        /// </summary>
        /// <typeparam name="TRequest">Request object</typeparam>
        /// <typeparam name="TResponse">Response object</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="baseUri">Base uri to endpoint - part of cache key</param>
        /// <param name="uri">Uri for endpoint - part of cache key</param>
        /// <param name="cacheCheck">Optional - Func to locate cached object</param>
        /// <returns>TResponse, null if not found</returns>
        Task<TResponse> CheckCache<TRequest, TResponse>(TRequest request, string baseUri, string uri,
                                                                              Func<string, Task<TResponse>> cacheCheck = null)
            where TResponse : class;

        /// <summary>
        /// Adds an item to the cache based on the setCache Func
        /// 
        /// This is optional, if no setCache func is defined nothing is added
        /// </summary>
        /// <typeparam name="TRequest">Request object</typeparam>
        /// <typeparam name="TResponse">Response object</typeparam>
        /// <param name="response">Response object to cache</param>
        /// <param name="baseUri">Base uri to endpoint - part of cache key</param>
        /// <param name="uri">Uri for endpoint - part of cache key</param>
        /// <param name="setCache">Optional - Func to add cached object</param>
        Task AddToCache<TResponse, TRequest>(TResponse response, TRequest request, string baseUri, string uri,
                                                                   Func<TResponse, string, Task> setCache = null);

        /// <summary>
        /// Voids an item stored in cache
        /// 
        /// This is optional, if no voidCache func is defined nothing is voided
        /// </summary>
        /// <param name="baseUri">Base uri to endpoint - part of cache key</param>
        /// <param name="uri">Uri for endpoint - part of cache key</param>
        /// <param name="voidCache">Optional - Func to void cached object</param>
        Task VoidCache(string baseUri, string uri,
                                             Func<string, Task> voidCache = null);

        /// <summary>
        /// Creates a cache key based on the composite key
        /// </summary>
        /// <typeparam name="TRequest">Request</typeparam>
        /// <param name="baseUri">Base uri to endpoint - part of cache key</param>
        /// <param name="uri">Uri for endpoint - part of cache key</param>
        /// <param name="request">Request sent, this is serialised to JSON</param>
        /// <returns>Composite cache key</returns>
        string CreateCacheKey<TRequest>(string baseUri, string uri, TRequest request);
    }
}