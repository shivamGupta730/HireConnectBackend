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
