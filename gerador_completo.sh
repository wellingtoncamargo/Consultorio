#!/bin/bash

set -e

ESTRUTURA="$1"
CONTEUDO="$2"

PROJECT_ROOT="$(pwd)"

if [ -z "$ESTRUTURA" ] || [ -z "$CONTEUDO" ]; then
  echo "Uso: ./gerador_completo.sh estrutura.txt conteudo.md"
  exit 1
fi

if [ ! -f "$ESTRUTURA" ]; then
  echo "❌ Estrutura não encontrada: $ESTRUTURA"
  exit 1
fi

if [ ! -f "$CONTEUDO" ]; then
  echo "❌ Conteúdo não encontrado: $CONTEUDO"
  exit 1
fi

echo "🚀 Criando estrutura..."

declare -a stack

# =========================
# 1. CRIAR ESTRUTURA
# =========================
while IFS= read -r line || [ -n "$line" ]; do

  [[ -z "$line" ]] && continue

  indent=$(echo "$line" | sed -E 's/^((│   |    )*).*/\1/' | sed 's/│   /    /g' | wc -c)
  level=$((indent / 4))

  name=$(echo "$line" | sed -E 's/^[│ ]*[├└]── //' | xargs)

  stack=("${stack[@]:0:$level}")
  stack[$level]="$name"

  path=""
  for ((i=0; i<=level; i++)); do
    if [ $i -eq 0 ]; then
      path="${stack[$i]}"
    else
      path="$path/${stack[$i]}"
    fi
  done

  if [[ "$name" == */ ]]; then
    mkdir -p "$path"
  else
    mkdir -p "$(dirname "$path")"
    touch "$path"
  fi

done < "$ESTRUTURA"

echo "📁 Estrutura criada!"

# =========================
# 2. APLICAR CONTEÚDO (.md)
# =========================

echo "✍️ Aplicando conteúdo..."

current_file=""
buffer=""
writing=false
skip_next=false

while IFS= read -r line || [ -n "$line" ]; do

  # Detecta filepath
  if [[ "$line" == *"// filepath:"* ]]; then
    echo "📌 Obtendo Caminho antes de remover o filepath: $line"

    current_file=$(echo "$line" | sed 's|// filepath:||')
    current_file=$(echo "$current_file" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')
    echo "📌 Obtendo Caminho antes da Normalização: $current_file"
    # Normaliza caminho
    current_file=$(echo "$current_file" | sed 's#\\#/#g')
    echo "📌 Validando SED: $current_file"

    # Torna absoluto
    current_file="$PROJECT_ROOT/$current_file"

    echo "📌 Arquivo alvo: $current_file"
    continue
  fi

  # Início/fim de bloco ```
  if [[ "$line" == \`\`\`* ]]; then
    if [ "$writing" = false ]; then
      writing=true
      buffer=""
      skip_next=true
    else
      # Fim do bloco → grava
      if [ -n "$current_file" ]; then
        mkdir -p "$(dirname "$current_file")"
        echo "$buffer" > "$current_file"
        echo "📄 Gravado: $current_file"
      fi
      writing=false
      current_file=""
    fi
    continue
  fi

  # Acumula conteúdo
  if [ "$writing" = true ]; then
    if [ "$skip_next" = true ]; then
      skip_next=false
      continue
    fi
    buffer+="$line"$'\n'
  fi

done < "$CONTEUDO"

echo "✅ Projeto completo gerado!"