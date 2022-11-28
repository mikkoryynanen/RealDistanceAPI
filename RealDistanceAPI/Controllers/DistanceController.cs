using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace RealDistanceAPI.Controllers;

[ApiController]
[Route("Distance")]
public class DistanceController : ControllerBase
{
    private readonly IHttpClientFactory _httpClient;

    public DistanceController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromBody] DistanceRequest distanceRequest)
    {
        const string API_KEY = "AIzaSyByRiPTvwk1pwnXA1DTiJ0-e9mwumxnAuw";
        var message = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={distanceRequest.Origin}&destinations={distanceRequest.Destination}&key={API_KEY}")
        {
            Headers =
            {
                { HeaderNames.Accept, "application/json" }
            }
        };
        var httpClient = _httpClient.CreateClient();
        var response = await httpClient.SendAsync(message);

        var value = response.Content.ReadFromJsonAsync<Root>().Result.rows[0].elements[0].distance.text;
        return Ok(new DistanceResponse
        {
            Distance = value,
            Unit = "km",
            Time = "99h 59min 59s"
        });
    }
}

