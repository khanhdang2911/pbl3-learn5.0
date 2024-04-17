using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3_Course.Migrations
{
    /// <inheritdoc />
    public partial class update_table_post_addImageLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageLink",
                table: "Post",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageLink",
                table: "Post");
        }
    }
}
