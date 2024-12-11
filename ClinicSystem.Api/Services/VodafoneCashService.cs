using Newtonsoft.Json;
using Stripe;
using System.Net.Http.Headers;
using System.Text;

namespace ClinicSystem.Api.Services;

public class VodafoneCashService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public VodafoneCashService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        // تعيين القيم المطلوبة من إعدادات التطبيق
        _httpClient.BaseAddress = new Uri(_configuration["VodafoneCash:BaseUrl"]);
        _httpClient.DefaultRequestHeaders.Add("ApiKey", _configuration["VodafoneCash:ApiKey"]);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _configuration["VodafoneCash:Token"]);
    }
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

    //stripe
    public async Task<string> CreatePaymentIntent(decimal amount, string currency = "usd")
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100), // تحويل إلى السنتات (Stripe يعمل بالدولار/السنت)
            Currency = currency,
            PaymentMethodTypes = new List<string> { "card" },
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        return paymentIntent.ClientSecret; // هذا المفتاح يُستخدم من جهة العميل لإكمال الدفع
    }

}

