## HauteCouture

TODO / Work in progress

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

*This is just a basic set; it will be revised over time!*

### 2. Documentation

The project includes a centralized navigation for all available documentation in `docs/docs-structure` (available via the link below). I decided to split the documentation into separate documents because it makes navigation much easier than scrolling through a single, text-heavy file. The documentation covers the domain, architecture, services, internal infrastructure, shared packages, and testing.

#### 2.1 Useful resources

- **Reference documentation on docs sturcture:** https://github.com/0x1args/HauteCouture/tree/main/docs/docs-structure.md

### 3. Used architectural patterns

- Domain-Driven Design
- Microservices
- Event-Driven Architecture
- Clean Architecture
- CQRS
- Outbox / Inbox patterns
- Saga pattern
- Circuit Breaker
- API Gateway

### 4. Getting started


#### For usage

- Ensure that Docker is installed on your machine. You can download it from the official [Docker website](https://docs.docker.com/).
- Will be written once the platform is far enough along to have a stable local setup worth documenting. See `infrastructure/docker/README.md` and `infrastructure/scripts/README.md` in the meantime for what's runnable today.
- All services will be available as containers. I will provide their addresses later.
- *This is just a basic set; it will be revised over time!*

#### For local development

- Ensure that you have the .NET 10 SDK (10.0.x) installed. You can download it from the offical [Microsoft website](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).
- Also, ensure that Docker is installed on your machine.
- Run the script to create the `.env` file via `infrastructure/scripts/{system}/create-docker-env.{sh/ps1}`.
- Start the required containers for the internal infrastructure via `infrastructure/scripts/{system}/run-docker-infra.{sh/ps1}`.
- Run the script to apply all database migrations via `infrastructure/scripts/{system}/run-docker-migrations.{sh/ps1}`.
- Start the observability stack by running via `infrastructure/scripts/{system}/run-docker-observability.{sh/ps1}`.
- Build the entire solution using the following command: `dotnet build src/Solution/HauteCouture.slnx`.
- Finally, start the required service using your IDE or the CLI with the following command: `dotnet run --project {path}`.
- Happy hacking!
- *This is just a basic set; it will be revised over time!*

### 5. Known limitations and bugs

Will be written once the system is functionally complete enough for its limitations to be meaningfully scoped.

- TBD

### 6. Available services

Will be filled in during finalization, as a table of service name, address, and a link to that service's endpoint documentation.

| Service name | Address |
|--|--|
| TBD | TBD |

### 7. Support

If this project helped you in any way, please leave a star ⭐ as a form of feedback. I tried to add extensive documentation covering the main and important aspects, and chose an interesting domain. A great deal of effort was invested in this project.

### 8. License

This project is licensed under the [MIT License](https://github.com/0x1args/HauteCouture/blob/main/LICENSE).

### 9. Authors

Only me.