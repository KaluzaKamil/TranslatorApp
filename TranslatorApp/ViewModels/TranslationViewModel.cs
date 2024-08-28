using System.ComponentModel.DataAnnotations;

namespace TranslatorApp.ViewModels
{
    public class TranslationViewModel
    {
        public int Id { get; set; }

        [Required]
        public string OriginalText { get; set; }
        public string TranslationText { get; set; }
        public DateTime DateTranslated { get; set; }
        public TranslatorViewModel Translator { get; set; }
        public int TranslatorId { get; set; }
    }
}
