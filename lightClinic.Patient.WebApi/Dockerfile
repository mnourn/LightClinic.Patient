# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["lightClinic.Patient.WebApi/lightClinic.Patient.WebApi.csproj", "lightClinic.Patient.WebApi/"]
COPY ["lightClinic.Patient.Infrastructure/lightClinic.Patient.Infrastructure.csproj", "lightClinic.Patient.Infrastructure/"]
COPY ["lightClinic.Patient.Application/lightClinic.Patient.Application.csproj", "lightClinic.Patient.Application/"]
COPY ["lightClinic.Patient.Contracts/lightClinic.Patient.Contracts.csproj", "lightClinic.Patient.Contracts/"]
COPY ["lightClinic.Patient.Shared/lightClinic.Patient.Shared.csproj", "lightClinic.Patient.Shared/"]
COPY ["lightClinic.Patient.Domain/lightClinic.Patient.Domain.csproj", "lightClinic.Patient.Domain/"]
COPY ["lightClinic.Patient.Persistence/lightClinic.Patient.Persistence.csproj", "lightClinic.Patient.Persistence/"]
COPY ["lightClinic.Patient.Presentation/lightClinic.Patient.Presentation.csproj", "lightClinic.Patient.Presentation/"]
RUN dotnet restore "./lightClinic.Patient.WebApi/lightClinic.Patient.WebApi.csproj"
COPY . .
WORKDIR "/src/lightClinic.Patient.WebApi"
RUN dotnet build "./lightClinic.Patient.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./lightClinic.Patient.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "lightClinic.Patient.WebApi.dll"]