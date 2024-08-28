using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TranslatorApp.Contexts;
using TranslatorApp.Models;
using TranslatorApp.Repositories;
using TranslatorApp.ViewModels;

namespace TranslatorApp.Controllers
{
    public class TranslatorsController : Controller
    {
        private readonly ITranslatorRepository _repository;

        public TranslatorsController(ITranslatorRepository repository)
        {
            _repository = repository;
        }

        // GET: Translators
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetTranslatorsAsync());
        }

        // GET: Translators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var translator = await _repository.GetTranslatorAsync(id);
            if (translator == null)
            {
                return NotFound();
            }

            return View(translator);
        }

        // GET: Translators/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Translators/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ApiUri")] TranslatorViewModel translator)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddTranslatorAsync(translator);
                return RedirectToAction(nameof(Index));
            }
            return View(translator);
        }

        // GET: Translators/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var translator = await _repository.GetTranslatorAsync(id);
            if (translator == null)
            {
                return NotFound();
            }
            return View(translator);
        }

        // POST: Translators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ApiUri")] TranslatorViewModel translator)
        {
            if (id != translator.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.EditTranslatorAsync(translator);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TranslatorExistsAsync(translator.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(translator);
        }

        // GET: Translators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var translator = await _repository.GetTranslatorAsync(id);
            if (translator == null)
            {
                return NotFound();
            }

            return View(translator);
        }

        // POST: Translators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var translator = await _repository.GetTranslatorAsync(id);
            if (translator != null)
            {
                await _repository.DeleteTranslatorAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TranslatorExistsAsync(int id)
        {
            var translators = await _repository.GetTranslatorsAsync();
            return translators.Any(e => e.Id == id);
        }
    }
}
