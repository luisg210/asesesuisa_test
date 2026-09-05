using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace Consultora.Tests.Integration;

/// <summary>
/// Ejecuta los scripts SQL del repositorio contra una instancia real de SQL
/// Server. Los scripts usan "GO" como separador de lotes (no es T-SQL, es del
/// cliente sqlcmd/SSMS), por lo que se dividen en lotes antes de ejecutarlos.
/// </summary>
internal static class SqlScriptRunner
{
    public static async Task RunScriptsAsync(string appConnection, params (string ScriptPath, string ConnectionString)[] scripts)
    {
        foreach (var (scriptPath, connectionString) in scripts)
        {
            var fullPath = Path.Combine(AppContext.BaseDirectory, scriptPath);
            var content = await File.ReadAllTextAsync(fullPath);

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            foreach (var batch in SplitBatches(content))
            {
                await using var command = new SqlCommand(batch, connection)
                {
                    CommandTimeout = 120
                };
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    private static IEnumerable<string> SplitBatches(string sql)
    {
        // Normaliza "GO" inline (solo separador en los scripts del repo) para
        // que siempre quede en una linea propia.
        var normalized = Regex.Replace(
            sql,
            @"(?i)(?<=\s|^)\bGO\b(?=\s|$)",
            Environment.NewLine + "GO" + Environment.NewLine);

        var current = new System.Text.StringBuilder();
        foreach (var line in normalized.Split('\n'))
        {
            if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
            {
                if (current.Length > 0)
                {
                    yield return current.ToString();
                    current.Clear();
                }
            }
            else
            {
                current.AppendLine(line);
            }
        }

        if (current.Length > 0)
        {
            yield return current.ToString();
        }
    }
}