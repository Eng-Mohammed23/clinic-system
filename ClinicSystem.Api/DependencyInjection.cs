using ClinicSystem.Api.Persistence;
using ClinicSystem.Api.Services;
using DinkToPdf.Contracts;
using DinkToPdf;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Api.Authentications;
using System.Reflection;
using System.Text;
using Hangfire;
using SurveyBasket.Settings;
using System.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasket.Services;

namespace ClinicSystem.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidCastException("Connection string 'DefaultConnection' not found!");

        services.AddAuthConfig(configuration);

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });

        services.AddDbContext<ApplicationDbContext>(options
            => options.UseSqlServer(connectionString));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IEmailSender, EmailService>();

        services.AddSingleton<IConverter, SynchronizedConverter>(provider => new SynchronizedConverter(new PdfTools()));

        //Email
        services.AddHttpContextAccessor();
        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        services.AddControllers();
        services
            .AddFluentValidationConfig()
            .AddOpenApi();

        services.AddBackgroundJobsConfig(configuration);

        //services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        //services.AddScoped<PdfService>();

        return services;
    }

    private static IServiceCollection AddAuthConfig(this IServiceCollection services,
            IConfiguration configuration)
    {


        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        //services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        //services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        var jwtSetting = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting?.Key!)),
                ValidIssuer = jwtSetting?.Issuer,
                ValidAudience = jwtSetting?.Audience

            };
        });

        services.Configure<IdentityOptions>(options =>
        {
            // Default User settings.

            options.SignIn.RequireConfirmedEmail = true;
            options.Password.RequiredLength = 6;

            options.User.RequireUniqueEmail = false;

        });

        return services;
    }
    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        //Add Mapster
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(new Mapper(mappingConfig));

        return services;
    }

    private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services,
            IConfiguration configuration)
    {


        // Add Hangfire services.
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        // Add the processing server as IHostedService
        services.AddHangfireServer();

        return services;
    }
    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        //Add FluentValidation
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
