using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNewStockFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "47178847-8fae-4af5-a236-9e15c99aebdc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "750c1fc8-5d12-4463-8044-d6555f0407f6");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Stocks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Stocks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "SharesOutstanding",
                table: "Stocks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "UpdateCount",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3474648c-112d-460c-bc8a-61be046b3078", null, "Admin", "ADMIN" },
                    { "eb4b3260-05fb-470c-af9f-b9e9b9bcd85e", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3474648c-112d-460c-bc8a-61be046b3078");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eb4b3260-05fb-470c-af9f-b9e9b9bcd85e");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "SharesOutstanding",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "UpdateCount",
                table: "Stocks");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "47178847-8fae-4af5-a236-9e15c99aebdc", null, "Admin", "ADMIN" },
                    { "750c1fc8-5d12-4463-8044-d6555f0407f6", null, "User", "USER" }
                });
        }
    }
}
