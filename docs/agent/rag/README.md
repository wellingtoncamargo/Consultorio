RAG Guidance (merged)

Resumo:
- Fontes: docs/, src/, IMPLEMENTACAO_USUARIOS.md, MIGRATION_USUARIOS.md
- Ingestão: chunk markdown and code comments, add metadata
- Retrieval: use filename, path, commit id
- Generation: combine top-k relevant chunks, include citations

Práticas:
- Atualizar índice após mudanças
- Human review for generated patches
