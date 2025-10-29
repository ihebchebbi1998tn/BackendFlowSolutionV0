using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyApi.Migrations
{
    public partial class AddPlanningTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create technician_working_hours table
            migrationBuilder.CreateTable(
                name: "technician_working_hours",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    technician_id = table.Column<int>(type: "integer", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    effective_from = table.Column<DateTime>(type: "date", nullable: true),
                    effective_until = table.Column<DateTime>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technician_working_hours", x => x.id);
                    table.ForeignKey(
                        name: "FK_technician_working_hours_Users_technician_id",
                        column: x => x.technician_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.CheckConstraint("CK_day_of_week_range", "day_of_week >= 0 AND day_of_week <= 6");
                });

            migrationBuilder.CreateIndex(
                name: "IX_technician_working_hours_technician_id",
                table: "technician_working_hours",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_working_hours_dates",
                table: "technician_working_hours",
                columns: new[] { "effective_from", "effective_until" });

            // 2. Create technician_leaves table
            migrationBuilder.CreateTable(
                name: "technician_leaves",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    technician_id = table.Column<int>(type: "integer", nullable: false),
                    leave_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "time", nullable: true),
                    end_time = table.Column<TimeSpan>(type: "time", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "pending"),
                    reason = table.Column<string>(type: "text", nullable: true),
                    approved_by = table.Column<int>(type: "integer", nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technician_leaves", x => x.id);
                    table.ForeignKey(
                        name: "FK_technician_leaves_Users_technician_id",
                        column: x => x.technician_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_technician_leaves_Users_approved_by",
                        column: x => x.approved_by,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.CheckConstraint("CK_valid_leave_type", "leave_type IN ('vacation', 'sick', 'personal', 'training', 'other')");
                    table.CheckConstraint("CK_valid_leave_status", "status IN ('pending', 'approved', 'rejected', 'cancelled')");
                    table.CheckConstraint("CK_valid_date_range", "end_date >= start_date");
                });

            migrationBuilder.CreateIndex(
                name: "IX_technician_leaves_technician_id",
                table: "technician_leaves",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_leaves_dates",
                table: "technician_leaves",
                columns: new[] { "start_date", "end_date" });

            migrationBuilder.CreateIndex(
                name: "IX_technician_leaves_status",
                table: "technician_leaves",
                column: "status");

            // 3. Create technician_status_history table
            migrationBuilder.CreateTable(
                name: "technician_status_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    technician_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    changed_from = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    changed_by = table.Column<int>(type: "integer", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technician_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_technician_status_history_Users_technician_id",
                        column: x => x.technician_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_technician_status_history_Users_changed_by",
                        column: x => x.changed_by,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.CheckConstraint("CK_valid_technician_status", "status IN ('available', 'busy', 'offline', 'on_leave', 'not_working', 'over_capacity')");
                });

            migrationBuilder.CreateIndex(
                name: "IX_technician_status_history_technician_id",
                table: "technician_status_history",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_status_history_changed_at",
                table: "technician_status_history",
                column: "changed_at");

            // 4. Create dispatch_history table
            migrationBuilder.CreateTable(
                name: "dispatch_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dispatch_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    old_value = table.Column<string>(type: "text", nullable: true),
                    new_value = table.Column<string>(type: "text", nullable: true),
                    changed_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    metadata = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispatch_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_dispatch_history_dispatches_dispatch_id",
                        column: x => x.dispatch_id,
                        principalTable: "dispatches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.CheckConstraint("CK_valid_dispatch_action", "action IN ('created', 'assigned', 'rescheduled', 'reassigned', 'status_changed', 'updated', 'cancelled', 'deleted')");
                });

            migrationBuilder.CreateIndex(
                name: "IX_dispatch_history_dispatch_id",
                table: "dispatch_history",
                column: "dispatch_id");

            migrationBuilder.CreateIndex(
                name: "IX_dispatch_history_changed_at",
                table: "dispatch_history",
                column: "changed_at");

            migrationBuilder.CreateIndex(
                name: "IX_dispatch_history_action",
                table: "dispatch_history",
                column: "action");

            // 5. Add columns to service_order_jobs table
            migrationBuilder.AddColumn<string[]>(
                name: "required_skills",
                table: "service_order_jobs",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "priority",
                table: "service_order_jobs",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "medium");

            migrationBuilder.AddColumn<DateTime>(
                name: "scheduled_date",
                table: "service_order_jobs",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "scheduled_start_time",
                table: "service_order_jobs",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "scheduled_end_time",
                table: "service_order_jobs",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "location_json",
                table: "service_order_jobs",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customer_name",
                table: "service_order_jobs",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customer_phone",
                table: "service_order_jobs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_service_order_jobs_status",
                table: "service_order_jobs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_service_order_jobs_scheduled_date",
                table: "service_order_jobs",
                column: "scheduled_date");

            migrationBuilder.CreateIndex(
                name: "IX_service_order_jobs_priority",
                table: "service_order_jobs",
                column: "priority");

            // 6. Add columns to Users table
            migrationBuilder.AddColumn<string[]>(
                name: "Skills",
                table: "Users",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentStatus",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "offline");

            migrationBuilder.AddColumn<string>(
                name: "LocationJson",
                table: "Users",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrentStatus",
                table: "Users",
                column: "CurrentStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop new tables
            migrationBuilder.DropTable(name: "dispatch_history");
            migrationBuilder.DropTable(name: "technician_status_history");
            migrationBuilder.DropTable(name: "technician_leaves");
            migrationBuilder.DropTable(name: "technician_working_hours");

            // Remove columns from service_order_jobs
            migrationBuilder.DropIndex(name: "IX_service_order_jobs_priority", table: "service_order_jobs");
            migrationBuilder.DropIndex(name: "IX_service_order_jobs_scheduled_date", table: "service_order_jobs");
            migrationBuilder.DropIndex(name: "IX_service_order_jobs_status", table: "service_order_jobs");
            migrationBuilder.DropColumn(name: "customer_phone", table: "service_order_jobs");
            migrationBuilder.DropColumn(name: "customer_name", table: "service_order_jobs");
            migrationBuilder.DropColumn(name: "location_json", table: "service_order_jobs");
            migrationBuilder.DropColumn(name: "scheduled_end_time", table: "service_order_jobs");
            migrationBuilder.DropColumn(name: "scheduled_start_time", table: "service_order_jobs");
            migrationBuilder.DropColumn(name: "scheduled_date", table: "service_order_jobs");
            migrationBuilder.DropColumn(name: "priority", table: "service_order_jobs");
            migrationBuilder.DropColumn(name: "required_skills", table: "service_order_jobs");

            // Remove columns from Users
            migrationBuilder.DropIndex(name: "IX_Users_CurrentStatus", table: "Users");
            migrationBuilder.DropIndex(name: "IX_Users_Role", table: "Users");
            migrationBuilder.DropColumn(name: "LocationJson", table: "Users");
            migrationBuilder.DropColumn(name: "CurrentStatus", table: "Users");
            migrationBuilder.DropColumn(name: "Skills", table: "Users");
        }
    }
}
