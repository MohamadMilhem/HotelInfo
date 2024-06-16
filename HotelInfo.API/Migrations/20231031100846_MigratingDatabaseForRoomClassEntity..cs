using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelInfo.API.Migrations
{
    /// <inheritdoc />
    public partial class MigratingDatabaseForRoomClassEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "Rooms",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomClassId",
                table: "Photos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoomAmenityRoomClass",
                columns: table => new
                {
                    RoomAmenitiesId = table.Column<int>(type: "int", nullable: false),
                    RoomClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAmenityRoomClass", x => new { x.RoomAmenitiesId, x.RoomClassId });
                    table.ForeignKey(
                        name: "FK_RoomAmenityRoomClass_RoomAmenities_RoomAmenitiesId",
                        column: x => x.RoomAmenitiesId,
                        principalTable: "RoomAmenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomAmenityRoomClass_RoomClasses_RoomClassId",
                        column: x => x.RoomClassId,
                        principalTable: "RoomClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_RoomClassId",
                table: "Photos",
                column: "RoomClassId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAmenityRoomClass_RoomClassId",
                table: "RoomAmenityRoomClass",
                column: "RoomClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_RoomClasses_RoomClassId",
                table: "Photos",
                column: "RoomClassId",
                principalTable: "RoomClasses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_RoomClasses_RoomClassId",
                table: "Photos");

            migrationBuilder.DropTable(
                name: "RoomAmenityRoomClass");

            migrationBuilder.DropIndex(
                name: "IX_Photos_RoomClassId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomClassId",
                table: "Photos");
        }
    }
}
