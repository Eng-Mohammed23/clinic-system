using ClinicSystem.Api.Hubs;
using ClinicSystem.Api.Persistence;
using ClinicSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using System.Security.Claims;

namespace ClinicSystem.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController(
    IHubContext<ChatHub> hubContext, 
    ApplicationDbContext context,
    VodafoneCashService vodafoneCashService,
    IBookService bookService) : ControllerBase
{
    private readonly VodafoneCashService _vodafoneCashService = vodafoneCashService;
    private readonly IBookService _bookService = bookService;
    private readonly IHubContext<ChatHub> _hubContext = hubContext;
    private readonly ApplicationDbContext _context = context;

    #region Reports
    //التقرير
    [Authorize(Roles = "Admin")]
    [HttpPost("report")]
    public async Task<IActionResult> Report(string bookId, string report, CancellationToken cancellationToken)
    {
        var result = await _bookService.ReportAcync(bookId, report, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("/report/generate")]
    public async Task<IActionResult> GenerateReport(string bookId, string content, CancellationToken cancellationToken)
    {
        var result = await _bookService.ReportAcync(bookId, content, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        // Example HTML for the report
        var htmlContent = $@"
            <html>
                <head><title>Report</title></head>
                <body>
                    <h1>PDF Report</h1>
                    <p>{content}</p>
                </body>
            </html>";

        byte[] pdf = _bookService.GeneratePdf(htmlContent);

        return File(pdf, "application/pdf", "report.pdf");
    }
    #endregion

    #region Payments
    //vodafone
    [HttpPost("payment")]
    public async Task<IActionResult> Pay([FromBody] PaymentRequest paymentRequest)
    {
        var result = await _vodafoneCashService.MakePaymentAsync(paymentRequest.PhoneNumber, paymentRequest.Amount);

        if (result)
            return Ok(new { Message = "Payment successful" });
        else
            return BadRequest(new { Message = "Payment failed" });
    }

    //stripe
    [HttpPost("payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentRequest request)
    {
        var Currency = request.PhoneNumber;  //test
        var clientSecret = await _vodafoneCashService.CreatePaymentIntent(request.Amount, Currency);
        return Ok(new { ClientSecret = clientSecret });
    }
    #endregion

    #region message old
    [Authorize]
    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] MessageRequest message, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var result = await _bookService.SendMessageAsync(userId!, message, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("message/{customerId}")]
    public async Task<IActionResult> GetAllMessages(CancellationToken cancellationToken)
    {
        var result = await _bookService.GetAllMessagesAsync(cancellationToken);

        return result.IsSuccess ? Ok(result) : result.ToProblem();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/message/answer")]
    public async Task<IActionResult> MessageAnswer([FromBody] MessageAnswer answer, CancellationToken cancellationToken)
    {
        var result = await _bookService.MessageAnswerAsync(answer, cancellationToken);

        return result.IsSuccess ? Ok(result) : result.ToProblem();
    }
    #endregion


    #region new way message
    // إرسال رسالة
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] Message message)
    {
        message.Timestamp = DateTime.UtcNow;
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // إرسال تحديث عبر SignalR
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.SenderId, message.ReceiverId, message.MessageText);

        return Ok(message);
    }

    // الحصول على رسائل لمستخدم معين
    [HttpGet("get/{user}")]
    public IActionResult GetMessages(string user)
    {
        var messages = _context.Messages
            .Where(m => m.ReceiverId == user || m.SenderId == user)
            .OrderBy(m => m.Timestamp)
            .ToList();
        return Ok(messages);
    }
    #endregion

}
