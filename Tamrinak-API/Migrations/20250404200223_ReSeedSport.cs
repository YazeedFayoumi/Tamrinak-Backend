using Microsoft.EntityFrameworkCore.Migrations;
using Tamrinak_API.DataAccess.Models;

#nullable disable

namespace Tamrinak_API.Migrations
{
    /// <inheritdoc />
    public partial class ReSeedSport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
        table: "Sports",
        columns: new[] { "SportId", "Name", "Description" },
        values: new object[,]
        {   
            { 3, "Swimming", "Individual sport performed in a pool or open water" },
            { 4, "Tennis", "Racket sport played individually or in pairs" },
            { 5, "Handball", "Team sport where players pass and shoot a ball" },
            { 6, "Table Tennis", "Indoor racket sport played on a table" },
            { 7, "E-Sports", "Gaming counter" },
            { 8, "Bowling", "Bowling is a target sport and recreational activity in which a player rolls a ball toward pins to get the highest score" },
            { 9, "Squash", "Racket sport played in an indoor court" },
            { 10, "Weightlifting", "Strength sport involving lifting heavy weights" },
            { 11, "Boxing", "Combat sport involving punching and defensive techniques" },
            { 12, "Wrestling", "Combat sport involving grappling techniques" },
            { 13, "Karate", "Japanese martial art focusing on striking techniques" },
            { 14, "Taekwondo", "Korean martial art known for high kicks and strikes" },
            { 15, "Yoga", "Mind-body practice focusing on strength, flexibility, and meditation" },
            { 16, "Bouldering", "Form of rock climbing performed without ropes" },
            { 17, "Volleyball", "Team sport played with a ball over a net" },
            { 18, "Athletics", "Track and field sports including running, jumping, and throwing" },
            { 19, "Brazilian Jiu-Jitsu",  "Martial art focusing on ground fighting and submission" }
        });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
