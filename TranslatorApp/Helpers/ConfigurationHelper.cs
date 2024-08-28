namespace TranslatorApp.Helpers
{
    public static class ConfigurationHelper
    {
        public static IConfiguration config;
        public static void Initialize(IConfiguration configuration)
        {
            config = configuration;
        }
    }
}
