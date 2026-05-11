SET timezone = 'UTC';
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
-- Auth Service Schema
-- Create auth schema if it doesn't exist
CREATE SCHEMA IF NOT EXISTS auth;

-- Create Users table in auth schema
CREATE TABLE IF NOT EXISTS auth.Users (
    Id SERIAL PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Role INTEGER NOT NULL, -- 1: Candidate, 2: Recruiter, 3: Admin
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE
);

-- Add indexes for better performance
CREATE INDEX IF NOT EXISTS idx_users_email ON auth.Users(Email);
CREATE INDEX IF NOT EXISTS idx_users_role ON auth.Users(Role);
CREATE INDEX IF NOT EXISTS idx_users_created_at ON auth.Users(CreatedAt);

-- Add comments for better documentation
COMMENT ON TABLE auth.Users IS 'Users table for authentication service';
COMMENT ON COLUMN auth.Users.Id IS 'Unique identifier for the user';
COMMENT ON COLUMN auth.Users.Email IS 'User email address (unique)';
COMMENT ON COLUMN auth.Users.PasswordHash IS 'Hashed password using BCrypt';
COMMENT ON COLUMN auth.Users.Role IS 'User role: 1=Candidate, 2=Recruiter, 3=Admin';
COMMENT ON COLUMN auth.Users.CreatedAt IS 'Account creation timestamp';
COMMENT ON COLUMN auth.Users.UpdatedAt IS 'Last update timestamp';
-- Profile Service Schema
-- Create profile schema if it doesn't exist
CREATE SCHEMA IF NOT EXISTS profile;

-- Create Addresses table
CREATE TABLE IF NOT EXISTS profile.Addresses (
    Id SERIAL PRIMARY KEY,
    Street VARCHAR(255),
    City VARCHAR(100),
    State VARCHAR(100),
    Country VARCHAR(100),
    PostalCode VARCHAR(20),
    UserId INTEGER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES auth.Users(Id) ON DELETE CASCADE
);

-- Create CandidateProfiles table
CREATE TABLE IF NOT EXISTS profile.CandidateProfiles (
    Id SERIAL PRIMARY KEY,
    FullName VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    Mobile VARCHAR(20),
    Skills TEXT[], -- Array of skills
    Experience TEXT,
    ResumeUrl VARCHAR(500),
    Education TEXT,
    PortfolioUrl VARCHAR(500),
    LinkedInUrl VARCHAR(500),
    GitHubUrl VARCHAR(500),
    UserId INTEGER NOT NULL UNIQUE,
    AddressId INTEGER,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY (UserId) REFERENCES auth.Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (AddressId) REFERENCES profile.Addresses(Id) ON DELETE SET NULL
);

-- Create RecruiterProfiles table
CREATE TABLE IF NOT EXISTS profile.RecruiterProfiles (
    Id SERIAL PRIMARY KEY,
    FullName VARCHAR(255) NOT NULL,
    CompanyName VARCHAR(255) NOT NULL,
    Industry VARCHAR(100),
    Website VARCHAR(500),
    Description TEXT,
    CompanySize VARCHAR(50),
    Headquarters VARCHAR(255),
    UserId INTEGER NOT NULL UNIQUE,
    AddressId INTEGER,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY (UserId) REFERENCES auth.Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (AddressId) REFERENCES profile.Addresses(Id) ON DELETE SET NULL
);

-- Add indexes for better performance
CREATE INDEX IF NOT EXISTS idx_addresses_user_id ON profile.Addresses(UserId);
CREATE INDEX IF NOT EXISTS idx_candidate_profiles_user_id ON profile.CandidateProfiles(UserId);
CREATE INDEX IF NOT EXISTS idx_candidate_profiles_email ON profile.CandidateProfiles(Email);
CREATE INDEX IF NOT EXISTS idx_candidate_profiles_created_at ON profile.CandidateProfiles(CreatedAt);
CREATE INDEX IF NOT EXISTS idx_recruiter_profiles_user_id ON profile.RecruiterProfiles(UserId);
CREATE INDEX IF NOT EXISTS idx_recruiter_profiles_company_name ON profile.RecruiterProfiles(CompanyName);
CREATE INDEX IF NOT EXISTS idx_recruiter_profiles_created_at ON profile.RecruiterProfiles(CreatedAt);

