using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cnf.Finance.Web.Services
{
    public interface IApiConnector
    {
        /// <summary>
        /// Invoke API with HTTP Get Method
        /// </summary>
        /// <typeparam name="TReturn">the strong type which will be returned from API</typeparam>
        /// <param name="route">API route which doesn't contains the Base URL</param>
        /// <param name="queryString">query parameters joined with amp(&) but NOT leading with a question mark(?)</param>
        /// <returns>return a strong typed object</returns>
        Task<TReturn> HttpGetAsync<TReturn>(string route, string queryString = "");

        /// <summary>
        /// Invoke API with HTTP Delete Method
        /// </summary>
        /// <typeparam name="TReturn">the strong type which will be returned from API</typeparam>
        /// <param name="route">API route which doesn't contains the Base URL</param>
        /// <param name="queryString">query parameters joined with amp(&) but NOT leading with a question mark(?)</param>
        /// <returns>return a strong typed object</returns>
        Task<TReturn> HttpDeleteAsync<TReturn>(string route, string queryString = "");

        /// <summary>
        /// Invoke API with HTTP Post Method
        /// </summary>
        /// <typeparam name="T">the strong type which is about to be sent to server</typeparam>
        /// <typeparam name="TReturn">the strong type which will be returned from API</typeparam>
        /// <param name="route">API route which doesn't contains the Base URL</param>
        /// <param name="data">an object of T that will be sent to server</param>
        /// <returns>return a strong typed object</returns>
        Task<TReturn> HttpPostAsync<T, TReturn>(string route, T data = default(T));

        /// <summary>
        /// Invoke API with HTTP Put method
        /// </summary>
        /// <typeparam name="T">the strong type which is about to be sent to server</typeparam>
        /// <typeparam name="TReturn">the strong type which will be returned from API</typeparam>
        /// <param name="route">API route which doesn't contains the Base URL</param>
        /// <param name="data">an object of T that will be sent to server</param>
        /// <returns>return a strong typed object</returns>
        Task<TReturn> HttpPutAsync<T, TReturn>(string route, T data = default(T));
    }
}
