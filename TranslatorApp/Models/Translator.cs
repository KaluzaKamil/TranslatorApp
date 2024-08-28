namespace TranslatorApp.Models
{
    public class Translator
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ApiUri { get; set; }
        public List<Translation> Translations { get; set; }
    }
}
