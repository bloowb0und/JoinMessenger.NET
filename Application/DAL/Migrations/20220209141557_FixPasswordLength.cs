using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class FixPasswordLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatPermissionRole_ChatPermission_ChatPermissionId",
                table: "ChatPermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerPermissionRole_ServerPermission_ServerPermissionId",
                table: "ServerPermissionRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServerPermission",
                table: "ServerPermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatPermission",
                table: "ChatPermission");

            migrationBuilder.RenameTable(
                name: "ServerPermission",
                newName: "ServerPermissions");

            migrationBuilder.RenameTable(
                name: "ChatPermission",
                newName: "ChatPermissions");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServerPermissions",
                table: "ServerPermissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatPermissions",
                table: "ChatPermissions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatPermissionRole_ChatPermissions_ChatPermissionId",
                table: "ChatPermissionRole",
                column: "ChatPermissionId",
                principalTable: "ChatPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServerPermissionRole_ServerPermissions_ServerPermissionId",
                table: "ServerPermissionRole",
                column: "ServerPermissionId",
                principalTable: "ServerPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatPermissionRole_ChatPermissions_ChatPermissionId",
                table: "ChatPermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerPermissionRole_ServerPermissions_ServerPermissionId",
                table: "ServerPermissionRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServerPermissions",
                table: "ServerPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatPermissions",
                table: "ChatPermissions");

            migrationBuilder.RenameTable(
                name: "ServerPermissions",
                newName: "ServerPermission");

            migrationBuilder.RenameTable(
                name: "ChatPermissions",
                newName: "ChatPermission");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServerPermission",
                table: "ServerPermission",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatPermission",
                table: "ChatPermission",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatPermissionRole_ChatPermission_ChatPermissionId",
                table: "ChatPermissionRole",
                column: "ChatPermissionId",
                principalTable: "ChatPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServerPermissionRole_ServerPermission_ServerPermissionId",
                table: "ServerPermissionRole",
                column: "ServerPermissionId",
                principalTable: "ServerPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
