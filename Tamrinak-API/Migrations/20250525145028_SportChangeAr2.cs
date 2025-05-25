using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tamrinak_API.Migrations
{
    /// <inheritdoc />
    public partial class SportChangeAr2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 4,
                column: "Name",
                value: "التنس");

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 8,
                column: "Name",
                value: "الملاكمة");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 4,
                column: "Name",
                value: "تنس");

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 8,
                column: "Name",
                value: "ملاكمة");
        }
    }
}
