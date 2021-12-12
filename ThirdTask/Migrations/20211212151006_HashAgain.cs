using Microsoft.EntityFrameworkCore.Migrations;

namespace ThirdTask.Migrations
{
    public partial class HashAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "imageHash",
                table: "AnalyzedImages",
                newName: "ImageHash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageHash",
                table: "AnalyzedImages",
                newName: "imageHash");
        }
    }
}
