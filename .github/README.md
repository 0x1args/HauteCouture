## HauteCouture

A multi-tenant SaaS platform for managing service-based businesses, built on a microservice architecture. A tenant can be a retail shop, a hair salon, a workshop, or any other appointment/service-driven business. The platform provides ready-made, abstract building blocks for configuring a service catalog, resources, staff, and booking workflows, on top of strong security, integration with external systems, and real-time notifications.

### 1. Technology stack

| Area | Technology |
|---|---|
| **Runtime** | ASP.NET Core + .NET 10 |
| **API Gateway** | YARP |
| **Primary database** | PostgreSQL |
| **Write side (CQRS)** | PostgreSQL |
| **Read side (CQRS)** | MongoDB |
| **ORM** | Entity Framework Core 10 |
| **Validation** | FluentValidation |
| **Cache** | Redis |
| **Message broker** | Kafka |
| **Messaging transport** | MassTransit |
| **Identity provider** | Keycloak |
| **Payment provider** | Stripe |
| **Blob storage** | S3 (MinIO) |
| **Observability** | Serilog, Seq, OpenTelemetry, Grafana, Prometheus, Jaeger |
| **Testing** | xUnit, Testcontainers, Respawn |
| **Containerization** | Docker, Docker Compose |

The write/read split above is deliberate, not incidental: commands are executed transactionally against PostgreSQL for correctness, while queries are served from denormalized projections in MongoDB, shaped and indexed for how the system is actually read rather than how it is written. This approach applies only to certain microservices, not everywhere.

### 2. Documentation

The project keeps one centralized place for quickly orienting yourself in whichever part of the system you need: `docs/docs-structure.md`. It's an index of every detailed document available in the project: the structure of each microservice, the overall system architecture, the shared-libraries module, testing, internal infrastructure, and the technology stack. So it's the right starting point regardless of what you're actually looking for.

**Reference documentation:** https://github.com/0x1args/HauteCouture/tree/main/docs/docs-structure.md

### 3. Architectural patterns

- Microservices
- Domain-Driven Design
- Clean Architecture
- CQRS
- Event-Driven Architecture
- Outbox / Inbox patterns
- Saga pattern
- Circuit Breaker
- API Gateway

### 4. Quick start

TODO: will be written once the platform is far enough along to have a stable local setup worth documenting. See `infrastructure/docker/README.md` and `infrastructure/scripts/README.md` in the meantime for what's runnable today.

### 5. Known limitations

TODO: will be written once the system is functionally complete enough for its limitations to be meaningfully scoped.

### 6. Available services

TODO: will be filled in during finalization, as a table of service name, API URL, and a link to that service's endpoint documentation.

### 7. License

This project is licensed under the [MIT License](https://github.com/0x1args/HauteCouture/blob/main/LICENSE).
