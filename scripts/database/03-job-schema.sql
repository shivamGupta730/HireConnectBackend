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
