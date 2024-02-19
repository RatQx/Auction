using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aukcionas.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "starting_price",
                table: "Auctions",
                type: "float",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<double>(
                name: "min_buy_price",
                table: "Auctions",
                type: "float",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<double>(
                name: "item_mass",
                table: "Auctions",
                type: "float",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<double>(
                name: "buy_now_price",
                table: "Auctions",
                type: "float",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldMaxLength: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "starting_price",
                table: "Auctions",
                type: "real",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<float>(
                name: "min_buy_price",
                table: "Auctions",
                type: "real",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<float>(
                name: "item_mass",
                table: "Auctions",
                type: "real",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<float>(
                name: "buy_now_price",
                table: "Auctions",
                type: "real",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldMaxLength: 10);
        }
    }
}
