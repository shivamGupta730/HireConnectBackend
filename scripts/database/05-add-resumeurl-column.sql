-- Add resumeurl column to candidates table
ALTER TABLE profile.candidates ADD COLUMN IF NOT EXISTS resumeurl TEXT;

-- Add resumeurl column to recruiters table (for future use)
ALTER TABLE profile.recruiters ADD COLUMN IF NOT EXISTS resumeurl TEXT;
