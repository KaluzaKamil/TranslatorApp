
namespace TranslatorApp.ApiCaller
{
    public interface ITranslatorApiCaller
    {
        Task<string> GetTranslationAsync(string originalText, string apiUri);
    }
}