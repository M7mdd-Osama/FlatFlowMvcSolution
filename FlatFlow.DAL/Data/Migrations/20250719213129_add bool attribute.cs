using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlatFlow.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class addboolattribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVideo",
                table: "ApartmentImages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVideo",
                table: "ApartmentImages");
        }
    }
}
