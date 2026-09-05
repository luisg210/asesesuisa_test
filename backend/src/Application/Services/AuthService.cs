using Consultora.Application.Common;
using Consultora.Application.Dtos;
using Consultora.Application.Exceptions;
using Consultora.Application.Ports;
using Consultora.Application.Security;
using Consultora.Application.Services.Contracts;
using Consultora.Application.Validation;
using Consultora.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Consultora.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IValidator<LoginRequest> _validator;
    private readonly IValidator<RefreshTokenRequest> _refreshValidator;
    private readonly IAuditService? _audit;
    private readonly IRefreshTokenRepository? _refreshRepository;
    private readonly IRefreshTokenGenerator? _refreshGenerator;
    private readonly JwtSettings _jwt;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IJwtTokenGenerator tokenGenerator,
        IValidator<LoginRequest> validator)
        : this(usuarioRepository, tokenGenerator, validator, null, null, null, null, null)
    {
    }

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IJwtTokenGenerator tokenGenerator,
        IValidator<LoginRequest> validator,
        IAuditService? audit)
        : this(usuarioRepository, tokenGenerator, validator, audit, null, null, null, null)
    {
    }

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IJwtTokenGenerator tokenGenerator,
        IValidator<LoginRequest> validator,
        IAuditService? audit,
        IRefreshTokenRepository? refreshRepository,
        IRefreshTokenGenerator? refreshGenerator,
        IValidator<RefreshTokenRequest>? refreshValidator,
        IOptions<JwtSettings>? jwt)
    {
        _usuarioRepository = usuarioRepository;
        _tokenGenerator = tokenGenerator;
        _validator = validator;
        _audit = audit;
        _refreshRepository = refreshRepository;
        _refreshGenerator = refreshGenerator;
        _refreshValidator = refreshValidator ?? new RefreshTokenValidator();
        _jwt = jwt?.Value ?? new JwtSettings();
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default, string? ip = null)
    {
        await _validator.ValidateAndThrowAsync(request, ct);

        var user = await _usuarioRepository.GetByEmailAsync(request.Email.Trim(), ct);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var response = await IssueTokensAsync(user, ct, ip);

        await RecordAuditAsync(user.Email, "LOGIN", user.Id, ip, ct);

        return response;
    }

    public async Task<LoginResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken ct = default, string? ip = null)
    {
        await _refreshValidator.ValidateAndThrowAsync(request, ct);

        var stored = await FindValidTokenAsync(request.RefreshToken, ct);
        if (stored is null)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        var user = await _usuarioRepository.GetByIdAsync(stored.UsuarioId, ct);
        if (user is null)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        // Rotacion: el refresh token usado queda invalidado y se emite uno nuevo.
        if (_refreshRepository is not null)
        {
            await _refreshRepository.RevokeAsync(stored.Id, ct);
        }

        var response = await IssueTokensAsync(user, ct, ip);

        await RecordAuditAsync(user.Email, "REFRESH", user.Id, ip, ct);

        return response;
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default, string? ip = null)
    {
        await _refreshValidator.ValidateAndThrowAsync(new RefreshTokenRequest(refreshToken), ct);

        var stored = await FindValidTokenAsync(refreshToken, ct);
        if (stored is null)
        {
            return; // Idempotente: un token invalido ya no tiene sesion activa.
        }

        var user = await _usuarioRepository.GetByIdAsync(stored.UsuarioId, ct);
        if (_refreshRepository is not null)
        {
            await _refreshRepository.RevokeAllByUserAsync(stored.UsuarioId, ct);
        }

        await RecordAuditAsync(user?.Email ?? "system", "LOGOUT", stored.UsuarioId, ip, ct);
    }

    private async Task<LoginResponse> IssueTokensAsync(Usuario user, CancellationToken ct, string? ip)
    {
        var access = _tokenGenerator.Generate(user);
        var refreshValue = _refreshGenerator?.Generate() ?? $"test-{Guid.NewGuid():N}";
        var refreshExpiry = DateTime.UtcNow.AddMinutes(_jwt.RefreshTokenExpiryMinutes);

        if (_refreshRepository is not null)
        {
            await _refreshRepository.InsertAsync(new RefreshToken
            {
                UsuarioId = user.Id,
                TokenHash = RefreshTokenHasher.Hash(refreshValue),
                ExpiresAt = refreshExpiry,
                Ip = ip
            }, ct);
        }

        return new LoginResponse(
            access.Token, access.ExpiresAt, refreshValue, refreshExpiry, user.Email, user.Rol.ToString());
    }

    private async Task<RefreshToken?> FindValidTokenAsync(string value, CancellationToken ct)
    {
        if (_refreshRepository is null)
        {
            return null;
        }

        var hash = RefreshTokenHasher.Hash(value.Trim());
        var stored = await _refreshRepository.GetByHashAsync(hash, ct);
        if (stored is null || stored.RevokedAt is not null || stored.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        return stored;
    }

    private async Task RecordAuditAsync(string actor, string action, int entityId, string? ip, CancellationToken ct)
    {
        if (_audit is null)
        {
            return;
        }

        await _audit.RecordAsync(new AuditContext(actor, action, "Usuario", entityId, null, ip), ct);
    }
}