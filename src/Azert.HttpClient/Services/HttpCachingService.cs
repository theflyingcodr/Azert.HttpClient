using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azert.HttpClient.Exceptions;
using Azert.HttpClient.Services.Interfaces;

namespace Azert.HttpClient.Services
{
    public class HttpCachingService : IHttpCachingService
    {

        /// <summary>
        /// Checks the cache, if found returns the object located by the cacheCheck function.
        /// If no cached item returns null.
        /// </summary>
        /// <typeparam name="TResponse">Response object</typeparam>
        /// <param name="baseAddress">Base address uri</param>
        /// <param name="uri">Uri for endpoint - part of cache key</param>
        /// <param name="headers">Optional headers - when performing a PUT or POST an identifier should be supplied in the header</param>
        /// <param name="cacheCheck">Optional - Func to locate cached object</param>
        /// <returns>TResponse, null if not found</returns>
        public async Task<TResponse> CheckCache<TResponse>(
            string baseAddress, string uri, IDictionary<string, string> headers = null, Func<string, Task<TResponse>> cacheCheck = null) where TResponse : class
        {
            if(cacheCheck == null)
                return null;

            // headers will be provided in POST and PUT requests
            var identifier = string.Empty;

            identifier = CheckHeaderForIdentifier(headers, identifier);

            var cached = await cacheCheck.Invoke(CreateCacheKey(baseAddress, uri, identifier));

            return cached;
        }

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
        public async Task AddToCache<TResponse>(TResponse response, string baseUri, string uri, IDictionary<string, string> headers = null,
                                                    Func<TResponse, string, Task> setCache = null)
        {
            if(setCache == null)
                return;

            // headers will be provided in POST and PUT requests
            var identifier = string.Empty;

            identifier = CheckHeaderForIdentifier(headers, identifier);

            await setCache(response, CreateCacheKey(baseUri, uri, identifier));
        }

        /// <summary>
        /// Checks header for x-resource-identifier header expected when caching PUT or POST requests
        /// </summary>
        /// <param name="headers">Dicitonary of headers</param>
        /// <param name="identifier">Header value</param>
        /// <returns>Value of header if found</returns>
        /// <exception cref="MissingHeaderException">Thrown if header missing or empty</exception>
        public string CheckHeaderForIdentifier(IDictionary<string, string> headers, string identifier)
        {
            if(headers == null)
                return identifier;

            var dictionary = headers.TryGetValue("x-resource-identifier", out identifier);

            if(!dictionary)
                throw new MissingHeaderException(
                    "In order to cache a PUT or POST, a unique identifier header 'x-resource-identifier' must be provided");

            if(string.IsNullOrEmpty(identifier))
                throw new MissingHeaderException(
                    "The header 'x-resource-identifier' used when caching PUT or POST request was provided but the value was null, " +
                    "this needs to be populated for caching to work effectively.");

            return identifier;
        }


        /// <summary>
        /// Voids an item stored in cache
        /// 
        /// This is optional, if no voidCache func is defined nothing is voided
        /// </summary>
        /// <param name="baseUri">Base uri to endpoint - part of cache key</param>
        /// <param name="uri">Uri for endpoint - part of cache key</param>
        /// <param name="voidCache">Optional - Func to void cached object</param>
        public async Task VoidCache(string baseUri, string uri,
                                                 Func<string, Task> voidCache = null)
        {
            if(voidCache != null)
                await voidCache(CreateCacheKey(baseUri, uri, null));
        }

        /// <summary>
        /// Creates a cache key based on the composite key
        /// </summary>
        /// <param name="baseUri">Base uri to endpoint - part of cache key</param>
        /// <param name="uri">Uri for endpoint - part of cache key</param>
        /// <param name="identifier">Unique identifier to use</param>
        /// <returns>Composite cache key</returns>
        public string CreateCacheKey(string baseUri, string uri, string identifier = "")
        {
            var cacheKey = $"{identifier}{baseUri}{uri}";

            return cacheKey;
        }
    }
}