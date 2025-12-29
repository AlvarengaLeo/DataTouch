using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DataTouch.Web.Services;

/// <summary>
/// Service for IP-based geolocation with privacy-first approach.
/// Uses free IP-API.com service with caching to minimize API calls.
/// </summary>
public class GeoLocationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GeoLocationService> _logger;
    
    // Cache TTL for geo lookups (24 hours - IPs don't change location often)
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);
    
    // Known location coordinates for demo/fallback
    private static readonly Dictionary<string, (double Lat, double Lng)> KnownLocations = new()
    {
        // El Salvador
        { "san salvador_sv", (13.6929, -89.2182) },
        { "santa ana_sv", (13.9942, -89.5597) },
        { "san miguel_sv", (13.4833, -88.1833) },
        { "la libertad_sv", (13.4883, -89.3225) },
        { "soyapango_sv", (13.7167, -89.1500) },
        
        // United States
        { "miami_us", (25.7617, -80.1918) },
        { "boston_us", (42.3601, -71.0589) },
        { "new york_us", (40.7128, -74.0060) },
        { "los angeles_us", (34.0522, -118.2437) },
        { "houston_us", (29.7604, -95.3698) },
        
        // Mexico
        { "ciudad de méxico_mx", (19.4326, -99.1332) },
        { "cdmx_mx", (19.4326, -99.1332) },
        { "mexico city_mx", (19.4326, -99.1332) },
        { "guadalajara_mx", (20.6597, -103.3496) },
        { "monterrey_mx", (25.6866, -100.3161) },
        
        // Central America
        { "guatemala city_gt", (14.6349, -90.5069) },
        { "tegucigalpa_hn", (14.0723, -87.1921) },
        { "san josé_cr", (9.9281, -84.0907) },
        { "panama city_pa", (8.9824, -79.5199) },
        { "managua_ni", (12.1150, -86.2362) },
        
        // South America
        { "bogotá_co", (4.7110, -74.0721) },
        { "lima_pe", (-12.0464, -77.0428) },
        { "buenos aires_ar", (-34.6037, -58.3816) },
        { "são paulo_br", (-23.5505, -46.6333) },
        
        // Europe
        { "madrid_es", (40.4168, -3.7038) },
        { "barcelona_es", (41.3851, 2.1734) },
        { "london_gb", (51.5074, -0.1278) },
        { "paris_fr", (48.8566, 2.3522) }
    };

    public GeoLocationService(
        IHttpClientFactory httpClientFactory, 
        IMemoryCache cache,
        ILogger<GeoLocationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Hash an IP address for privacy-compliant storage
    /// </summary>
    public static string HashIpAddress(string ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress)) return string.Empty;
        
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(ipAddress + "datatouch_salt_2024");
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash)[..16]; // First 16 chars for shorter storage
    }

    /// <summary>
    /// Get geolocation data for an IP address
    /// </summary>
    public async Task<GeoLocationResult> GetLocationAsync(string? ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1" || ipAddress == "127.0.0.1")
        {
            // Local development - return default demo location
            return new GeoLocationResult
            {
                Success = true,
                City = "San Salvador",
                Region = "San Salvador",
                CountryCode = "SV",
                Country = "El Salvador",
                Latitude = 13.6929,
                Longitude = -89.2182,
                Source = "default"
            };
        }

        var cacheKey = $"geo_{HashIpAddress(ipAddress)}";
        
        if (_cache.TryGetValue(cacheKey, out GeoLocationResult? cached) && cached != null)
        {
            cached.Source = "cache";
            return cached;
        }

        try
        {
            var result = await FetchGeoLocationAsync(ipAddress);
            
            if (result.Success)
            {
                _cache.Set(cacheKey, result, CacheDuration);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get geolocation for IP. Continuing without geo data.");
            return new GeoLocationResult { Success = false };
        }
    }

    private async Task<GeoLocationResult> FetchGeoLocationAsync(string ipAddress)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            
            // Using free ip-api.com (limited to 45 requests/minute for non-commercial)
            // For production, consider ipinfo.io, maxmind, or similar
            var response = await client.GetStringAsync($"http://ip-api.com/json/{ipAddress}?fields=status,message,country,countryCode,region,regionName,city,lat,lon");
            
            var json = JsonDocument.Parse(response);
            var root = json.RootElement;
            
            if (root.GetProperty("status").GetString() == "success")
            {
                return new GeoLocationResult
                {
                    Success = true,
                    City = root.TryGetProperty("city", out var city) ? city.GetString() : null,
                    Region = root.TryGetProperty("regionName", out var region) ? region.GetString() : null,
                    CountryCode = root.TryGetProperty("countryCode", out var cc) ? cc.GetString() : null,
                    Country = root.TryGetProperty("country", out var country) ? country.GetString() : null,
                    Latitude = root.TryGetProperty("lat", out var lat) ? lat.GetDouble() : null,
                    Longitude = root.TryGetProperty("lon", out var lon) ? lon.GetDouble() : null,
                    Source = "ip"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "IP geolocation API call failed");
        }

        return new GeoLocationResult { Success = false };
    }

    /// <summary>
    /// Get coordinates for a known city/country combination
    /// </summary>
    public (double? Lat, double? Lng) GetCoordinatesForLocation(string? city, string? countryCode)
    {
        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(countryCode))
            return (null, null);

        var key = $"{city.ToLowerInvariant()}_{countryCode.ToLowerInvariant()}";
        
        if (KnownLocations.TryGetValue(key, out var coords))
        {
            return (coords.Lat, coords.Lng);
        }

        // Try partial match
        var partialKey = KnownLocations.Keys.FirstOrDefault(k => k.StartsWith(city.ToLowerInvariant().Split(',')[0]));
        if (partialKey != null)
        {
            return (KnownLocations[partialKey].Lat, KnownLocations[partialKey].Lng);
        }

        return (null, null);
    }

    /// <summary>
    /// Extract device type from User-Agent string
    /// </summary>
    public static string GetDeviceType(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return "unknown";

        var ua = userAgent.ToLowerInvariant();
        
        if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone"))
            return "mobile";
        
        if (ua.Contains("tablet") || ua.Contains("ipad"))
            return "tablet";
        
        return "desktop";
    }

    /// <summary>
    /// Extract referrer domain only (privacy-safe)
    /// </summary>
    public static string? GetReferrerDomain(string? referrer)
    {
        if (string.IsNullOrEmpty(referrer))
            return null;

        try
        {
            var uri = new Uri(referrer);
            return uri.Host;
        }
        catch
        {
            return null;
        }
    }
}

public class GeoLocationResult
{
    public bool Success { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? CountryCode { get; set; }
    public string? Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Source { get; set; }
}
