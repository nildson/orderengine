# OrderEngine

OrderEngine is a .NET 10 Minimal API project designed to manage orders with a clean architecture, focused on clarity, maintainability and business alignment. It combines CQRS with MediatR, SQLite persistence through EF Core, JWT authentication, FluentValidation, Serilog, xUnit tests, Docker, SonarQube and basic OpenTelemetry telemetry.

This README was written to serve two audiences at the same time:

- business/analyst readers who want to understand the business context, rules and functionality
- new developers who need a clear onboarding guide and local setup instructions

## 1. Business context

The project models a simple order management domain. It supports the main operational flows for an order lifecycle:

- create an order
- list all orders
- find an order by id
- update order status
- protect order access with authentication

From the business point of view, the application is intentionally small and pragmatic. The domain is limited to the items required by the project requirements:

- Order
  - Id
  - CustomerId
  - CreatedAt
  - Status
  - Items
  - Total
- OrderItem
  - Id
  - OrderId
  - ProductId
  - ProductName
  - Quantity
  - UnitPrice
  - Total
- OrderStatus
  - Pending
  - Confirmed
  - Cancelled

### Business rules

The order state machine is intentionally controlled by the domain model:

- Pending can be changed to Confirmed or Cancelled
- Confirmed cannot be cancelled
- Cancelled cannot be confirmed again
- Only valid status transitions are accepted
- An order must contain at least one item
- Quantity must be greater than zero
- UnitPrice must be greater than or equal to one
- Total is calculated in the domain from Quantity * UnitPrice

This ensures that business rules are not scattered throughout the API layer.

## 2. Solution architecture

The solution follows a clean architecture approach with a clear separation of concerns:

- Domain: business entities and business rules
- Application: commands, queries, handlers, contracts, validators and pipeline behaviors
- Infrastructure: EF Core and persistence implementation
- API: Minimal API endpoints, JWT auth, startup configuration and HTTP exposure
- Tests: xUnit coverage for logic and integration behavior

### Project structure

```text
OrderEngine/
├── OrderEngine.Domain/
│   └── OrderAggregate.cs
├── OrderEngine.Application/
│   ├── OrderContracts.cs
│   ├── ValidationBehavior.cs
│   ├── LoggingBehavior.cs
│   └── ...
├── OrderEngine.Infrastructure/
│   ├── OrderDbContext.cs
│   ├── EfCoreOrderRepository.cs
│   └── ...
├── OrderEngine.Api/
│   ├── Auth/
│   ├── Endpoints/
│   ├── Extensions/
│   ├── Program.cs
│   └── ...
├── OrderEngine.Tests/
│   ├── OrderServiceTests.cs
│   ├── OrderApiIntegrationTests.cs
│   └── ...
├── docker-compose.yml
├── docker-compose.sonarqube.yml
├── Dockerfile
├── OrderEngine.slnx
├── README.md
├── README-recruiter.md
└── .sonarqube/
```

## 3. Requirements implemented

The project was built to satisfy the required technical scope:

### 3.1 Clean architecture and Minimal API

- domain, application, infrastructure and API layers are separated
- API is implemented with Minimal API instead of controllers
- business logic is not embedded in endpoint code

### 3.2 CQRS with MediatR

- commands and queries are isolated in request contracts
- handlers centralize orchestration and business behavior
- the API layer remains thin and focused on HTTP concerns

### 3.3 EF Core + SQLite with automatic migrations

- SQLite is the database used for local development
- migrations are applied automatically during startup
- the database is created and kept aligned without manual migration execution in a normal local workflow

### 3.4 JWT authentication

- login endpoint is available at /auth/login
- a fixed developer user is validated using environment variables
- a JWT is returned and used to authorize access to protected order endpoints

### 3.5 FluentValidation pipeline behavior

- input validation is handled through a MediatR pipeline behavior
- invalid requests are rejected before command execution reaches business logic

### 3.6 Serilog logging behavior

- requests and responses are logged
- command/query execution time is measured
- exceptions are captured with logs for debugging

### 3.7 xUnit tests

