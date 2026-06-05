# Checklist de Implementação - Persistência de Usuários e Autenticação

## ✅ Alterações Implementadas

### 1. Entidades
- [x] Criada entidade `Usuario` (src/Consultorio.Domain/Entities/Usuario.cs)
  - Campo: Id, Nome, Email, PasswordHash, Role, Ativo, DataCriacao

### 2. Repositórios
- [x] Criada interface `IUsuarioRepository` (src/Consultorio.Domain/Interfaces/IUsuarioRepository.cs)
- [x] Implementada classe `UsuarioRepository` em Repositories.cs
  - Métodos: GetByEmailAsync, AnyAsync

### 3. Serviços
- [x] Criada classe `PasswordHasherService` (src/Consultorio.Application/Services/PasswordHasherService.cs)
  - Implementa PBKDF2 (10.000 iterações, SHA-256)
  - Métodos: HashPassword, Verify

### 4. Controllers
- [x] Atualizado `AuthController` (src/Consultorio.API/Controllers/AuthController.cs)
  - Login: Valida email/senha contra BD
  - Register: Protegido por role Admin, persiste novo usuário

### 5. DbContext
- [x] Adicionado `DbSet<Usuario>` ao `ConsultorioDbContext`
- [x] Configurada entidade Usuario na modelagem

### 6. DI (Program.cs)
- [x] Registrados serviços no container:
  - `IUsuarioRepository` → `UsuarioRepository`
  - `IPasswordHasherService` → `PasswordHasherService`

### 7. Startup
- [x] Criada tabela Usuarios automaticamente se não existir
- [x] Seed: Admin user (`admin@local` / `Admin123!`)

### 8. Documentação
- [x] Criado arquivo `MIGRATION_USUARIOS.md` com detalhes completos

## 🔄 Controllers com Autorização

| Controller       | GET   | POST  | PUT   | DELETE | Protegidos por |
|------------------|-------|-------|-------|--------|---|
| Paciente         | ✓     | ✓     | ✓     | ✓      | [Authorize] |
| Medico           | ✓     | ✓     | ✓     | ✓      | [Authorize] |
| LocalTrabalho    | ✓     | ✓     | ✓     | ✓      | [Authorize] |
| Agendamento      | ✓     | ✓     | ✓     | ✓      | [Authorize] |
| Historico        | ✓     | ✓     | ✓     | ✓      | [Authorize] |
| Auth             | -     | ✓ Login<br>✓ Register | - | - | Login: Anônimo<br>Register: Admin |

## 🗂️ Banco de Dados

### Tabela: Usuarios (SQLite)
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

## 🔐 Fluxo de Uso

### 1. Inicializar Sistema
```bash
dotnet run --project src/Consultorio.API
```
- Tabela `Usuarios` será criada se não existir
- Admin user será criado se nenhum usuário existir
  - Email: `admin@local`
  - Senha: `Admin123!`

### 2. Admin Login
```bash
POST /api/auth/login
{
  "email": "admin@local",
  "senha": "Admin123!"
}
```
Resposta inclui AccessToken JWT

### 3. Register Novo Usuário (como Admin)
```bash
POST /api/auth/register
Authorization: Bearer {admin_token}
{
  "nome": "João Recepcao",
  "email": "joao@clinic.com",
  "senha": "Senha123!",
  "confirmSenha": "Senha123!",
  "role": "Recepcao"
}
```

### 4. Novo Usuário Login
```bash
POST /api/auth/login
{
  "email": "joao@clinic.com",
  "senha": "Senha123!"
}
```

### 5. Usar Token em Endpoints
```bash
GET /api/paciente
Authorization: Bearer {joao_token}
```

## 📋 Roles Disponíveis

- **Admin**: Cria/deleta usuários, acesso total a todas as entidades
- **Recepcao**: Cria/edita pacientes, médicos, agendamentos (POST, PUT com restrições)
- **Medico**: Pode visualizar agendamentos próprios e históricos (a refinar)

## ⚙️ Refinamentos Futuros (Opcional)

1. **Refresh Token Rotation**: Implementar renovação automática de tokens
2. **Forgot Password**: Endpoint para reset de senha
3. **Role-Based Access**: Refinar permissões por campo (ex: Medico só vê agendamentos próprios)
4. **Audit Log**: Rastrear mudanças por usuário
5. **Integração OAuth**: Permitir login via Google, GitHub, etc.

## 🧪 Teste Rápido (cURL/Postman)

1. **Swagger UI**: https://localhost:37311 (verá todos os endpoints)
2. **Login Admin**:
   - POST: `/api/auth/login`
   - Body: `{"email":"admin@local","senha":"Admin123!"}`
3. **Copiar token** da resposta → Clique **Authorize** no Swagger
4. **Register novo user**: POST `/api/auth/register` com body de register

## 📝 Notas

- Senha é armazenada como `salt.base64 + "." + hash.base64` (nunca armazenada em texto puro)
- JWT tem validade de 15 minutos (configurável em appsettings.json)
- Email é único (índice no BD)
- Usuário começa como Ativo por padrão

