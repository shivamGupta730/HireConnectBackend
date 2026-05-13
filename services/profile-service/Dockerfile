# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared library project
COPY src/HireConnect.Shared/ src/HireConnect.Shared/

# Copy profile-service project
COPY services/profile-service/ services/profile-service/

# Restore dependencies
WORKDIR /src/services/profile-service
RUN dotnet restore HireConnect.ProfileService.csproj

# Publish in Release mode
RUN dotnet publish HireConnect.ProfileService.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose port 8080
EXPOSE 8080

# Set ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "HireConnect.ProfileService.dll"]
