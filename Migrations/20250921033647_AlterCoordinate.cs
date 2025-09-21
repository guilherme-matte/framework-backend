using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace framework_backend.Migrations
{
    /// <inheritdoc />
    public partial class AlterCoordinate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stats_Coordinates",
                table: "Projects");

            migrationBuilder.AddColumn<double>(
                name: "Stats_Cordinates_Latitude",
                table: "Projects",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Stats_Cordinates_Longitude",
                table: "Projects",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stats_Cordinates_Latitude",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Stats_Cordinates_Longitude",
                table: "Projects");

            migrationBuilder.AddColumn<List<double>>(
                name: "Stats_Coordinates",
                table: "Projects",
                type: "double precision[]",
                nullable: false);
        }
    }
}
