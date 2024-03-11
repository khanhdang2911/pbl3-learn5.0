using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3_Course.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableCourse_AddDateEditColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateEdited",
                table: "Course",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateEdited",
                table: "Course");
        }
    }
}
