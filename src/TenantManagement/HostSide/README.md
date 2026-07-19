## TenantManagement

### 1.  Responsibilities

`TenantManagement` is one of the core microservices of the system. It owns the full lifecycle of a tenant from initial registration through membership management, invitations, billing subscription state, and invoicing. This document is the functional map of the service: every aggregate and entity it owns, every WebApi operation it exposes (both externally and internally), and the end-to-end business processes that tie them together.

### 2. Domain Overview

| Aggregate | Responsibility | Rate of change |
|---|---|---|
| `Tenant` | Identity, status, custom domains, settings | Low |
| `TenantMember` | A user's membership in a tenant, plus their role | High |
| `TenantInvitation` | The process of inviting a new member | High, short-lived |
| `SubscriptionPlan` | Platform-level catalog of pricing plans | Low |
| `Subscription` | A tenant's payment state (a projection of Stripe state, with additional domain behavior on top) | High (driven by webhooks) |
| `Invoice` | Append-only invoice history | High (driven by webhooks) |


### 3. External Endpoints


#### 3.1 Tenants

| Endpoint | Domain call | Who can access |
|---|---|---|
| `POST /tenants` | `Tenant.Create` + `TenantMember.CreateOwner` + `Subscription.CreateTrial` | Public (self-service registration) |
| `GET /tenants/{id}` | query | Tenant member |
| `GET /tenants/by-slug/{slug}` | query | Public (subdomain resolution) |
| `POST /tenants/{id}/deactivate` | `Tenant.Deactivate` | `TenantOwner`, `PlatformAdministrator` |
| `PATCH /tenants/{id}/settings` | `Tenant.UpdateSettings` | `TenantOwner`, `PlatformAdministrator` |
| `POST /tenants/{id}/domains` | `Tenant.AddCustomDomain` | `TenantOwner`, `PlatformAdministrator` |

#### 3.2 Tenant Member

| Endpoint | Domain call | Who can access |
|---|---|---|
| `GET /tenants/{id}/members` | query | Any tenant member |
| `POST /tenants/{id}/members/{memberId}/role` | `TenantMember.UpdateRole` | `TenantOwner`, `TenantAdministrator` |
| `POST /tenants/{id}/members/{memberId}/promote-to-owner` | `PromoteToOwner` + `DemoteFromOwner` | Current `TenantOwner` |
| `DELETE /tenants/{id}/members/{memberId}` | `Tenant.Remove` | `TenantOwner`, `TenantAdministrator` |
| `GET /me/tenants` | query | Any authenticated user |

#### 3.3 Tenant Invitation

| Endpoint | Domain call | Who can access |
|---|---|---|
| `POST /tenants/{id}/invitations` | `TenantInvitation.Create` | `TenantOwner`, `TenantAdministrator` |
| `GET /tenants/{id}/invitations` | query (pending only) | `TenantOwner`, `TenantAdministrator` |
| `POST /invitations/{token}/accept` | `TenantInvitation.Accept` → `TenantMember.CreateFromInvitation` | Public (via invitation token) |
| `POST /tenants/{id}/invitations/{invitationId}/revoke` | `TenantInvitation.Revoke` | `TenantOwner`, `TenantAdministrator` |

#### 3.4 Subscription Plan

| Endpoint | Domain call | Who can access |
|---|---|---|
| `POST /admin/plans` | `SubscriptionPlan.Create` | `PlatformAdministrator` |
| `GET /plans` | query (active + public plans only) | Public (pricing page) |
| `GET /admin/plans` | query (all plans) | `PlatformAdministrator` |
| `POST /admin/plans/{id}/features` | `SubscriptionPlan.AddFeature` | `PlatformAdministrator` |
| `POST /admin/plans/{id}/deprecate` | `SubscriptionPlan.Deprecate` | `PlatformAdministrator` |
| `POST /admin/plans/{id}/activate` | `SubscriptionPlan.Activate` | `PlatformAdministrator` |

#### 3.5 Subscription

| Endpoint | Domain call | Who can access |
|---|---|---|
| `GET /tenants/{id}/subscription` | query | `TenantOwner`, billing role |
| `POST /tenants/{id}/subscription/checkout` | creates a Stripe Checkout Session | `TenantOwner` |
| `POST /webhooks/stripe` | `Activate`, `RecordSuccessfulPayment`, `MarkPastDue`, `MarkUnpaid`, `Cancel` | Stripe (verified via webhook signature) |
| `POST /tenants/{id}/subscription/change-plan` | `Subscription.ChangePlan` | `TenantOwner` |
| `POST /tenants/{id}/subscription/cancel` | `Subscription.ScheduleCancellation` | `TenantOwner` |
| `POST /tenants/{id}/subscription/resume` | `Subscription.ResumeCancellation` | `TenantOwner` |


#### 3.6 Invoice

| Endpoint | Domain call | Who can access |
|---|---|---|
| `GET /tenants/{id}/invoices` | query | `TenantOwner`, billing role |
| `GET /tenants/{id}/invoices/{invoiceId}` | query (includes `PdfUrl`) | `TenantOwner`, billing role |
| *(no public route — created via `POST /webhooks/stripe`)* | `CreateFromProvider`, `MarkPaid`, `MarkFailed` | Stripe (via webhook) |

### 4. Internal Endpoints

| Endpoint | Domain call | Caller |
|---|---|---|
| `POST /tenants/{id}/activate` | `Tenant.Activate` | Provisioning saga |
| `POST /tenants/{id}/suspend` | `Tenant.Suspend` | Billing saga *(also reachable externally by `PlatformAdministrator`, see [Section 3.1](#31-tenants))* |
| `POST /tenants/{id}/reactivate` | `Tenant.Reactivate` | Billing saga *(also reachable externally by `PlatformAdministrator`, see [Section 3.1](#31-tenants))* |
| `POST /tenants/{id}/domains/{domainName}/verify` | `Tenant.VerifyCustomDomain` | DNS-checker worker |

## 5. Used databases

- PostgreSQL


## 6. External dependencies

- Stripe
- DNS resolution