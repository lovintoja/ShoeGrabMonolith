using Microsoft.EntityFrameworkCore;
using ShoeGrabMonolith.Database.Contexts;
using ShoeGrabMonolith.Extensions;
using ShoeGrabMonolith.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<UserContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSQL"),
        o => o
            .SetPostgresVersion(17, 0)));
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

var app = builder.Build();

app.ApplyMigrations();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();