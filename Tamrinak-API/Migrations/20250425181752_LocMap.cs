using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tamrinak_API.Migrations
{
    /// <inheritdoc />
    public partial class LocMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Fields",
                newName: "LocationDesc");

            migrationBuilder.AlterColumn<bool>(
                name: "IsIndoor",
                table: "Fields",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Fields",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "LocationMap",
                table: "Fields",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationMap",
                table: "Fields");

            migrationBuilder.RenameColumn(
                name: "LocationDesc",
                table: "Fields",
                newName: "Location");

            migrationBuilder.AlterColumn<bool>(
                name: "IsIndoor",
                table: "Fields",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Fields",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
