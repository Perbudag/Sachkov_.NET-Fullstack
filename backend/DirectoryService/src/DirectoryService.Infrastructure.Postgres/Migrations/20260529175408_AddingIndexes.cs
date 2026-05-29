using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddingIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_departments_positions_department_id",
                table: "departments_positions");

            migrationBuilder.DropIndex(
                name: "IX_departments_locations_department_id",
                table: "departments_locations");

            migrationBuilder.CreateIndex(
                name: "IX_positions_name",
                table: "positions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_locations_name",
                table: "locations",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_departments_positions_department_id_position_id",
                table: "departments_positions",
                columns: new[] { "department_id", "position_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_departments_locations_department_id_location_id",
                table: "departments_locations",
                columns: new[] { "department_id", "location_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_departments_name",
                table: "departments",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_positions_name",
                table: "positions");

            migrationBuilder.DropIndex(
                name: "IX_locations_name",
                table: "locations");

            migrationBuilder.DropIndex(
                name: "IX_departments_positions_department_id_position_id",
                table: "departments_positions");

            migrationBuilder.DropIndex(
                name: "IX_departments_locations_department_id_location_id",
                table: "departments_locations");

            migrationBuilder.DropIndex(
                name: "IX_departments_name",
                table: "departments");

            migrationBuilder.CreateIndex(
                name: "IX_departments_positions_department_id",
                table: "departments_positions",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_departments_locations_department_id",
                table: "departments_locations",
                column: "department_id");
        }
    }
}
