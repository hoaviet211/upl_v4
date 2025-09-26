using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UPL.Common;
using UPL.Data;
using UPL.Infrastructure.Repositories;
using UPL.Infrastructure.Services;
using UPL.Infrastructure.Services.Video;
using UPL.Infrastructure.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Kestrel: hide server header
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

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

// Video providers + HttpClients
builder.Services.Configure<YouTubeOptions>(builder.Configuration.GetSection(YouTubeOptions.SectionName));
builder.Services.Configure<TikTokOptions>(builder.Configuration.GetSection(TikTokOptions.SectionName));
builder.Services.AddTransient<SimpleRetryHandler>();
builder.Services.AddHttpClient<IYouTubeVideoProvider, YouTubeVideoProvider>(c =>
{
    c.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<SimpleRetryHandler>();

builder.Services.AddHttpClient<ITikTokVideoProvider, TikTokVideoProvider>(c =>
{
    c.Timeout = TimeSpan.FromSeconds(45);
}).AddHttpMessageHandler<SimpleRetryHandler>();

// Cookie Authentication (uses existing Users table)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Denied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(RoleConstants.StaffAccessPolicy, policy =>
        policy.RequireAssertion(context => StaffRoleHelper.HasStaffAccess(context.User)));
});

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

// Basic security headers
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "DENY";
    headers["Referrer-Policy"] = "no-referrer";
    headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
    headers["Content-Security-Policy"] = string.Join("; ", new[]
    {
        "default-src 'self' blob:",
        "img-src 'self' data:",
        "object-src 'none'",
        "frame-ancestors 'none'",
        "base-uri 'self'",
        // Allow Bootstrap CSS and JS/jQuery from known CDNs
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net",
        "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://code.jquery.com",
        // Optional: fonts if needed by CSS
        "font-src 'self' data:"
    });
    await next();
});

app.UseRouting();

// Serve files from wwwroot (e.g., Vite bundles under /dist)
app.UseStaticFiles();

// AuthN/AuthZ
app.UseAuthentication();
app.UseAuthorization();

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

// Optional: auto apply migrations when requested (e.g., in Docker)
if ((Environment.GetEnvironmentVariable("AUTO_MIGRATE") ?? "false").Equals("true", StringComparison.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<UplDbContext>();
    db.Database.Migrate();
}

app.Run();
