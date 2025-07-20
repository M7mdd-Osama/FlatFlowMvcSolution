using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlatFlow.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class delete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "FacebookGroups");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "FacebookGroups");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FacebookGroups");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "FacebookGroups");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "FacebookGroups");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ApartmentImages");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ApartmentImages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ApartmentImages");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ApartmentImages");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "ApartmentImages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ApartmentGroupPosts");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ApartmentGroupPosts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ApartmentGroupPosts");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ApartmentGroupPosts");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "ApartmentGroupPosts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "FacebookGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "FacebookGroups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FacebookGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LastModifiedBy",
                table: "FacebookGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "FacebookGroups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Clients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LastModifiedBy",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Clients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Apartments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Apartments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LastModifiedBy",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Apartments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "ApartmentImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ApartmentImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ApartmentImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LastModifiedBy",
                table: "ApartmentImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "ApartmentImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "ApartmentGroupPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ApartmentGroupPosts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ApartmentGroupPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LastModifiedBy",
                table: "ApartmentGroupPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "ApartmentGroupPosts",
                type: "datetime2",
                nullable: true);
        }
    }
}
