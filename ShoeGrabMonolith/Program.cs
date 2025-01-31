using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ShoeGrabCommonModels.Contexts;
using ShoeGrabMonolith.Database.Mappers;
using ShoeGrabMonolith.Extensions;
using ShoeGrabOrderManagement.Controllers;
using ShoeGrabProductManagement.Controllers;
using ShoeGrabUserManagement.Controllers;
using ShoeGrabUserManagement.Services;

var builder = WebApplication.CreateBuilder(args);

//Controllers
builder.Services.AddControllers()
    .AddApplicationPart(typeof(UserManagementController).Assembly)
    .AddApplicationPart(typeof(OrderManagementController).Assembly)
    .AddApplicationPart(typeof(ProductManagementController).Assembly);

//Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

//Contexts
builder.Services.AddDbContextPool<UserContext>(opt =>
  opt.UseNpgsql(
    builder.Configuration.GetConnectionString("PostgreSQL"),
    o => o
      .SetPostgresVersion(17, 0)
      .MigrationsAssembly("ShoeGrabMonolith")));

//Services registration
builder.Services.AddTransient<ITokenService, JWTAuthenticationService>();

//Security
builder.Services.AddAuthorization();
builder.AddJWTAuthentication();

// Add AutoMapper with all profiles in the assembly
builder.Services.AddAutoMapper(typeof(OrderMappingProfile).Assembly);
////APP PART////
var app = builder.Build();

//Migrations
app.ApplyMigrations();

//Security
app.UseAuthentication();
app.UseAuthorization();

//Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();