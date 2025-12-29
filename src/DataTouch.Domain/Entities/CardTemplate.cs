namespace DataTouch.Domain.Entities;

/// <summary>
/// Plantillas predefinidas por industria/sector para tarjetas digitales.
/// Las plantillas del sistema (IsSystemTemplate=true) están disponibles para todas las organizaciones.
/// </summary>
public class CardTemplate
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Null = plantilla del sistema, disponible para todos.
    /// Con valor = plantilla personalizada de una organización específica.
    /// </summary>
    public Guid? OrganizationId { get; set; }
    
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// Industria/sector: "Sales", "Tech", "Finance", "Health", "Fashion", "Education"
    /// </summary>
    public string Industry { get; set; } = default!;
    
    public string? Description { get; set; }
    
    /// <summary>
    /// URL del thumbnail/preview de la plantilla
    /// </summary>
    public string ThumbnailUrl { get; set; } = default!;
    
    /// <summary>
    /// Estilos por defecto en JSON: colores, fuentes, etc.
    /// </summary>
    public string DefaultStyleJson { get; set; } = "{}";
    
    /// <summary>
    /// Componentes incluidos por defecto en JSON
    /// </summary>
    public string DefaultComponentsJson { get; set; } = "[]";
    
    /// <summary>
    /// True = plantilla del sistema, False = creada por usuario/organización
    /// </summary>
    public bool IsSystemTemplate { get; set; } = true;
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public Organization? Organization { get; set; }
    public ICollection<Card> Cards { get; set; } = new List<Card>();
}
