using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tamrinak_API.Migrations
{
    /// <inheritdoc />
    public partial class FacProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Facilities",
                newName: "LocationDesc");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Fields",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerMonth",
                table: "Facilities",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "LocationMap",
                table: "Facilities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OfferDurationInMonths",
                table: "Facilities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OfferPrice",
                table: "Facilities",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Facilities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "LocationMap",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "OfferDurationInMonths",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "OfferPrice",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Facilities");

            migrationBuilder.RenameColumn(
                name: "LocationDesc",
                table: "Facilities",
                newName: "Location");

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerMonth",
                table: "Facilities",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
