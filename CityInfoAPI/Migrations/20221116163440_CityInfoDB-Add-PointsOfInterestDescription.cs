﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityInfoAPI.Migrations
{
    /// <inheritdoc />
    public partial class CityInfoDBAddPointsOfInterestDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PointOfInterests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "PointOfInterests");
        }
    }
}
