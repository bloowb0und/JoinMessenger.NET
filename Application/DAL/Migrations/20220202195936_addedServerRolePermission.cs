using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addedServerRolePermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_ServerRole_Id",
                table: "ServerRole",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ServerRolePermission",
                columns: table => new
                {
                    ServerRoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerRolePermission", x => new { x.ServerRoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_ServerRolePermission_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServerRolePermission_ServerRole_ServerRoleId",
                        column: x => x.ServerRoleId,
                        principalTable: "ServerRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServerRolePermission_PermissionId",
                table: "ServerRolePermission",
                column: "PermissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerRolePermission");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ServerRole_Id",
                table: "ServerRole");
        }
    }
}
