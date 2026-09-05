using Consultora.Application.Dtos;
using Consultora.Application.Exceptions;
using Consultora.Application.Security;
using Consultora.Application.Services;
using Consultora.Application.Validation;
using Consultora.Domain.Entities;
using Consultora.Domain.Enums;
using Consultora.Tests.Fakes;
using FluentValidation;

namespace Consultora.Tests.Services;

public class AuthServiceTests
{
    private static readonly string AdminHash =
        BCrypt.Net.BCrypt.HashPassword("Admin@123");

    private static readonly Usuario Admin = new()
    {
        Id = 1,
        Email = "admin@consultora.test",
        PasswordHash = AdminHash,
        Rol = Rol.Admin
    };

    private sealed record Harness(
        AuthService Service,
        FakeRefreshTokenRepository RefreshRepository,
        FakeAuditService Audit);

    private static Harness CreateService(Usuario? usuario)
    {
        var refreshRepository = new FakeRefreshTokenRepository();
        var audit = new FakeAuditService();
        var service = new AuthService(
            new FakeUsuarioRepository(usuario),
            new FakeJwtTokenGenerator(),
            new LoginValidator(),
            audit,
            refreshRepository,
            new RefreshTokenGenerator(),
            refreshValidator: null,
            jwt: null);
        return new Harness(service, refreshRepository, audit);
    }

    private static async Task<LoginResponse> LogInAdminAsync(Harness harness)
        => await harness.Service.LoginAsync(new LoginRequest("admin@consultora.test", "Admin@123"));

    [Fact]
    public async Task Login_UsuarioNoExiste_LanzaUnauthorized()
    {
        var harness = CreateService(null);
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            harness.Service.LoginAsync(new LoginRequest("nobody@correo.test", "pass")));
    }

    [Fact]
    public async Task Login_PasswordIncorrecta_LanzaUnauthorized()
    {
        var harness = CreateService(Admin);
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            harness.Service.LoginAsync(new LoginRequest("admin@consultora.test", "Wrong.Pass")));
    }

    [Fact]
    public async Task Login_CredencialesValidas_DevuelveTokenRolYRefresh()
    {
        var harness = CreateService(Admin);

        var response = await LogInAdminAsync(harness);

        Assert.Equal("fake-token", response.Token);
        Assert.Equal("Admin", response.Role);
        Assert.Equal("admin@consultora.test", response.Email);
        Assert.False(string.IsNullOrWhiteSpace(response.RefreshToken));
        Assert.True(response.RefreshExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_EmailVacio_LanzaValidacion()
    {
        var harness = CreateService(new Usuario());
        await Assert.ThrowsAsync<ValidationException>(() =>
            harness.Service.LoginAsync(new LoginRequest("", "Admin@123")));
    }

    [Fact]
    public async Task Refresh_TokenValido_RotaYDevuelveNuevaSesion()
    {
        var harness = CreateService(Admin);
        var login = await LogInAdminAsync(harness);

        var refreshed = await harness.Service.RefreshAsync(new RefreshTokenRequest(login.RefreshToken));

        Assert.Equal("fake-token", refreshed.Token);
        Assert.NotEqual(login.RefreshToken, refreshed.RefreshToken);
        Assert.Equal("admin@consultora.test", refreshed.Email);

        // El refresh token usado queda revocado (rotacion).
        var stored = await harness.RefreshRepository.GetByHashAsync(
            RefreshTokenHasher.Hash(login.RefreshToken));
        Assert.NotNull(stored!.RevokedAt);
    }

    [Fact]
    public async Task Refresh_TokenUsadoDosVeces_LanzaUnauthorized()
    {
        var harness = CreateService(Admin);
        var login = await LogInAdminAsync(harness);
        await harness.Service.RefreshAsync(new RefreshTokenRequest(login.RefreshToken));

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            harness.Service.RefreshAsync(new RefreshTokenRequest(login.RefreshToken)));
    }

    [Fact]
    public async Task Refresh_TokenInexistente_LanzaUnauthorized()
    {
        var harness = CreateService(Admin);
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            harness.Service.RefreshAsync(new RefreshTokenRequest("no-existe")));
    }

    [Fact]
    public async Task Refresh_TokenExpirado_LanzaUnauthorized()
    {
        var harness = CreateService(Admin);
        var valor = "expirado-003";
        await harness.RefreshRepository.InsertAsync(new RefreshToken
        {
            UsuarioId = Admin.Id,
            TokenHash = RefreshTokenHasher.Hash(valor),
            ExpiresAt = DateTime.UtcNow.AddMinutes(-1)
        });

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            harness.Service.RefreshAsync(new RefreshTokenRequest(valor)));
    }

    [Fact]
    public async Task Logout_RevocaTodosLosTokensDelUsuario()
    {
        var harness = CreateService(Admin);
        var primera = await LogInAdminAsync(harness);
        var segunda = await LogInAdminAsync(harness);

        await harness.Service.LogoutAsync(primera.RefreshToken);

        var stored = await harness.RefreshRepository.GetByHashAsync(RefreshTokenHasher.Hash(primera.RefreshToken));
        var stored2 = await harness.RefreshRepository.GetByHashAsync(RefreshTokenHasher.Hash(segunda.RefreshToken));
        Assert.NotNull(stored!.RevokedAt);
        Assert.NotNull(stored2!.RevokedAt);
    }

    [Fact]
    public async Task Logout_YaCerrado_NoLanza()
    {
        var harness = CreateService(Admin);
        var login = await LogInAdminAsync(harness);
        await harness.Service.LogoutAsync(login.RefreshToken);

        await harness.Service.LogoutAsync(login.RefreshToken);
    }
}