-- Add comments for better documentation
COMMENT ON TABLE profile.Addresses IS 'Address information for users';
COMMENT ON TABLE profile.CandidateProfiles IS 'Candidate profile information';
COMMENT ON TABLE profile.RecruiterProfiles IS 'Recruiter profile information';
COMMENT ON COLUMN profile.CandidateProfiles.Skills IS 'Array of candidate skills';
COMMENT ON COLUMN profile.RecruiterProfiles.CompanySize IS 'Company size (e.g., Small, Medium, Large)';
-- Job Service Schema
-- Create job schema if it doesn't exist
CREATE SCHEMA IF NOT EXISTS job;

-- Create Jobs table
CREATE TABLE IF NOT EXISTS job.Jobs (
    Id SERIAL PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Category VARCHAR(100) NOT NULL,
    Type INTEGER NOT NULL, -- 1: FullTime, 2: PartTime, 3: Contract, 4: Internship, 5: Remote
    Location VARCHAR(255) NOT NULL,
    IsRemote BOOLEAN DEFAULT FALSE,
    SalaryMin DECIMAL(10,2) NOT NULL,
    SalaryMax DECIMAL(10,2),
    Currency VARCHAR(10) DEFAULT 'USD',
    Skills TEXT[], -- Array of required skills
    ExperienceRequired TEXT,
    Description TEXT NOT NULL,
    Requirements TEXT,
    Benefits TEXT,
    PostedBy INTEGER NOT NULL,
    Status INTEGER DEFAULT 1, -- 1: Active, 2: Inactive, 3: Closed
    PostedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    ExpiresAt TIMESTAMP WITH TIME ZONE,
    UpdatedAt TIMESTAMP WITH TIME ZONE,
    ViewCount INTEGER DEFAULT 0,
    ApplicationCount INTEGER DEFAULT 0,
    FOREIGN KEY (PostedBy) REFERENCES auth.Users(Id) ON DELETE CASCADE
);

-- Add indexes for better performance
CREATE INDEX IF NOT EXISTS idx_jobs_posted_by ON job.Jobs(PostedBy);
CREATE INDEX IF NOT EXISTS idx_jobs_category ON job.Jobs(Category);
CREATE INDEX IF NOT EXISTS idx_jobs_type ON job.Jobs(Type);
CREATE INDEX IF NOT EXISTS idx_jobs_location ON job.Jobs(Location);
CREATE INDEX IF NOT EXISTS idx_jobs_status ON job.Jobs(Status);
CREATE INDEX IF NOT EXISTS idx_jobs_posted_at ON job.Jobs(PostedAt);
CREATE INDEX IF NOT EXISTS idx_jobs_expires_at ON job.Jobs(ExpiresAt);
CREATE INDEX IF NOT EXISTS idx_jobs_salary_range ON job.Jobs(SalaryMin, SalaryMax);
CREATE INDEX IF NOT EXISTS idx_jobs_skills ON job.Jobs USING GIN(Skills);

-- Add comments for better documentation
COMMENT ON TABLE job.Jobs IS 'Job postings table';
COMMENT ON COLUMN job.Jobs.Type IS 'Job type: 1=FullTime, 2=PartTime, 3=Contract, 4=Internship, 5=Remote';
COMMENT ON COLUMN job.Jobs.Status IS 'Job status: 1=Active, 2=Inactive, 3=Closed';
COMMENT ON COLUMN job.Jobs.Skills IS 'Array of required skills using PostgreSQL array type';
COMMENT ON COLUMN job.Jobs.ViewCount IS 'Number of times the job has been viewed';
COMMENT ON COLUMN job.Jobs.ApplicationCount IS 'Number of applications received for this job';
-- Application Service Schema
-- Create application schema if it doesn't exist
CREATE SCHEMA IF NOT EXISTS application;

