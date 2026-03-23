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
            System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] Starting verification for reference: {reference}");
            
            if (string.IsNullOrWhiteSpace(reference))
            {
                System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] Reference is empty!");
                return false;
            }

            var secretKey = _configuration["Payment:PaystackSecretKey"];
            var verifyBaseUrl = _configuration["Payment:PaystackVerifyUrl"] ?? "https://api.paystack.co/transaction/verify/";

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] Secret key not configured!");
                return false;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var verifyUrl = verifyBaseUrl.TrimEnd('/') + "/" + reference;
            System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] Calling Paystack API: {verifyUrl}");

            try
            {
                var response = await _httpClient.GetAsync(verifyUrl);
                System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] HTTP Response Status: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] Non-success HTTP status: {response.StatusCode}");
                    return false;
                }

                using var stream = await response.Content.ReadAsStreamAsync();
                var document = await JsonDocument.ParseAsync(stream);
                
                System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] Response JSON: {document.RootElement}");

                // Paystack response format: { status: true/false, data: { status: "success" }}
                if (document.RootElement.TryGetProperty("status", out var apiStatus))
                {
                    var apiStatusBool = apiStatus.GetBoolean();
                    System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] API status property: {apiStatusBool}");
                    
                    if (apiStatusBool)
                    {
                        if (document.RootElement.TryGetProperty("data", out var data))
                        {
                            if (data.TryGetProperty("status", out var txnStatus))
                            {
                                var txnStatusStr = txnStatus.GetString();
                                System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] Transaction status: {txnStatusStr}");
                                
                                var isSuccess = string.Equals(txnStatusStr, "success", StringComparison.OrdinalIgnoreCase);
                                System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] Verification result: {isSuccess}");
                                return isSuccess;
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] Could not find expected properties in response");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] Exception: {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[PaymentService.VerifyPayment] Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}
