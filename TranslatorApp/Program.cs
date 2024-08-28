using TranslatorApp.Contexts;
using TranslatorApp.Extensions;
using Microsoft.EntityFrameworkCore;
using TranslatorApp.Helpers;
using Microsoft.AspNetCore.DataProtection;
using TranslatorApp.Repositories;
using TranslatorApp.ApiCaller;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TranslatorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TranslatorDbContext")));

builder.Services.AddDataProtection()
                .SetApplicationName("translation-program")
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"\\server\share\directory\"));

builder.Services.AddScoped<ITranslatorRepository, TranslatorRepository>();
builder.Services.AddScoped<ITranslatorApiCaller, TranslatorApiCaller>();

ConfigurationHelper.Initialize(builder.Configuration);
var app = builder.Build();

await app.MigrateDatabase().AddBaseTranslatorsAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
