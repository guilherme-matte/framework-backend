using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace framework_backend.Migrations
{
    /// <inheritdoc />
    public partial class addImageUrlFieldInNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Images",
                table: "NewsLetter",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                table: "NewsLetter");
        }
    }
}
