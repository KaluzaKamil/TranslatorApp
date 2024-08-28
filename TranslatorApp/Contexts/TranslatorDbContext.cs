using TranslatorApp.Models;
using Microsoft.EntityFrameworkCore;
using TranslatorApp.ViewModels;

namespace TranslatorApp.Contexts
{
    public class TranslatorDbContext : DbContext
    {
        public TranslatorDbContext(DbContextOptions<TranslatorDbContext> options) : base(options) { }

        public DbSet<Translation> Translations { get; set; }
        public DbSet<Translator> Translators { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Translation>()
                .HasOne(tn => tn.Translator)
                .WithMany(tr => tr.Translations)
                .HasForeignKey(tn => tn.TranslatorId)
                .IsRequired();
        }
        public DbSet<TranslatorApp.ViewModels.TranslatorViewModel> TranslatorViewModel { get; set; } = default!;
    }
}
