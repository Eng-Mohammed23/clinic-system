using ClinicSystem.Api.Contracts.Users;
using ClinicSystem.Api.Hubs;
using ClinicSystem.Api.Persistence;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using System.Security.Claims;

namespace ClinicSystem.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, IHubContext<ChatHub> hubContext,ApplicationDbContext context) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IHubContext<ChatHub> _hubContext = hubContext;
    private readonly ApplicationDbContext _context = context;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginRequest request,CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request.Email, request.Password, request.FullName, request.PhoneNum, cancellationToken);

        return result.IsSuccess? Ok(result.Value) : result.ToProblem();
    }

    [Authorize]
    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordRequest request,CancellationToken cancellationToken)
    {
        var userId= User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var result = await _authService.ChangePasswordAsync(userId!,request, cancellationToken);

        return result.IsSuccess ? NoContent() :result.ToProblem();
    }

    [HttpPost("/password/forget")]
    public async Task<IActionResult> ForgetPassword([FromBody] string email, CancellationToken cancellationToken )
    {
        var result = await _authService.SendResetPasswordCodeAsync(email,cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("/password/reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request,CancellationToken cancellationToken)
    {
        var result = await _authService.ResetPasswordAsync(request,cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    //[HttpPost("refresh")]
    //public async Task<IActionResult> Refresh([FromBody]RefreshToken request,CancellationToken cancellationToken)
    //{


    //}

    
}
