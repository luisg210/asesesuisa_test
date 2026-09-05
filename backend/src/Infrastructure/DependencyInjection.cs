using Consultora.Application.Ports;
using Consultora.Infrastructure.Data;
using Consultora.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consultora.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(options =>
        {
            options.ConsultoraDb = configuration
                .GetConnectionString("ConsultoraDb") ?? string.Empty;
        });

        services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

        services.AddScoped<IPaqueteRepository, PaqueteRepository>();
        services.AddScoped<IConsultorRepository, ConsultorRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IReporteRepository, ReporteRepository>();
        services.AddScoped<IAreaRepository, AreaRepository>();
        services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
        services.AddScoped<IConsultorPaqueteRepository, ConsultorPaqueteRepository>();

        return services;
    }
}