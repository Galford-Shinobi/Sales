using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sales.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddTemporalSalesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodeBar",
                table: "Products",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TemporalSales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemporalSaleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporalSales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemporalSales_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TemporalSales_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemporalSales_TemporalSales_TemporalSaleId",
                        column: x => x.TemporalSaleId,
                        principalTable: "TemporalSales",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CodeBar",
                table: "Products",
                column: "CodeBar",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemporalSales_ProductId",
                table: "TemporalSales",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TemporalSales_TemporalSaleId",
                table: "TemporalSales",
                column: "TemporalSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_TemporalSales_UserId",
                table: "TemporalSales",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemporalSales");

            migrationBuilder.DropIndex(
                name: "IX_Products_CodeBar",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CodeBar",
                table: "Products");
        }
    }
}
