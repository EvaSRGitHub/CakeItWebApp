using Microsoft.EntityFrameworkCore.Migrations;

namespace CakeItWebApp.Data.Migrations
{
    public partial class DeletePostsColumn_ShortContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortContent",
                table: "Posts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortContent",
                table: "Posts",
                nullable: true);
        }
    }
}
