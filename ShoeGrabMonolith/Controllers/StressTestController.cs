using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ShoeGrabMonolith.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StressTestController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public StressTestController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("external-data")]
        public async Task<IActionResult> GetExternalData()
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await _httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/posts/1");
                stopwatch.Stop();
                return Ok(new { Response = response, TimeTakenMs = stopwatch.ElapsedMilliseconds });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { Message = "Failed to fetch data", Error = ex.Message });
            }
        }

        [HttpPost("send-data")]
        public async Task<IActionResult> PostExternalData([FromBody] object payload)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://jsonplaceholder.typicode.com/posts", payload);
                stopwatch.Stop();
                return Ok(new { Status = response.StatusCode, TimeTakenMs = stopwatch.ElapsedMilliseconds });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { Message = "Failed to send data", Error = ex.Message });
            }
        }

        [HttpGet("simulate-timeout")]
        public async Task<IActionResult> SimulateTimeout()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            try
            {
                // Intentional delay to simulate a slow API response
                var response = await _httpClient.GetStringAsync("https://httpbin.org/delay/5", cts.Token);
                return Ok(new { Message = "Success", Response = response });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(504, new { Message = "Request timed out" });
            }
        }

        [HttpGet("cpu-intensive")]
        public IActionResult SimulateCpuIntensiveTask()
        {
            var stopwatch = Stopwatch.StartNew();
            var sum = 0L;

            // Stressing the CPU with intensive calculations
            for (long i = 0; i < 5_000_000_000L; i++)
            {
                sum += i;
            }

            stopwatch.Stop();
            return Ok(new { Message = "CPU stress test completed", TimeTakenMs = stopwatch.ElapsedMilliseconds, Result = sum });
        }
    }
}