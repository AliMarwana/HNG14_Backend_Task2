using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HNG14_Backend_Task1.Migrations
{
    /// <inheritdoc />
    public partial class grba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SampleSize",
                table: "Profiles");

            migrationBuilder.AddColumn<string>(
                name: "CountryName",
                table: "Profiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryName",
                table: "Profiles");

            migrationBuilder.AddColumn<int>(
                name: "SampleSize",
                table: "Profiles",
                type: "integer",
                nullable: true);
        }
    }
}
