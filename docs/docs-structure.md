---
title: Documentation
document-type: Documentation Index
system: HauteCouture
scope: Global
tags: [ documentation, architecture, services, shared-libraries, tests, infrastructure ]
created-at: 20-07-2026
last-updated-at: 16-08-2026
---

## Documentation

This file is the central place describing how documentation is organized across the project. It explains where documentation lives, how it is structured, and how to navigate the project to find the information you need. Documentation is a living artifact: individual documents are expected to be revised over time as the corresponding part of the system evolves, and this file itself should be kept up to date whenever a new documentation category is introduced.

#### HauteCouture: Consolidated documentation matrix

| Category / Section | Link | Description and Purpose | Use Case (When to use) | Status |
| :--- | :--- | :--- | :--- | :--- |
| **1. Architecture** | | | | |
| System Architecture | [architecture.md](https://github.com/0x1args/HauteCouture/tree/main/docs/architecture.md) | System topology, functional and non-functional requirements, distributed interaction patterns, context boundaries and external integrations. | Designing high-level modules/microservices, overview of the overall system picture. | In progress |
| **2. Shared components** | | | | |
| Shared Code Base | [Shared/README.md](https://github.com/0x1args/HauteCouture/tree/main/src/Shared/README.md) | Contracts and implementations of shared code (Abstractions, Base). Architectural decisions regarding DI and reuse. | Searching for ready-made solutions or extending base functionality before implementation in a specific service. | Done |
| **3. Services** | | | | |
| Service Structure | [service-structure.md](https://github.com/0x1args/HauteCouture/tree/main/docs/service-structure.md) | Reference service architecture, rules of layer responsibilities and dependencies. | Bootstrapping a new service, getting acquainted with the general architecture within the service. | Done |
| Service Docs | *README.md in each service folder* | Domain specifics, API contracts (internal/external), background processes (consumers/jobs/signalr) + external dependencies. | Integration with a service, changing its domain boundaries or extending existing API. | Up to date |
| **4. Testing** | | | | |
| Tests Overview | [tests/README.md](https://github.com/0x1args/HauteCouture/blob/main/tests/README.md) | Testing strategy (Unit + Integration) and code coverage policies and mapping of test projects to the solution structure. | Setting up the test environment, writing new tests or updating QA infrastructure. | Done |
| **5. Infrastructure** | | | | |
| Docker | [docker/README.md](https://github.com/0x1args/HauteCouture/blob/main/infrastructure/docker/README.md) | Local environment (Docker Compose) + other infra-dependencies. | Deploying and troubleshooting the local development environment. | Done |
| Kubernetes | [kubernetes/README.md](https://github.com/0x1args/HauteCouture/blob/main/infrastructure/kubernetes/README.md) | Cluster manifests, configuration and secrets management. | Deploying and orchestrating services in different environments. | In progress |
| Terraform (IaC) | [terraform/README.md](https://github.com/0x1args/HauteCouture/blob/main/infrastructure/terraform/README.md) | IaC configurations for automated provisioning of cloud resources. | Modifying cloud infrastructure, changing IAM policies or adding new resources. | In progress |
| Scripts | [scripts/README.md](https://github.com/0x1args/HauteCouture/blob/main/infrastructure/scripts/README.md) | Set of utilities for automation under Windows + Unix. | Optimizing local processes or extending CI/CD pipelines. | Done |