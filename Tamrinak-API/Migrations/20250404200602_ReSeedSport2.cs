using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tamrinak_API.Migrations
{
    /// <inheritdoc />
    public partial class ReSeedSport2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
        table: "Sports",
        columns: new[] { "SportId", "Name", "Description" },
        values: new object[,]
        {
            { 1, "Football", "Popular team sport played with a spherical ball" },
            { 2, "Basketball", "Fast-paced team sport played on a rectangular court" }
        });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
