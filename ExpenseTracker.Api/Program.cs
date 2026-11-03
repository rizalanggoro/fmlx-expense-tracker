using System.Text;
using dotenv.net;
using ExpenseTracker.Api.Common;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.DTOs.Auth;
using ExpenseTracker.Api.DTOs.Categories;
using ExpenseTracker.Api.DTOs.Transactions;
using ExpenseTracker.Api.DTOs.Wallets;
using ExpenseTracker.Api.Mappings;
using ExpenseTracker.Api.Repositories.Implementations;
using ExpenseTracker.Api.Repositories.Interfaces;
using ExpenseTracker.Api.Services.Implementations;
using ExpenseTracker.Api.Services.Interfaces;
using ExpenseTracker.Api.Validators.Auth;
using ExpenseTracker.Api.Validators.Categories;
using ExpenseTracker.Api.Validators.Transactions;
using ExpenseTracker.Api.Validators.Wallets;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Load environment variables from .env file
DotEnv.Load(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 5));

var builder = WebApplication.CreateBuilder(args);

// 1. Database Connection (PostgreSQL from environment)
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")!;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Microsoft Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 3. JWT Authentication (from environment)
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse("Unauthorized. Token is missing or invalid.");
                await context.Response.WriteAsJsonAsync(response);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse("Forbidden. You do not have permission to access this resource.");
                await context.Response.WriteAsJsonAsync(response);
            }
        };
    });

builder.Services.AddAuthorization();

// 4. Repositories
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// 5. Application Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// 6. AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AuthMappingProfile>();
    cfg.AddProfile<WalletMappingProfile>();
    cfg.AddProfile<CategoryMappingProfile>();
    cfg.AddProfile<TransactionMappingProfile>();
});

// 7. FluentValidation
builder.Services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
builder.Services.AddScoped<IValidator<CreateWalletDto>, CreateWalletValidator>();
builder.Services.AddScoped<IValidator<UpdateWalletDto>, UpdateWalletValidator>();
builder.Services.AddScoped<IValidator<CreateCategoryDto>, CreateCategoryValidator>();
builder.Services.AddScoped<IValidator<UpdateCategoryDto>, UpdateCategoryValidator>();
builder.Services.AddScoped<IValidator<CreateTransactionDto>, CreateTransactionValidator>();
builder.Services.AddScoped<IValidator<UpdateTransactionDto>, UpdateTransactionValidator>();

// 8. Controllers
builder.Services.AddControllers();

// 9. Swagger / OpenAPI with Bearer Authorization
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ExpenseTracker API",
        Version = "v1",
        Description = "Financial Management Web API for Income and Expenses"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token directly (without the 'Bearer ' prefix). Swagger UI will automatically prepend 'Bearer '."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "ExpenseTracker API v1"
            );
        }
    );
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();