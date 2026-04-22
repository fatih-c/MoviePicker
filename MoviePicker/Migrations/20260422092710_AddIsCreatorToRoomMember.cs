using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviePicker.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCreatorToRoomMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCreator",
                table: "RoomMembers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreator",
                table: "RoomMembers");
        }
    }
}
