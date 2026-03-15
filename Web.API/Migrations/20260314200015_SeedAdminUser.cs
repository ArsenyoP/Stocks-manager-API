using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3474648c-112d-460c-bc8a-61be046b3078");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eb4b3260-05fb-470c-af9f-b9e9b9bcd85e");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "ab4c6115-25ed-49ba-a054-0583b78cb484", null, "Admin", "ADMIN" },
                    { "bf171b92-9b8b-4eaf-8b68-ee51e64841de", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ab4c6115-25ed-49ba-a054-0583b78cb484");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bf171b92-9b8b-4eaf-8b68-ee51e64841de");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3474648c-112d-460c-bc8a-61be046b3078", null, "Admin", "ADMIN" },
                    { "eb4b3260-05fb-470c-af9f-b9e9b9bcd85e", null, "User", "USER" }
                });
        }
    }
}
