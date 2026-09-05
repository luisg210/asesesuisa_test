using System.Reflection;
using Consultora.Application.Security;
using Consultora.Application.Services;
using Consultora.Application.Services.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Consultora.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPaqueteService, PaqueteService>();
        services.AddScoped<IConsultorService, ConsultorService>();
        services.AddScoped<IReporteService, ReporteService>();
        services.AddScoped<IAreaService, AreaService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();
        services.AddScoped<IConsultorPaqueteService, ConsultorPaqueteService>();

        return services;
    }
}