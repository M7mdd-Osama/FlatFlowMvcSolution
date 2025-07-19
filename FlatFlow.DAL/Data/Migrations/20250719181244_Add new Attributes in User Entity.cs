using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlatFlow.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddnewAttributesinUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordCodeExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPasswordCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ResetPasswordCodeExpiry",
                table: "AspNetUsers");
        }
    }
}