- unit tests cover handlers and business behavior
- integration tests verify API responses with WebApplicationFactory

### 3.8 Docker support

- application can be run through Docker Compose
- a separate compose file exists for SonarQube and its PostgreSQL dependency

### 3.9 SonarQube analysis

- project can be analyzed locally through SonarQube
- the stack is isolated from the application container to avoid conflicts
- Although SonarQube and dotnet-sonarscanner do not solve the same issue, I choose SonarQube to present all data.

### 3.10 OpenTelemetry with console exporter

- ASP.NET Core instrumentation is enabled
- HttpClient instrumentation is enabled
- traces and metrics are exported to the console for local monitoring

## 4. Main technical stack

- .NET 10
- ASP.NET Core Minimal API
- EF Core
- SQLite
- MediatR
- FluentValidation
- JWT Bearer authentication
- Serilog
- xUnit
- Docker / Docker Compose
- SonarQube
- OpenTelemetry

## 5. Local setup

### Prerequisites

Make sure the following are installed locally:

- .NET 10 SDK
- Docker Desktop or Docker Engine
- Docker Compose
- optional: SonarScanner for .NET

### Required environment variables

For local execution, set the following values before running the API:

```powershell
$env:ORDERENGINE_AUTH_EMAIL = "dev@martech.com"
$env:ORDERENGINE_AUTH_PASSWORD = "Senha@123"
$env:ORDERENGINE_JWT_KEY = "local-dev-jwt-key-very-long-secret-value-2026"
$env:ORDERENGINE_JWT_ISSUER = "OrderEngine"
$env:ORDERENGINE_JWT_AUDIENCE = "OrderEngineAudience"
```

These values are also configured in the Docker compose files to keep local and containerized execution aligned.

## 6. Running the application

### Option A: local execution via dotnet

From the project root:

```powershell
cd C:\projects\orderengine

dotnet restore
dotnet build
dotnet run --project .\OrderEngine.Api\OrderEngine.Api.csproj --urls http://localhost:5164
```

The API will be available at:

```text
http://localhost:5164
```

### Option B: Docker Compose

```powershell
cd C:\projects\orderengine
docker compose up --build
```

The application service exposes:

```text
http://localhost:5164
```

## 7. Database behavior and migrations

The application initializes the database on startup:

```csharp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.Migrate();
}
```

This means that when the app starts, it checks the SQLite schema and applies the required migrations automatically, reducing the need for manual setup steps.

The `AddOrderIdToOrderItem` migration promotes the order-item relationship to the explicit `OrderItem.OrderId` property used by the domain and EF Core mapping.

## 8. Authentication and authorization

### Login endpoint

```http
POST /auth/login
Content-Type: application/json
```

Request example:

```json
{
  "email": "dev@martech.com",
  "password": "Senha@123"
}
```

Success response:

```json
{
  "token": "<jwt-token>"
}
```

The JWT is then sent in the Authorization header for protected routes:

```http
Authorization: Bearer <jwt-token>
```

### Protected routes

All order routes require authentication.

## 9. API endpoints

### 9.1 Get all orders

```http
GET /api/orders?page=1&pageSize=10
Authorization: Bearer <token>
```

The `page` parameter starts at 1. `pageSize` defaults to 10 and accepts values from 1 to 100.

### 9.2 Get order by id

```http
GET /api/orders/{id}
Authorization: Bearer <token>
```

### 9.3 Create order

```http
POST /api/orders
Authorization: Bearer <token>
Content-Type: application/json
```

Example body:

```json
{
  "customerId": "89f8d3ac-b94d-4512-aae9-c17727ccfc60",
  "items": [
    {
      "productId": "sku-1",
      "productName": "Keyboard",
      "quantity": 2,
      "unitPrice": 59.90
    }
  ]
}
```

### 9.4 Cancel order

```http
PATCH /api/orders/{id}/cancel
Authorization: Bearer <token>
```

The endpoint has no request body and cancels the order only when its current status is `Pending`.

## 10. Example requests with curl

### Login

