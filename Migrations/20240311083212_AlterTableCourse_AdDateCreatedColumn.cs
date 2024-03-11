using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3_Course.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableCourse_AdDateCreatedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {




            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Course",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Course");


        }
    }
}
