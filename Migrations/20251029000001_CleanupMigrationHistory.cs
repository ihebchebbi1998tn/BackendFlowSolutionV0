using Microsoft.EntityFrameworkCore.Migrations;

namespace MyApi.Migrations
{
    public partial class CleanupMigrationHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove the old Planning migration entry (wrong date)
            migrationBuilder.Sql(@"
                DELETE FROM ""__EFMigrationsHistory"" 
                WHERE ""MigrationId"" = '20250129000001_AddPlanningTables';
            ");

            // Remove the Dispatches migration entry if it exists
            // This will allow it to run fresh and create all tables
            migrationBuilder.Sql(@"
                DELETE FROM ""__EFMigrationsHistory"" 
                WHERE ""MigrationId"" = '20251024000001_AddDispatchesModule';
            ");

            // Remove the new Planning migration entry if it exists
            // This will allow it to run fresh after Dispatches
            migrationBuilder.Sql(@"
                DELETE FROM ""__EFMigrationsHistory"" 
                WHERE ""MigrationId"" = '20251025000001_AddPlanningTables';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No rollback needed - this is a one-time cleanup
        }
    }
}
