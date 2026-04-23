using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HNG14_Backend_Task2.Migrations
{
    /// <inheritdoc />
    public partial class graf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    gender = table.Column<string>(type: "text", nullable: false),
                    gender_probability = table.Column<float>(type: "real", nullable: true),
                    age = table.Column<int>(type: "integer", nullable: false),
                    age_group = table.Column<string>(type: "text", nullable: false),
                    country_id = table.Column<string>(type: "text", nullable: false),
                    country_name = table.Column<string>(type: "text", nullable: false),
                    country_probability = table.Column<double>(type: "double precision", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_name",
                table: "Profiles",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
