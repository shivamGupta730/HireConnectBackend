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
