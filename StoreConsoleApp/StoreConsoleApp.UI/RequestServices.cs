﻿using StoreConsoleApp.UI.Dtos;
using StoreConsoleApp.UI.Exceptions;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

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
        public async Task<HttpResponseMessage> GetResponseAsync(string requestUri)
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
        /// Get the server respond for sending a POST request of adding a new customer.
        /// </summary>
        /// <param name="customerName">contains customer first & last name</param>
        /// <returns>the response from the server</returns>
        public async Task<HttpResponseMessage> GetResponseForAddCustomerAsync(CustomerName customerName)
        {
            HttpRequestMessage request = new(HttpMethod.Post, "/api/customer");
            request.Content = new StringContent(JsonSerializer.Serialize(customerName), Encoding.UTF8, MediaTypeNames.Application.Json);
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

        /// <summary>
        /// Get the server respond for sending a POST request of placing order
        /// </summary>
        /// <param name="orders"></param>
        /// <returns>the response from the server</returns>
        /// <exception cref="UnexpectedServerBehaviorException"></exception>
        public async Task<HttpResponseMessage> GetResponseForPlaceOrderAsync(List<Order> orders)
        {
            HttpRequestMessage request = new(HttpMethod.Post, "/api/order");
            OrderList list = new();
            list.orderlist = orders;
            var json = JsonSerializer.Serialize(list);
            request.Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
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