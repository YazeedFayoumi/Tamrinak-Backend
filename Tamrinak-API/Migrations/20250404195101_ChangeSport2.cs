using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tamrinak_API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSport2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Individual sport performed in a pool or open water", "Swimming" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Racket sport played individually or in pairs", "Tennis" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Team sport where players pass and shoot a ball", "Handball" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 6,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Indoor racket sport played on a table", "Table Tennis" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 7,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Gaming counter", "E-Sports" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 8,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Bowling is a target sport and recreational activity in which a player rolls a ball toward pins to get the highest score", "Bowling" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 9,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Racket sport played in an indoor court", "Squash" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 10,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Strength sport involving lifting heavy weights", "Weightlifting" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 11,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Combat sport involving punching and defensive techniques", "Boxing" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 12,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Combat sport involving grappling techniques", "Wrestling" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 13,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Japanese martial art focusing on striking techniques", "Karate" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 14,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Korean martial art known for high kicks and strikes", "Taekwondo" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 15,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Mind-body practice focusing on strength, flexibility, and meditation", "Yoga" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 16,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Form of rock climbing performed without ropes", "Bouldering" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 17,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Team sport played with a ball over a net", "Volleyball" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 18,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Track and field sports including running, jumping, and throwing", "Athletics" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Team sport played with a ball over a net", "Volleyball" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Bat-and-ball game popular in many countries", "Cricket" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Physical contact team sport with oval-shaped ball", "Rugby" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 6,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Team sport where players pass and shoot a ball", "Handball" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 7,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Team sport played on ice with a puck", "Ice Hockey" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 8,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Bat-and-ball game popular in North America", "Baseball" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 9,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Racket sport played individually or in pairs", "Tennis" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 10,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Racket sport played with a lightweight shuttlecock", "Badminton" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 11,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Indoor racket sport played on a table", "Table Tennis" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 12,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Individual sport performed in a pool or open water", "Swimming" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 13,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Track and field sports including running, jumping, and throwing", "Athletics" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 14,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Combat sport involving punching and defensive techniques", "Boxing" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 15,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Combat sport involving grappling techniques", "Wrestling" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 16,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Japanese martial art focusing on striking techniques", "Karate" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 17,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Japanese martial art focusing on throws and grappling", "Judo" });

            migrationBuilder.UpdateData(
                table: "Sports",
                keyColumn: "SportId",
                keyValue: 18,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Korean martial art known for high kicks and strikes", "Taekwondo" });

            migrationBuilder.InsertData(
                table: "Sports",
                columns: new[] { "SportId", "Description", "Name" },
                values: new object[,]
                {
                    { 20, "Racket sport played in an indoor court", "Squash" },
                    { 21, "Racket sport combining elements of tennis and squash", "Padel" },
                    { 22, "Mind-body practice focusing on strength, flexibility, and meditation", "Yoga" },
                    { 23, "Low-impact exercise method focusing on core strength", "Pilates" },
                    { 24, "Strength sport involving lifting heavy weights", "Weightlifting" },
                    { 25, "Sport involving climbing natural rock formations or artificial walls", "Rock Climbing" },
                    { 26, "Form of rock climbing performed without ropes", "Bouldering" },
                    { 27, "Simulated skydiving experience in a vertical wind tunnel", "Indoor Skydiving" },
                    { 28, "Team sport played in a swimming pool", "Water Polo" },
                    { 29, "Artistic swimming performed in synchronized routines", "Synchronized Swimming" }
                });
        }
    }
}
