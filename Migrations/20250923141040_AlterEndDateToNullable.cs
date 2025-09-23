using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace framework_backend.Migrations
{
    /// <inheritdoc />
    public partial class AlterEndDateToNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ongoing",
                table: "Projects",
                newName: "OnGoing");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "Projects",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OnGoing",
                table: "Projects",
                newName: "Ongoing");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "Projects",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
