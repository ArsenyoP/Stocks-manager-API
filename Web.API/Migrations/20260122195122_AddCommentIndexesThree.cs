using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentIndexesThree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    { "98794c6e-8a61-4e77-97fa-fc402e0b0210", null, "Admin", "ADMIN" },
                    { "c798e1c5-51ac-44c1-8bd1-4e14dfc9a843", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "98794c6e-8a61-4e77-97fa-fc402e0b0210");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c798e1c5-51ac-44c1-8bd1-4e14dfc9a843");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6aeff350-3f37-48ff-b8bb-0b42c9333364", null, "Admin", "ADMIN" },
                    { "c02e4fad-f28a-4854-a140-cca45c2ffcde", null, "User", "USER" }
                });
        }
    }
}
