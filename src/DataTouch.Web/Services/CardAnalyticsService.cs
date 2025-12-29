using DataTouch.Domain.Entities;
using DataTouch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DataTouch.Web.Services;

/// <summary>
/// Servicio para registrar y consultar analíticas de tarjetas digitales.
/// Registra eventos como escaneos de QR, clics en enlaces, envío de formularios, etc.
/// </summary>
public class CardAnalyticsService
{
    private readonly DataTouchDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CardAnalyticsService(DataTouchDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Registra un evento de visualización de página
    /// </summary>
    public async Task TrackPageViewAsync(Guid cardId)
    {
        await TrackEventAsync(cardId, "page_view");
    }

    /// <summary>
    /// Registra un escaneo de código QR
    /// </summary>
    public async Task TrackQrScanAsync(Guid cardId)
    {
        await TrackEventAsync(cardId, "qr_scan");
    }

    /// <summary>
    /// Registra un clic en un enlace o red social
    /// </summary>
    public async Task TrackLinkClickAsync(Guid cardId, string linkType, string? url = null)
    {
        var metadata = new { link_type = linkType, url = url };
        await TrackEventAsync(cardId, "link_click", System.Text.Json.JsonSerializer.Serialize(metadata));
    }

    /// <summary>
    /// Registra un clic en un botón CTA (WhatsApp, Llamar, Email)
    /// </summary>
    public async Task TrackCtaClickAsync(Guid cardId, string buttonType)
    {
        var metadata = new { button = buttonType };
        await TrackEventAsync(cardId, "cta_click", System.Text.Json.JsonSerializer.Serialize(metadata));
    }

    /// <summary>
    /// Registra cuando el visitante guarda el contacto
    /// </summary>
    public async Task TrackContactSaveAsync(Guid cardId)
    {
        await TrackEventAsync(cardId, "contact_save");
    }

    /// <summary>
    /// Registra el envío de un formulario de contacto
    /// </summary>
    public async Task TrackFormSubmitAsync(Guid cardId, Guid? leadId = null)
    {
        var metadata = leadId.HasValue ? new { lead_id = leadId.Value.ToString() } : null;
        await TrackEventAsync(cardId, "form_submit", metadata != null ? System.Text.Json.JsonSerializer.Serialize(metadata) : null);
    }

    /// <summary>
    /// Registra cuando la tarjeta es compartida
    /// </summary>
    public async Task TrackShareAsync(Guid cardId, string? method = null)
    {
        var metadata = !string.IsNullOrEmpty(method) ? new { method = method } : null;
        await TrackEventAsync(cardId, "share", metadata != null ? System.Text.Json.JsonSerializer.Serialize(metadata) : null);
    }

    /// <summary>
    /// Método genérico para registrar cualquier evento
    /// </summary>
    private async Task TrackEventAsync(Guid cardId, string eventType, string? metadataJson = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        var analytics = new CardAnalytics
        {
            Id = Guid.NewGuid(),
            CardId = cardId,
            EventType = eventType,
            Timestamp = DateTime.UtcNow,
            UserAgent = httpContext?.Request.Headers.UserAgent.ToString(),
            IpAddress = GetClientIpAddress(httpContext),
            Referrer = httpContext?.Request.Headers.Referer.ToString(),
            DeviceType = DetectDeviceType(httpContext?.Request.Headers.UserAgent.ToString()),
            MetadataJson = metadataJson
        };

        _context.CardAnalytics.Add(analytics);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene estadísticas de una tarjeta
    /// </summary>
    public async Task<CardStats> GetCardStatsAsync(Guid cardId, DateTime? from = null, DateTime? to = null)
    {
        var query = _context.CardAnalytics
            .Where(a => a.CardId == cardId);

        if (from.HasValue)
            query = query.Where(a => a.Timestamp >= from.Value);
        
        if (to.HasValue)
            query = query.Where(a => a.Timestamp <= to.Value);

        var events = await query.ToListAsync();

        return new CardStats
        {
            TotalViews = events.Count(e => e.EventType == "page_view"),
            QrScans = events.Count(e => e.EventType == "qr_scan"),
            LinkClicks = events.Count(e => e.EventType == "link_click"),
            CtaClicks = events.Count(e => e.EventType == "cta_click"),
            ContactSaves = events.Count(e => e.EventType == "contact_save"),
            FormSubmits = events.Count(e => e.EventType == "form_submit"),
            Shares = events.Count(e => e.EventType == "share"),
            UniqueVisitors = events.Select(e => e.IpAddress).Where(ip => !string.IsNullOrEmpty(ip)).Distinct().Count(),
            DeviceBreakdown = events
                .Where(e => !string.IsNullOrEmpty(e.DeviceType))
                .GroupBy(e => e.DeviceType!)
                .ToDictionary(g => g.Key, g => g.Count()),
            DailyViews = events
                .Where(e => e.EventType == "page_view")
                .GroupBy(e => e.Timestamp.Date)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    /// <summary>
    /// Obtiene los eventos recientes de una tarjeta
    /// </summary>
    public async Task<List<CardAnalytics>> GetRecentEventsAsync(Guid cardId, int count = 50)
    {
        return await _context.CardAnalytics
            .Where(a => a.CardId == cardId)
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    private string? GetClientIpAddress(HttpContext? context)
    {
        if (context == null) return null;

        // Check for forwarded IP (behind proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',').First().Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString();
    }

    private string DetectDeviceType(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return "unknown";

        userAgent = userAgent.ToLower();

        if (userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone"))
            return "mobile";
        
        if (userAgent.Contains("tablet") || userAgent.Contains("ipad"))
            return "tablet";

        return "desktop";
    }
}

/// <summary>
/// Estadísticas agregadas de una tarjeta
/// </summary>
public class CardStats
{
    public int TotalViews { get; set; }
    public int QrScans { get; set; }
    public int LinkClicks { get; set; }
    public int CtaClicks { get; set; }
    public int ContactSaves { get; set; }
    public int FormSubmits { get; set; }
    public int Shares { get; set; }
    public int UniqueVisitors { get; set; }
    public Dictionary<string, int> DeviceBreakdown { get; set; } = new();
    public Dictionary<DateTime, int> DailyViews { get; set; } = new();
}
