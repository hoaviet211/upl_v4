using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace UPL.Common;

public static class LocalizationExtensions
{
    public static IServiceCollection AddVietnameseLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.AddControllersWithViews()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();
        return services;
    }

    public static IApplicationBuilder UseVietnameseLocalization(this IApplicationBuilder app)
    {
        var supportedCultures = new[] { new CultureInfo("vi-VN") };
        var options = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("vi-VN"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        };
        return app.UseRequestLocalization(options);
    }
}

