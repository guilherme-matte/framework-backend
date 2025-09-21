using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace framework_backend.Migrations
{
    /// <inheritdoc />
    public partial class fixedLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location_Address",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Location_Coordinates_Latitude",
                table: "Projects",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Location_Coordinates_Longitude",
                table: "Projects",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location_Address",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Location_Coordinates_Latitude",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Location_Coordinates_Longitude",
                table: "Projects");
        }
    }
}
