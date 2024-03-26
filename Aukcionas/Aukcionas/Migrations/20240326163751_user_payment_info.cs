using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aukcionas.Migrations
{
    /// <inheritdoc />
    public partial class user_payment_info : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Payment_Currency",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Payout_Amount",
                table: "Payments",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Payout_Currency",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Payout_Successful",
                table: "Payments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Payout_Time",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Account_Holder_Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Account_Number",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Bank",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bank_Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bic_Swift_Code",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Paypal",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

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
                name: "Payment_Currency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Payout_Amount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Payout_Currency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Payout_Successful",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Payout_Time",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Account_Holder_Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Account_Number",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Bank",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Bank_Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Bic_Swift_Code",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Paypal",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Paypal_Email",
                table: "AspNetUsers");
        }
    }
}
