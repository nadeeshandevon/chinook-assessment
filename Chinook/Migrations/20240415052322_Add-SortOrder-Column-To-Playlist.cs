using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chinook.Migrations
{
    /// <inheritdoc />
    public partial class AddSortOrderColumnToPlaylist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Playlist",
                keyColumn: "PlaylistId",
                keyValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "SortOrder",
                table: "Playlist",
                type: "NUMERIC(10)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Playlist");

            migrationBuilder.InsertData(
                table: "Playlist",
                columns: new[] { "PlaylistId", "Name" },
                values: new object[] { 0L, "My favorite tracks" });
        }
    }
}
