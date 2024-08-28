using TranslatorApp.ViewModels;

namespace TranslatorApp.Repositories
{
    public interface ITranslatorRepository
    {
        Task<int> AddTranslationAsync(TranslationViewModel translationViewModel);
        Task<int> AddTranslatorAsync(TranslatorViewModel translatorViewModel);
        Task<int> DeleteTranslationAsync(int Id);
        Task<int> DeleteTranslatorAsync(int Id);
        Task<int> EditTranslatorAsync(TranslatorViewModel translatorViewModel);
        Task<TranslationViewModel> GetTranslationAsync(int? Id);
        Task<List<TranslationViewModel>> GetTranslationsAsync();
        Task<TranslatorViewModel> GetTranslatorAsync(int? Id);
        Task<List<TranslatorViewModel>> GetTranslatorsAsync();
    }
}