using System.Net.Http.Headers;
using System.Text.Json;

namespace kanimeclothing.Services
{
    public interface IPaymentService
    {
        Task<bool> VerifyPaymentAsync(string reference);
    }

    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PaymentService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> VerifyPaymentAsync(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return false;

            var secretKey = _configuration["Payment:PaystackSecretKey"];
            var verifyBaseUrl = _configuration["Payment:PaystackVerifyUrl"] ?? "https://api.paystack.co/transaction/verify/";

            if (string.IsNullOrWhiteSpace(secretKey))
                return false; // not configured; avoid clearing cart blindly.

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var verifyUrl = verifyBaseUrl.TrimEnd('/') + "/" + reference;

            try
            {
                var response = await _httpClient.GetAsync(verifyUrl);
                if (!response.IsSuccessStatusCode)
                    return false;

                using var stream = await response.Content.ReadAsStreamAsync();
                var document = await JsonDocument.ParseAsync(stream);

                // Paystack response format: { status: true/false, data: { status: "success" }}
                if (document.RootElement.TryGetProperty("status", out var apiStatus) && apiStatus.GetBoolean())
                {
                    if (document.RootElement.TryGetProperty("data", out var data) &&
                        data.TryGetProperty("status", out var txnStatus))
                    {
                        return string.Equals(txnStatus.GetString(), "success", StringComparison.OrdinalIgnoreCase);
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
