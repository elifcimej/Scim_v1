using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scım_v1.Migrations
{
    /// <inheritdoc />
    public partial class fixeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActice",
                table: "Users",
                newName: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users",
                newName: "IsActice");
        }
    }
}
