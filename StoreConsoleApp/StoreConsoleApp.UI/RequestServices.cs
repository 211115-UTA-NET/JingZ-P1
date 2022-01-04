using StoreConsoleApp.UI.Exceptions;
using System.Net.Mime;

namespace StoreConsoleApp.UI
{
    public class RequestServices : IRequestServices
    {
        private readonly HttpClient _httpClient = new();
        Uri server = new("https://localhost:7282");
        public RequestServices()
        {
            _httpClient.BaseAddress = server;
        }
        /// <summary>
        /// Get response message from a GET method
        /// </summary>
        /// <param name="requestUri">request URI</param>
        /// <returns>the response from the server</returns>
        /// <exception cref="UnexpectedServerBehaviorException"></exception>
        public async Task<HttpResponseMessage> GetResponseMessageAsync(string requestUri)
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
    }
}
