using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyApi.Migrations
{
    public partial class AddDispatchesModuleFresh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create dispatches table
            migrationBuilder.CreateTable(
                name: "dispatches",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dispatch_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    service_order_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    job_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "pending"),
                    priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "medium"),
                    required_skills = table.Column<string[]>(type: "text[]", nullable: true),
                    scheduled_date = table.Column<DateTime>(type: "date", nullable: true),
                    scheduled_start_time = table.Column<TimeSpan>(type: "time", nullable: true),
                    scheduled_end_time = table.Column<TimeSpan>(type: "time", nullable: true),
                    estimated_duration = table.Column<int>(type: "integer", nullable: true),
                    actual_start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actual_end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actual_duration = table.Column<int>(type: "integer", nullable: true),
                    work_location = table.Column<string>(type: "jsonb", nullable: true),
                    completion_percentage = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    dispatched_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    dispatched_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispatches", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_dispatches_dispatch_number",
                table: "dispatches",
                column: "dispatch_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_dispatches_status",
                table: "dispatches",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_dispatches_scheduled_date",
                table: "dispatches",
                column: "scheduled_date");

            // Create dispatch_technicians table
            migrationBuilder.CreateTable(
                name: "dispatch_technicians",
                columns: table => new
                {
                    dispatch_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    technician_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispatch_technicians", x => new { x.dispatch_id, x.technician_id });
                    table.ForeignKey(
                        name: "FK_dispatch_technicians_dispatches_dispatch_id",
                        column: x => x.dispatch_id,
                        principalTable: "dispatches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create dispatch_time_entries table
            migrationBuilder.CreateTable(
                name: "dispatch_time_entries",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dispatch_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    technician_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    work_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    duration = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    billable = table.Column<bool>(type: "boolean", nullable: true),
                    hourly_rate = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    total_cost = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValue: "pending"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    approved_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispatch_time_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_dispatch_time_entries_dispatches_dispatch_id",
                        column: x => x.dispatch_id,
                        principalTable: "dispatches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_time_entries_dispatch_id",
                table: "dispatch_time_entries",
                column: "dispatch_id");

            // Create dispatch_expenses table
            migrationBuilder.CreateTable(
                name: "dispatch_expenses",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dispatch_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    technician_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    date = table.Column<DateTime>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValue: "pending"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    approved_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispatch_expenses", x => x.id);
                    table.ForeignKey(
                        name: "FK_dispatch_expenses_dispatches_dispatch_id",
                        column: x => x.dispatch_id,
                        principalTable: "dispatches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_expenses_dispatch_id",
                table: "dispatch_expenses",
                column: "dispatch_id");

            // Create dispatch_materials table
            migrationBuilder.CreateTable(
                name: "dispatch_materials",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dispatch_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    article_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    article_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: true),
                    unit_price = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    total_price = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    used_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValue: "pending"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    approved_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispatch_materials", x => x.id);
                    table.ForeignKey(
                        name: "FK_dispatch_materials_dispatches_dispatch_id",
                        column: x => x.dispatch_id,
                        principalTable: "dispatches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create dispatch_attachments table
            migrationBuilder.CreateTable(
                name: "dispatch_attachments",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dispatch_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    file_size_mb = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    uploaded_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    storage_path = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispatch_attachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_dispatch_attachments_dispatches_dispatch_id",
                        column: x => x.dispatch_id,
                        principalTable: "dispatches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create dispatch_notes table
            migrationBuilder.CreateTable(
                name: "dispatch_notes",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dispatch_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    priority = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispatch_notes", x => x.id);
                    table.ForeignKey(
                        name: "FK_dispatch_notes_dispatches_dispatch_id",
                        column: x => x.dispatch_id,
                        principalTable: "dispatches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "dispatch_notes");
            migrationBuilder.DropTable(name: "dispatch_attachments");
            migrationBuilder.DropTable(name: "dispatch_materials");
            migrationBuilder.DropTable(name: "dispatch_expenses");
            migrationBuilder.DropTable(name: "dispatch_time_entries");
            migrationBuilder.DropTable(name: "dispatch_technicians");
            migrationBuilder.DropTable(name: "dispatches");
        }
    }
}
