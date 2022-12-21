using System;
using System.Net.Http;
using ApiUsers.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using RealDistanceAPI.Models;

namespace RealDistanceAPI.Middlewares
{
	public class ApiKeyMiddleware
	{
        private readonly RequestDelegate _next;

        const string API_KEY = "X-API-KEY";

        public ApiKeyMiddleware(RequestDelegate next)
		{
			_next = next;
		}

        public async Task InvokeAsync(HttpContext context, IHttpClientFactory httpClientFactory)
		{
            if (!context.Request.Headers.TryGetValue(API_KEY, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api key is missing");
                return;
            }

            if (!context.Request.Query.TryGetValue("userId", out var extractedUserId))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("userId is missing");
                return;
            }

            // We check if the api key exists in the database
            var URL = $"{Environment.GetEnvironmentVariable("API_USERS_BASE_URL")}/user?userId={extractedUserId}";
            var message = new HttpRequestMessage(
                HttpMethod.Get,
                URL)
            {
                Headers =
            {
                    { HeaderNames.Accept, "application/json" }
            }
            };
            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(message);
            var responseString = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("User does not have API key");
                return;
            }

            await _next(context);
        }
	}
}

 