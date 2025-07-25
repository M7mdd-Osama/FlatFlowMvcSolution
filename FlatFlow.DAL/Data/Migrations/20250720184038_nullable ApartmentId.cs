﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlatFlow.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class nullableApartmentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Apartments_ApartmentId",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "ApartmentId",
                table: "Clients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Apartments_ApartmentId",
                table: "Clients",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Apartments_ApartmentId",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "ApartmentId",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Apartments_ApartmentId",
                table: "Clients",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
