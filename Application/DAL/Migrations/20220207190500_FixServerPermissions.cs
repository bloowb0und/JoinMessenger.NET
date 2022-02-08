using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class FixServerPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatPermissionRole_ChatPermission_ChatPermissionId",
                table: "ChatPermissionRole");

            migrationBuilder.RenameColumn(
                name: "ServerPermissionStatus",
                table: "ServerPermissionRole",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "ServerPermissionRole",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChatPermissionId",
                table: "ChatPermissionRole",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServerPermissionRole_ServerId",
                table: "ServerPermissionRole",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatPermissionRole_ChatPermission_ChatPermissionId",
                table: "ChatPermissionRole",
                column: "ChatPermissionId",
                principalTable: "ChatPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServerPermissionRole_Servers_ServerId",
                table: "ServerPermissionRole",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatPermissionRole_ChatPermission_ChatPermissionId",
                table: "ChatPermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerPermissionRole_Servers_ServerId",
                table: "ServerPermissionRole");

            migrationBuilder.DropIndex(
                name: "IX_ServerPermissionRole_ServerId",
                table: "ServerPermissionRole");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "ServerPermissionRole");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ServerPermissionRole",
                newName: "ServerPermissionStatus");

            migrationBuilder.AlterColumn<int>(
                name: "ChatPermissionId",
                table: "ChatPermissionRole",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatPermissionRole_ChatPermission_ChatPermissionId",
                table: "ChatPermissionRole",
                column: "ChatPermissionId",
                principalTable: "ChatPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
