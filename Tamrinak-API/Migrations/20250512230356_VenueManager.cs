using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tamrinak_API.Migrations
{
    /// <inheritdoc />
    public partial class VenueManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasVenueOwnershipRequest",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VenueRequestDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Fields",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Facilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fields_OwnerId",
                table: "Fields",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_OwnerId",
                table: "Facilities",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Users_OwnerId",
                table: "Facilities",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Users_OwnerId",
                table: "Fields",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Users_OwnerId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Users_OwnerId",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "IX_Fields_OwnerId",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_OwnerId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "HasVenueOwnershipRequest",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VenueRequestDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Facilities");
        }
    }
}
