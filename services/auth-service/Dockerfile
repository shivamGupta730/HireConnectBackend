# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy complete repo
COPY . .

# Go to auth-service folder
WORKDIR /src/services/auth-service

# Restore
RUN dotnet restore HireConnect.AuthService.csproj

# Publish
RUN dotnet publish HireConnect.AuthService.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "HireConnect.AuthService.dll"]