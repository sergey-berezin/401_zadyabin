using Microsoft.EntityFrameworkCore.Migrations;

namespace ThirdTask.Migrations
{
    public partial class Hash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imageHash",
                table: "AnalyzedImages",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageHash",
                table: "AnalyzedImages");
        }
    }
}
