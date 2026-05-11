-- Master migration script
-- Run this script to create all database schemas and tables
-- Execute in order: 01-auth-schema.sql through 08-analytics-schema.sql

-- Set timezone to UTC for consistency
SET timezone = 'UTC';

-- Enable required extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Run all migration scripts in order
\i 01-auth-schema.sql
\i 02-profile-schema.sql
\i 03-job-schema.sql
\i 04-application-schema.sql
\i 05-interview-schema.sql
\i 06-notification-schema.sql
\i 07-subscription-schema.sql
\i 08-analytics-schema.sql

-- Verify all schemas were created
SELECT schema_name 
FROM information_schema.schemata 
WHERE schema_name IN ('auth', 'profile', 'job', 'application', 'interview', 'notification', 'subscription', 'analytics')
ORDER BY schema_name;

-- Verify all tables were created
SELECT table_schema, table_name 
FROM information_schema.tables 
WHERE table_schema IN ('auth', 'profile', 'job', 'application', 'interview', 'notification', 'subscription', 'analytics')
ORDER BY table_schema, table_name;

-- Display migration completion message
SELECT 'All database migrations completed successfully!' AS status;
