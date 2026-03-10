using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace proj1;

public class http1
{
    private readonly ILogger<http1> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public http1(ILogger<http1> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    [Function("http1")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var url = Environment.GetEnvironmentVariable("URL");
        if (string.IsNullOrEmpty(url))
        {
            return new BadRequestObjectResult("The 'URL' environment variable is not set.");
        }

        var payload = new { test = "testvalue" };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("POST to {Url} returned {StatusCode}", url, response.StatusCode);

        return new OkObjectResult(responseBody);
    }
}
