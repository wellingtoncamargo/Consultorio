#!/bin/bash

echo "🚀 Criando estrutura do projeto Consultorio..."

# Root
mkdir -p Consultorio/src

# =========================
# API
# =========================
PROJECT_ROOT="$(pwd)"
API_PATH="$PROJECT_ROOT/src/Consultorio.API"

mkdir -p $API_PATH/{Controllers,Services,Repositories,Models,DTOs}

# Controllers
touch $API_PATH/Controllers/PacientesController.cs
touch $API_PATH/Controllers/MedicosController.cs
touch $API_PATH/Controllers/AgendamentosController.cs
touch $API_PATH/Controllers/HistoricoController.cs

# Arquivos base
touch $API_PATH/appsettings.json
touch $API_PATH/Program.cs
touch $API_PATH/Consultorio.API.csproj

# =========================
# MAUI
# =========================
MAUI_PATH="$PROJECT_ROOT/src/Consultorio.MAUI"

mkdir -p $MAUI_PATH/Resources/{Images,Fonts,Styles}

# Arquivos base MAUI
touch $MAUI_PATH/App.xaml
touch $MAUI_PATH/App.xaml.cs
touch $MAUI_PATH/MainPage.xaml
touch $MAUI_PATH/MainPage.xaml.cs
touch $MAUI_PATH/Consultorio.MAUI.csproj

# =========================
# Conteúdo inicial (boilerplate)
# =========================

echo "📄 Adicionando conteúdo inicial..."

# Program.cs
cat <<EOL > $API_PATH/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
EOL

# appsettings.json
cat <<EOL > $API_PATH/appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
EOL

# Exemplo Controller
cat <<EOL > $API_PATH/Controllers/PacientesController.cs
using Microsoft.AspNetCore.Mvc;

namespace Consultorio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Lista de pacientes");
        }
    }
}
EOL

# csproj API
cat <<EOL > $API_PATH/Consultorio.API.csproj
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
EOL

# csproj MAUI
cat <<EOL > $MAUI_PATH/Consultorio.MAUI.csproj
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
EOL

echo "✅ Estrutura criada com sucesso!"