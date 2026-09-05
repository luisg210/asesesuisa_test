using Testcontainers.MsSql;

namespace Consultora.Tests.Integration;

// Coleccion compartida: un solo contenedor SQL Server para toda la suite de
// integration tests (evita levantar una instancia por clase).
[CollectionDefinition("SqlServer")]
public sealed class SqlServerCollection : ICollectionFixture<SqlServerFixture>
{
}