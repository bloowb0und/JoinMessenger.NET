using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class fixingUserServer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserServerRole");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UserServer_Id",
                table: "UserServer");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserServer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserServer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_UserServer_Id",
                table: "UserServer",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserServerRole",
                columns: table => new
                {
                    UserServerID = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    DateApplied = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserServerRole", x => new { x.UserServerID, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserServerRole_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserServerRole_UserServer_UserServerID",
                        column: x => x.UserServerID,
                        principalTable: "UserServer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserServerRole_RoleId",
                table: "UserServerRole",
                column: "RoleId");
        }
    }
}
