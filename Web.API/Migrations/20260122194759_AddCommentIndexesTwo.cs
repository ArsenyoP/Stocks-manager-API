using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentIndexesTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87fe25d4-9ca3-4eec-b304-f57568be5434");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f4371960-768e-45b3-a2ab-787e70a3be93");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6aeff350-3f37-48ff-b8bb-0b42c9333364", null, "Admin", "ADMIN" },
                    { "c02e4fad-f28a-4854-a140-cca45c2ffcde", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6aeff350-3f37-48ff-b8bb-0b42c9333364");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c02e4fad-f28a-4854-a140-cca45c2ffcde");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "87fe25d4-9ca3-4eec-b304-f57568be5434", null, "User", "USER" },
                    { "f4371960-768e-45b3-a2ab-787e70a3be93", null, "Admin", "ADMIN" }
                });
        }
    }
}
