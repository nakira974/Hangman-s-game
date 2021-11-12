using Microsoft.EntityFrameworkCore.Migrations;

namespace GameLib.Migrations
{
    public partial class migrateServerclass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentIpAddress",
                table: "Servers",
                newName: "IpAddress");

            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "Servers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "Servers");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "Servers",
                newName: "CurrentIpAddress");
        }
    }
}
