### Api-Doc Endpoint

After successful start of the solution in any of above option, check useful endpoints:

* API interactive documentation - <http://localhost:5000/api-doc/>
* health check - <http://localhost:5000/weather>
* version - <http://localhost:5000/version>

### Start Project

Execute `dotnet run --project src/HappyCode.NetCoreBoilerplate.Api` in the root directory.

#### Execute with Docker 

* Docker Hub - <https://hub.docker.com/r/lkurzyniec/netcore-boilerplate>
* GitHub Container Registry - <https://github.com/lkurzyniec/netcore-boilerplate/pkgs/container/netcore-boilerplate>

Simply execute `docker run --rm -p 5000:8080 --name netcore-boilerplate lkurzyniec/netcore-boilerplate` to download and spin up a container.

#### Build your own image

To run in docker with your own image, execute `docker build . -t netcore-boilerplate:local` in the root directory to build an image,
and then `docker run --rm -p 5000:8080 --name netcore-boilerplate netcore-boilerplate:local` to spin up a container with it.

### Docker compose

> When running on `Linux` (i.e. [WSL](https://learn.microsoft.com/en-us/windows/wsl/install)), make sure that all docker files
([dockerfile](dockerfile), [docker-compose](docker-compose.yml) and all [mssql files](db/mssql)) have line endings `LF`.

Just execute `docker-compose up` command in the root directory.

#### Migrations

When the entire environment is up and running, you can additionally run a migration tool to add some new schema objects into MsSQL DB.
To do that, go to `src/HappyCode.NetCoreBoilerplate.Db` directory and execute `dotnet run` command.

## Architecture

### Api

[HappyCode.NetCoreBoilerplate.Api](src/HappyCode.NetCoreBoilerplate.Api)

* The entry point of the app - [Program.cs](src/HappyCode.NetCoreBoilerplate.Api/Program.cs)
* Simple Startup class - [Startup.cs](src/HappyCode.NetCoreBoilerplate.Api/Startup.cs)
  * Logging and Global exception middleware
  * MvcCore
  * DbContext (with MySQL)
  * DbContext (with MsSQL)
  * OpenAPI
  * HostedService and HttpClient
  * Core components and [Books module](#books-module) registration
  * FeatureManagement
  * HealthChecks
    * MySQL
    * MsSQL
* Infrastructure
  * `Banner` configuration place - [BannerConfigurator.cs](src/HappyCode.NetCoreBoilerplate.Api/Infrastructure/Configurations/BannerConfigurator.cs)
  * `Serilog` configuration place - [SerilogConfigurator.cs](src/HappyCode.NetCoreBoilerplate.Api/Infrastructure/Configurations/SerilogConfigurator.cs)
  * Filters
    * Simple `ApiKey` Authorization filter - [ApiKeyAuthorizationFilter.cs](src/HappyCode.NetCoreBoilerplate.Api/Infrastructure/Filters/ApiKeyAuthorizationFilter.cs)
    * MVC Global exception filter - [HttpGlobalExceptionFilter.cs](src/HappyCode.NetCoreBoilerplate.Api/Infrastructure/Filters/HttpGlobalExceptionFilter.cs)
  * Logging
    * Custom enricher to have version properties in logs - [VersionEnricher.cs](src/HappyCode.NetCoreBoilerplate.Api/Infrastructure/Logging/VersionEnricher.cs)
  * Middlewares
    * Simple middleware - [ConnectionInfoMiddleware.cs](src/HappyCode.NetCoreBoilerplate.Api/Infrastructure/Middlewares/ConnectionInfoMiddleware.cs)
    * Global exception handler - [ExceptionMiddleware.cs](src/HappyCode.NetCoreBoilerplate.Api/Infrastructure/Middlewares/ExceptionMiddleware.cs)
  * `OpenAPI`
    * Registration place - [OpenApiRegistrations.cs](src/HappyCode.NetCoreBoilerplate.Api/Infrastructure/OpenApi/OpenApiRegistrations.cs)
    * Mark disabled feature as Deprecated - [FeatureFlagOperationTransformer.cs](src/HappyCode.NetCoreBoilerplate.Api/Infrastructure/OpenApi/FeatureFlagOperationTransformer.cs)
    * Remove Deprecated operations - [RemoveDeprecatedDocumentTransformer.cs](src/HappyCode.NetCoreBoilerplate.Api/Infrastructure/OpenApi/RemoveDeprecatedDocumentTransformer.cs)
    * Add security requirement - [SecurityRequirementOperationTransformer.cs](src/HappyCode.NetCoreBoilerplate.Api/Infrastructure/OpenApi/SecurityRequirementOperationTransformer.cs)
  * Simple custom middleware that logs connection info - [ConnectionInfoMiddleware.cs](src/HappyCode.NetCoreBoilerplate.Api/Infrastructure/Middlewares/ConnectionInfoMiddleware.cs)
* Simple exemplary API controllers - [WeatherController.cs](src/HappyCode.NetCoreBoilerplate.Api/Controllers/WeatherController.cs), [CarsController.cs](src/HappyCode.NetCoreBoilerplate.Api/Controllers/CarsController.cs), [PingsController.cs](src/HappyCode.NetCoreBoilerplate.Api/Controllers/PingsController.cs)
* Example of BackgroundService - [PingWebsiteBackgroundService.cs](src/HappyCode.NetCoreBoilerplate.Api/BackgroundServices/PingWebsiteBackgroundService.cs)

![HappyCode.NetCoreBoilerplate.Api](.assets/api.png "HappyCode.NetCoreBoilerplate.Api")

### Core

[HappyCode.NetCoreBoilerplate.Core](src/HappyCode.NetCoreBoilerplate.Core)

* Models
  * Dto models
  * DB models
  * AppSettings models - [Settings](src/HappyCode.NetCoreBoilerplate.Core/Settings)
* DbContexts
  * MySQL DbContext - [WeatherContext.cs](src/HappyCode.NetCoreBoilerplate.Core/WeatherContext.cs)
  * MsSQL DbContext - [CarsContext.cs](src/HappyCode.NetCoreBoilerplate.Core/CarsContext.cs)
* Providers
  * Version provider - [VersionProvider.cs](src/HappyCode.NetCoreBoilerplate.Core/Providers/VersionProvider.cs)
* Core registrations - [CoreRegistrations.cs](src/HappyCode.NetCoreBoilerplate.Core/Registrations/CoreRegistrations.cs)
* Exemplary MySQL repository - [WeatherRepository.cs](src/HappyCode.NetCoreBoilerplate.Core/Repositories/WeatherRepository.cs)
* Exemplary MsSQL service - [CarService.cs](src/HappyCode.NetCoreBoilerplate.Core/Services/CarService.cs)

![HappyCode.NetCoreBoilerplate.Core](.assets/core.png "HappyCode.NetCoreBoilerplate.Core")

## DB Migrations

[HappyCode.NetCoreBoilerplate.Db](src/HappyCode.NetCoreBoilerplate.Db)

* Console application as a simple db migration tool - [Program.cs](src/HappyCode.NetCoreBoilerplate.Db/Program.cs)
* Sample migration scripts, both `.sql` and `.cs` - [S001_AddCarTypesTable.sql](src/HappyCode.NetCoreBoilerplate.Db/Scripts/Sql/S001_AddCarTypesTable.sql), [S002_ModifySomeRows.cs](src/HappyCode.NetCoreBoilerplate.Db/Scripts/Code/S002_ModifySomeRows.cs)

![HappyCode.NetCoreBoilerplate.Db](.assets/db.png "HappyCode.NetCoreBoilerplate.Db")

## Tests

### Integration tests

[HappyCode.NetCoreBoilerplate.Api.IntegrationTests](test/HappyCode.NetCoreBoilerplate.Api.IntegrationTests)

* Infrastructure
  * Fixture with TestServer - [TestServerClientFixture.cs](test/HappyCode.NetCoreBoilerplate.Api.IntegrationTests/Infrastructure/TestServerClientFixture.cs)
  * TestStartup with InMemory databases - [TestStartup.cs](test/HappyCode.NetCoreBoilerplate.Api.IntegrationTests/Infrastructure/TestStartup.cs)
  * Simple data feeders - [WeatherContextDataFeeder.cs](test/HappyCode.NetCoreBoilerplate.Api.IntegrationTests/Infrastructure/DataFeeders/WeatherContextDataFeeder.cs), [CarsContextDataFeeder.cs](test/HappyCode.NetCoreBoilerplate.Api.IntegrationTests/Infrastructure/DataFeeders/CarsContextDataFeeder.cs)
  * Fakes - [FakePingService.cs](test/HappyCode.NetCoreBoilerplate.Api.IntegrationTests/Infrastructure/Fakes/FakePingService.cs)
* Exemplary tests - [WeatherTests.cs](test/HappyCode.NetCoreBoilerplate.Api.IntegrationTests/WeatherTests.cs), [CarsTests.cs](test/HappyCode.NetCoreBoilerplate.Api.IntegrationTests/CarsTests.cs), [PingsTests.cs](test/HappyCode.NetCoreBoilerplate.Api.IntegrationTests/PingsTests.cs)

![HappyCode.NetCoreBoilerplate.Api.IntegrationTests](.assets/itests.png "HappyCode.NetCoreBoilerplate.Api.IntegrationTests")

### Unit tests

[HappyCode.NetCoreBoilerplate.Api.UnitTests](test/HappyCode.NetCoreBoilerplate.Api.UnitTests)

* Exemplary tests - [WeatherControllerTests.cs](test/HappyCode.NetCoreBoilerplate.Api.UnitTests/Controllers/WeatherControllerTests.cs), [CarsControllerTests.cs](test/HappyCode.NetCoreBoilerplate.Api.UnitTests/Controllers/CarsControllerTests.cs), [PingsControllerTests.cs](test/HappyCode.NetCoreBoilerplate.Api.UnitTests/Controllers/PingsControllerTests.cs)
* API Infrastructure Unit tests
  * [ApiKeyAuthorizationFilterTests.cs](test/HappyCode.NetCoreBoilerplate.Api.UnitTests/Infrastructure/Filters/ApiKeyAuthorizationFilterTests.cs)
  * [ValidateModelStateFilterTests.cs](test/HappyCode.NetCoreBoilerplate.Api.UnitTests/Infrastructure/Filters/ValidateModelStateFilterTests.cs)
  * [VersionEnricherTests.cs](test/HappyCode.NetCoreBoilerplate.Api.UnitTests/Infrastructure/Logging/VersionEnricherTests.cs)

[HappyCode.NetCoreBoilerplate.Core.UnitTests](test/HappyCode.NetCoreBoilerplate.Core.UnitTests)

* Extension methods to mock `DbSet` faster - [EnumerableExtensions.cs](test/HappyCode.NetCoreBoilerplate.Core.UnitTests/Extensions/EnumerableExtensions.cs)
* Exemplary tests - [WeatherRepositoryTests.cs](test/HappyCode.NetCoreBoilerplate.Core.UnitTests/Repositories/WeatherRepositoryTests.cs), [CarServiceTests.cs](test/HappyCode.NetCoreBoilerplate.Core.UnitTests/Services/CarServiceTests.cs)
* Providers tests
  * [VersionProviderTests.cs](test/HappyCode.NetCoreBoilerplate.Core.UnitTests/Providers/VersionProviderTests.cs) with [HappyCode.NetCoreBoilerplate.Core.UnitTests.runsettings](test/HappyCode.NetCoreBoilerplate.Core.UnitTests/HappyCode.NetCoreBoilerplate.Core.UnitTests.runsettings)

![HappyCode.NetCoreBoilerplate.Core.UnitTests](.assets/utests.png "HappyCode.NetCoreBoilerplate.Core.UnitTests")

### Architectural tests

[HappyCode.NetCoreBoilerplate.ArchitecturalTests](test/HappyCode.NetCoreBoilerplate.ArchitecturalTests)

* Exemplary tests - [ApiArchitecturalTests.cs](test/HappyCode.NetCoreBoilerplate.ArchitecturalTests/ApiArchitecturalTests.cs), [CoreArchitecturalTests.cs](test/HappyCode.NetCoreBoilerplate.ArchitecturalTests/CoreArchitecturalTests.cs)

![HappyCode.NetCoreBoilerplate.ArchitecturalTests](.assets/atests.png "HappyCode.NetCoreBoilerplate.ArchitecturalTests")

### Integration Tests

[HappyCode.NetCoreBoilerplate.BooksModule.IntegrationTests](test/HappyCode.NetCoreBoilerplate.BooksModule.IntegrationTests)

* Infrastructure
  * Fixture with TestServer - [TestServerClientFixture.cs](test/HappyCode.NetCoreBoilerplate.BooksModule.IntegrationTests/Infrastructure/TestServerClientFixture.cs)

## To Do

* any idea? Please [create an issue](https://github.com/lkurzyniec/netcore-boilerplate/issues/new).

## Be like a star, give me a star! :star:

If:

* you like this repo/code,
* you learn something,
* you are using it in your project/application,

then please give me a `star`, appreciate my work. Thanks!
