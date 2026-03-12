using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System;
using System.Linq;
using WebAPIBackend.Services;

namespace WebAPIBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeolocationController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string AFGHANISTAN_COUNTRY_CODE = "AF";

        public GeolocationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Check if the request is coming from Afghanistan
        /// Uses IP whitelist first, then falls back to external API
        /// </summary>
        [HttpGet("check-afghanistan")]
        public async Task<IActionResult> CheckAfghanistanAccess()
        {
            try
            {
                var clientIp = GetClientIpAddress();
                
                // First check IP whitelist (fast and reliable)
                var isAfghanistanIp = AfghanistanIpWhitelist.IsAfghanistanIp(clientIp);
                
                if (isAfghanistanIp)
                {
                    return Ok(new
                    {
                        success = true,
                        isAllowed = true,
                        clientIp = clientIp,
                        method = "whitelist",
                        message = "Access allowed from Afghanistan (whitelist)"
                    });
                }

                // If not in whitelist, try external API
                var isAfghanistanApi = await IsIpFromAfghanistanViaApi(clientIp);

                return Ok(new
                {
                    success = true,
                    isAllowed = isAfghanistanApi,
                    clientIp = clientIp,
                    method = "api",
                    message = isAfghanistanApi ? "Access allowed from Afghanistan (API)" : "Access denied: Not from Afghanistan"
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Geolocation] Error: {ex.Message}");
                return Ok(new
                {
                    success = false,
                    isAllowed = false,
                    message = "Unable to verify location. Access denied.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get geolocation information for the current IP
        /// </summary>
        [HttpGet("info")]
        public async Task<IActionResult> GetGeolocationInfo()
        {
            try
            {
                var clientIp = GetClientIpAddress();
                var geoInfo = await GetGeolocationInfo(clientIp);

                return Ok(new
                {
                    success = true,
                    data = geoInfo
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Geolocation] Error: {ex.Message}");
                return Ok(new
                {
                    success = false,
                    message = "Unable to fetch geolocation information",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Check if IP is from Afghanistan via external API
        /// </summary>
        private async Task<bool> IsIpFromAfghanistanViaApi(string ipAddress)
        {
            try
            {
                var geoInfo = await GetGeolocationInfo(ipAddress);
                
                if (geoInfo.TryGetProperty("country_code", out var countryCodeElement))
                {
                    var countryCode = countryCodeElement.GetString()?.ToUpper();
                    var isAfghanistan = countryCode == AFGHANISTAN_COUNTRY_CODE;
                    System.Diagnostics.Debug.WriteLine($"[Geolocation] IP: {ipAddress}, Country: {countryCode}, Allowed: {isAfghanistan}");
                    return isAfghanistan;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Geolocation] Error checking IP via API: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get geolocation information from IP
        /// </summary>
        private async Task<JsonElement> GetGeolocationInfo(string ipAddress)
        {
            // Skip localhost and private IPs
            if (ipAddress == "127.0.0.1" || ipAddress == "::1" || ipAddress.StartsWith("192.168") || ipAddress.StartsWith("10."))
            {
                System.Diagnostics.Debug.WriteLine($"[Geolocation] Local IP detected: {ipAddress}, allowing access");
                var json = JsonSerializer.Serialize(new { country_code = AFGHANISTAN_COUNTRY_CODE, country_name = "Afghanistan (Local)" });
                return JsonSerializer.Deserialize<JsonElement>(json);
            }

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            
            try
            {
                var response = await client.GetAsync($"https://ipapi.co/{ipAddress}/json/");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<JsonElement>(content);
                }

                throw new Exception($"API returned status: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Geolocation] HTTP Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get client IP address from request
        /// </summary>
        private string GetClientIpAddress()
        {
            // Check for IP behind proxy
            if (!string.IsNullOrEmpty(Request.Headers["X-Forwarded-For"]))
            {
                var ips = Request.Headers["X-Forwarded-For"].ToString().Split(',');
                return ips[0].Trim();
            }

            // Check for IP behind load balancer
            if (!string.IsNullOrEmpty(Request.Headers["X-Real-IP"]))
            {
                return Request.Headers["X-Real-IP"].ToString();
            }

            // Check for CloudFlare
            if (!string.IsNullOrEmpty(Request.Headers["CF-Connecting-IP"]))
            {
                return Request.Headers["CF-Connecting-IP"].ToString();
            }

            // Get direct connection IP
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            return remoteIp;
        }
    }
}
