## Domain

Contains shared logic that supports the tactical implementation of Domain-Driven Design. This package defines components that simplify the organization of domain models. It should only be referenced by other domain layers that define business logic. To use it, simply add a reference to `HauteCouture.Shared.Domain`.

### Auditing Domain Models

To track the history of changes to domain models, the `AuditableEntity<TId>` base class is provided. It allows tracking when a model was created, when it was modified, when it was deleted, and whether it is currently deleted. To comply with the DDD approach, state changes are performed only through dedicated methods. Inside the model these are: `MarkAsCreated`, `MarkAsUpdated`, and `MarkAsDeleted`; externally they are exposed through the `Delete` and `Restore` methods.

#### Entity Framework Core integration

To avoid duplicating configuration or introducing inconsistencies when configuring domain models that support auditing, a dedicated Entity Framework Core extension method is provided. It is applied to the entity configuration through `ConfigureAuditableEntity<TEntity, TId>`. This method only configures all properties inherited from `AuditableEntity<TId>`.

### Domain Exception

Whenever business rules are violated, invariants are broken, unsupported operations are performed, or values contain invalid data, an exception derived from `DomainException` should be thrown.