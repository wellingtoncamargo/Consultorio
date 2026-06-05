# Migração: Adicionar Tabela de Usuários (AddUsuarioTable)

## Resumo
Esta migração adiciona persistência de usuários/autenticação ao sistema, permitindo gerenciar roles (Admin, Recepcao, etc.) e autenticar usuários com senha segura (PBKDF2).

## Arquivos Alterados

### 1. **Nova Entidade: `Usuario`**
- **Caminho**: `src/Consultorio.Domain/Entities/Usuario.cs`
- **Descrição**: Entidade que representa um usuário no sistema.
- **Campos**:
  - `Id` (Guid): Chave primária
  - `Nome` (string): Nome completo do usuário
  - `Email` (string): Email único, obrigatório
  - `PasswordHash` (string): Hash da senha (PBKDF2)
  - `Role` (string): Perfil (Admin, Recepcao, Medico, etc.)
  - `Ativo` (bool): Flag de ativação (default: true)
  - `DataCriacao` (DateTime): Timestamp de criação

### 2. **Novo Repositório: `IUsuarioRepository`**
- **Caminho**: `src/Consultorio.Domain/Interfaces/IUsuarioRepository.cs`
- **Interface**:
  - `GetByEmailAsync(string email)`: Busca usuário por email
  - `AnyAsync()`: Verifica se existe algum usuário (usado para seed)

- **Implementação**: `UsuarioRepository` em `src/Consultorio.Data/Repositories/Repositories.cs`

### 3. **Novo Serviço: `PasswordHasherService`**
- **Caminho**: `src/Consultorio.Application/Services/PasswordHasherService.cs`
- **Descrição**: Fornece hash e verificação de senhas com PBKDF2 (10.000 iterações, SHA-256)
- **Métodos**:
  - `HashPassword(string password)`: Retorna `salt.base64 + "." + hash.base64`
  - `Verify(string hashed, string password)`: Verifica se a senha corresponde ao hash

### 4. **Alterações no DbContext**
- **Arquivo**: `src/Consultorio.Data/Context/ConsultorioDbContext.cs`
- **Mudanças**:
  - Adicionado `public DbSet<Usuario> Usuarios { get; set; }`
  - Configuração de modelagem para Usuario (chave primária, índice único em Email, comprimento máximo de campos)

### 5. **Alterações no AuthController**
- **Arquivo**: `src/Consultorio.API/Controllers/AuthController.cs`
- **Mudanças**:
  - **Login**: Agora verifica email/senha contra a tabela Usuarios
  - **Register**: Protegido por role Admin; persiste novo usuário com senha criptografada
  - Adicionadas dependências: `IUsuarioRepository`, `IPasswordHasherService`

### 6. **Alterações no Program.cs**
- **Arquivo**: `src/Consultorio.API/Program.cs`
- **Mudanças**:
  - Registrado `IUsuarioRepository` e `UsuarioRepository` no DI
  - Registrado `IPasswordHasherService` e `PasswordHasherService` como Singleton
  - Modificado startup: cria tabela Usuarios se não existir e semeia usuário Admin

## Schema da Tabela Usuarios

```sql
CREATE TABLE Usuarios (
    Id TEXT PRIMARY KEY,
    Nome TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Role TEXT,
    Ativo INTEGER,
    DataCriacao TEXT
);
```

## Dados de Seed

Ao iniciar a API, um usuário Admin é automaticamente criado se nenhum usuário existir:
- **Email**: `admin@local`
- **Senha**: `Admin123!`
- **Role**: `Admin`

## Como Aplicar

### Opção 1: Automático (recomendado)
Ao executar `dotnet run --project src/Consultorio.API`, a tabela Usuarios será criada automaticamente no startup se não existir.

### Opção 2: Migração EF (futuro)
Quando o dotnet-ef estiver totalmente integrado:
```bash
dotnet-ef database update -p src/Consultorio.Data -s src/Consultorio.API
```

## Fluxo de Autenticação

1. **Admin Seed**: Na inicialização, um Admin é criado (se não existir usuário algum)
2. **Admin Login**: Admin faz login com `admin@local` / `Admin123!`
3. **Register**: Admin usa POST `/api/auth/register` para criar novos usuários com roles específicas
4. **User Login**: Novos usuários fazem login com POST `/api/auth/login`
5. **Token**: Cada login gera JWT com claims (Id, Email, Role)
6. **Authorization**: Controllers usam `[Authorize(Roles = "Admin,Recepcao")]` para proteger endpoints

## Roles Suportadas

- **Admin**: Acesso total (cria/deleta usuários, todos os CRUDs)
- **Recepcao**: Cria/edita pacientes, medicos, agendamentos
- **Medico**: Pode visualizar agendamentos e históricos (pode ser refinado)

## Próximos Passos

1. Registrar novos usuários via POST `/api/auth/register` com role desejada
2. Refinar permissões por controller (ex: MedicoController, PacienteController)
3. Implementar refresh token rotation (opcional)
4. Adicionar forgot password / reset (opcional)

