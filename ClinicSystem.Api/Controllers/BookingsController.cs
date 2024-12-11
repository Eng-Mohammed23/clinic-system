using ClinicSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Abstractions;
using System.Composition;
using System.Security.Claims;
using System.Threading;

namespace ClinicSystem.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BookingsController(IBookService bookService) : ControllerBase
{
    
    private readonly IBookService _bookService = bookService;
   
    [HttpGet("appointment")]
    public async Task<IActionResult> AvailableBooking( CancellationToken cancellationToken)
    {
        var result = await _bookService.AvailableBookingAsync(cancellationToken);

        return Ok(result);
    }
    [Authorize(Roles ="Admin")]
    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _bookService.GetAllAsync(cancellationToken);

        return Ok(result.Value);
    }
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _bookService.GetByIdAsync(id);

        return result.IsSuccess? Ok(result.Value) : result.ToProblem();
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("/date/range")]
    public async Task<IActionResult> GetByDateInRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken cancellationToken )
    {
        var result = await _bookService.GetByDateAsync(startDate, endDate, cancellationToken);

        return result.IsSuccess? Ok(result.Value):result.ToProblem();
    }
    [Authorize]
    [HttpPost("{userId?}")]
    public async Task<IActionResult> Add([FromRoute]string? userId, [FromBody] BookRequest bookRequest, CancellationToken cancellationToken )
    {
        userId = userId?? User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var result = await _bookService.AddAsync(userId!, bookRequest, cancellationToken );

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    
}
