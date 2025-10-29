using Microsoft.EntityFrameworkCore.Migrations;

namespace MyApi.Migrations
{
    public partial class AddDispatchesModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create dispatches table
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatches') THEN
                        CREATE TABLE dispatches (
                            id VARCHAR(50) PRIMARY KEY,
                            dispatch_number VARCHAR(100) NOT NULL,
                            service_order_id VARCHAR(50),
                            job_id VARCHAR(50),
                            status VARCHAR(50) NOT NULL DEFAULT 'pending',
                            priority VARCHAR(20) NOT NULL DEFAULT 'medium',
                            required_skills TEXT[],
                            scheduled_date DATE,
                            scheduled_start_time TIME,
                            scheduled_end_time TIME,
                            estimated_duration INTEGER,
                            actual_start_time TIMESTAMP,
                            actual_end_time TIMESTAMP,
                            actual_duration INTEGER,
                            work_location JSONB,
                            completion_percentage INTEGER DEFAULT 0,
                            dispatched_by VARCHAR(50),
                            dispatched_at TIMESTAMP,
                            created_at TIMESTAMP NOT NULL,
                            updated_at TIMESTAMP NOT NULL,
                            is_deleted BOOLEAN NOT NULL DEFAULT false
                        );
                    END IF;
                END $$;
            ");

            // Create dispatch technicians table
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatch_technicians') THEN
                        CREATE TABLE dispatch_technicians (
                            dispatch_id VARCHAR(50) NOT NULL,
                            technician_id VARCHAR(50) NOT NULL,
                            name VARCHAR(255),
                            email VARCHAR(255),
                            phone VARCHAR(50),
                            assigned_at TIMESTAMP,
                            PRIMARY KEY (dispatch_id, technician_id)
                        );
                    END IF;
                END $$;
            ");

            // Create time entries table
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatch_time_entries') THEN
                        CREATE TABLE dispatch_time_entries (
                            id VARCHAR(50) PRIMARY KEY,
                            dispatch_id VARCHAR(50) NOT NULL,
                            technician_id VARCHAR(50) NOT NULL,
                            work_type VARCHAR(50),
                            start_time TIMESTAMP NOT NULL,
                            end_time TIMESTAMP NOT NULL,
                            duration INTEGER,
                            description VARCHAR(2000),
                            billable BOOLEAN,
                            hourly_rate NUMERIC(10,2),
                            total_cost NUMERIC(10,2),
                            status VARCHAR(50) DEFAULT 'pending',
                            created_at TIMESTAMP NOT NULL,
                            approved_by VARCHAR(50),
                            approved_at TIMESTAMP
                        );
                    END IF;
                END $$;
            ");

            // Create expenses table
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatch_expenses') THEN
                        CREATE TABLE dispatch_expenses (
                            id VARCHAR(50) PRIMARY KEY,
                            dispatch_id VARCHAR(50) NOT NULL,
                            technician_id VARCHAR(50) NOT NULL,
                            type VARCHAR(100),
                            amount NUMERIC(10,2) NOT NULL,
                            currency VARCHAR(10),
                            description VARCHAR(2000),
                            date DATE,
                            status VARCHAR(50) DEFAULT 'pending',
                            created_at TIMESTAMP NOT NULL,
                            approved_by VARCHAR(50),
                            approved_at TIMESTAMP
                        );
                    END IF;
                END $$;
            ");

            // Create materials table
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatch_materials') THEN
                        CREATE TABLE dispatch_materials (
                            id VARCHAR(50) PRIMARY KEY,
                            dispatch_id VARCHAR(50) NOT NULL,
                            article_id VARCHAR(50) NOT NULL,
                            article_name VARCHAR(255),
                            sku VARCHAR(100),
                            quantity INTEGER,
                            unit_price NUMERIC(10,2),
                            total_price NUMERIC(10,2),
                            used_by VARCHAR(50),
                            used_at TIMESTAMP,
                            status VARCHAR(50) DEFAULT 'pending',
                            created_at TIMESTAMP NOT NULL,
                            approved_by VARCHAR(50),
                            approved_at TIMESTAMP
                        );
                    END IF;
                END $$;
            ");

            // Create attachments table
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatch_attachments') THEN
                        CREATE TABLE dispatch_attachments (
                            id VARCHAR(50) PRIMARY KEY,
                            dispatch_id VARCHAR(50) NOT NULL,
                            file_name VARCHAR(255) NOT NULL,
                            file_type VARCHAR(100),
                            file_size_mb NUMERIC(10,2),
                            category VARCHAR(100),
                            uploaded_by VARCHAR(50),
                            uploaded_at TIMESTAMP NOT NULL,
                            storage_path VARCHAR(1000)
                        );
                    END IF;
                END $$;
            ");

            // Create notes table
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatch_notes') THEN
                        CREATE TABLE dispatch_notes (
                            id VARCHAR(50) PRIMARY KEY,
                            dispatch_id VARCHAR(50) NOT NULL,
                            content VARCHAR(2000) NOT NULL,
                            category VARCHAR(100),
                            priority VARCHAR(50),
                            created_by VARCHAR(50),
                            created_at TIMESTAMP NOT NULL
                        );
                    END IF;
                END $$;
            ");

            // Indexes
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF to_regclass('public.idx_dispatches_dispatch_number') IS NULL THEN
                        CREATE UNIQUE INDEX idx_dispatches_dispatch_number ON dispatches(dispatch_number);
                    END IF;
                    IF to_regclass('public.idx_dispatches_status') IS NULL THEN
                        CREATE INDEX idx_dispatches_status ON dispatches(status);
                    END IF;
                    IF to_regclass('public.idx_dispatches_scheduled_date') IS NULL THEN
                        CREATE INDEX idx_dispatches_scheduled_date ON dispatches(scheduled_date);
                    END IF;
                    IF to_regclass('public.idx_time_entries_dispatch_id') IS NULL THEN
                        CREATE INDEX idx_time_entries_dispatch_id ON dispatch_time_entries(dispatch_id);
                    END IF;
                    IF to_regclass('public.idx_expenses_dispatch_id') IS NULL THEN
                        CREATE INDEX idx_expenses_dispatch_id ON dispatch_expenses(dispatch_id);
                    END IF;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatch_notes') THEN
                        DROP TABLE IF EXISTS dispatch_notes CASCADE;
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatch_attachments') THEN
                        DROP TABLE IF EXISTS dispatch_attachments CASCADE;
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatch_materials') THEN
                        DROP TABLE IF EXISTS dispatch_materials CASCADE;
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatch_expenses') THEN
                        DROP TABLE IF EXISTS dispatch_expenses CASCADE;
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatch_time_entries') THEN
                        DROP TABLE IF EXISTS dispatch_time_entries CASCADE;
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatch_technicians') THEN
                        DROP TABLE IF EXISTS dispatch_technicians CASCADE;
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'dispatches') THEN
                        DROP TABLE IF EXISTS dispatches CASCADE;
                    END IF;
                END $$;
            ");
        }
    }
}
