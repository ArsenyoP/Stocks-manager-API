using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndexesForComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_CreatedOn",
                table: "Comments");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "23954dd0-4754-4507-98a5-785a997d6f60");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7d7b2eb3-5bcd-4051-b4a5-39f95819dd8e");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2c86f87c-707d-42b2-88a4-d698326ee170", null, "User", "USER" },
                    { "ddd1150d-bb7c-4634-8f8d-8e856b83e94d", null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedOn_Covering",
                table: "Comments",
                column: "CreatedOn",
                descending: new bool[0])
                .Annotation("SqlServer:Include", new[] { "Title", "Content", "AppUserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_CreatedOn_Covering",
                table: "Comments");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c86f87c-707d-42b2-88a4-d698326ee170");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ddd1150d-bb7c-4634-8f8d-8e856b83e94d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "23954dd0-4754-4507-98a5-785a997d6f60", null, "User", "USER" },
                    { "7d7b2eb3-5bcd-4051-b4a5-39f95819dd8e", null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedOn",
                table: "Comments",
                column: "CreatedOn");
        }
    }
}
