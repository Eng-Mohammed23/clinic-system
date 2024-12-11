namespace ClinicSystem.Api.Services;

public interface IBookService
{
    Task<DateTime?> AvailableBookingAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<BookRequest>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<BookRequest>> GetByIdAsync(int id, CancellationToken cancellation = default);
    Task<Result<IEnumerable<BookRequest>>> GetByDateAsync(DateTime startDate,DateTime endDate, CancellationToken cancellation = default);
    Task<Result<BookRequest>> AddAsync(string id, BookRequest bookRequest, CancellationToken cancellationToken = default);
    Task<Result> ReportAcync(string bookId, string report, CancellationToken cancellationToken = default);
    byte[] GeneratePdf(string htmlContent);
    Task<bool> MakePaymentAsync(string phoneNumber, decimal amount);

    #region old message way
    Task<Result> SendMessageAsync(string userId, MessageRequest request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<MessageResponse>>> GetAllMessagesAsync(CancellationToken cancellationToken = default);
    Task<Result<MessageResponse>> MessageAnswerAsync(MessageAnswer answer, CancellationToken cancellationToken = default);

    #endregion
}
