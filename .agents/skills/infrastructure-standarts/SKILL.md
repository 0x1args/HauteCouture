---
name: infrastructure-standards
description: Defines the infrastructure standards, technologies, and best practices for the HauteCouture project, including Docker, Kubernetes, Terraform, observability, and automation.
globs: ["/infrastructure/**", "src/**/*.cs", "tests/**/*.cs"]
---

## Purpose

Use this skill whenever working on the DevOps or infrastructure side of the HauteCouture project. It defines the project's infrastructure stack, explains how the development and deployment environments are organized, and provides standards, conventions, and best practices for working with infrastructure related code and configuration.

### Stack

| Area | What it covers |
|---|---|
| Docker / Docker Compose | Each service's `Dockerfile`, the `docker-compose.yml` used to run services and their dependencies locally (PostgreSQL, MongoDB, Kafka, Keycloak, MinIO, Redis) |
| Kubernetes | Deployment, Service, Ingress, ConfigMap, and Secret manifests, Helm charts (if used), and namespace strategy |
| Terraform | Modules and state for provisioning cloud infrastructure, including environment specific configuration (dev, stage, prod) |
| Observability | Serilog configuration, Seq, the OpenTelemetry Collector, Grafana dashboards, Prometheus scrape configurations, and Jaeger tracing |
| Scripts | Automation scripts for bootstrapping the project and managing the local development environment |

### Infrastructure change checklist

#### Docker / Docker Compose

- [ ] Do all images use a specific version tag instead of `latest`?
- [ ] Does every image use the latest stable version that has been verified by the project?
- [ ] Does every container that other services depend on (PostgreSQL, MongoDB, Kafka, Keycloak, Redis) define a health check?
- [ ] Is `restart: unless-stopped` configured for long running services unless there is a justified exception?
- [ ] Are secrets (passwords, API keys, certificates, etc.) stored outside `docker-compose.yml` and provided through `.env` files or a dedicated secrets manager?
- [ ] Are persistent volumes configured for stateful services such as PostgreSQL, MongoDB, and MinIO?
- [ ] Are service networks isolated appropriately so that services communicate only with the components they require?
- [ ] Are reasonable CPU and memory limits configured for the local development environment?
- [ ] Does every `Dockerfile` use a multi stage build and exclude unnecessary build time dependencies from the final image?

#### Kubernetes

- [ ] Are `readinessProbe` and `livenessProbe` configured for every Deployment?
- [ ] Are `resources.requests` and `resources.limits` (CPU and memory) specified?
- [ ] Are secrets provided through Kubernetes `Secret` resources or an external secret manager instead of `ConfigMap` files or hardcoded values?
- [ ] Are the project's naming and labeling conventions (`app`, `component`, `environment`, etc.) followed consistently?
- [ ] Is a `NetworkPolicy` configured wherever traffic between namespaces or services should be restricted?
- [ ] Is the deployment strategy (`RollingUpdate` or `Recreate`) appropriate for the service and its stateful dependencies?
- [ ] Do all manifests pass schema validation and `kubectl apply --dry-run` before being merged?

#### Terraform

- [ ] Is Terraform state stored in a remote backend instead of locally?
- [ ] Are environment specific values defined in `variables.tf` or `.tfvars` files instead of being hardcoded?
- [ ] Are reusable modules used wherever infrastructure is shared across services?
- [ ] Has `terraform plan` been reviewed before running `terraform apply`, with no unexpected resource replacements or deletions?
- [ ] Are sensitive values protected (`sensitive = true` where applicable) and prevented from appearing in plaintext?
- [ ] Is infrastructure isolated between environments (dev, stage, prod) with no risk of cross environment changes?

#### Observability

- [ ] Does the service configure Serilog correctly for structured logging and Seq integration?
- [ ] Is trace context propagated across HTTP, MassTransit, and Kafka communication?
- [ ] Have Prometheus metrics been added or updated for new functionality where appropriate (latency, throughput, error rate, etc.)?
- [ ] Have the relevant Grafana dashboards been updated when service behavior changes?
- [ ] Are Jaeger traces sufficiently detailed for diagnostics without producing unnecessary noise?
- [ ] Are alerts configured for critical metrics such as availability, error rate, and resource usage?

#### Automation scripts (`/infrastructure/scripts`)

- [ ] Is the script idempotent so it can be executed multiple times safely?
- [ ] Does the script handle failures gracefully and provide clear error messages?
- [ ] Is the script documented in `infrastructure/scripts/README.md`?
- [ ] Does the script avoid hardcoded machine specific paths or configuration values?
- [ ] Is the script compatible with the CI environment instead of relying only on local developer tools?
- [ ] Is the script available in both PowerShell (`.ps1`) and POSIX shell (`.sh`) versions?

#### Infrastructure security

- [ ] Does the configuration follow the principle of least privilege for service accounts, Keycloak roles, and Terraform IAM roles?
- [ ] Are secrets completely excluded from Git (including `.gitignore`, `.dockerignore`, and commit history)?
- [ ] Are only the required ports exposed externally?
- [ ] Do S3 or MinIO buckets avoid public access unless there is a documented and justified reason?