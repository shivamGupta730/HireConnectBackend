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
