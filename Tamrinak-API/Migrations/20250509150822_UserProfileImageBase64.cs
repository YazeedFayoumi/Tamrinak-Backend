using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tamrinak_API.Migrations
{
    /// <inheritdoc />
    public partial class UserProfileImageBase64 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "Users",
                newName: "ProfileImageBase64");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImageBase64",
                table: "Users",
                newName: "ProfileImageUrl");
        }
    }
}
