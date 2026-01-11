📦 Inventory and Order App

Overview

The Inventory and Order App is a backend-driven solution designed to manage product inventories, customer orders, and fulfillment workflows. It emphasizes scalability, security, and maintainability, leveraging modern cloud-native practices and industry-standard patterns.

Features:

Inventory ManagementTrack stock levels, product details, and availability in real time.

Order ProcessingCreate, update, and fulfill customer orders with robust validation.

Authentication & AuthorizationSecure endpoints with JWT-based authentication and role-based authorization.

Error Handling MiddlewareConsistent, maintainable error responses across APIs.

DTOs for separation between domain models and API contracts, avoiding serialization cycles.

Caching Strategies

Version tokens for cache invalidation

Dynamic cache keys for paginated endpoints

Database ManagementEF Core migrations for schema evolution and query optimization.

🛠️ Tech Stack

Layer

Technology

Backend Framework

.NET 8 (ASP.NET Core)

Database

SQLite (EF Core ORM)

Authentication

JWT, Role-based Authorization

Caching

Distributed cache with version tokens & dynamic keys


⚙️ Setup & Installation

Clone the repository

git clone https://github.com/ChrisPerz/InventaryAndOrderApp
cd inventory-order-app

Configure environment variables in appsettings.json for example:

"Jwt": {
    "Issuer": "IssuerData",
    "Audience": "LogiTrackClient",
    "Key": "yourkey",
    "ExpirationMinutes": 60
  }

Apply EF Core migrations

dotnet ef database update

Install libraries.

Run the app

dotnet run 

📜 License

MIT License – free to use, modify, and distribute.