-- Create Applications table
CREATE TABLE IF NOT EXISTS application.Applications (
    Id SERIAL PRIMARY KEY,
    JobId INTEGER NOT NULL,
    CandidateId INTEGER NOT NULL,
    Status INTEGER DEFAULT 1, -- 1: Applied, 2: Shortlisted, 3: InterviewScheduled, 4: Offered, 5: Rejected
    CoverLetter TEXT,
    ResumeUrl VARCHAR(500),
    ExpectedSalary DECIMAL(10,2),
    Notes TEXT,
    AppliedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE,
    StatusChangedAt TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY (JobId) REFERENCES job.Jobs(Id) ON DELETE CASCADE,
    FOREIGN KEY (CandidateId) REFERENCES auth.Users(Id) ON DELETE CASCADE,
    UNIQUE (JobId, CandidateId) -- Prevent duplicate applications
);

-- Add indexes for better performance
CREATE INDEX IF NOT EXISTS idx_applications_job_id ON application.Applications(JobId);
CREATE INDEX IF NOT EXISTS idx_applications_candidate_id ON application.Applications(CandidateId);
CREATE INDEX IF NOT EXISTS idx_applications_status ON application.Applications(Status);
CREATE INDEX IF NOT EXISTS idx_applications_applied_at ON application.Applications(AppliedAt);
CREATE INDEX IF NOT EXISTS idx_applications_status_changed_at ON application.Applications(StatusChangedAt);
CREATE INDEX IF NOT EXISTS idx_applications_job_candidate ON application.Applications(JobId, CandidateId);

-- Add comments for better documentation
COMMENT ON TABLE application.Applications IS 'Job applications table';
COMMENT ON COLUMN application.Applications.Status IS 'Application status: 1=Applied, 2=Shortlisted, 3=InterviewScheduled, 4=Offered, 5=Rejected';
COMMENT ON COLUMN application.Applications.ExpectedSalary IS 'Expected salary of the candidate';
COMMENT ON COLUMN application.Applications.StatusChangedAt IS 'Timestamp when status was last changed';
-- Interview Service Schema
-- Create interview schema if it doesn't exist
CREATE SCHEMA IF NOT EXISTS interview;

-- Create Interviews table
CREATE TABLE IF NOT EXISTS interview.Interviews (
    Id SERIAL PRIMARY KEY,
    ApplicationId INTEGER NOT NULL,
    ScheduledAt TIMESTAMP WITH TIME ZONE NOT NULL,
    Mode INTEGER NOT NULL, -- 1: Video, 2: Phone, 3: InPerson
    MeetLink VARCHAR(500),
    Location VARCHAR(255),
    Status INTEGER DEFAULT 1, -- 1: Scheduled, 2: Confirmed, 3: Rescheduled, 4: Cancelled, 5: Completed
    Notes TEXT,
    ScheduledBy INTEGER NOT NULL,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY (ApplicationId) REFERENCES application.Applications(Id) ON DELETE CASCADE,
    FOREIGN KEY (ScheduledBy) REFERENCES auth.Users(Id) ON DELETE CASCADE
);

-- Add indexes for better performance
CREATE INDEX IF NOT EXISTS idx_interviews_application_id ON interview.Interviews(ApplicationId);
CREATE INDEX IF NOT EXISTS idx_interviews_scheduled_by ON interview.Interviews(ScheduledBy);
CREATE INDEX IF NOT EXISTS idx_interviews_scheduled_at ON interview.Interviews(ScheduledAt);
CREATE INDEX IF NOT EXISTS idx_interviews_status ON interview.Interviews(Status);
CREATE INDEX IF NOT EXISTS idx_interviews_mode ON interview.Interviews(Mode);
CREATE INDEX IF NOT EXISTS idx_interviews_created_at ON interview.Interviews(CreatedAt);

