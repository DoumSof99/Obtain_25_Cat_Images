# Steal 25 Cats game

## Overview
## A .NET 8 Web API project that fetches 25 cats from [TheCatAPI](https://thecatapi.com/), extracts and stores their image and temperament data, and provides a paginated, tag-filterable REST interface.

### Tech Stack
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQL Server (via Docker)
- AutoMapper
- FluentResults
- xUnit for testing
- Swagger (OpenAPI)

### 1. Clone the repository
```bash
git clone https://github.com/DoumSof99/Obtain_25_Cat_Images.git
cd Obtain_25_Cat_Images
```
### 2. Start SQL Server with Docker Container
- Ensure Docker is running, then from the project root (where docker-compose.yml is), run:
```bash
docker compose up -d
```
:grey_exclamation: _**Optional**, You can also connect to this server via SSMS or Azure Data Studio:_
```bash
Server: localhost,1433  
User: sa  
Password: Password1!
```
### 3. Apply Database Migrations
- In Visual Studio, use the .NET CLI to apply EF Core migrations and create the schema:
```bash
- dotnet build
- dotnet ef database update
```
:globe_with_meridians: _This creates a database named **CatDb** with the required tables._
