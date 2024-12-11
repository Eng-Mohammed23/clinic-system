using ClinicSystem.Api.Entities;
using ClinicSystem.Api.Errors;
using ClinicSystem.Api.Persistence;
using DinkToPdf;
using DinkToPdf.Contracts;
using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using WhatsAppCloudApi;
using WhatsAppCloudApi.Services;

namespace ClinicSystem.Api.Services;

public class BookService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,IWebHostEnvironment webHostEnvironment,
   IBackgroundJobClient backgroundJobClient,IWhatsAppClient whatsAppClient,IConverter converter) : IBookService
{
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
    private readonly IWhatsAppClient _whatsAppClient = whatsAppClient;
    private readonly IConverter _converter = converter;
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    //public static DateTime Emg;
    private readonly HttpClient _httpClient;

    public async Task<DateTime?> AvailableBookingAsync( CancellationToken cancellationToken = default)
    {
        TimeSpan targetTime = new TimeSpan(18, 0, 0); // 18:00:00
        //if (_context.Bookings.OrderByDescending(x => x.Id).Select(t => t.TimeOfBook).FirstOrDefault() is not { } time)
        DateTime availableTime;
        var lastUser = _context.Bookings.OrderByDescending(x => x.Id).Select(t => t.TimeOfBook).FirstOrDefault().AddMinutes(10);

        if (lastUser.TimeOfDay >= targetTime)
            return new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day, 9, 0, 0); // تعيين 22 سبتمبر 2024، الساعة 3:30 مساءً

        //if (lastUser != null)
        //    availableTime = lastUser;

        if ((DateTime.UtcNow > lastUser || lastUser == default(DateTime).AddMinutes(10)) && !(DateTime.UtcNow.TimeOfDay > targetTime))
            lastUser = DateTime.UtcNow;
        else if ((DateTime.UtcNow > lastUser || lastUser == default(DateTime).AddMinutes(10)) && (DateTime.UtcNow.TimeOfDay >= targetTime))
            return new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day, 9, 0, 0); // تعيين 22 سبتمبر 2024، الساعة 3:30 مساءً

        return lastUser;
    }
    public async Task<Result<IEnumerable<BookRequest>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        //var res = await _whatsAppClient.SendMessage("201158085361", WhatsAppLanguageCode.English_US,"hello_world");
        var bookings = await _context.Bookings.ToListAsync(cancellationToken);

        var viewModel = bookings.Adapt<IEnumerable<BookRequest>>();

        return Result.Success(viewModel);
    }
    public async Task<Result<BookRequest>> GetByIdAsync(int id, CancellationToken cancellation = default)
    {
        if (await _context.Bookings.FindAsync(id) is not { } booking)
            return Result.Failure<BookRequest>(UserErrors.InvalidCredentials);

        var viewModel = booking.Adapt<BookRequest>();

        return Result.Success(viewModel);
    }
    public async Task<Result<IEnumerable<BookRequest>>> GetByDateAsync(DateTime startDate,DateTime endDate, CancellationToken cancellation = default)
    {
        var bookings = await _context.Bookings.Where(b => b.TimeOfBook >= startDate && b.TimeOfBook <= endDate).ToListAsync(cancellation);

        var viewModel = bookings.Adapt<IEnumerable<BookRequest>>();

        return Result.Success(viewModel);
    }
        
    public async Task<Result<BookRequest>> AddAsync(string userId, BookRequest bookRequest, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure<BookRequest>(UserErrors.NotFound);

        if (_context.Bookings.SingleOrDefault(b => b.TimeOfBook == bookRequest.TimeOfBook) is { } book)
            return Result.Failure<BookRequest>(UserErrors.DuplicatedAppointment);

        var bookModel = bookRequest.Adapt<Booking>();

        await _context.AddAsync(bookModel);
        await _context.SaveChangesAsync(cancellationToken);

        if(bookRequest.HasWhats)
            _backgroundJobClient.Schedule(() => SendNotificationToPatient(userId, bookRequest.Name, bookRequest.PhoneNumber), bookRequest.TimeOfBook.AddMinutes(-30));

        return Result.Success(bookRequest);
    }
     
    //public async Task emergency(DateTime date)
    //{
        
    //}

    public async Task<Result> ReportAcync(string bookId, string report, CancellationToken cancellationToken = default)
    {
        if (await _context.Bookings.FindAsync(bookId) is not { } booking)
            return Result.Failure(UserErrors.InvalidCredentials);

        booking.Report = report;

        _context.Update(booking);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public byte[] GeneratePdf(string htmlContent)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4
        };

        var objectSettings = new ObjectSettings
        {
            PagesCount = true,
            HtmlContent = htmlContent,
            WebSettings = { DefaultEncoding = "utf-8" }
        };

        var pdfDocument = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };

        return _converter.Convert(pdfDocument);
    }

    public async Task SendNotificationToPatient(string userId,string userName,string mobileNumber)
    {
        var components = new List<WhatsAppComponent>()
            {
                new WhatsAppComponent
                {
                    Type = "body",
                    Parameters = new List<object>()
                    {
                        new WhatsAppTextParameter { Text = userName }
                    }
                }
            };

        mobileNumber = _webHostEnvironment.IsDevelopment() ? "201158085361" : mobileNumber;

        var res = await _whatsAppClient.SendMessage(mobileNumber, WhatsAppLanguageCode.English_US, "booking_near", components);


        //        //Change 2 with your country code
        //        BackgroundJob.Enqueue(() => _whatsAppClient
        //            .SendMessage($"245886", WhatsAppLanguageCode.English_US,
        //            "this your appointment", components));
        //    }

    }
    //test علشات 
    public async Task<bool> MakePaymentAsync(string phoneNumber, decimal amount)
    {
        var paymentData = new
        {
            PhoneNumber = phoneNumber,
            Amount = amount,
            Currency = "EGP",
            // قد تحتاج لإضافة تفاصيل إضافية حسب متطلبات فودافون
        };

        var content = new StringContent(JsonConvert.SerializeObject(paymentData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/payment", content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            // معالجة النتيجة حسب ما تحتاج
            return true;
        }
        else
        {
            // التعامل مع الخطأ
            return false;
        }
    }


    #region old message way
    public async Task<Result> SendMessageAsync(string userId, MessageRequest request, CancellationToken cancellationToken = default)
    {
        var message = request.Adapt<Message>();
        message.SenderId = userId;

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return Result.Success();
    }
    public async Task<Result<IEnumerable<MessageResponse>>> GetAllMessagesAsync(CancellationToken cancellationToken = default)
    {
        //if (await _userManager.FindByIdAsync(customerId) is not { } customer)
        //    return Result.Failure<IEnumerable<MessageResponse>>(UserErrors.NotFound);

        var messages = await _context.Messages
            .Where(m => !m.IsRead)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        return Result.Success(messages.Adapt<IEnumerable<MessageResponse>>());
    }

    public async Task<Result<MessageResponse>> MessageAnswerAsync(MessageAnswer answer, CancellationToken cancellationToken = default)
    {

        var messages = _context.Messages.Find(answer.MessageId);
        // .Where(m => m.MessageId == answer.MessageId && !m.IsRead);

        messages!.MessageAnswer = answer.Content;
        messages.IsRead = true;


        await _context.SaveChangesAsync();

        var response = messages.Adapt<MessageResponse>();

        return Result.Success(response);
    }
    #endregion

}


