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

## Packages

| Category / Package| Documentation reference |
| :--- | :--- |
| **Common** | |
| Authorization | [Authorization/README.md](https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Common/Authorization/README.md) |
| Exceptions | [Exceptions/README.md](https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Common/Exceptions/README.md)|
| Pagination | — |
| **Databases** | |
| Postgres | [Postgres/README.md](https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Databases/Postgres/Base/README.md) |
| **Shared base** | |
| CQS | [CQS/README.md](https://github.com/0x1args/HauteCouture/blob/main/src/Shared/CQS/Base/README.md) |
| Domain | [Domain/README.md](https://github.com/0x1args/HauteCouture/blob/main/src/Shared/Domain/README.md) |
| WebApi | [WebApi/README.md](https://github.com/0x1args/HauteCouture/blob/main/src/Shared/WebApi/README.md) |