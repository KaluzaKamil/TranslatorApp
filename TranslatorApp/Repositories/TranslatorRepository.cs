using Microsoft.EntityFrameworkCore;
using TranslatorApp.Contexts;
using TranslatorApp.Models;
using TranslatorApp.ViewModels;

namespace TranslatorApp.Repositories
{
    public class TranslatorRepository : ITranslatorRepository
    {
        private readonly TranslatorDbContext _context;

        public TranslatorRepository(TranslatorDbContext context)
        {
            _context = context;
        }

        public async Task<List<TranslationViewModel>> GetTranslationsAsync()
        {
            var translationsViewModels = new List<TranslationViewModel>();
            var translations = _context.Translations.Include(t => t.Translator);

            await translations.ForEachAsync(t => translationsViewModels.Add(
                new TranslationViewModel
                {
                    Id = t.Id,
                    OriginalText = t.OriginalText,
                    TranslationText = t.TranslationText,
                    DateTranslated = t.DateTranslated,
                    TranslatorId = t.TranslatorId,
                    Translator = new TranslatorViewModel
                    {
                        Id = t.Translator.Id,
                        Name = t.Translator.Name,
                        ApiUri = t.Translator.ApiUri
                    }
                }));

            return translationsViewModels;
        }

        public async Task<TranslationViewModel> GetTranslationAsync(int? Id)
        {
            var translations = _context.Translations.Include(t => t.Translator);

            var translation = await translations.FirstAsync(t => t.Id == Id);

            return new TranslationViewModel
            {
                Id = translation.Id,
                OriginalText = translation.OriginalText,
                TranslationText = translation.TranslationText,
                DateTranslated = translation.DateTranslated,
                TranslatorId = translation.TranslatorId,
                Translator = new TranslatorViewModel
                {
                    Id = translation.Translator.Id,
                    Name = translation.Translator.Name,
                    ApiUri = translation.Translator.ApiUri
                }
            };
        }

        public async Task<int> AddTranslationAsync(TranslationViewModel translationViewModel)
        {
            var translator = await _context.Translators.Where(t => t.Id == translationViewModel.TranslatorId).FirstAsync();
            var translation = new Translation
            {
                OriginalText = translationViewModel.OriginalText,
                TranslationText = translationViewModel.TranslationText,
                DateTranslated = translationViewModel.DateTranslated,
                Translator = translator,
                TranslatorId = translationViewModel.TranslatorId
            };

            _context.Add(translation);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteTranslationAsync(int Id)
        {
            var translation = await _context.Translations.Where(t => t.Id == Id).FirstAsync();

            _context.Remove(translation);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<TranslatorViewModel>> GetTranslatorsAsync()
        {
            var translators = await _context.Translators.ToListAsync();
            var translatorsViewModels = new List<TranslatorViewModel>();

            translators.ForEach(t => translatorsViewModels.Add(new TranslatorViewModel { Id = t.Id, Name = t.Name, ApiUri = t.ApiUri }));

            return translatorsViewModels;
        }

        public async Task<TranslatorViewModel> GetTranslatorAsync(int? Id)
        {
            var translators = await _context.Translators.ToListAsync();

            var translator = translators.First(t => t.Id == Id);

            return new TranslatorViewModel
            {
                Id = translator.Id,
                Name = translator.Name,
                ApiUri = translator.ApiUri,
            };
        }

        public async Task<int> AddTranslatorAsync(TranslatorViewModel translatorViewModel)
        {
            var translator = new Translator
            {
                Name = translatorViewModel.Name,
                ApiUri = translatorViewModel.ApiUri
            };

            _context.Add(translator);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> EditTranslatorAsync(TranslatorViewModel translatorViewModel)
        {
            var translator = await _context.Translators.Where(t => t.Id == translatorViewModel.Id).FirstAsync();

            translator.Name = translatorViewModel.Name;
            translator.ApiUri = translatorViewModel.ApiUri;

            _context.Update(translator);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteTranslatorAsync(int Id)
        {
            var translator = await _context.Translators.Where(t => t.Id == Id).FirstAsync();

            _context.Remove(translator);
            return await _context.SaveChangesAsync();
        }
    }
}
