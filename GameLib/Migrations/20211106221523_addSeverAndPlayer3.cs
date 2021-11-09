using Microsoft.EntityFrameworkCore.Migrations;

namespace GameLib.Migrations
{
    public partial class addSeverAndPlayer3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Players_ServerId",
                table: "Servers");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "Servers",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_Servers_ServerId",
                table: "Servers",
                newName: "IX_Servers_PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_Players_PlayerId",
                table: "Servers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Players_PlayerId",
                table: "Servers");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "Servers",
                newName: "ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_Servers_PlayerId",
                table: "Servers",
                newName: "IX_Servers_ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_Players_ServerId",
                table: "Servers",
                column: "ServerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
