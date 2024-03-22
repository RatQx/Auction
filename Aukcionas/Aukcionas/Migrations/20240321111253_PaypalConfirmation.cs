using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aukcionas.Migrations
{
    /// <inheritdoc />
    public partial class PaypalConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Paypal_Email",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Paypal_Email",
                table: "AspNetUsers");
        }
    }
}
