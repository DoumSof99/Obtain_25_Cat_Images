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
### 3. Apply the XML file for API Documentation:
- In the path _ROOT\Obtain_25_Cat_Images\XML file_, there is a file with name: _Obtain_25_Cat_Images.xml_
- Copy this and paste it to this path: ROOT\Obtain_25_Cat_Images\Obtain_25_Cat_Images\bin\Debug\net8.0
- Then in **Visual Studio**, go to -> Project -> Obtain_25_Cat_Images Properties -> Build, and find the properties:
```bash
- Documentation file: check it
- XML Documentation file path, and select this path (where you placed the Obtain_25_Cat_Images.xml file): ..\ROOT\Obtain_25_Cat_Images\Obtain_25_Cat_Images\bin\Debug\net8.0\Obtain_25_Cat_Images.xml
- Save
```
**- It should look like this:**
![image](https://github.com/user-attachments/assets/f43202e3-87eb-4363-9e42-49c838b0862c)

### 4. Apply Database Migrations
- In Visual Studio, use the .NET CLI to apply EF Core migrations and create the schema:
```bash
- dotnet build
- cd Obtain_25_Cat_Images
- dotnet ef database update
```
:globe_with_meridians: _This creates a database named **CatDb** with the required tables._

:warning: If this error occurred: 
```bash
Could not execute because the specified command or file was not found
```
- First, run this command and try again:
```bash
dotnet tool install --global dotnet-ef
```
### 5. Add Your TheCatAPI Key
- In the CLI of Visual Studio, _cd_ to the path that the file _appsettings.Development.template.json_ exists and run:
```bash
cp .\appsettings.Development.template.json .\appsettings.Development.json
```
1. A new file will be generated: _appsettings.Development.json_
2. Then visit the site: _https://developers.thecatapi.com/view-account/ylX4blBYT9FaoVd6OhvR?report=bOoHBz-8t_
3. Then go to **Start Here** section.
4. Follow steps 1 and 2. Then copy your _ApiKey_ and place it in the _appsettings.Development.json_, where it says **<YOUR_API_KEY>**
### 6. Run the Web API
- Make sure you're inside the root of the project _(where .csproj is)_, then run:
```bash
dotnet run
```
- This will start the Web API. Once the app is running, open your browser and navigate to: **_http://localhost:<your_port>/swagger_**
- Example: If it says Now listening on _http://localhost:5072_, go to: _http://localhost:5072/swagger_
### 7. Available API Endpoints (Swagger)
- POST /api/cats/fetch  -> Fetches 25 cat images with breed info from TheCatAPI and stores them into the local database.
- GET /api/cats         -> Returns a list of cats stored in the database, supports pagination and filtering by temperament tag.
- GET /api/cats/{id}    -> Returns a single cat by its internal DB ID (not CatAPI's ID).
### 8. Running Unit & Integration Tests
- To run all tests (unit + integration), navigate to the Test Explorer in VS, and run all the tests:
![image](https://github.com/user-attachments/assets/12c04299-2e9c-4b23-a49c-e919fe4f1879)

## :dart: Troubleshooting Tips:
- Make sure your Docker container is running when testing endpoints that interact with the database.
- If you see database-related errors during tests, ensure migrations are applied correctly with:
```bash
dotnet ef database update
```
- If the API key is missing, the fetch endpoint will fail. Ensure your appsettings.Development.json contains a valid key.
