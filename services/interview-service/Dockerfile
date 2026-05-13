# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared library project
COPY src/HireConnect.Shared/ src/HireConnect.Shared/

# Copy interview-service project
COPY services/interview-service/ services/interview-service/

# Restore dependencies
WORKDIR /src/services/interview-service
RUN dotnet restore HireConnect.InterviewService.csproj

# Publish in Release mode
RUN dotnet publish HireConnect.InterviewService.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose port 8080
EXPOSE 8080

# Set ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "HireConnect.InterviewService.dll"]
