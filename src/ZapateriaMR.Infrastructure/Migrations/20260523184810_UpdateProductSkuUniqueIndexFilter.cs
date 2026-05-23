using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZapateriaMR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductSkuUniqueIndexFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Productos_CodigoSku",
                table: "Productos");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CodigoSku",
                table: "Productos",
                column: "CodigoSku",
                unique: true,
                filter: "[EstaEliminado] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Productos_CodigoSku",
                table: "Productos");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CodigoSku",
                table: "Productos",
                column: "CodigoSku",
                unique: true);
        }
    }
}
