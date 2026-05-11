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
