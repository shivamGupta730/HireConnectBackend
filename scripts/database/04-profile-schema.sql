-- Create profile schema
CREATE SCHEMA IF NOT EXISTS profile;

-- Create candidates table
CREATE TABLE IF NOT EXISTS profile.candidates (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    skills TEXT,
    experience_years INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_candidates_user_id FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE
);

-- Create recruiters table
CREATE TABLE IF NOT EXISTS profile.recruiters (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    company_name VARCHAR(255) NOT NULL,
    designation VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_recruiters_user_id FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_candidates_user_id ON profile.candidates(user_id);
CREATE INDEX IF NOT EXISTS idx_recruiters_user_id ON profile.recruiters(user_id);

-- Ensure one profile per user
CREATE UNIQUE INDEX IF NOT EXISTS idx_candidates_user_id_unique ON profile.candidates(user_id);
CREATE UNIQUE INDEX IF NOT EXISTS idx_recruiters_user_id_unique ON profile.recruiters(user_id);
