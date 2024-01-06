using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelInfo.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomClassTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomClassId",
                table: "Rooms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoomClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StandardCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomClasses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomClassId",
                table: "Rooms",
                column: "RoomClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomClasses_RoomClassId",
                table: "Rooms",
                column: "RoomClassId",
                principalTable: "RoomClasses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomClasses_RoomClassId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "RoomClasses");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomClassId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomClassId",
                table: "Rooms");
        }
    }
}
