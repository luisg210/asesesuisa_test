using Microsoft.Data.SqlClient;

namespace Consultora.Infrastructure.Data;

/// <summary>
/// Extensions para construir parametros de SqlCommand.
/// </summary>
public static class SqlCommandExtensions
{
    public static void AddParameter(this SqlCommand command, string name, object? value, bool nullable = false)
    {
        if (value is null && nullable)
        {
            command.Parameters.AddWithValue(name, DBNull.Value);
            return;
        }

        command.Parameters.AddWithValue(name, value ?? DBNull.Value);
    }
}