using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelInfo.API.Migrations
{
    /// <inheritdoc />
    public partial class AlterHotelAmenitiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelHotelAmenity_HotelAmenity_hotelAmenitiesId",
                table: "HotelHotelAmenity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HotelAmenity",
                table: "HotelAmenity");

            migrationBuilder.RenameTable(
                name: "HotelAmenity",
                newName: "HotelAmenities");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HotelAmenities",
                table: "HotelAmenities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelHotelAmenity_HotelAmenities_hotelAmenitiesId",
                table: "HotelHotelAmenity",
                column: "hotelAmenitiesId",
                principalTable: "HotelAmenities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelHotelAmenity_HotelAmenities_hotelAmenitiesId",
                table: "HotelHotelAmenity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HotelAmenities",
                table: "HotelAmenities");

            migrationBuilder.RenameTable(
                name: "HotelAmenities",
                newName: "HotelAmenity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HotelAmenity",
                table: "HotelAmenity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelHotelAmenity_HotelAmenity_hotelAmenitiesId",
                table: "HotelHotelAmenity",
                column: "hotelAmenitiesId",
                principalTable: "HotelAmenity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
