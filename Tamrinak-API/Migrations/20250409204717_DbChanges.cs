using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tamrinak_API.Migrations
{
    /// <inheritdoc />
    public partial class DbChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Facilities_FacilityId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Facilities_FacilityId",
                table: "Fields");

            migrationBuilder.DropTable(
                name: "FieldSports");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_FacilityId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "PricePerHour",
                table: "Facilities",
                newName: "PricePerMonth");

            migrationBuilder.AlterColumn<bool>(
                name: "HasLighting",
                table: "Fields",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "FacilityId",
                table: "Fields",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Fields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Fields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SportId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SportFacilities",
                columns: table => new
                {
                    FacilityId = table.Column<int>(type: "int", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportFacilities", x => new { x.FacilityId, x.SportId });
                    table.ForeignKey(
                        name: "FK_SportFacilities_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "FacilityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SportFacilities_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "SportId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fields_SportId",
                table: "Fields",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SportId_FacilityId",
                table: "Bookings",
                columns: new[] { "SportId", "FacilityId" });

            migrationBuilder.CreateIndex(
                name: "IX_SportFacilities_SportId",
                table: "SportFacilities",
                column: "SportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_SportFacilities_SportId_FacilityId",
                table: "Bookings",
                columns: new[] { "SportId", "FacilityId" },
                principalTable: "SportFacilities",
                principalColumns: new[] { "FacilityId", "SportId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Facilities_FacilityId",
                table: "Fields",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "FacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Sports_SportId",
                table: "Fields",
                column: "SportId",
                principalTable: "Sports",
                principalColumn: "SportId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_SportFacilities_SportId_FacilityId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Facilities_FacilityId",
                table: "Fields");

            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Sports_SportId",
                table: "Fields");

            migrationBuilder.DropTable(
                name: "SportFacilities");

            migrationBuilder.DropIndex(
                name: "IX_Fields_SportId",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_SportId_FacilityId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "SportId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "PricePerMonth",
                table: "Facilities",
                newName: "PricePerHour");

            migrationBuilder.AlterColumn<bool>(
                name: "HasLighting",
                table: "Fields",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FacilityId",
                table: "Fields",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "FieldSports",
                columns: table => new
                {
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldSports", x => new { x.FieldId, x.SportId });
                    table.ForeignKey(
                        name: "FK_FieldSports_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "FieldId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldSports_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "SportId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_FacilityId",
                table: "Bookings",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldSports_SportId",
                table: "FieldSports",
                column: "SportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Facilities_FacilityId",
                table: "Bookings",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "FacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Facilities_FacilityId",
                table: "Fields",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "FacilityId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
