using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelInfo.API.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelAmenitiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Cities_CityId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Hotels_HotelId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Rooms_RoomId",
                table: "Photos");

            migrationBuilder.AlterColumn<int>(
                name: "RoomId",
                table: "Photos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Photos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Photos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "HotelAmenity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelAmenity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HotelHotelAmenity",
                columns: table => new
                {
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    hotelAmenitiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelHotelAmenity", x => new { x.HotelId, x.hotelAmenitiesId });
                    table.ForeignKey(
                        name: "FK_HotelHotelAmenity_HotelAmenity_hotelAmenitiesId",
                        column: x => x.hotelAmenitiesId,
                        principalTable: "HotelAmenity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HotelHotelAmenity_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HotelHotelAmenity_hotelAmenitiesId",
                table: "HotelHotelAmenity",
                column: "hotelAmenitiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Cities_CityId",
                table: "Photos",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Hotels_HotelId",
                table: "Photos",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Rooms_RoomId",
                table: "Photos",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Cities_CityId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Hotels_HotelId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Rooms_RoomId",
                table: "Photos");

            migrationBuilder.DropTable(
                name: "HotelHotelAmenity");

            migrationBuilder.DropTable(
                name: "HotelAmenity");

            migrationBuilder.AlterColumn<int>(
                name: "RoomId",
                table: "Photos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Photos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Photos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Cities_CityId",
                table: "Photos",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Hotels_HotelId",
                table: "Photos",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Rooms_RoomId",
                table: "Photos",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
