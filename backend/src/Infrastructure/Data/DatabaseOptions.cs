namespace Consultora.Infrastructure.Data;

/// <summary>
/// Opciones de conexion a base de datos (section "ConnectionStrings").
/// </summary>
public class DatabaseOptions
{
    public const string SectionName = "ConnectionStrings";

    /// <summary>Cadena de conexion principal.</summary>
    public string ConsultoraDb { get; set; } = string.Empty;
}
