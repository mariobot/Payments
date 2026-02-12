using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hyip_Payments.Context.Migrations
{
    /// <inheritdoc />
    public partial class Add_Status_ProductID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "InvoiceItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "INIT");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_ProductId",
                table: "InvoiceItem",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItem_Product_ProductId",
                table: "InvoiceItem",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItem_Product_ProductId",
                table: "InvoiceItem");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItem_ProductId",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Invoice");
        }
    }
}
