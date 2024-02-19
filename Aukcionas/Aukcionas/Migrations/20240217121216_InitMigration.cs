using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aukcionas.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    country = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    city = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    starting_price = table.Column<float>(type: "real", maxLength: 10, nullable: false),
                    bid_ammount = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    min_buy_price = table.Column<float>(type: "real", maxLength: 10, nullable: false),
                    auction_start_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    auction_end_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    auction_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    auction_stopped = table.Column<bool>(type: "bit", nullable: false),
                    auction_ended = table.Column<bool>(type: "bit", nullable: false),
                    auction_won = table.Column<bool>(type: "bit", nullable: false),
                    auction_winner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    auction_biders_list = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bidding_amount_history = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bidding_times_history = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    buy_now_price = table.Column<float>(type: "real", maxLength: 10, nullable: false),
                    category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    item_build_year = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    item_mass = table.Column<float>(type: "real", maxLength: 10, nullable: false),
                    condition = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    material = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    auction_likes = table.Column<int>(type: "int", nullable: false),
                    SavedUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SavedFileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auctions");
        }
    }
}
