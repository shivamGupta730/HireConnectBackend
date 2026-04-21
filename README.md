# HireConnect Backend System

A complete microservices-based backend system for a Job Portal application built with ASP.NET Core (.NET 8) and PostgreSQL.

## Architecture Overview

This system follows a microservices architecture with 9 independent services:

1. **Auth Service** (Port 5001) - User authentication and JWT token management
2. **Profile Service** (Port 5002) - Candidate and Recruiter profile management
3. **Job Service** (Port 5003) - Job posting and search functionality
4. **Application Service** (Port 5004) - Job application management
5. **Interview Service** (Port 5005) - Interview scheduling and management
6. **Notification Service** (Port 5006) - User notifications and email alerts
7. **Subscription Service** (Port 5007) - Subscription plans and invoicing
8. **Analytics Service** (Port 5008) - Platform analytics and reporting
9. **API Gateway** (Port 5000) - Single entry point using Ocelot

## Database Schema

The system uses PostgreSQL with separate schemas for each service:
- `auth` - Users table
- `profile` - CandidateProfile, RecruiterProfile, Address tables
- `job` - Jobs table
- `application` - Applications table
- `interview` - Interviews table
- `notification` - Notifications table
- `subscription` - Subscriptions, Invoices tables
- `analytics` - AnalyticsSummary table

## Prerequisites

- .NET 8.0 SDK
- PostgreSQL (Neon DB recommended)
- Visual Studio 2022 or VS Code
- Git

## Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd HireConnectBackend
```

### 2. Database Setup

1. **Create PostgreSQL Database**
   ```sql
   CREATE DATABASE hireconnect;
   ```

2. **Update Connection Strings**
   Replace `PASTE_YOUR_NEON_CONNECTION_STRING_HERE` in all `appsettings.json` files with your actual connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=your-host;Database=hireconnect;Username=your-username;Password=your-password"
   }
   ```

3. **Run Database Migrations**
   ```bash
   cd scripts/database
   psql -h your-host -U your-username -d hireconnect -f run-migrations.sql
   ```

### 3. Configuration

1. **JWT Settings**
   Update the `JwtSettings` section in all `appsettings.json` files:
   ```json
   "JwtSettings": {
     "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
     "Issuer": "HireConnect",
     "Audience": "HireConnectUsers",
     "ExpirationMinutes": 60
   }
   ```

2. **SendGrid (Optional)**
   For email notifications, update the `SendGrid` section in Notification Service:
   ```json
   "SendGrid": {
     "ApiKey": "YOUR_SENDGRID_API_KEY_HERE",
     "FromEmail": "noreply@hireconnect.com",
     "FromName": "HireConnect"
   }
   ```

### 4. Build and Run

1. **Build the Solution**
   ```bash
   dotnet build HireConnect.sln
   ```

2. **Run Individual Services**
   ```bash
   # Auth Service
   cd services/auth-service
   dotnet run

   # Profile Service
   cd ../profile-service
   dotnet run

   # Continue for all services...
   ```

3. **Run API Gateway**
   ```bash
   cd services/api-gateway
   dotnet run
   ```

### 5. Testing

All services include Swagger documentation. Access them at:
- API Gateway: `http://localhost:5000`
- Auth Service: `http://localhost:5001/swagger`
- Profile Service: `http://localhost:5002/swagger`
- And so on...

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `GET /api/auth/profile` - Get user profile
- `POST /api/auth/validate` - Validate JWT token

### Profiles
- `GET /api/profile/candidate` - Get candidate profile
- `POST /api/profile/candidate` - Create candidate profile
- `PUT /api/profile/candidate` - Update candidate profile
- `GET /api/profile/recruiter` - Get recruiter profile
- `POST /api/profile/recruiter` - Create recruiter profile
- `PUT /api/profile/recruiter` - Update recruiter profile

### Jobs
- `GET /api/job` - Get all jobs
- `GET /api/job/{id}` - Get job by ID
- `POST /api/job` - Create new job (Recruiter)
- `PUT /api/job/{id}` - Update job (Recruiter)
- `DELETE /api/job/{id}` - Delete job (Recruiter)
- `GET /api/job/search` - Search jobs
- `GET /api/job/my-jobs` - Get recruiter's jobs

### Applications
- `POST /api/application` - Apply for job (Candidate)
- `GET /api/application/my-applications` - Get candidate's applications
- `GET /api/application/job/{jobId}` - Get applications for job (Recruiter)
- `PUT /api/application/{id}/status` - Update application status (Recruiter)

### Interviews
- `POST /api/interview` - Schedule interview (Recruiter)
- `POST /api/interview/{id}/confirm` - Confirm interview (Candidate)
- `POST /api/interview/{id}/reschedule` - Reschedule interview
- `POST /api/interview/{id}/cancel` - Cancel interview

### Notifications
- `GET /api/notification` - Get user notifications
- `POST /api/notification` - Create notification
- `POST /api/notification/{id}/mark-read` - Mark as read
- `POST /api/notification/mark-all-read` - Mark all as read

### Subscriptions
- `POST /api/subscription` - Create subscription
- `GET /api/subscription/my-subscription` - Get user subscription
- `POST /api/subscription/cancel` - Cancel subscription
- `POST /api/subscription/renew` - Renew subscription

### Analytics
- `GET /api/analytics/summary` - Get platform analytics
- `GET /api/analytics/job/{jobId}` - Get job analytics
- `GET /api/analytics/recruiter/{recruiterId}` - Get recruiter analytics
- `GET /api/analytics/applications/status-breakdown` - Application status breakdown

## User Roles

- **Candidate (1)**: Can view jobs, apply, manage profile, view applications
- **Recruiter (2)**: Can post jobs, view applications, schedule interviews, manage company profile
- **Admin (3)**: Full access to all endpoints and analytics

## Inter-Service Communication

Services communicate via HTTP calls:
- Application Service validates jobs via Job Service
- Notification Service triggered by Application and Interview Services
- Analytics Service aggregates data from all services

## Security

- JWT-based authentication
- BCrypt password hashing
- Role-based authorization
- HTTPS enforcement
- CORS configuration

## Deployment

Each service is containerizable and can be deployed independently. The API Gateway provides a single entry point for the entire system.

## Development Notes

- All services follow clean architecture: Entity -> Repository -> Service -> Controller
- Dependency injection is used throughout
- Swagger documentation is enabled for all services
- Proper error handling and logging
- Database migrations are versioned and repeatable

## Troubleshooting

1. **Database Connection Issues**: Verify connection strings and PostgreSQL server is running
2. **JWT Token Issues**: Check that secret keys match across all services
3. **Service Communication**: Ensure all services are running on correct ports
4. **Migration Errors**: Run migration scripts in correct order

## Contributing

1. Follow the existing architecture patterns
2. Add proper error handling and logging
3. Update documentation for new features
4. Test thoroughly before committing
