# HireConnect Dependencies Installation Script
# This script installs all required NuGet packages for all microservices

Write-Host "Starting HireConnect Dependencies Installation..." -ForegroundColor Green

# Function to restore packages for a project
function Restore-Packages {
    param(
        [string]$ProjectPath
    )
    
    Write-Host "Restoring packages for: $ProjectPath" -ForegroundColor Yellow
    Set-Location $ProjectPath
    
    try {
        dotnet restore --no-cache --force
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Successfully restored packages for: $ProjectPath" -ForegroundColor Green
        } else {
            Write-Host "Failed to restore packages for: $ProjectPath" -ForegroundColor Red
            return $false
        }
    } catch {
        Write-Host "Error restoring packages for: $ProjectPath - $_" -ForegroundColor Red
        return $false
    }
    
    return $true
}

# Get the root directory
$RootDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $RootDir\..

# List of all project paths
$Projects = @(
    "src\HireConnect.Shared",
    "services\auth-service",
    "services\profile-service", 
    "services\job-service",
    "services\application-service",
    "services\interview-service",
    "services\notification-service",
    "services\subscription-service",
    "services\analytics-service",
    "services\api-gateway"
)

$SuccessCount = 0
$TotalCount = $Projects.Count

Write-Host "Found $TotalCount projects to process..." -ForegroundColor Cyan

# Restore packages for each project
foreach ($Project in $Projects) {
    $ProjectPath = Join-Path $RootDir\.. $Project
    
    if (Test-Path $ProjectPath) {
        if (Restore-Packages -ProjectPath $ProjectPath) {
            $SuccessCount++
        }
    } else {
        Write-Host "Project path not found: $ProjectPath" -ForegroundColor Red
    }
}

# Build the entire solution
Write-Host "`nBuilding the entire solution..." -ForegroundColor Yellow
Set-Location $RootDir\..

try {
    dotnet build HireConnect.sln --no-restore --configuration Release
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Solution built successfully!" -ForegroundColor Green
        $SuccessCount++
    } else {
        Write-Host "Solution build failed!" -ForegroundColor Red
    }
} catch {
    Write-Host "Error building solution: $_" -ForegroundColor Red
}

# Summary
Write-Host "`n=== Installation Summary ===" -ForegroundColor Cyan
Write-Host "Projects processed: $SuccessCount/$TotalCount" -ForegroundColor White
Write-Host "Solution build: $(if ($LASTEXITCODE -eq 0) { 'Success' } else { 'Failed' })" -ForegroundColor White

if ($SuccessCount -eq $TotalCount -and $LASTEXITCODE -eq 0) {
    Write-Host "`nAll dependencies installed successfully!" -ForegroundColor Green
    Write-Host "You can now run the microservices." -ForegroundColor Green
} else {
    Write-Host "`nSome dependencies failed to install. Check the errors above." -ForegroundColor Red
}

Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Update connection strings in appsettings.json files" -ForegroundColor White
Write-Host "2. Run database migrations" -ForegroundColor White
Write-Host "3. Start individual services or use the API Gateway" -ForegroundColor White
