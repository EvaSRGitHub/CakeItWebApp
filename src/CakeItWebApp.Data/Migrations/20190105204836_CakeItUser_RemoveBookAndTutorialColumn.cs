using Microsoft.EntityFrameworkCore.Migrations;

namespace CakeItWebApp.Data.Migrations
{
    public partial class CakeItUser_RemoveBookAndTutorialColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_CakeItUserId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Tutorials_AspNetUsers_CakeItUserId",
                table: "Tutorials");

            migrationBuilder.DropIndex(
                name: "IX_Tutorials_CakeItUserId",
                table: "Tutorials");

            migrationBuilder.DropIndex(
                name: "IX_Books_CakeItUserId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CakeItUserId",
                table: "Tutorials");

            migrationBuilder.DropColumn(
                name: "CakeItUserId",
                table: "Books");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CakeItUserId",
                table: "Tutorials",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CakeItUserId",
                table: "Books",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tutorials_CakeItUserId",
                table: "Tutorials",
                column: "CakeItUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CakeItUserId",
                table: "Books",
                column: "CakeItUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_CakeItUserId",
                table: "Books",
                column: "CakeItUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tutorials_AspNetUsers_CakeItUserId",
                table: "Tutorials",
                column: "CakeItUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
