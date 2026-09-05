namespace Consultora.Application.Security;

public record TokenResult(string Token, DateTime ExpiresAt);