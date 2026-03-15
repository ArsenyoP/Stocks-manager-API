using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web.API.Migrations
{
    /// <inheritdoc />
    public partial class Innit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f5b7c8d9-e0a1-4b2c-3d4e-5f6a7b8c9d0e", 0, "b2c3d4e5-f6a7-8901-bcde-111111111111", "arsenyo198510@gmail.com", true, false, null, "ARSENYO198510@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEG8u0LViCiVRdh60FgU7usvJyNW4Fh1IpPE1srnkarlpf15mhjwgrx+tOoynMpWxEg==", null, false, "a1b2c3d4-e5f6-7890-abcd-000000000000", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "3474648c-112d-460c-bc8a-61be046b3078", "f5b7c8d9-e0a1-4b2c-3d4e-5f6a7b8c9d0e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eb4b3260-05fb-470c-af9f-b9e9b9bcd85e");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3474648c-112d-460c-bc8a-61be046b3078", "f5b7c8d9-e0a1-4b2c-3d4e-5f6a7b8c9d0e" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3474648c-112d-460c-bc8a-61be046b3078");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f5b7c8d9-e0a1-4b2c-3d4e-5f6a7b8c9d0e");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "ab4c6115-25ed-49ba-a054-0583b78cb484", null, "Admin", "ADMIN" },
                    { "bf171b92-9b8b-4eaf-8b68-ee51e64841de", null, "User", "USER" }
                });
        }
    }
}
