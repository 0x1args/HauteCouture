---
name: aspnet-testing
description: Defines the standards and best practices for writing unit and integration tests in the HauteCouture project, including testing approaches, patterns, tools, and conventions.
globs: ["tests/**/*.cs"]
---

## Purpose

Use this skill whenever unit or integration tests need to be written or reviewed. It defines the project's testing standards, best practices, naming conventions, recommended tools, and patterns to ensure reliable, maintainable, and comprehensive automated tests.

### Stack

- Use xUnit exclusively for writing tests. All tests must use xUnit.
- Use FluentAssertions for expressive and readable assertions. Always prefer FluentAssertions over the built in xUnit assertion methods whenever possible.
- Use NSubstitute exclusively for mocking dependencies. The use of Moq is strictly prohibited.
- Use Testcontainers, Respawn, and Microsoft.AspNetCore.Mvc.Testing for integration tests. Docker must be running before executing integration tests.

### Conventions to follow

- Test classes are named `<ClassName>UnitTests` or `<ClassName>IntegrationTests`, matching the class under test.
- Test methods are named `<Method>_Should<ExpectedBehavior>_When<Condition>`.
- Every test follows the Arrange Act Assert (AAA) pattern, with each section clearly separated by comments.
- Each test should verify a single behavior or scenario. Complex scenarios should be split into multiple focused tests rather than combined into one large test.
- Tests live under `/tests`, split into `Unit` and `Integration`, mirroring the structure of the module being tested.
- Unit tests must never access a real database, message broker, or network. All external dependencies must be mocked, stubbed, or faked.
- Integration tests must exercise real infrastructure (via Docker) instead of in memory substitutes to accurately validate the behavior of PostgreSQL, MongoDB, Kafka, and other infrastructure components.
- Domain level tests should verify behavior through the public API of the domain model (factory methods, domain methods, etc.) rather than accessing implementation details.
- Application level tests (handlers, services, etc.) should verify observable behavior and side effects (state changes, published events, returned results) rather than implementation details.

### Test writing checklist

- [ ] Does the test name clearly describe the scenario being verified?
- [ ] Does every new or changed public behavior have at least one corresponding test?
- [ ] Are both the happy path and realistic failure or edge cases covered (null values, empty collections, boundary values, invalid input, concurrent access where applicable)?
- [ ] Do domain level tests verify that business invariants are enforced (for example, invalid operations throw the expected domain exception)?
- [ ] Do command and query handler tests verify the resulting state and published events instead of internal implementation details or call sequences?
- [ ] Are all external dependencies properly mocked, stubbed, or faked in unit tests, with no accidental access to real infrastructure?
- [ ] Do integration tests validate the required real infrastructure (for example, schema per tenant PostgreSQL behavior, Kafka message flow through the Outbox and Inbox patterns, or Keycloak issued tokens)?
- [ ] Are tests fully independent and order agnostic, with no shared mutable state or reliance on execution order?
- [ ] Is the test data minimal, easy to understand, and directly related to the scenario being verified?
- [ ] Are assertions precise and specific, verifying the expected outcome rather than only confirming that no exception was thrown?
- [ ] Can the failed scenario be understood by reading only the test name, without opening the test body?