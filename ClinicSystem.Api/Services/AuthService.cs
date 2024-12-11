
using ClinicSystem.Api.Abstructions;
using ClinicSystem.Api.Contracts.Users;
using ClinicSystem.Api.Errors;
using ClinicSystem.Api.Persistence;
using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Api.Authentications;
using SurveyBasket.Helpers;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class AuthService(UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, IEmailSender emailSender
    , IJwtProvider jwtProvider,ApplicationDbContext context,ILogger<AuthService> logger) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<AuthService> _logger = logger;

    //private readonly ApplicationDbContext _context = context;

    public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var Test = await _userManager.Users.ToListAsync();

        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.NotFound);
        
        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

        if (result.Succeeded)
        {
            var (token, expiresIn) = _jwtProvider.GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });
            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(user.Id, user.Email, user.FullName, token, expiresIn, refreshToken,refreshTokenExpiration);
            return Result.Success<AuthResponse>(response);
        }
        
        return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
    }
    public async Task<Result<AuthResponse>> RegisterAsync(string email,string password,string fullName,string phoneNum,CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not null)
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

        var user = new ApplicationUser
        {
            Email = email,
            FullName = fullName,
            UserName = email,
            MobileNumber = phoneNum,
            EmailConfirmed = true
        };

        var (token, expiresIn) = _jwtProvider.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.CreateAsync(user,password);

        var response = new AuthResponse(user.Id, user.Email, user.FullName, token, expiresIn, refreshToken, refreshTokenExpiration);
        return Result.Success<AuthResponse>(response);
    }
    public async Task<Result> ChangePasswordAsync(string userId,ChangePasswordRequest request,CancellationToken cancellationToken=default)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.NotFound); 

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if(result.Succeeded)
            return Result.Success(result);

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status404NotFound));
    }

    public async Task<Result> SendResetPasswordCodeAsync(string email, CancellationToken cancellationToken = default)
    {
        if(await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<Result>(UserErrors.NotFound);

        if(!user.EmailConfirmed)
            return Result.Failure<Result>(UserErrors.EmailNotConfirmed);  

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Reset code: {code}", code);

        await SendConfirmationEmail(user, code);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken=default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);

        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));

    }
    public async Task<Result<AuthResponse>> GetRefreshToken(string Token, string refreshToken, CancellationToken cancellationToken = default)
    {
        if (_jwtProvider.ValidateToken(Token) is not { } userId)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.NotFound);

        if (user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive) is not { } userRefreshToken)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        //var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);

        var (token, expiresIn) = _jwtProvider.GenerateToken(user);
        var NewRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = NewRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email, user.FullName, token, expiresIn, refreshToken, refreshTokenExpiration);
        return Result.Success<AuthResponse>(response);
    }

    
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private async Task SendConfirmationEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            templateModel: new Dictionary<string, string>
            {
                { "{{name}}", user.FullName },
                    { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
            }
        );

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Email Confirmation", emailBody));

        await Task.CompletedTask;
    }
}
