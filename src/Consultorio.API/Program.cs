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

// Register SMTP configuration and notification service (NotificacaoService is used by AgendamentoService)
var smtpConfig = builder.Configuration.GetSection("Smtp").Get<Consultorio.Services.ConfiguracaoSmtp>() ?? new Consultorio.Services.ConfiguracaoSmtp();
builder.Services.AddSingleton(smtpConfig);
builder.Services.AddTransient<NotificacaoService>();

builder.Services.AddTransient<PacienteService>();
builder.Services.AddTransient<MedicoService>();
builder.Services.AddTransient<AgendamentoService>();

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
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build(); // CORRIGIDO: era builder.CreateBuilder()

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
        context.Database.EnsureCreated();
        Log.Information("Database initialized successfully");
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred while initializing the database");
    throw;
}

app.Run();