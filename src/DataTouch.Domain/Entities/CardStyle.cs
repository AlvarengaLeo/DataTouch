namespace DataTouch.Domain.Entities;

/// <summary>
/// Estilos visuales personalizados para tarjetas digitales.
/// Incluye colores, tipografía, fondos y configuración del QR.
/// </summary>
public class CardStyle
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Null = estilo personal del usuario.
    /// Con valor = estilo compartido de la organización (corporativo).
    /// </summary>
    public Guid? OrganizationId { get; set; }
    
    /// <summary>
    /// Null = estilo reutilizable/guardado.
    /// Con valor = estilo específico de una tarjeta.
    /// </summary>
    public Guid? CardId { get; set; }
    
    public string Name { get; set; } = "Custom";
    
    // ═══════════════════════════════════════════════════════════════
    // COLORES
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>Color primario para botones y acentos</summary>
    public string PrimaryColor { get; set; } = "#6366F1";
    
    /// <summary>Color secundario para gradientes</summary>
    public string SecondaryColor { get; set; } = "#EC4899";
    
    /// <summary>Color del texto principal</summary>
    public string TextColor { get; set; } = "#1F2937";
    
    /// <summary>Color de fondo del contenedor de la tarjeta</summary>
    public string BackgroundColor { get; set; } = "#FFFFFF";
    
    // ═══════════════════════════════════════════════════════════════
    // FONDO DE LA PÁGINA
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>Tipo de fondo: "solid", "gradient", "image", "video"</summary>
    public string BackgroundType { get; set; } = "gradient";
    
    /// <summary>
    /// Valor del fondo según el tipo:
    /// - solid: color hex
    /// - gradient: CSS gradient string
    /// - image/video: URL del recurso
    /// </summary>
    public string? BackgroundValue { get; set; } = "linear-gradient(135deg, #667eea 0%, #764ba2 100%)";
    
    // ═══════════════════════════════════════════════════════════════
    // TIPOGRAFÍA
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>Familia de fuente (Google Fonts o sistema)</summary>
    public string FontFamily { get; set; } = "Inter";
    
    /// <summary>Tamaño del heading principal</summary>
    public string HeadingSize { get; set; } = "1.5rem";
    
    // ═══════════════════════════════════════════════════════════════
    // QR CODE
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>Forma del QR: "square", "rounded", "dots"</summary>
    public string QrShape { get; set; } = "square";
    
    /// <summary>Color de primer plano del QR</summary>
    public string QrForeground { get; set; } = "#000000";
    
    /// <summary>Color de fondo del QR</summary>
    public string QrBackground { get; set; } = "#FFFFFF";
    
    /// <summary>URL del logo a incluir en el centro del QR</summary>
    public string? QrLogoUrl { get; set; }
    
    // ═══════════════════════════════════════════════════════════════
    // CONTENEDOR DE TARJETA
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>Border radius del contenedor de la tarjeta</summary>
    public string CardBorderRadius { get; set; } = "16px";
    
    /// <summary>Box shadow del contenedor</summary>
    public string CardShadow { get; set; } = "0 4px 20px rgba(0,0,0,0.1)";
    
    /// <summary>Animación de carga: "fade", "slide", "none"</summary>
    public string LoadingAnimation { get; set; } = "fade";
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public Organization? Organization { get; set; }
    public Card? Card { get; set; }
}
