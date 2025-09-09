using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace framework_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddArchitectAndSpecialities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Architects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Nationality = table.Column<string>(type: "text", nullable: false),
                    Subtitle = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Biography = table.Column<string>(type: "text", nullable: false),
                    Picture = table.Column<string>(type: "text", nullable: false),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    Trending = table.Column<bool>(type: "boolean", nullable: false),
                    Training_Name = table.Column<string>(type: "text", nullable: false),
                    Training_Year = table.Column<int>(type: "integer", nullable: false),
                    SocialMedia_Linkedin = table.Column<string>(type: "text", nullable: false),
                    SocialMedia_Instagram = table.Column<string>(type: "text", nullable: false),
                    SocialMedia_Portfolio = table.Column<string>(type: "text", nullable: false),
                    Stats_TotalProjects = table.Column<int>(type: "integer", nullable: false),
                    Stats_ESGProjects = table.Column<int>(type: "integer", nullable: false),
                    Stats_Views = table.Column<int>(type: "integer", nullable: false),
                    Stats_Likes = table.Column<int>(type: "integer", nullable: false),
                    Stats_Followers = table.Column<int>(type: "integer", nullable: false),
                    Location_City = table.Column<string>(type: "text", nullable: false),
                    Location_Country = table.Column<string>(type: "text", nullable: false),
                    Location_State = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Architects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specialities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ArchitectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Specialities_Architects_ArchitectId",
                        column: x => x.ArchitectId,
                        principalTable: "Architects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Specialities_ArchitectId",
                table: "Specialities",
                column: "ArchitectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Specialities");

            migrationBuilder.DropTable(
                name: "Architects");
        }
    }
}
