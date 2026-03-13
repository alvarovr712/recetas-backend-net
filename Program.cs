using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RecetasAPINet.Data;
using RecetasAPINet.Models;
using RecetasAPINet.Services;
using RecetasAPINet.Security;
using RecetasAPINet.Extensions;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Base de datos (DbContext + enums + snake_case)
builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddHttpContextAccessor();
// Servicios de la aplicación (UserService, AuthService, JwtService, PasswordHasher)
builder.Services.AddApplicationServices();

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Extensiones de infraestructura
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsForAngular();
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Probar conexión a la base de datos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RecetasDbContext>();
    if (db.Database.CanConnect())
        Console.WriteLine("✔ Conexión a la base de datos OK");
    else
        Console.WriteLine("❌ No se pudo conectar a la base de datos");
}

// Pipeline HTTP
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerDocumentation(app.Environment);
app.UseStaticFiles();
app.MapControllers();

app.Run();
