## Exceptions

Provides a unified set of exceptions for representing failures across different application layers. The package standardizes exception types for client, server, and integration scenarios, allowing error handling middleware to consistently map exceptions to appropriate HTTP responses. This package should be referenced by any layer that needs to signal application errors. To use it, simply add a reference to `HauteCouture.Shared.Common.Exceptions`.

### Client Exceptions

Client exceptions represent failures caused by an invalid request or insufficient client permissions. They typically correspond to 4xx HTTP status codes.

| Exception | HTTP Status | Description |
|---|---:|---|
| BadRequestException | 400 | The request contains invalid data or cannot be processed. |
| UnauthorizedException | 401 | Authentication is required or has failed. |
| ForbiddenException | 403 | The authenticated user does not have permission to perform the operation. |
| NotFoundException | 404 | The requested resource could not be found. |
| ConflictException | 409 | The request conflicts with the current state of the resource. |
| GoneException | 410 | The requested resource is no longer available and has been permanently removed. |
| TooManyRequestsException | 429 | The client has exceeded the allowed request rate. |

### Integration Exceptions

Integration exceptions represent failures that occur while communicating with external systems, APIs, or services.

| Exception | Description |
|---|---|
| ExternalAuthException | Authentication or authorization with an external service has failed. |
| ExternalClientException | The external system rejected the request due to a client-side error. |
| ExternalTimeoutException | The external service did not respond within the expected time. |
| ExternalUnavailableException | The external dependency is currently unavailable. |

These exceptions make it possible to distinguish integration failures from internal application failures and apply dedicated retry or fallback strategies where appropriate.

### Server Exceptions

Server exceptions represent unexpected failures that originate inside the application itself. They typically correspond to 5xx HTTP status codes.

| Exception | HTTP Status | Description |
|---|---:|---|
| InternalServerErrorException | 500 | An unexpected server-side error occurred. |
| ServiceUnavailableException | 503 | The service is temporarily unavailable and cannot process requests. |

These exceptions should be used only when the failure originates within the application rather than from client input or external integrations.