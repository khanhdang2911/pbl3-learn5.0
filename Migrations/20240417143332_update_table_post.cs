using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3_Course.Migrations
{
    /// <inheritdoc />
    public partial class update_table_post : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostName",
                table: "Post",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostName",
                table: "Post");
        }
    }
}
