BACKEND - API DE PEDIDOS (.NET 8)

Este projeto é uma API feita em **.NET 8**, desenvolvida como parte de um teste técnico.  

Tecnologias Utilizadas
- .NET 8  
- Entity Framework Core  
- SQL Server  
- AutoMapper  
- Swagger  
- ILogger
- 
Estrutura do Projeto
Api/ → Controllers e configurações iniciais
Application/ → Serviços, DTOs, perfis de mapeamento e interfaces
Domain/ → Entidades e regras de negócio
Infrastructure/ → Persistência (DbContext e migrações)
testeTecnico.sln → Solução principal

### 1. Clonar o repositório
```bash
git clone https://github.com/calebedantas/teste-tecnico-backend.git
cd BackEnd

A API ficará disponível em:
https://localhost:5001/swagger
