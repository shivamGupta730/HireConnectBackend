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