-- Add comments for better documentation
COMMENT ON TABLE interview.Interviews IS 'Interview scheduling table';
COMMENT ON COLUMN interview.Interviews.Mode IS 'Interview mode: 1=Video, 2=Phone, 3=InPerson';
COMMENT ON COLUMN interview.Interviews.Status IS 'Interview status: 1=Scheduled, 2=Confirmed, 3=Rescheduled, 4=Cancelled, 5=Completed';
COMMENT ON COLUMN interview.Interviews.MeetLink IS 'Video conference link for video interviews';
COMMENT ON COLUMN interview.Interviews.Location IS 'Physical location for in-person interviews';
-- Notification Service Schema
-- Create notification schema if it doesn't exist
CREATE SCHEMA IF NOT EXISTS notification;

-- Create Notifications table
CREATE TABLE IF NOT EXISTS notification.Notifications (
    Id SERIAL PRIMARY KEY,
    UserId INTEGER NOT NULL,
    Title VARCHAR(255) NOT NULL,
    Message TEXT NOT NULL,
    Type INTEGER NOT NULL, -- 1: ApplicationReceived, 2: ApplicationStatusChanged, 3: InterviewScheduled, 4: JobPosted, 5: System
    IsRead BOOLEAN DEFAULT FALSE,
    ActionUrl VARCHAR(500),
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    ReadAt TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY (UserId) REFERENCES auth.Users(Id) ON DELETE CASCADE
);

-- Add indexes for better performance
CREATE INDEX IF NOT EXISTS idx_notifications_user_id ON notification.Notifications(UserId);
CREATE INDEX IF NOT EXISTS idx_notifications_type ON notification.Notifications(Type);
CREATE INDEX IF NOT EXISTS idx_notifications_is_read ON notification.Notifications(IsRead);
CREATE INDEX IF NOT EXISTS idx_notifications_created_at ON notification.Notifications(CreatedAt);
CREATE INDEX IF NOT EXISTS idx_notifications_read_at ON notification.Notifications(ReadAt);

-- Add comments for better documentation
COMMENT ON TABLE notification.Notifications IS 'User notifications table';
COMMENT ON COLUMN notification.Notifications.Type IS 'Notification type: 1=ApplicationReceived, 2=ApplicationStatusChanged, 3=InterviewScheduled, 4=JobPosted, 5=System';
COMMENT ON COLUMN notification.Notifications.IsRead IS 'Whether the notification has been read';
COMMENT ON COLUMN notification.Notifications.ActionUrl IS 'URL to navigate to when notification is clicked';
-- Subscription Service Schema
-- Create subscription schema if it doesn't exist
CREATE SCHEMA IF NOT EXISTS subscription;

-- Create Subscriptions table
CREATE TABLE IF NOT EXISTS subscription.Subscriptions (
    Id SERIAL PRIMARY KEY,
    UserId INTEGER NOT NULL UNIQUE,
    Type INTEGER NOT NULL, -- 1: Basic, 2: Premium, 3: Enterprise
    Status INTEGER DEFAULT 1, -- 1: Active, 2: Cancelled, 3: Expired
    StartedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    ExpiresAt TIMESTAMP WITH TIME ZONE NOT NULL,
    Price DECIMAL(10,2) DEFAULT 0,
    Currency VARCHAR(10) DEFAULT 'USD',
    CancelledAt TIMESTAMP WITH TIME ZONE,
    UpdatedAt TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY (UserId) REFERENCES auth.Users(Id) ON DELETE CASCADE
);

-- Create Invoices table
CREATE TABLE IF NOT EXISTS subscription.Invoices (
    Id SERIAL PRIMARY KEY,
    SubscriptionId INTEGER NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Currency VARCHAR(10) DEFAULT 'USD',
    DueDate TIMESTAMP WITH TIME ZONE NOT NULL,
    PaidAt TIMESTAMP WITH TIME ZONE,
    PaymentMethod VARCHAR(100),
    TransactionId VARCHAR(255),
    Status INTEGER DEFAULT 1, -- 1: Pending, 2: Paid, 3: Overdue, 4: Cancelled
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (SubscriptionId) REFERENCES subscription.Subscriptions(Id) ON DELETE CASCADE
);

