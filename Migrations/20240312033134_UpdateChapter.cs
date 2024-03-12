using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3_Course.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChapter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Course_CourseId",
                table: "Lesson");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Lesson",
                newName: "ChapterId");

            migrationBuilder.RenameIndex(
                name: "IX_Lesson_CourseId",
                table: "Lesson",
                newName: "IX_Lesson_ChapterId");

            migrationBuilder.CreateTable(
                name: "Chapter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChapterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chapter_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_CourseId",
                table: "Chapter",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Chapter_ChapterId",
                table: "Lesson",
                column: "ChapterId",
                principalTable: "Chapter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Chapter_ChapterId",
                table: "Lesson");

            migrationBuilder.DropTable(
                name: "Chapter");

            migrationBuilder.RenameColumn(
                name: "ChapterId",
                table: "Lesson",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Lesson_ChapterId",
                table: "Lesson",
                newName: "IX_Lesson_CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Course_CourseId",
                table: "Lesson",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
