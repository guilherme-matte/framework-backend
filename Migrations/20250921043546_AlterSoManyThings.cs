using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace framework_backend.Migrations
{
    /// <inheritdoc />
    public partial class AlterSoManyThings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stats_Cordinates_Longitude",
                table: "Projects",
                newName: "Stats_Coordinates_Longitude");

            migrationBuilder.RenameColumn(
                name: "Stats_Cordinates_Latitude",
                table: "Projects",
                newName: "Stats_Coordinates_Latitude");

            migrationBuilder.AddColumn<string>(
                name: "Location_Address",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location_Address",
                table: "Architects",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location_Address",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Location_Address",
                table: "Architects");

            migrationBuilder.RenameColumn(
                name: "Stats_Coordinates_Longitude",
                table: "Projects",
                newName: "Stats_Cordinates_Longitude");

            migrationBuilder.RenameColumn(
                name: "Stats_Coordinates_Latitude",
                table: "Projects",
                newName: "Stats_Cordinates_Latitude");
        }
    }
}
