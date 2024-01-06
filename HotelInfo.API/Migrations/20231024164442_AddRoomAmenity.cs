using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelInfo.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomAmenity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelHotelAmenity_HotelAmenities_hotelAmenitiesId",
                table: "HotelHotelAmenity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HotelHotelAmenity",
                table: "HotelHotelAmenity");

            migrationBuilder.DropIndex(
                name: "IX_HotelHotelAmenity_hotelAmenitiesId",
                table: "HotelHotelAmenity");

            migrationBuilder.RenameColumn(
                name: "hotelAmenitiesId",
                table: "HotelHotelAmenity",
                newName: "HotelAmenitiesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HotelHotelAmenity",
                table: "HotelHotelAmenity",
                columns: new[] { "HotelAmenitiesId", "HotelId" });

            migrationBuilder.CreateTable(
                name: "RoomAmenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAmenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomRoomAmenity",
                columns: table => new
                {
                    RoomAmenitiesId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomRoomAmenity", x => new { x.RoomAmenitiesId, x.RoomId });
                    table.ForeignKey(
                        name: "FK_RoomRoomAmenity_RoomAmenities_RoomAmenitiesId",
                        column: x => x.RoomAmenitiesId,
                        principalTable: "RoomAmenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomRoomAmenity_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HotelHotelAmenity_HotelId",
                table: "HotelHotelAmenity",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomRoomAmenity_RoomId",
                table: "RoomRoomAmenity",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelHotelAmenity_HotelAmenities_HotelAmenitiesId",
                table: "HotelHotelAmenity",
                column: "HotelAmenitiesId",
                principalTable: "HotelAmenities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelHotelAmenity_HotelAmenities_HotelAmenitiesId",
                table: "HotelHotelAmenity");

            migrationBuilder.DropTable(
                name: "RoomRoomAmenity");

            migrationBuilder.DropTable(
                name: "RoomAmenities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HotelHotelAmenity",
                table: "HotelHotelAmenity");

            migrationBuilder.DropIndex(
                name: "IX_HotelHotelAmenity_HotelId",
                table: "HotelHotelAmenity");

            migrationBuilder.RenameColumn(
                name: "HotelAmenitiesId",
                table: "HotelHotelAmenity",
                newName: "hotelAmenitiesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HotelHotelAmenity",
                table: "HotelHotelAmenity",
                columns: new[] { "HotelId", "hotelAmenitiesId" });

            migrationBuilder.CreateIndex(
                name: "IX_HotelHotelAmenity_hotelAmenitiesId",
                table: "HotelHotelAmenity",
                column: "hotelAmenitiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelHotelAmenity_HotelAmenities_hotelAmenitiesId",
                table: "HotelHotelAmenity",
                column: "hotelAmenitiesId",
                principalTable: "HotelAmenities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
