using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UPL.Common;
using UPL.Data;
using UPL.Infrastructure.Repositories;
using UPL.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// MVC + Localization
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// DbContext
builder.Services.AddDbContext<UplDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Redis cache (registration only; not used yet)
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration["Redis:Configuration"]);

// DI: Repository & sample service
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IProgrammeService, ProgrammeService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

// Request localization (default vi-VN)
var supportedCultures = new[] { new CultureInfo("vi-VN") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("vi-VN"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Static files
app.MapStaticAssets();

// Routes with Areas then default
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
