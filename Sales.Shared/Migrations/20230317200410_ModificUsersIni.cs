using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sales.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ModificUsersIni : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MyFileStorageImage",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MyFileStorageImage",
                table: "AspNetUsers");
        }
    }
}
