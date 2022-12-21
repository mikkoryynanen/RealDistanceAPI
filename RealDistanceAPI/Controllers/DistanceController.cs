using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    public async Task<ActionResult<DistanceResponse>> Get([FromBody] DistanceRequest distanceRequest)
    {
        var URL = $"{Environment.GetEnvironmentVariable("DISTANCE_MATRIX_API_BASE_URL")}/{Environment.GetEnvironmentVariable("DISTANCE_MATRIX_API_DATA_TYPE")}?origins={distanceRequest.Origin}&destinations={distanceRequest.Destination}&mode={distanceRequest.Mode}&departure_time={DateTimeOffset.Now.ToUnixTimeSeconds()}&key={Environment.GetEnvironmentVariable("DISTANCE_MATRIX_API_KEY")}";
        Console.WriteLine(URL);
        var message = new HttpRequestMessage(
            HttpMethod.Get,
            URL)
        {
            Headers =
            {
                { HeaderNames.Accept, "application/json" }
            }
        };
        var httpClient = _httpClient.CreateClient();
        var response = await httpClient.SendAsync(message);
        var responseString = await response.Content.ReadAsStringAsync();
        var distanceResponse = JsonConvert.DeserializeObject<Response>(responseString);

        if (distanceResponse != null && distanceResponse.Status == "OK")
        {
            var distance = distanceResponse.Rows[0].Elements[0].Distance;
            var duration = distanceResponse.Rows[0].Elements[0].Duration;
            var elementStatus = distanceResponse.Rows[0].Elements[0].Status;
            if (elementStatus == "ZERO_RESULTS")
            {
                return NotFound($"distance elements status is: {elementStatus}");
            }
            else
            {
                // TODO Cache results in database
                // We can look in database if there is precomputed value
                // So there is no need to do the request to dsitance matrix api
                return Ok(new DistanceResponse
                {
                    Distance = (distance.Value / 1000).ToString(), // NOTE distance is in meters
                    Unit = "km",
                    Time = duration.Value.ToString(),
                    Mode = distanceRequest.Mode
                });
            }

        }
        else
        {
            return NotFound(distanceResponse.Status);
        }
    }
}

