using System.IdentityModel.Tokens.Jwt;
using Application.Features.Features.Account;
using Application.Infrastructure.AutoMapper;
using Application.Interfaces;
using Application.Security;
using Domain.Entities.User;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using SkillsetLab.Infrastructure;
using SkillsetLab.Security;
using JwtTokenProvider = SkillsetLab.Security.JwtTokenProvider;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerDocument(configure =>
{
    configure.DocumentName = "SkillSetLab";

    configure.PostProcess = document =>
    {
        document.Info.Title = "SkillSetLab";
        document.Info.Description = "Online problem book";
    };
    configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the text box: Bearer {your JWT token}."
    });
    configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

services.AddMediatR(typeof(Login).Assembly);

services.Configure<AdminUser>(builder.Configuration.GetSection("AdminUser"));
services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
services.AddTransient<IMyDbContext, MyDbContext>();

services.AddIdentity<User, Role>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.User.RequireUniqueEmail = true;

        //For email confirmations and reset passwords using email token provider that generate 6 digits short lived code.
        options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
        options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    })
    .AddEntityFrameworkStores<MyDbContext>()
    .AddDefaultTokenProviders();

//Add authentication            
services.Configure<TokenManagement>(builder.Configuration.GetSection("TokenManagement"));
var token = builder.Configuration.GetSection("TokenManagement").Get<TokenManagement>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", p =>
    {
        p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        p.RequireRole(Role.Administrator);
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateActor = false,
                ValidateLifetime = true,
                IssuerSigningKey = token.SecurityKey
            };
    });

services.AddTransient<ITokenProvider, JwtTokenProvider>();
services.AddScoped<ICurrentUser, CurrentUser>();
services.AddTransient<CurrentUserMiddleware>();

var app = builder.Build();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Register the Swagger generator and the Swagger UI middlewares
app.UseOpenApi(config =>
{
    config.PostProcess = (document, request) =>
    {
        document.Schemes = new[] { OpenApiSchema.Http, OpenApiSchema.Https };
    };
});
app.UseSwaggerUi();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CurrentUserMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var servicesProvider = scope.ServiceProvider;
    var logger = servicesProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        await DatabaseInitializer.InitializeAsync(servicesProvider);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Applying database migrations error.");
    }
}

app.MapControllers();

app.Run();