# Documentation

This file is the central place describing how documentation is organized across the project. It explains where documentation lives, how it is structured, and how to navigate the project to find the information you need. It also describes the principle behind adding new documentation yourself — where it belongs, and what shape it should take. Documentation is a living artifact: individual documents are expected to be revised over time as the corresponding part of the system evolves, and this file itself should be kept up to date whenever a new documentation category is introduced.

## 1. Architecture

The overall system architecture, illustrative diagrams together with an explanation of how all services work together as a single system  has its own dedicated place in the documentation. It covers how services interact with one another (synchronously and asynchronously), how they interact with external systems, how security is organized (authentication, authorization, and network boundaries), and what the overall system design and deployment topology look like. This document is the right place to look for the "big picture" before diving into any single service's documentation.

**Reference documentation:** https://github.com/0x1args/HauteCouture/tree/main/docs/architecture.md *(not written yet)*

## 2. Shared libraries

Every layer inside `Shared` must contain the information needed to understand that specific layer and its full usage process: a description of the package, a description of its DI registrations, a description of its individual features, and an explanation of the design choices behind it along with the benefit they provide. Every layer in `Shared` exists to keep the overall architecture flexible and to hold code that is meant to be reused across every service. Before adding something new to a service, always check whether it is already solved in `Shared`. `Shared` packages are typically organized into two sublayers:

- **Abstractions** — used for integration into the corresponding layers of a consuming service (i.e., the interfaces and contracts a service depends on).
- **Base** — contains the actual configuration and implementation work behind those abstractions.

Documentation must always live inside **Base**, next to the implementation it describes. Beyond these two sublayers, additional ones may be created where needed, so that a service does not have to pull in a large number of components it will never use.

**Reference documentation:** https://github.com/0x1args/HauteCouture/tree/main/src/Shared/README.md

## 3. Services

A dedicated document already describes the general structure shared by all services, together with recommendations for building each of its layers. Any service added to the system must strictly follow this structure and these recommendations — it serves as the baseline knowledge document for building or reviewing any service in the system.

**Reference documentation:** https://github.com/0x1args/HauteCouture/tree/main/docs/service-structure.md

### 3.1 Per-service documentation

Every service must contain its own documentation file at the **HostSide** level. This file must describe:

- the general purpose and responsibilities of the service;
- an overview of its domain;
- the endpoints exposed for integration with external systems (including their routes, request shapes, and response shapes);
- the endpoints intended for internal-only interaction between services;
- the technology stack the service uses;
- its background jobs and consumers;
- any special/third-party libraries it relies on;
- its external dependencies (other services, external APIs, data stores).

## 4. Tests

Tests in the system are classified into Unit tests and Integration tests. The project has an integrated analytics system that tracks code coverage percentage over time. All tests are placed under `/tests` at the root of the repository, and their internal structure mirrors the layer being tested, whether that layer belongs to `Shared` or to a specific service (e.g. a test project for **Applications.Handlers** of a given service, or for a specific `Shared.*` package).

**Reference documentation:** https://github.com/0x1args/HauteCouture/blob/main/tests/README.md

## 5. Interanl infrastructure

Local development relies on a combination of Docker and Kubernetes to orchestrate the microservice architecture. All related configuration files live under the `/infrastructure` folder at the root of the repository, which is split into three subfolders:

- **`/docker`** — configuration for running services and their dependencies (databases, message brokers, etc.) locally via Docker/Docker Compose.
- **`/kubernetes`** — manifests and configuration used to orchestrate the services in a cluster, for environments beyond local development.
- **`/scripts`** — automation scripts that support environment setup and day-to-day development tasks.
- **`/terraform`** — Infrastructure as Code (IaC) configuration used to provision and manage cloud infrastructure in a consistent, reproducible, and version-controlled manner.

**Reference documentation (Docker):** https://github.com/0x1args/HauteCouture/blob/main/infrastructure/docker/README.md

**Reference documentation (Kubernetes):** https://github.com/0x1args/HauteCouture/blob/main/infrastructure/kubernetes/README.md *(not written yet)*

**Reference documentation (Scripts):** https://github.com/0x1args/HauteCouture/blob/main/infrastructure/scripts/README.md

**Reference documentation (Terraform):** https://github.com/0x1args/HauteCouture/blob/main/infrastructure/terraform/README.md *(not written yet)*

## 6. AI Agent Integration

The project is set up for integration with AI assistants. The root-level `AGENTS.md` file lays out the core information an agent needs, and points to this very file (`/docs/docs-structure.md`) as the entry point from which an agent can navigate to whatever information is relevant to its current context. The `/.agents` folder contains two subfolders:

- **`skills/`** — describes reusable skills for working with specific features of the project. Each skill lives in its own folder with a `SKILL.md` file describing it.
- **`agents/`** — describes pseudo-personas, each specializing in a specific area of the project. An agent persona draws on the documentation described in this file and can use one or more of the skills above. Each persona is defined in its own file, named `<agent-name>-agent.md`.