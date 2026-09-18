# Franquias API

API REST em C# (.NET 10 / ASP.NET Core) para gestão de uma rede de franquias: franqueadora e unidades franqueadas, usuários e perfis de acesso, catálogo de produtos/serviços, fornecedores, estoque por unidade, vendas, royalties, chamados de suporte e relatórios gerenciais.

Trabalho acadêmico da disciplina de Desenvolvimento Back-end.

## Tecnologias

- **C# / ASP.NET Core Web API** (.NET 10)
- **Entity Framework Core** (SQL Server) para persistência e migrations
- **JWT Bearer** para autenticação, com autorização por perfil (`Administrador`, `Gestor`, `Operador`) e por unidade
- **BCrypt.Net** para hash de senha
- **Swagger / OpenAPI** (Swashbuckle) para documentação e testes interativos
- **xUnit + Moq** para testes unitários das regras de negócio
- Arquivo `.http` (REST Client) com a coleção de requisições ponta a ponta

## Estrutura de pastas

```
Franquias.Api/
├── Controllers/       # Endpoints HTTP, autorização por perfil/unidade
├── Models/             # Entidades de domínio e enums (Models/Enums)
├── DTOs/               # Objetos de entrada/saída dos endpoints
├── Services/           # Regras de negócio (interface + implementação)
├── Repositories/       # Acesso a dados via EF Core (interface + implementação)
├── Data/               # AppDbContext
├── Migrations/         # Migrations do EF Core
├── Configurations/     # Fluent API (mapeamento de entidades) e middleware de exceções
└── Program.cs           # Composição da aplicação (DI, autenticação, Swagger, pipeline)
Franquias.Api.Tests/     # Testes unitários (xUnit + Moq) dos Services
Franquias.slnx           # Solução com os dois projetos acima
```

## Como rodar localmente

Pré-requisitos: .NET SDK 10, SQL Server (local ou container) acessível via a connection string configurada.

1. Ajuste a connection string em `Franquias.Api/appsettings.Development.json` (`ConnectionStrings:DefaultConnection`) se necessário. Por padrão aponta para `Server=localhost;Database=FranquiasDb` com autenticação integrada do Windows.
2. Aplique as migrations para criar o banco:

   ```bash
   cd Franquias.Api
   dotnet ef database update
   ```

3. Rode a API:

   ```bash
   dotnet run --project Franquias.Api
   ```

   A API sobe em `http://localhost:5192` por padrão (ver `Franquias.Api/Properties/launchSettings.json`). O Swagger fica disponível em `http://localhost:5192/swagger` (apenas em ambiente `Development`).

### Primeiro usuário Administrador

Nenhum endpoint de cadastro de usuário é público (`POST /api/usuarios` exige `[Authorize(Roles = "Administrador")]`). Para criar o primeiro administrador em um banco novo, use uma das opções:

- Insira manualmente um registro na tabela `Usuarios` com `SenhaHash` gerado via BCrypt (`BCrypt.Net.BCrypt.HashPassword("suaSenha")`); ou
- Remova temporariamente o atributo `[Authorize(Roles = "Administrador")]` de `UsuariosController.Criar`, cadastre o primeiro administrador via `POST /api/usuarios`, e devolva o atributo antes de commitar/rodar em produção.

## Autenticação no Swagger

O Swagger está configurado com o esquema de segurança **Bearer** (botão **Authorize** disponível na UI):

1. `POST /api/auth/login` com e-mail/senha de um usuário ativo → copie o `token` da resposta.
2. Clique em **Authorize** no topo da página do Swagger e cole **apenas o token** (sem o prefixo `Bearer `) no campo indicado.
3. As chamadas seguintes aos endpoints protegidos já enviam o header `Authorization: Bearer <token>` automaticamente.

Alternativamente, use o arquivo `Franquias.Api/Franquias.Api.http` (extensão REST Client do VS Code) ou importe as requisições em Postman/Insomnia — o token retornado no login é reaproveitado automaticamente nas requisições seguintes do `.http` via `{{login.response.body.token}}`.

## Como rodar os testes

```bash
dotnet test Franquias.slnx
```

Os testes cobrem as regras de negócio mais sensíveis: e-mail de usuário duplicado, CNPJ de unidade duplicado, estoque não pode ficar negativo (movimentação de saída maior que o saldo), venda sem itens é rejeitada, cálculo do valor total da venda com baixa de estoque, e cálculo/recálculo de royalty (incluindo o bloqueio de recálculo após pagamento confirmado).

## Perfis de acesso

- **Administrador**: acesso irrestrito a todos os módulos e unidades.
- **Gestor** / **Operador**: vinculados a uma `UnidadeFranqueadaId` (claim no token); só acessam dados da própria unidade. Endpoints agregados de todas as unidades (ex.: ranking, ou listagens sem filtro de unidade) são restritos a Administrador.

## Principais regras de negócio

- CNPJ de unidade e e-mail de usuário são únicos (`400` ao duplicar).
- Unidade inativa não pode registrar vendas.
- Venda deve ter ao menos um item; o valor total é calculado a partir do preço de catálogo (não aceito no payload) e a baixa de estoque é automática e transacional (sem baixa parcial em caso de erro).
- Estoque nunca fica negativo: saída maior que o saldo disponível é rejeitada com `400`.
- Royalty é calculado sobre o faturamento (vendas confirmadas) da unidade no período, usando o percentual configurado na unidade; recalcular um royalty já pago é bloqueado.
- Chamados possuem máquina de estados simples (Aberto → Em Andamento → Encerrado); chamado encerrado não pode mais ser atualizado, e o encerramento tem endpoint dedicado.
- Registros centrais (`Usuario`, `UnidadeFranqueada`, `ProdutoServico`, `Categoria`, `Fornecedor`) usam ativação/inativação em vez de exclusão física.

O diagrama entidade-relacionamento completo (14 entidades) está no relatório final; o modelo de dados também pode ser inspecionado diretamente pelas migrations em `Franquias.Api/Migrations/`.
