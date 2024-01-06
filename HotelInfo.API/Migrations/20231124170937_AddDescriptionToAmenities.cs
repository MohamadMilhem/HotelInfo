using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelInfo.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToAmenities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RoomAmenities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "HotelAmenities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "RoomAmenities");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "HotelAmenities");
        }
    }
}
