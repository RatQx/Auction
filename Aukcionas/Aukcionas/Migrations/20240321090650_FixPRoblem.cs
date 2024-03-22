using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aukcionas.Migrations
{
    /// <inheritdoc />
    public partial class FixPRoblem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PicturePaths",
                table: "Auctions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PicturePaths",
                table: "Auctions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
