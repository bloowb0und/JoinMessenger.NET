using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class FixedRequiredFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatPermissionRole_ChatPermissions_ChatPermissionId",
                table: "ChatPermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerPermissionRole_ServerPermissions_ServerPermissionId",
                table: "ServerPermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserServerRole_Roles_RoleId",
                table: "UserServerRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserServerRole_UserServer_UserServerId",
                table: "UserServerRole");

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

            migrationBuilder.AlterColumn<int>(
                name: "UserServerId",
                table: "UserServerRole",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "UserServerRole",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)");

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
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServerPermissionRole_ServerPermission_ServerPermissionId",
                table: "ServerPermissionRole",
                column: "ServerPermissionId",
                principalTable: "ServerPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserServerRole_Roles_RoleId",
                table: "UserServerRole",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserServerRole_UserServer_UserServerId",
                table: "UserServerRole",
                column: "UserServerId",
                principalTable: "UserServer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatPermissionRole_ChatPermission_ChatPermissionId",
                table: "ChatPermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerPermissionRole_ServerPermission_ServerPermissionId",
                table: "ServerPermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserServerRole_Roles_RoleId",
                table: "UserServerRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserServerRole_UserServer_UserServerId",
                table: "UserServerRole");

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

            migrationBuilder.AlterColumn<int>(
                name: "UserServerId",
                table: "UserServerRole",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "UserServerRole",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "varchar(150)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

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
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServerPermissionRole_ServerPermissions_ServerPermissionId",
                table: "ServerPermissionRole",
                column: "ServerPermissionId",
                principalTable: "ServerPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserServerRole_Roles_RoleId",
                table: "UserServerRole",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserServerRole_UserServer_UserServerId",
                table: "UserServerRole",
                column: "UserServerId",
                principalTable: "UserServer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
