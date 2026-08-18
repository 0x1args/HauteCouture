---
title: Architecture
document-type: Design Document
system: HauteCouture
scope: Global
tags: [ architecture, ddd, microservices, distributed-systems ]
created-at: 20-07-2026
last-updated-at: 16-08-2026
---

## Architecture

Since the entire system is built based on the Domain-Driven Design methodology, the whole system is designed proceeding from this. The project has a separate document [docs/domain-driven-design.md](https://github.com/0x1args/HauteCouture/tree/main/docs/domain-driven-design.md), which explains the domain itself in detail at the strategic design level. Therefore, before reviewing this document, it is recommended to become familiar with the domain.

### 1. Requirements

#### 1.1. Functional requirements

| Functional requirement / Category | Description |
| :--- | :--- |
| **FR.1. Tenant Provisioning** | |
| FR.1.1. Clinic registration | The system must allow the creation of a new tenant, which is a clinic with an isolated data environment. |
| FR.1.2. Subscription management | The system must allow activation, deactivation, and changing of tariff plans (i.e., subscriptions) for a tenant. |
| FR.1.3. Modularity | The system must automatically hide or block access to modules/subdomains that are not included in the tenant's current subscription. |
| FR.1.4. Platform configuration | The tenant administrator must be able to configure local parameters (clinic working hours, logo, branding, customization, contact details). |
| FR.1.5. Tenant lifecycle | The system must support tenant statuses, such as active, suspended, deleted, and block any activity for inactive statuses. |
| **FR.2. Identity** | |
| FR.2.1. Authentication | The system must support login via email/phone and a reliable password through an enforced strict filter. |
| FR.2.2. Multi-factor authentication | The system must support/require MFA (SMS, Authenticator App) for medical staff and administrators. |
| FR.2.3. Role-based access model | The system must support assignment of roles (roles TBD) within a specific tenant. |
| FR.2.4. Session management | The system must allow the user to see active sessions and forcibly terminate them. |
| FR.2.5. Security transparency | The system must display to the user the login history with date, IP address, and device information. |
| FR.2.6. Contextual binding | A single account can be linked to several tenants (for example, a doctor may work at two clinics), but a session always operates only within a single tenant. Binding of the account to a physical person. |
| **FR.3. Patient Registry** | |
| FR.3.1 Profile creation | The system must allow the creation of a patient's demographic profile (full name, date of birth, contacts). |
| FR.3.2 Deduplication | The system must automatically check for duplicates by a combination (full name + date of birth) or a unique identifier (for example, national tax ID) within a tenant. |
| FR.3.3. Emergency contacts | The ability to add and edit persons for emergency cases. |
| FR.3.4. Data processing consents | The system must record the date, time, and version of the consent signed by the patient for the processing of personal and medical data, including special categories (GDPR compliance). |
| FR.3.5. Data updates | The system must store the history of changes to the patient's contact data and profile. |
| **FR.4. Staff Management** | |
| FR.4.1. Doctor profiles | The system must store data on the specialization, classification, and contact information of doctors. |
| FR.4.2. License management | The ability to upload medical licenses indicating the issue date and expiration date. |
| FR.4.3. License validation | The system must block the ability to add a doctor to the schedule if their license is expired or inactive. |
| FR.4.4. Shift schedule | The administrator must be able to create, edit, and approve doctors' work shifts. |
| **FR.5. Scheduling** | |
| FR.5.1. Slot generation | The system must automatically generate available slots for booking based on the intersection of the doctor's work schedule and the clinic's work schedule. |
| FR.5.2. Collision prevention | The system must, at the transaction level, block double booking of the same slot or overlapping of slots for the same doctor/room. |
| FR.5.3. Booking | A receptionist or patient must be able to book an available slot. |
| FR.5.4. Calendar synchronization | The system must support two-way synchronization with doctors' external calendars (Google Calendar, Outlook). |
| FR.5.5. Booking statuses | The system must support the slot lifecycle (Free, Booked, Confirmed, Cancelled, No-show). |
| **FR.6. Medical Care** | |
| FR.6.1. Electronic medical record | The system must create a Visit, linked to a specific patient and slot. |
| FR.6.2. Symptom coding | The system must provide search and selection of complaints and symptoms by the ICPC-2 classifier. |
| FR.6.3. Diagnosis coding | A diagnosis must necessarily be linked to a valid ICD-10 code. |
| FR.6.4. Treatment prescription | The doctor must be able to enter textual recommendations and form treatment plans. |
| FR.6.5. Medical record audit | The system must prohibit retroactive deletion or invisible editing of a saved medical record (any change creates a new version of the record with a note of who made the change and when). |
| **FR.7. Prescription Management** | |
| FR.7.1. E-prescription generation | The system must allow the creation of an electronic prescription only if there is a confirmed diagnosis in the visit and a valid doctor's license. |
| FR.7.2. Drug coding | Prescriptions must use industry-standard drug coding (RxNorm, ATC). |
| FR.7.3. Pharmacy integration | The system must transmit the e-prescription to external pharmaceutical gateways and receive its identifier. |
| FR.7.4. Prescription statuses | Tracking of the status (Issued, Redeemed, Expired). |
| **FR.8. Lab Diagnostics** | |
| FR.8.1. Electronic referrals | Creation of e-referrals for tests linked to a visit. |
| FR.8.2. Exchange standard | Data exchange with laboratories must be carried out according to the HL7/FHIR standard. |
| FR.8.3. Test coding | Use of the LOINC standard to identify types of laboratory tests. |
| FR.8.4. Receiving results | Received results must be automatically matched to the corresponding referral, visit, and patient, changing the referral status to "Completed". |
| **FR.9. Tariff and Billing Management** | |
| FR.9.1. Tariff directory | The ability to create and edit services with prices (tariffs) that have an effective start date. |
| FR.9.2. Invoice generation | After a visit is completed, the system must automatically generate an invoice. |
| FR.9.3. Tariff freezing | An invoice must be formed exclusively based on the tariff that was in effect at the time the service was provided, regardless of the current prices in the directory. |
| FR.9.4. Discounts and surcharges | The ability to manually or automatically apply discounts to a generated invoice. |
| FR.9.5. Invoice statuses | Management of payment statuses (Unpaid, Partially paid, Paid, Cancelled) |
| **FR.10. Payment Processing** | |
| FR.10.1. Fund collection | Integration with payment systems (Stripe) for online payment or prepayment for a slot. |
| FR.10.2. Idempotency | The system must guarantee that funds for the same invoice will not be charged twice on repeated attempts or network errors. |
| FR.10.3. Refunds | Support for full and partial refunds when a slot is cancelled, according to clinic rules. |
| **FR.11. Notification** | |
| FR.11.1. Multi-channel | The system must be able to send SMS, Email, and Push notifications. |
| FR.11.2. Notification triggers | Automatic sending of visit reminders (X hours before), invoice links, messages about test results readiness. |
| FR.11.3. Delivery tracking | Receiving and storing delivery statuses (Delivered, Bounced, Read) from external providers. |
| **FR.12. Analytics and Reporting** | |
| FR.12.1. Dashboards | Display of aggregated metrics: Revenue, doctor workload, no-show percentage. |
| FR.12.2. Report generation | The ability to generate reports for a selected period (day, month, year) and export them to CSV/PDF. |
| FR.12.3. Read-only mode | The analytics context must be built solely as an aggregate of events and must not be able to change data in other contexts. |

#### 1.2 Non-functional requirements

| Non-Functional requirement / Category | Description |
| :--- | :--- |
| **NFR.1. Security and confidentiality** | |
| NFR.1.1. Data isolation | Data of different clinics must be strictly isolated. At the database level, this must be implemented through separate schemas. Data leakage between tenants is absolutely unacceptable. |
| NFR.1.2. Regulatory compliance | The system must fully comply with GDPR requirements regarding the processing of special category data. |
| NFR.1.3. Encryption at rest | All personal and medical data must be encrypted in the database according to the AES-256 standard. This also applies to brokers that store data on the file system. |
| NFR.1.5. Global audit | The system must maintain an immutable audit log for data read and write operations (Who, What, When, from which IP address accessed or changed the data). |
| NFR.1.6. Patient control of personal data | Ensuring the patient's right to access their own data and its deletion or anonymization within limits that do not contradict the requirements of medical legislation regarding EHR retention. |
| **NFR.2. Reliability and availability** | |
| NFR.2.1. Availability level | The platform must have an availability rate of no less than 99.9% of the time per month. |
| NFR.2.2. Recovery Time Objective | The recovery time after a critical system failure must not exceed 4 hours. |
| NFR.2.3. Recovery Point Objective | The maximum data loss in the event of a catastrophic database failure must not exceed 15 minutes. |
| NFR.2.4. Fault tolerance | Failure of a third-party service must not block the operation of Core domains. Circuit Breaker and Retry patterns must be applied. |
| **NFR.3. Performance and Scalability** | |
| NFR.3.1. Response time | The 95th percentile of API response time for synchronous read/write operations must not exceed 300ms. |
| NFR.3.2. Schedule generation | The algorithm for searching the intersection of free slots must complete faster than 500ms even under load with a high number of doctors in the tenant. |
| NFR.3.3. Horizontal scaling | The architecture must support independent scaling of each Bounded Context as a separate microservice. |
| NFR.3.4. Asynchronous processing | All processes that do not require an immediate response must be executed asynchronously through the Kafka message broker. |
| **NFR.4. Interoperability** | |
| NFR.4.1. API First | All functions of the system must be accessible through a documented RESTful API according to the OpenAPI specification. |
| NFR.4.2. Medical data standardization | The system must support export/import of medical data in FHIR Release 4 resource format. |
| NFR.4.3. Webhooks | The system must be able to send unbounded webhooks for integration into external systems. |
| **NFR.5. Maintenance and Architecture** | |
| NFR.5.1 DDD compliance | The code structure must strictly reflect the defined Bounded Contexts. Connections between contexts must occur exclusively through clearly defined contracts (OHL, ACL). |
| NFR.6.2. CI/CD | The deployment process must be fully automated. |
| NFR.6.3. Observability | All services must export metrics, centralized logs, and support distributed tracing to track a request across various subdomains. |
| NFR.6.4. Databases | Each Bounded Context must own its own database and must not have direct access to the tables of other contexts (prohibition of shared database integration). |

### 2. ATAM analysis of architectural solutions

TODO

#### 2.1 Ranking of Key Qualities

TODO

#### 2.2 Quality attribute scenarios

TODO

#### 2.3 Analysis of Key Architectural Solutions

TODO

### 3. Microservices

TODO

### 4. TBD

TODO