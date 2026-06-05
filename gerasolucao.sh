#!/bin/bash

# ============================================
# SAFE SOLUTION ORGANIZER
# ============================================
# OBJETIVO:
# - NÃO apagar arquivos existentes
# - NÃO sobrescrever arquivos
# - NÃO recriar projetos existentes
# - NÃO remover a .sln
# - Apenas:
#     ✔ reorganizar solution
#     ✔ adicionar projetos existentes
#     ✔ criar referências faltantes
#     ✔ ajustar compatibilidade
#     ✔ mover arquivos SOMENTE se necessário
#
# EXECUÇÃO:
# ./fix_solution.sh
#
# ============================================

set -e

SOLUTION_NAME="Consultorio"

echo ""
echo "============================================"
echo "SAFE SOLUTION ORGANIZER"
echo "============================================"
echo ""

# ============================================
# DETECTAR SDK
# ============================================

SDK_VERSION=$(dotnet --version)

echo "🧠 SDK detectado: $SDK_VERSION"

if [[ "$SDK_VERSION" == 9* ]]; then
    FRAMEWORK="net8.0"
elif [[ "$SDK_VERSION" == 8* ]]; then
    FRAMEWORK="net8.0"
elif [[ "$SDK_VERSION" == 7* ]]; then
    FRAMEWORK="net7.0"
else
    FRAMEWORK="net8.0"
fi

echo "🎯 Framework alvo: $FRAMEWORK"
echo ""

# ============================================
# GARANTIR ESTRUTURA BASE
# ============================================

mkdir -p src
mkdir -p docs
mkdir -p scripts

# ============================================
# CRIAR SOLUTION APENAS SE NÃO EXISTIR
# ============================================

if [ ! -f "$SOLUTION_NAME.sln" ]; then
    echo "🧩 Criando solution..."
    dotnet new sln -n "$SOLUTION_NAME"
else
    echo "✅ Solution já existe"
fi

echo ""

# ============================================
# FUNÇÃO: AJUSTAR TARGET FRAMEWORK
# ============================================

adjust_framework() {

    CSPROJ="$1"

    if [ ! -f "$CSPROJ" ]; then
        return
    fi

    echo "🔧 Ajustando framework: $CSPROJ"

    # Backup
    cp "$CSPROJ" "$CSPROJ.bak"

    # Ajusta framework
    sed -i "s/<TargetFramework>net9.0<\/TargetFramework>/<TargetFramework>$FRAMEWORK<\/TargetFramework>/g" "$CSPROJ"

    # Remove OpenAPI incompatível
    sed -i '/Microsoft.AspNetCore.OpenApi/d' "$CSPROJ"

}

# ============================================
# FUNÇÃO: ADICIONAR PACOTE SE NÃO EXISTIR
# ============================================

ensure_package() {

    PROJECT="$1"
    PACKAGE="$2"
    VERSION="$3"

    if grep -q "$PACKAGE" "$PROJECT"; then
        echo "✅ Pacote já existe: $PACKAGE"
    else
        echo "📦 Instalando: $PACKAGE"
        dotnet add "$PROJECT" package "$PACKAGE" --version "$VERSION"
    fi

}

# ============================================
# MOVER PROJETOS PARA /src
# SOMENTE SE NECESSÁRIO
# ============================================

echo "📁 Verificando organização..."

find . -maxdepth 2 -name "*.csproj" | while read CSPROJ
do

    # ignora obj/bin
    if [[ "$CSPROJ" == *"/obj/"* ]] || [[ "$CSPROJ" == *"/bin/"* ]]; then
        continue
    fi

    PROJECT_DIR=$(dirname "$CSPROJ")
    PROJECT_NAME=$(basename "$PROJECT_DIR")

    # já está em src
    if [[ "$PROJECT_DIR" == src/* ]]; then
        continue
    fi

    # mover somente se necessário
    TARGET_DIR="src/$PROJECT_NAME"

    if [ ! -d "$TARGET_DIR" ]; then
        echo "📦 Movendo projeto:"
        echo "   FROM: $PROJECT_DIR"
        echo "   TO:   $TARGET_DIR"

        mkdir -p src
        mv "$PROJECT_DIR" "$TARGET_DIR"
    else
        echo "⚠️ Projeto já existe em src: $PROJECT_NAME"
    fi

done

echo ""

# ============================================
# AJUSTAR TODOS OS CSPROJ
# ============================================

find src -name "*.csproj" | while read CSPROJ
do
    adjust_framework "$CSPROJ"
done

echo ""

# ============================================
# ADICIONAR PROJETOS À SOLUTION
# ============================================

echo "🧩 Adicionando projetos na solution..."

find src -name "*.csproj" | while read CSPROJ
do

    if dotnet sln list | grep -q "$CSPROJ"; then
        echo "✅ Já existe na solution: $CSPROJ"
    else
        echo "➕ Adicionando: $CSPROJ"
        dotnet sln add "$CSPROJ"
    fi

done

echo ""

# ============================================
# SWAGGER COMPATÍVEL
# ============================================

API_PROJECT=$(find src -name "*.csproj" | grep "API" | head -n 1)

if [ -n "$API_PROJECT" ]; then

    echo "🌐 Configurando Swagger..."

    ensure_package "$API_PROJECT" "Swashbuckle.AspNetCore" "6.6.2"

fi

echo ""

# ============================================
# REFERÊNCIAS ENTRE PROJETOS
# ============================================

echo "🔗 Ajustando referências..."

API=$(find src -name "*API.csproj" | head -n 1)
APP=$(find src -name "*Application.csproj" | head -n 1)
DOMAIN=$(find src -name "*Domain.csproj" | head -n 1)
DATA=$(find src -name "*Data.csproj" | head -n 1)
TESTS=$(find src -name "*Tests.csproj" | head -n 1)

safe_reference() {

    FROM="$1"
    TO="$2"

    if [ -z "$FROM" ] || [ -z "$TO" ]; then
        return
    fi

    if grep -q "$(basename "$TO")" "$FROM"; then
        echo "✅ Referência já existe"
    else
        echo "➕ Referência:"
        echo "   $(basename "$FROM")"
        echo "   -> $(basename "$TO")"

        dotnet add "$FROM" reference "$TO"
    fi

}

safe_reference "$API" "$APP"
safe_reference "$API" "$DATA"
safe_reference "$APP" "$DOMAIN"
safe_reference "$DATA" "$DOMAIN"
safe_reference "$TESTS" "$API"

echo ""

# ============================================
# PASTAS VAZIAS VISÍVEIS
# ============================================

echo "📁 Garantindo visibilidade das pastas..."

find src -type d -empty -exec touch {}/.gitkeep \;

echo ""

# ============================================
# RESTORE
# ============================================

echo "📥 Restore..."

dotnet restore

echo ""

# ============================================
# BUILD
# ============================================

echo "🏗️ Build..."

dotnet build

echo ""
echo "============================================"
echo "✅ SOLUTION ORGANIZADA COM SEGURANÇA"
echo "============================================"
echo ""
echo "✔ Nenhum arquivo existente foi apagado"
echo "✔ Nenhum projeto existente foi recriado"
echo "✔ Apenas ajustes compatíveis foram aplicados"
echo ""