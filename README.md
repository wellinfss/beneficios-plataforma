# Benefícios Plataforma

Plataforma de gestão de benefícios corporativos construída com .NET 9 + Clean Architecture + React/Vite.

## Stack Tecnológico

- **Backend**: .NET 9, ASP.NET Core, Entity Framework Core
- **Frontend**: React 18, TypeScript, Vite, Zustand, React Query
- **Database**: PostgreSQL 16
- **Message Queue**: RabbitMQ 3
- **Cache**: Redis 7
- **Architecture**: Clean Architecture, CQRS (MediatR)

## Estrutura do Projeto

```
├── src/
│   ├── BeneficiosPlataforma.Domain/        # Domain Layer
│   ├── BeneficiosPlataforma.Application/   # Application Layer
│   ├── BeneficiosPlataforma.Infrastructure/# Infrastructure Layer
│   └── BeneficiosPlataforma.API/           # API Layer
├── frontend/
│   ├── apps/web/                           # React Vite App
│   └── packages/shared/                    # Shared TypeScript Types
├── docker-compose.yml                      # Local development infrastructure
└── .env.example                            # Environment variables template
```

## Quick Start

### Prerequisites

- .NET 9 SDK
- Node.js 18+
- Docker & Docker Compose
- PostgreSQL (or use Docker)

### Setup Local Environment

1. **Clone and setup**:
```bash
cp .env.example .env
```

2. **Start infrastructure** (PostgreSQL, Redis, RabbitMQ):
```bash
docker-compose up -d
```

3. **Backend**:
```bash
cd src/BeneficiosPlataforma.API
dotnet restore
dotnet run
```
API will be available at `http://localhost:5000`

4. **Frontend**:
```bash
cd frontend/apps/web
npm install
npm run dev
```
Frontend will be available at `http://localhost:5173`

## Architecture Layers

### Domain Layer
- Entities, Value Objects, Domain Events
- Pure business logic, no external dependencies
- Interfaces for repositories and services

### Application Layer
- Use Cases (Commands/Queries via MediatR)
- DTOs and Validation (FluentValidation)
- Business rules orchestration

### Infrastructure Layer
- EF Core DbContext and Migrations
- Repository implementations
- External service integrations (JWT, Cache, Message Queue)
- Multi-tenancy context

### API Layer
- HTTP Controllers
- Middleware (TenantMiddleware, Auth)
- Dependency Injection composition root
- Swagger/OpenAPI documentation

## Multi-Tenancy

- Request header `X-Tenant-Id` or subdomain-based tenant resolution
- Query filters automatically scope data to tenant
- Cached in Redis for performance

## Authentication & Authorization

- JWT Bearer tokens (15 min expiry)
- Refresh tokens stored in Redis (7 days)
- Role-based access control (RBAC)
- Granular permission system

## Outbox Pattern

- Domain events published to outbox table within transactional context
- OutboxDispatcherWorker processes pending messages every 10 seconds
- MassTransit publishes to RabbitMQ with retry logic
- Ensures reliable event processing

## Default Tenant

- Slug: `default`
- Name: Plataforma Admin
- Admin user credentials available after initial migration

## API Endpoints

- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/logout` - User logout

## Development Notes

- Migrations: `dotnet ef migrations add <name> --project BeneficiosPlataforma.Infrastructure`
- Run migrations on startup automatically
- Audit logging on all entity changes
- Soft delete support via `IsDeleted` field

## Contributing

Follow Clean Architecture principles:
- Keep domain layer pure
- Use MediatR for commands/queries
- Validate at application boundaries
- Handle errors gracefully with Result<T> pattern
