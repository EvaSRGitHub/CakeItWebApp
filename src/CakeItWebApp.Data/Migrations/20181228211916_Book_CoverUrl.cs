using Microsoft.EntityFrameworkCore.Migrations;

namespace CakeItWebApp.Data.Migrations
{
    public partial class Book_CoverUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DownloadUrl",
                table: "Books",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "CoverUrl",
                table: "Books",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverUrl",
                table: "Books");

            migrationBuilder.AlterColumn<int>(
                name: "DownloadUrl",
                table: "Books",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
