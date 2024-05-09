using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3_Course.Migrations
{
    /// <inheritdoc />
    public partial class Add_UserTest_EditKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersTest",
                table: "UsersTest");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersTest",
                table: "UsersTest",
                columns: new[] { "UsersId", "TestId", "DateSubmited" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersTest",
                table: "UsersTest");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersTest",
                table: "UsersTest",
                columns: new[] { "UsersId", "TestId" });
        }
    }
}
