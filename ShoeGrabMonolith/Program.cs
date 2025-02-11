using Microsoft.EntityFrameworkCore;
using ShoeGrabAdminService.Controllers;
using ShoeGrabCommonModels.Contexts;
using ShoeGrabCRMService.Services;
using ShoeGrabMonolith.Database.Mappers;
using ShoeGrabMonolith.Extensions;
using ShoeGrabOrderManagement.Controllers;
using ShoeGrabOrderManagement.Services;
using ShoeGrabProductManagement;
using ShoeGrabProductManagement.Controllers;
using ShoeGrabProductManagement.Services;
using ShoeGrabUserManagement.Controllers;
using ShoeGrabUserManagement.Services;

var builder = WebApplication.CreateBuilder(args);

//Controllers
builder.Services.AddControllers()
    .AddApplicationPart(typeof(UserController).Assembly)
    .AddApplicationPart(typeof(OrderController).Assembly)
    .AddApplicationPart(typeof(BasketController).Assembly)
    .AddApplicationPart(typeof(ProductController).Assembly)
    .AddApplicationPart(typeof(ProductManagementController).Assembly)
    .AddApplicationPart(typeof(OrderManagementController).Assembly);

builder.SetupKestrel();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAutoMapper(typeof(GlobalMappingProfile));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

//Contexts
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContextPool<UserContext>(opt =>
        opt.UseNpgsql(
            builder.Configuration.GetConnectionString("DB_CONNECTION_STRING"),
            o => o
                .SetPostgresVersion(17, 0)
                .MigrationsAssembly("ShoeGrabMonolith")));
}
else
{
    builder.Services.AddDbContextPool<UserContext>(opt =>
        opt.UseNpgsql(
            Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"),
            o => o
                .SetPostgresVersion(17, 0)
                .MigrationsAssembly("ShoeGrabMonolith")));

        
}

//Services registration
builder.Services.AddTransient<ITokenService, JWTAuthenticationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordManagement, PasswordManagement>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IOrderService, OrderService>();

//Security
builder.Services.AddAuthorization();
builder.AddJWTAuthenticationAndAuthorization();

////APP PART////
var app = builder.Build();

//Migrations
app.ApplyMigrations();

app.UseCors("AllowAllOrigins");

//Security
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();