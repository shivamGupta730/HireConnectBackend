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
