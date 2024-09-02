using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TranslatorApp.ApiCaller;
using TranslatorApp.Contexts;
using TranslatorApp.Models;
using TranslatorApp.Repositories;
using TranslatorApp.ViewModels;

namespace TranslatorApp.Controllers
{
    public class TranslationsController : Controller
    {
        private readonly ITranslatorRepository _repository;
        private readonly ILogger<TranslationsController> _logger;
        private readonly ITranslatorApiCaller _apiCaller;

        public TranslationsController(
            ITranslatorRepository repository, 
            ILogger<TranslationsController> logger,
            ITranslatorApiCaller apiCaller)
        {
            _repository = repository;
            _logger = logger;
            _apiCaller = apiCaller;
        }

        // GET: Translations
        public async Task<IActionResult> Index()
        {
            var translations = await _repository.GetTranslationsAsync();

            return View(translations);
        }

        // GET: Translations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var translation = await _repository.GetTranslationAsync(id);
            if (translation == null)
            {
                return NotFound();
            }

            return View(translation);
        }

        // GET: Translations/Translate
        [HttpGet]
        public async Task<IActionResult> Translate()
        {
            var translators = await _repository.GetTranslatorsAsync(); 

            ViewBag.Translators = new SelectList(translators, "Id", "Name");
            return View();
        }

        // POST: Translations/Translate
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Translate(TranslationViewModel translationViewModel)
        {
            translationViewModel.DateTranslated = DateTime.Now;

            var translator = await _repository.GetTranslatorAsync(translationViewModel.TranslatorId);

            translationViewModel.TranslationText = await _apiCaller.GetTranslationAsync(translationViewModel.OriginalText, 
                                                                                        translator.ApiUri, 
                                                                                        translator.ApiUriParameters);
            

            translationViewModel.Translator = new TranslatorViewModel 
            {
                Id = translator.Id, 
                Name = translator.Name, 
                ApiUri = translator.ApiUri,
                ApiUriParameters = translator.ApiUriParameters,
            };

            ModelState.Clear();
            TryValidateModel(translationViewModel);

            if (ModelState.IsValid)
            {         
                var result = await _repository.AddTranslationAsync(translationViewModel);

                if(result >  0) 
                    return Content(translationViewModel.TranslationText);

                return Content("Something went wrong");
            }

            var translators = await _repository.GetTranslatorsAsync();

            ViewBag.Translators = new SelectList(translators, "Id", "Name");
            return Content("Something went wrong");
        }

        // GET: Translations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var translation = await _repository.GetTranslationAsync(id);
            if (translation == null)
            {
                return NotFound();
            }

            return View(translation);
        }

        // POST: Translations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var translation = await _repository.GetTranslationAsync(id);
            if (translation != null)
            {
                await _repository.DeleteTranslationAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