```powershell
curl -X POST "http://localhost:5164/auth/login" `
  -H "Content-Type: application/json" `
  -d '{"email":"dev@martech.com","password":"Senha@123"}'
```

### List orders

```powershell
curl "http://localhost:5164/api/orders" `
  -H "Authorization: Bearer <token>"
```

### Create order

```powershell
curl -X POST "http://localhost:5164/api/orders" `
  -H "Authorization: Bearer <token>" `
  -H "Content-Type: application/json" `
  -d '{"customerId":"89f8d3ac-b94d-4512-aae9-c17727ccfc60","items":[{"productId":"sku-1","productName":"Keyboard","quantity":2,"unitPrice":59.90}]}'
```

### Cancel order

```powershell
curl -X PATCH "http://localhost:5164/api/orders/{id}/cancel" `
  -H "Authorization: Bearer <token>" `
```

## 11. Domain and business logic

The domain layer contains the core business rules and state transitions. This is important because the API and infrastructure layers do not decide whether a status change is valid. The dominant rule is: the domain entity decides.

This keeps the system consistent and easier to evolve.

## 12. Testing

The project includes xUnit tests under the OrderEngine.Tests project.

Run the test suite:

```powershell
cd C:\projects\orderengine
dotnet test OrderEngine.Tests\OrderEngine.Tests.csproj -nologo
```

The tests cover:

- command and query handlers
- validation behavior
- status transitions
- pagination behavior
- valid and invalid login credentials
- explicit OrderItem to Order relationships
- logging behavior
- integration scenarios via WebApplicationFactory

## 13. Logs and observability

### Serilog

The API is configured with Serilog and emits logs to the console.

The MediatR logging pipeline captures:

- request start
- request completion
- elapsed time
- exception details

### OpenTelemetry

The project includes basic OpenTelemetry instrumentation:

- ASP.NET Core instrumentation
- HttpClient instrumentation
- console exporter
- service name `OrderEngine.Api`

This gives local visibility into request traces and runtime metrics.

## 14. SonarQube

A dedicated Docker Compose file is provided for SonarQube.

Start it:

```powershell
cd C:\projects\orderengine
docker compose -f docker-compose.sonarqube.yml up -d
```

Open:

```text
http://localhost:9000
```

Run a standard scanner flow:

```powershell
$env:SONAR_TOKEN = "sqp_0868062e4b5560b1db1ea428d897c3dca61b8ac5"

dotnet sonarscanner begin /k:"orderengine" /n:"OrderEngine" /d:sonar.host.url="http://localhost:9000" /d:sonar.token="$env:SONAR_TOKEN"

dotnet test OrderEngine.Tests\OrderEngine.Tests.csproj -nologo

dotnet sonarscanner end /d:sonar.token="$env:SONAR_TOKEN"
```

The token above is used for the local SonarQube instance.

## 15. Troubleshooting

### Port already in use

If port 5164 is already allocated:

```powershell
netstat -ano | findstr :5164
```

Then stop the process that is using the port.

### Sonar authentication issue

If the scanner returns “Not authorized”, verify:

- the user is correct
- the password is literal and not interpreted as a variable
- the SonarQube instance is running

### API fails to start

Check the presence of required environment variables:

```powershell
$env:ORDERENGINE_AUTH_EMAIL
$env:ORDERENGINE_AUTH_PASSWORD
$env:ORDERENGINE_JWT_KEY
$env:ORDERENGINE_JWT_ISSUER
$env:ORDERENGINE_JWT_AUDIENCE
```

## 16. Why this project matters

For a business stakeholder, this project demonstrates a working order workflow with rules, persistence and secure access.

For a technical newcomer, it demonstrates how a modern .NET backend is structured with clean architecture, CQRS, validation, logging, tests and operational tooling.

## 17. Summary

OrderEngine is a compact but representative backend project that combines strong engineering principles with a realistic business domain. It is easy to understand for new contributors, while still showing the patterns and practices expected in a production-oriented .NET solution.

The main idea is simple: a small domain, well-defined rules, clear architecture, and a clean execution environment that supports local development, quality analysis and observability.

