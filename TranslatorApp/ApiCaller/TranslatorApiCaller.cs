using System.Text.Json;
using TranslatorApp.ApiCaller.Model;

namespace TranslatorApp.ApiCaller
{
    public class TranslatorApiCaller : ITranslatorApiCaller
    {
        private readonly ILogger<TranslatorApiCaller> _logger;

        public TranslatorApiCaller(ILogger<TranslatorApiCaller> logger)
        {
            _logger = logger;
        }
        public async Task<string> GetTranslationAsync(string originalText, string apiUri)
        {
            var apiResponse = new ApiResponseModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUri);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

                var response = await client.GetAsync($"?text={originalText}");

                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Response body: {responseBody}");
                apiResponse = JsonSerializer.Deserialize<ApiResponseModel>(responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                _logger.LogInformation($"ApiResponse: {apiResponse}");

                _logger.LogInformation($"{apiResponse.Success} ;;;;; {apiResponse.Contents}");
            }

            if (apiResponse == null)
                return "Something went wrong on Api call";

            return apiResponse.Contents.Translated;
        }
    }
}
