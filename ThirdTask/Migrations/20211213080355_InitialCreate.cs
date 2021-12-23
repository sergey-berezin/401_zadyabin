using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThirdTask.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalyzedImages",
                columns: table => new
                {
                    AnalyzedImageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: true),
                    ImageHash = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyzedImages", x => x.AnalyzedImageId);
                });

            migrationBuilder.CreateTable(
                name: "BoundingBox",
                columns: table => new
                {
                    BoundingBoxId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    x1 = table.Column<float>(type: "REAL", nullable: false),
                    y1 = table.Column<float>(type: "REAL", nullable: false),
                    x2 = table.Column<float>(type: "REAL", nullable: false),
                    y2 = table.Column<float>(type: "REAL", nullable: false),
                    Label = table.Column<string>(type: "TEXT", nullable: true),
                    Confidence = table.Column<float>(type: "REAL", nullable: false),
                    AnalyzedImageId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoundingBox", x => x.BoundingBoxId);
                    table.ForeignKey(
                        name: "FK_BoundingBox_AnalyzedImages_AnalyzedImageId",
                        column: x => x.AnalyzedImageId,
                        principalTable: "AnalyzedImages",
                        principalColumn: "AnalyzedImageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoundingBox_AnalyzedImageId",
                table: "BoundingBox",
                column: "AnalyzedImageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoundingBox");

            migrationBuilder.DropTable(
                name: "AnalyzedImages");
        }
    }
}
