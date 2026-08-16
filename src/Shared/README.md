---
title: Shared
document-type: Design Document
system: HauteCouture
scope: Global
tags: [ shared, cqs, postgres, webapi, authorization, exceptions, pagination ]
created-at: 20-07-2026
last-updated-at: 16-08-2026
---

## Shared

This is part of the system that already contains ready-made solutions for solving many problems. It houses the shared codebase that can be reused across many services. Most packages are adapted to be extended as needed; all packages mostly contain corresponding documentation describing their purpose and usage; **Shared** is the main supplier for working with many tools and libraries, wrapping them behind abstractions.

#### Shared components reference

| Category / Package| Documentation reference | Key Dependencies / Tech |
| :--- | :--- | :--- |
| **1. Common** | |
| Authorization | [Authorization/README.md](https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Common/Authorization/README.md) | `UAParser`, general models and utilities for authorization. |
| Exceptions | [Exceptions/README.md](https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Common/Exceptions/README.md)| The usual .NET exceptions. |
| Pagination | — | General models for offset and keyset pagination + filters. |
| **2. Databases** | |
| Postgres | [Postgres/README.md](https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Databases/Postgres/Base/README.md) | `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`, `EFCore.NamingConventions`. |
| **3. Shared base** | |
| CQS | [CQS/README.md](https://github.com/0x1args/HauteCouture/blob/main/src/Shared/CQS/Base/README.md) | `MediatR`, `FluentValidation`. |
| Domain | [Domain/README.md](https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Domain/README.md) | Basic shared utilities for the domain. |
| WebApi | [WebApi/README.md](https://github.com/0x1args/HauteCouture/blob/main/src/Shared/WebApi/README.md) | `Microsoft.OpenApi`, `Microsoft.AspNetCore.OpenApi` `Microsoft.AspNetCore.Http.Abstractions`, `Microsoft.AspNetCore.Http.Extensions`, `Microsoft.Extensions.Caching.StackExchangeRedis`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`, `Microsoft.Extensions.Configuration.Abstractions`, `Microsoft.Extensions.Hosting.Abstractions`, `Swashbuckle.AspNetCore`, `Serilog.AspNetCore` + additional dependencies, `Npgsql`, `FluentValidation`. |