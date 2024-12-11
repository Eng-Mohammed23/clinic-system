using ClinicSystem.Api.Contracts.Users;
using System.Threading;

namespace ClinicSystem.Api.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RegisterAsync(string email, string password, string fullName,string phoneNum, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(string userId,ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest requestCancellationToken, CancellationToken cancellationToken = default);
    Task<Result> SendResetPasswordCodeAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GetRefreshToken(string Token, string RefreshToken, CancellationToken cancellationToken = default);

    }
