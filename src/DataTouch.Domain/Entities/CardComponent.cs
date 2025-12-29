namespace DataTouch.Domain.Entities;

/// <summary>
/// Componentes modulares para tarjetas digitales.
/// Cada componente representa una sección configurable de la tarjeta.
/// </summary>
public class CardComponent
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    
    /// <summary>
    /// Tipo de componente:
    /// - "profile": Información de perfil (nombre, cargo, bio, foto)
    /// - "contact": Información de contacto (email, teléfono, dirección)
    /// - "social": Enlaces a redes sociales
    /// - "cta": Botones de acción (Guardar contacto, WhatsApp, Email, Llamar)
    /// - "links": Enlaces web personalizados
    /// - "gallery": Galería de imágenes
    /// - "video": Video embebido
    /// - "pdf": Galería de documentos PDF
    /// - "form": Formulario de contacto/lead capture
    /// - "testimonials": Testimonios de clientes
    /// - "hours": Horarios de atención
    /// - "team": Sección de equipo
    /// - "products": Productos o servicios
    /// - "text": Bloque de texto libre
    /// </summary>
    public string Type { get; set; } = default!;
    
    /// <summary>
    /// Orden de visualización del componente (menor número = primero)
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Configuración específica del componente en JSON.
    /// El esquema varía según el tipo de componente.
    /// Ejemplos:
    /// - profile: {"showPhoto": true, "photoSize": "large"}
    /// - social: {"platforms": ["linkedin", "instagram", "twitter"]}
    /// - cta: {"buttons": [{"type": "whatsapp", "label": "Enviar mensaje"}]}
    /// - gallery: {"columns": 3, "spacing": "medium"}
    /// </summary>
    public string ConfigJson { get; set; } = "{}";
    
    /// <summary>
    /// Datos del componente en JSON (el contenido real).
    /// Ejemplos:
    /// - gallery: {"images": [{"url": "...", "caption": "..."}]}
    /// - video: {"url": "https://youtube.com/...", "autoplay": false}
    /// - testimonials: {"items": [{"name": "...", "text": "...", "rating": 5}]}
    /// </summary>
    public string DataJson { get; set; } = "{}";
    
    /// <summary>
    /// Si el componente está visible en la tarjeta pública
    /// </summary>
    public bool IsVisible { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public Card Card { get; set; } = default!;
}
