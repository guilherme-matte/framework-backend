using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace framework_backend.Migrations
{
    /// <inheritdoc />
    public partial class fixedProjectModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stats_Coordinates_Latitude",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Stats_Coordinates_Longitude",
                table: "Projects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Stats_Coordinates_Latitude",
                table: "Projects",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Stats_Coordinates_Longitude",
                table: "Projects",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
