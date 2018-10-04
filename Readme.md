# HttpApiClientDemo

.Net Core solution created to Demonstrate the use of my HttpApiClient Library.
https://github.com/chrisckc/HttpApiClient.git

The projects were created using SDK v2.1.402

Plenty of comments and commented-out code to demonstrate various usage options

## Cloning the Demo

This demo includes the HttpApiClient as a Git Submodule

To clone use the --recursive option


## Running the Demo

Run the DemoServer first

```
cd DemoServer
export ASPNETCORE_ENVIRONMENT=development
dotnet run
```
or
```
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

Followed by the DemoClient

## DemoClient

DemoClient is a .Net Core Console App configured with DI

AutoMapper has been added to demonstrate mapping to POCO's or DTO's

DemoClient Project was created using:
```
dotnet new console
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Logging.Console
dotnet add package Microsoft.Extensions.DependencyInjection
```

The relevant code was then added to build the app configuration and the DI container

## DemoServer

DemoServer is a .Net Core Web Api project

DemoServer Project was created using:
```
dotnet new webapi
```
