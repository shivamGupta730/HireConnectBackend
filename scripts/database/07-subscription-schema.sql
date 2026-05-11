-- Subscription Service Schema
-- Create subscription schema if it doesn't exist
CREATE SCHEMA IF NOT EXISTS subscription;

-- Create Subscriptions table
CREATE TABLE IF NOT EXISTS subscription.Subscriptions (
    Id SERIAL PRIMARY KEY,
    UserId INTEGER NOT NULL UNIQUE,
    Type INTEGER NOT NULL, -- 1: Basic, 2: Premium, 3: Enterprise
    Status INTEGER DEFAULT 1, -- 1: Active, 2: Cancelled, 3: Expired
    StartedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    ExpiresAt TIMESTAMP WITH TIME ZONE NOT NULL,
    Price DECIMAL(10,2) DEFAULT 0,
    Currency VARCHAR(10) DEFAULT 'USD',
    CancelledAt TIMESTAMP WITH TIME ZONE,
    UpdatedAt TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY (UserId) REFERENCES auth.Users(Id) ON DELETE CASCADE
);

-- Create Invoices table
CREATE TABLE IF NOT EXISTS subscription.Invoices (
    Id SERIAL PRIMARY KEY,
    SubscriptionId INTEGER NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Currency VARCHAR(10) DEFAULT 'USD',
    DueDate TIMESTAMP WITH TIME ZONE NOT NULL,
    PaidAt TIMESTAMP WITH TIME ZONE,
    PaymentMethod VARCHAR(100),
    TransactionId VARCHAR(255),
    Status INTEGER DEFAULT 1, -- 1: Pending, 2: Paid, 3: Overdue, 4: Cancelled
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (SubscriptionId) REFERENCES subscription.Subscriptions(Id) ON DELETE CASCADE
);

-- Add indexes for better performance
CREATE INDEX IF NOT EXISTS idx_subscriptions_user_id ON subscription.Subscriptions(UserId);
CREATE INDEX IF NOT EXISTS idx_subscriptions_type ON subscription.Subscriptions(Type);
CREATE INDEX IF NOT EXISTS idx_subscriptions_status ON subscription.Subscriptions(Status);
CREATE INDEX IF NOT EXISTS idx_subscriptions_expires_at ON subscription.Subscriptions(ExpiresAt);
CREATE INDEX IF NOT EXISTS idx_subscriptions_started_at ON subscription.Subscriptions(StartedAt);

CREATE INDEX IF NOT EXISTS idx_invoices_subscription_id ON subscription.Invoices(SubscriptionId);
CREATE INDEX IF NOT EXISTS idx_invoices_status ON subscription.Invoices(Status);
CREATE INDEX IF NOT EXISTS idx_invoices_due_date ON subscription.Invoices(DueDate);
CREATE INDEX IF NOT EXISTS idx_invoices_created_at ON subscription.Invoices(CreatedAt);
CREATE INDEX IF NOT EXISTS idx_invoices_transaction_id ON subscription.Invoices(TransactionId);

-- Add comments for better documentation
COMMENT ON TABLE subscription.Subscriptions IS 'User subscription plans';
COMMENT ON COLUMN subscription.Subscriptions.Type IS 'Subscription type: 1=Basic, 2=Premium, 3=Enterprise';
COMMENT ON COLUMN subscription.Subscriptions.Status IS 'Subscription status: 1=Active, 2=Cancelled, 3=Expired';
COMMENT ON COLUMN subscription.Subscriptions.Price IS 'Subscription price in the specified currency';

COMMENT ON TABLE subscription.Invoices IS 'Subscription invoices';
COMMENT ON COLUMN subscription.Invoices.Status IS 'Invoice status: 1=Pending, 2=Paid, 3=Overdue, 4=Cancelled';
COMMENT ON COLUMN subscription.Invoices.TransactionId IS 'Payment transaction identifier';
COMMENT ON COLUMN subscription.Invoices.PaymentMethod IS 'Payment method used (e.g., Credit Card, PayPal)';
