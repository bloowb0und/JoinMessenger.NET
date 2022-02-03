using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addedRepliedMessageToMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RepliedMessageId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RepliedMessageId",
                table: "Messages",
                column: "RepliedMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_RepliedMessageId",
                table: "Messages",
                column: "RepliedMessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_RepliedMessageId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_RepliedMessageId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "RepliedMessageId",
                table: "Messages");
        }
    }
}
