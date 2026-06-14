using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Luminaria.API.Data;
using DotNetEnv;
using Luminaria.API.Interfaces;
using Luminaria.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Luminaria.API.Models;


var builder = WebApplication.CreateBuilder(args);
Env.Load();
// conexion a sql server
var connectionString = $"Server={Env.GetString("DB_HOST")},{Env.GetString("DB_PORT")};" +
                       $"Database={Env.GetString("DB_NAME")};" +
                       $"User Id={Env.GetString("DB_USER")};" +
                       $"Password={Env.GetString("DB_PASSWORD")};" +
                       $"TrustServerCertificate=True;";

builder.Services.AddDbContext<LuminariaContext>(options =>
    options.UseSqlServer(connectionString));

    builder.Services.AddCors(options =>
{
    options.AddPolicy("LuminariaPolicy", policy =>
    {
                policy.WithOrigins(
                    "http://localhost:4321",         // Astro dev
                    "http://localhost:5173",         
                    "https://luminarias.nodesv.com",
                    "https://lumiadmin.nodesv.com"
                    
                )
              .AllowAnyHeader()
              .AllowCredentials(); 
    });
});

// 4. Configuración de Autenticación con JWT vía Cookies
var jwtKey = Env.GetString("JWT_KEY");
var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Env.GetString("JWT_ISSUER"),
        ValidAudience = Env.GetString("JWT_AUDIENCE"),
        IssuerSigningKey = securityKey
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["X-Access-Token"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddSignalR();

// 5. Inyección de Dependencias (Services)
builder.Services.AddScoped<IAuthService, AuthService>();  

// 6. Swagger con soporte de cookie HttpOnly
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Luminaria API", Version = "v1" });

    c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Cookie,
        Name = "X-Access-Token",
        Description = "Primero ejecuta /api/auth/login, la cookie se guarda automáticamente"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "cookieAuth"
                }
            },
            Array.Empty<string>()
        }
    });
     c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Description = "ApiKey del agente (viene de la tabla servers)"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();

// --- Middleware Pipeline ---

// 7. Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("LuminariaPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LuminariaContext>();
    if(!context.Usuarios.Any())
    {
        var adminUser = new Usuarios
        {
            NombreUsuario = "admin",
            ContraseñaHash = BCrypt.Net.BCrypt.HashPassword(Env.GetString("PASSWORD_ADMIN")),
            RolID = 1, 
            FechaCreacion = DateTime.UtcNow
        };
        context.Usuarios.Add(adminUser);
        context.SaveChanges();
    }
}

app.Run();