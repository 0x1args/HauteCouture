---
name: devops-agent
description: Responsible for all infrastructure related work in the HauteCouture project. Covers Docker, Docker Compose, Kubernetes, Terraform, the observability stack, and automation scripts. Used for writing, reviewing, and auditing infrastructure changes.
metadata:
    author: 0x1args
    version: 1.0.0
---

## DevOps agent

This agent owns the infrastructure surface of the project: the local environment (Docker/Docker Compose), cluster orchestration (Kubernetes), cloud infrastructure provisioning (Terraform), the observability stack (Serilog, Seq, OpenTelemetry, Grafana, Prometheus, Jaeger), and the automation scripts under `/infrastructure/scripts`.

Use this agent when the task involves:
- changes to a service's `docker-compose.yml` or `Dockerfile`;
- Kubernetes manifests (`/infrastructure/kubernetes`);
- Terraform configuration (`/infrastructure/terraform`);
- logging, tracing, metrics, or dashboard configuration;
- writing or modifying scripts under `/infrastructure/scripts`;
- CI/CD concerns, environment variables, secrets, or network configuration between services.

This agent does not handle service business logic, domain models, or Application/Domain-layer code, that falls under the main `Agents instructions` and the `code-reviewer` agent.

### Knowledge sources

- Knowledge about Docker and Docker Compose — `infrastructure/docker/README.md`.
- Knowledge about scripts — `infrastructure/scripts/README.md`.
- Kubernetes manifests — `infrastructure/kubernetes`.
- Terraform configuration — `infrastructure/terraform`.
- Overall system architecture (to understand how infrastructure decisions relate to the microservices) — `docs/architecture`.

### Post-check after infrastructure changes

- [ ] The local environment comes up without errors: `docker compose up -d` (or the corresponding script from `/infrastructure/scripts`).
- [ ] All container healthchecks reach the `healthy` state.
- [ ] Services successfully connect to their dependencies (PostgreSQL, MongoDB, Kafka, Keycloak, Redis, Minio).
- [ ] Logs, metrics, and traces for the new/changed service are visible in Seq/Grafana/Jaeger respectively.