-- Add indexes for better performance
CREATE INDEX IF NOT EXISTS idx_subscriptions_user_id ON subscription.Subscriptions(UserId);
CREATE INDEX IF NOT EXISTS idx_subscriptions_type ON subscription.Subscriptions(Type);
CREATE INDEX IF NOT EXISTS idx_subscriptions_status ON subscription.Subscriptions(Status);
CREATE INDEX IF NOT EXISTS idx_subscriptions_expires_at ON subscription.Subscriptions(ExpiresAt);
CREATE INDEX IF NOT EXISTS idx_subscriptions_started_at ON subscription.Subscriptions(StartedAt);

CREATE INDEX IF NOT EXISTS idx_invoices_subscription_id ON subscription.Invoices(SubscriptionId);
CREATE INDEX IF NOT EXISTS idx_invoices_status ON subscription.Invoices(Status);
CREATE INDEX IF NOT EXISTS idx_invoices_due_date ON subscription.Invoices(DueDate);
CREATE INDEX IF NOT EXISTS idx_invoices_created_at ON subscription.Invoices(CreatedAt);
CREATE INDEX IF NOT EXISTS idx_invoices_transaction_id ON subscription.Invoices(TransactionId);

-- Add comments for better documentation
COMMENT ON TABLE subscription.Subscriptions IS 'User subscription plans';
COMMENT ON COLUMN subscription.Subscriptions.Type IS 'Subscription type: 1=Basic, 2=Premium, 3=Enterprise';
COMMENT ON COLUMN subscription.Subscriptions.Status IS 'Subscription status: 1=Active, 2=Cancelled, 3=Expired';
COMMENT ON COLUMN subscription.Subscriptions.Price IS 'Subscription price in the specified currency';

COMMENT ON TABLE subscription.Invoices IS 'Subscription invoices';
COMMENT ON COLUMN subscription.Invoices.Status IS 'Invoice status: 1=Pending, 2=Paid, 3=Overdue, 4=Cancelled';
COMMENT ON COLUMN subscription.Invoices.TransactionId IS 'Payment transaction identifier';
COMMENT ON COLUMN subscription.Invoices.PaymentMethod IS 'Payment method used (e.g., Credit Card, PayPal)';
-- Analytics Service Schema
-- Create analytics schema if it doesn't exist
CREATE SCHEMA IF NOT EXISTS analytics;

-- Create AnalyticsSummary table
CREATE TABLE IF NOT EXISTS analytics.AnalyticsSummary (
    Id SERIAL PRIMARY KEY,
    TotalJobs INTEGER DEFAULT 0,
    TotalApplications INTEGER DEFAULT 0,
    ShortlistedCount INTEGER DEFAULT 0,
    OfferedCount INTEGER DEFAULT 0,
    RejectedCount INTEGER DEFAULT 0,
    ViewToApplyRatio DECIMAL(10,2) DEFAULT 0,
    AverageTimeToHire DECIMAL(10,2) DEFAULT 0, -- in days
    ActiveJobs INTEGER DEFAULT 0,
    ClosedJobs INTEGER DEFAULT 0,
    TotalCandidates INTEGER DEFAULT 0,
    TotalRecruiters INTEGER DEFAULT 0,
    LastUpdated TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Add indexes for better performance
CREATE INDEX IF NOT EXISTS idx_analytics_summary_last_updated ON analytics.AnalyticsSummary(LastUpdated);

-- Add comments for better documentation
COMMENT ON TABLE analytics.AnalyticsSummary IS 'Analytics summary statistics for the platform';
COMMENT ON COLUMN analytics.AnalyticsSummary.ViewToApplyRatio IS 'Ratio of job views to applications';
COMMENT ON COLUMN analytics.AnalyticsSummary.AverageTimeToHire IS 'Average time in days from application to hire';
COMMENT ON COLUMN analytics.AnalyticsSummary.LastUpdated IS 'Timestamp when analytics were last calculated';

-- Insert initial analytics summary record
INSERT INTO analytics.AnalyticsSummary (
    TotalJobs, TotalApplications, ShortlistedCount, OfferedCount, 
    RejectedCount, ViewToApplyRatio, AverageTimeToHire, 
    ActiveJobs, ClosedJobs, TotalCandidates, TotalRecruiters
) VALUES (
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
) ON CONFLICT DO NOTHING;
