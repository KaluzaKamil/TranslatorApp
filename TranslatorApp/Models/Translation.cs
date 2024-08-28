using System.ComponentModel.DataAnnotations;

namespace TranslatorApp.Models
{
    public class Translation
    {
        public int Id { get; set; }

        [Required]
        public string OriginalText { get; set; }
        public string TranslationText { get; set; }
        public DateTime DateTranslated { get; set; }
        public Translator Translator { get; set; }
        public int TranslatorId { get; set; }
    }
}
