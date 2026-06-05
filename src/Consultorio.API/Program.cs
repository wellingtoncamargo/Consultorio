using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using FluentValidation;
using Consultorio.Data.Context;
using Consultorio.Data.Repositories;
using Consultorio.Domain.Repositories;
using Consultorio.Services;
using Consultorio.Application.Mappings;
using Consultorio.Application.Services;
using Consultorio.Application.Validators.Paciente;
using Consultorio.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string DefaultConnection not found.");

builder.Services.AddDbContext<ConsultorioDbContext>(options =>
    options.UseSqlite(connectionString),
    ServiceLifetime.Transient);

var jwtSecret = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey not configured");
var jwtExpiration = int.Parse(builder.Configuration["Jwt:ExpirationMinutes"] ?? "15");

builder.Services.AddSingleton<ITokenService>(new TokenService(jwtSecret, jwtExpiration));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = "ConsultorioAPI",
        ValidateAudience = true,
        ValidAudience = "ConsultorioApp",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining(typeof(CreatePacienteValidator));

builder.Services.AddTransient<IPacienteRepository, PacienteRepository>();
builder.Services.AddTransient<IMedicoRepository, MedicoRepository>();
builder.Services.AddTransient<ILocalTrabalhoRepository, LocalTrabalhoRepository>();
builder.Services.AddTransient<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();

// Register SMTP configuration and notification service (NotificacaoService is used by AgendamentoService)
var smtpConfig = builder.Configuration.GetSection("Smtp").Get<Consultorio.Services.ConfiguracaoSmtp>() ?? new Consultorio.Services.ConfiguracaoSmtp();
builder.Services.AddSingleton(smtpConfig);
builder.Services.AddTransient<NotificacaoService>();

builder.Services.AddTransient<PacienteService>();
builder.Services.AddTransient<MedicoService>();
builder.Services.AddTransient<AgendamentoService>();

// Password hasher for user passwords
builder.Services.AddSingleton<IPasswordHasherService, PasswordHasherService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Consultorio Medico API",
        Version = "v1.0",
        Description = "API para gerenciamento de consultorio medico"
    });

    // JWT Bearer auth in Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build(); // CORRIGIDO: era builder.CreateBuilder()

// Enable Swagger UI (available in all environments)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Consultorio Medico API V1");
    // Serve Swagger UI at application root (/)
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ConsultorioDbContext>();
        // Ensure database exists and apply migrations
        try
        {
            context.Database.Migrate();
            Log.Information("Database migrated successfully");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Migration failed or not available, falling back to EnsureCreated");
            context.Database.EnsureCreated();
        }

        // Ensure Usuarios table exists for SQLite (in case DB was created before adding entity)
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn != null && conn.GetType().Name.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                var exists = false;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Usuarios';";
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                    var res = cmd.ExecuteScalar();
                    exists = res != null;
                }

                if (!exists)
                {
                    context.Database.ExecuteSqlRaw(@"CREATE TABLE IF NOT EXISTS Usuarios (
                        Id TEXT PRIMARY KEY,
                        Nome TEXT NOT NULL,
                        Email TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        Role TEXT,
                        Ativo INTEGER,
                        DataCriacao TEXT
                    );");
                    Log.Information("Usuarios table created via SQL.");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Could not verify/create Usuarios table via raw SQL");
        }

        // Seed admin user if none exist
        try
        {
            var usuarioRepo = scope.ServiceProvider.GetService<Consultorio.Domain.Repositories.IUsuarioRepository>();
            var hasher = scope.ServiceProvider.GetService<Consultorio.Application.Services.IPasswordHasherService>();
            if (usuarioRepo != null)
            {
                var any = await usuarioRepo.AnyAsync();
                if (!any)
                {
                    var admin = new Consultorio.Domain.Entities.Usuario
                    {
                        Nome = "Administrator",
                        Email = "admin@local",
                        Role = "Admin",
                        PasswordHash = hasher != null ? hasher.HashPassword("Admin123!") : ""
                    };
                    await usuarioRepo.AddAsync(admin);
                    Log.Information("Admin user seeded: admin@local (senha: Admin123!)");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to seed admin user");
        }

        Log.Information("Database initialized successfully");
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred while initializing the database");
    throw;
}

app.Run();