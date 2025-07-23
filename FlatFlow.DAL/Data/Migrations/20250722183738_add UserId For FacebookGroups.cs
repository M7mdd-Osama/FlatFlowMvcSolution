using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlatFlow.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class addUserIdForFacebookGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FacebookGroups",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookGroups_UserId",
                table: "FacebookGroups",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacebookGroups_AspNetUsers_UserId",
                table: "FacebookGroups",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacebookGroups_AspNetUsers_UserId",
                table: "FacebookGroups");

            migrationBuilder.DropIndex(
                name: "IX_FacebookGroups_UserId",
                table: "FacebookGroups");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FacebookGroups");
        }
    }
}
