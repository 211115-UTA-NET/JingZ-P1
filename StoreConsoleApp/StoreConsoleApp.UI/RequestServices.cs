using StoreConsoleApp.UI.Exceptions;
using System.Net.Mime;
using System.Text;

namespace StoreConsoleApp.UI
{
    public class RequestServices : IRequestServices
    {
        private readonly HttpClient _httpClient = new();
        public RequestServices(Uri service)
        {
            _httpClient.BaseAddress = service;
        }
        /// <summary>
        /// Get response message from a GET method
        /// </summary>
        /// <param name="requestUri">request URI</param>
        /// <returns>the response from the server</returns>
        /// <exception cref="UnexpectedServerBehaviorException"></exception>
        public async Task<HttpResponseMessage> GetResponseForGETAsync(string requestUri)
        {
            HttpRequestMessage request = new(HttpMethod.Get, requestUri);
            // expect application/json reply. ("content negotiation" in http/rest)
            request.Headers.Accept.Add(new(MediaTypeNames.Application.Json));
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                throw new UnexpectedServerBehaviorException("Network Error", ex);
            }
            response.EnsureSuccessStatusCode();
            // if response is not json format
            if (response.Content.Headers.ContentType?.MediaType != MediaTypeNames.Application.Json)
            {
                throw new UnexpectedServerBehaviorException();
            }
            return response;
        }

        /// <summary>
        /// Get the server respond for sending a POST request
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>the response from the server</returns>
        /// <exception cref="UnexpectedServerBehaviorException"></exception>
        public async Task<HttpResponseMessage> GetResponseForPOSTAsync(string jsonContent, string requestUri)
        {
            HttpRequestMessage request = new(HttpMethod.Post, requestUri);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                throw new UnexpectedServerBehaviorException("Network Error", ex);
            }
            return response;
        }
    }
}
