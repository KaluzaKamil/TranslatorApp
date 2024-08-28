using TranslatorApp.Contexts;
using Microsoft.EntityFrameworkCore;
using TranslatorApp.Helpers;
using TranslatorApp.Models;
using Microsoft.Extensions.DependencyInjection;

namespace TranslatorApp.Extensions
{
    public static class WebApplicationExtensions
    {

        public static WebApplication MigrateDatabase(this WebApplication webApplication)
        {
            using (var serviceScope = webApplication.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var db = serviceScope.ServiceProvider.GetRequiredService<TranslatorDbContext>().Database;

                logger.LogInformation("Database migration start");
                while (!db.CanConnect())
                {
                    logger.LogInformation("Connecting...");
                    Thread.Sleep(1000);
                }

                try
                {
                    serviceScope.ServiceProvider.GetRequiredService<TranslatorDbContext>().Database.Migrate();
                    logger.LogInformation("Database migrated");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while migrating database");
                }
            }
            return webApplication;
        }

        public static async Task<WebApplication> AddBaseTranslatorsAsync(this WebApplication webApplication)
        {
            var translators = ConfigurationHelper.config.GetSection("Translators").Get<List<Translator>>();

            using (var scope = webApplication.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<TranslatorDbContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("Adding Translators");

                await context.Translators.AddRangeAsync(translators);
                await context.SaveChangesAsync();

                logger.LogInformation("Translators added");
            }

            return webApplication;
        }
    }
}
