using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azert.HttpClient.Exceptions;

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
        /// <typeparam name="TResponse">Response object</typeparam>
        /// <param name="baseAddress">Base address uri</param>
        /// <param name="uri">Uri for endpoint - part of cache key</param>
        /// <param name="headers">Optional headers - when performing a PUT or POST an identifier should be supplied in the header</param>
        /// <param name="cacheCheck">Optional - Func to locate cached object</param>
        /// <returns>TResponse, null if not found</returns>
        Task<TResponse> CheckCache<TResponse>(
            string baseAddress, string uri, IDictionary<string, string> headers = null, Func<string, Task<TResponse>> cacheCheck = null) where TResponse : class;

        /// <summary>
        /// Adds an item to the cache based on the setCache Func
        /// 
        /// This is optional, if no setCache func is defined nothing is added
        /// </summary>
        /// <typeparam name="TResponse">Response to cachType of object to cache</typeparam>
        /// <param name="response">Response object to cache</param>
        /// <param name="baseUri">Base uri to endpoint - part of cache key</param>
        /// <param name="uri">Uri for endpoint - part of cache key</param>
        /// <param name="headers">Optional headers - when performing a PUT or POST an identifier should be supplied in the header</param>
        /// <param name="setCache">Optional - Func to add cached object</param>
        Task AddToCache<TResponse>(TResponse response, string baseUri, string uri, IDictionary<string, string> headers = null,
                                                         Func<TResponse, string, Task> setCache = null);

        /// <summary>
        /// Checks header for x-resource-identifier header expected when caching PUT or POST requests
        /// </summary>
        /// <param name="headers">Dicitonary of headers</param>
        /// <param name="identifier">Header value</param>
        /// <returns>Value of header if found</returns>
        /// <exception cref="MissingHeaderException">Thrown if header missing or empty</exception>
        string CheckHeaderForIdentifier(IDictionary<string, string> headers, string identifier);

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
        /// <param name="baseUri">Base uri to endpoint - part of cache key</param>
        /// <param name="uri">Uri for endpoint - part of cache key</param>
        /// <param name="identifier">Unique identifier to use</param>
        /// <returns>Composite cache key</returns>
        string CreateCacheKey(string baseUri, string uri, string identifier = "");
    }
}