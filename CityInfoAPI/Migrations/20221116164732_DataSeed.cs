using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CityInfoAPI.Migrations
{
    /// <inheritdoc />
    public partial class DataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Greatest city in the world but not really", "Sao Paulo" },
                    { 2, "Fish and chips! Home!", "Manchester" },
                    { 3, "Stolen art across the globe can be found here", "Paris" }
                });

            migrationBuilder.InsertData(
                table: "PointOfInterests",
                columns: new[] { "Id", "CityId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Malls and malls and shops buy buy buy!", "Avenida Paulista" },
                    { 2, 1, "Finally some green in this concrete jungle", "Ibirapuera Park" },
                    { 3, 2, "Where Capitalism always win, sadly", "Arndale Mall" },
                    { 4, 3, "Stolen art across the globe can be found here", "Neymar" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PointOfInterests",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
