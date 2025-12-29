namespace DataTouch.Domain.Entities;

/// <summary>
/// Eventos de interacción para analíticas de tarjetas digitales.
/// Registra escaneos de QR, clics en enlaces, envío de formularios, etc.
/// This is the Single Source of Truth for all engagement events.
/// </summary>
public class CardAnalytics
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    
    /// <summary>
    /// Tipo de evento:
    /// - "page_view": Vista de la página de la tarjeta
    /// - "qr_scan": Escaneo del código QR
    /// - "nfc_tap": Tap de NFC
    /// - "cta_click": Clic en un botón de acción (WhatsApp, Email, Llamar, Calendar)
    /// - "link_click": Clic en un enlace o red social
    /// - "contact_save": El visitante guardó el contacto (vCard download)
    /// - "form_submit": Envío del formulario de contacto
    /// - "meeting_click": Clic en "Book on Calendly" o similar
    /// - "directions_click": Clic en dirección/mapa
    /// - "share": La tarjeta fue compartida
    /// </summary>
    public string EventType { get; set; } = default!;
    
    /// <summary>
    /// Momento exacto del evento (UTC)
    /// </summary>
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// User-Agent del dispositivo del visitante
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Hash SHA256 de la IP del visitante (NUNCA guardar IP en claro por privacidad)
    /// </summary>
    public string? IpHash { get; set; }
    
    /// <summary>
    /// Referrer - de dónde vino el visitante (dominio only, not full URL)
    /// </summary>
    public string? Referrer { get; set; }
    
    /// <summary>
    /// País detectado del visitante (ISO country code, e.g., "SV", "US")
    /// </summary>
    public string? Country { get; set; }
    
    /// <summary>
    /// Código de país ISO 3166-1 alpha-2 (e.g., "SV", "US", "MX")
    /// </summary>
    public string? CountryCode { get; set; }
    
    /// <summary>
    /// Región/Estado del visitante
    /// </summary>
    public string? Region { get; set; }
    
    /// <summary>
    /// Ciudad detectada del visitante (basado en IP)
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Latitud de la ubicación (para mapas)
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Longitud de la ubicación (para mapas)
    /// </summary>
    public double? Longitude { get; set; }
    
    /// <summary>
    /// Fuente de la geolocalización: "ip", "gps", "manual", "lookup"
    /// </summary>
    public string? GeoSource { get; set; }
    
    /// <summary>
    /// Tipo de dispositivo: "mobile", "tablet", "desktop"
    /// </summary>
    public string? DeviceType { get; set; }
    
    /// <summary>
    /// Session ID anónimo para agrupar eventos de un mismo visitante
    /// </summary>
    public string? SessionId { get; set; }
    
    /// <summary>
    /// Canal de interacción (para link/cta clicks):
    /// "whatsapp", "email", "linkedin", "call", "calendar", "website", "portfolio", "instagram", etc.
    /// </summary>
    public string? Channel { get; set; }
    
    /// <summary>
    /// Metadatos adicionales en JSON según el tipo de evento.
    /// Ejemplos:
    /// - link_click: {"platform": "linkedin", "url": "..."}
    /// - cta_click: {"button": "whatsapp"}
    /// - form_submit: {"leadId": "..."}
    /// </summary>
    public string? MetadataJson { get; set; }
    
    // Legacy field - kept for backward compatibility, maps to IpHash
    [Obsolete("Use IpHash instead. This field is kept for migration purposes.")]
    public string? IpAddress { get; set; }
    
    // Navigation
    public Card Card { get; set; } = default!;
}
