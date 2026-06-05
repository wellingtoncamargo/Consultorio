Prompt Template: Gerar testes unitários

Context:
- Fornecer paths de código, spec (se disponível) e convenções de teste

Template:
"Você é um engenheiro de qualidade. Gere testes unitários para os arquivos: {{targets}}. Siga os padrões do repositório em src/Consultorio.Tests/Consultorio.Tests.Unit. Use Moq para dependências, nomeie testes como {Class}_{Method}_Should_{Behavior} e inclua casos de erro e validações. Produza o conteúdo dos arquivos de teste em formato pronto para commit e liste os novos arquivos." 

Checklist de segurança:
- Não alterar código de produção
- Garantir que testes compilam localmente
- Incluir comentários explicativos nos testes gerados
