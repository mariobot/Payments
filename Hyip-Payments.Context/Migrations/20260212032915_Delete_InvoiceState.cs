using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hyip_Payments.Context.Migrations
{
    /// <inheritdoc />
    public partial class Delete_InvoiceState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceState",
                table: "Invoice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceState",
                table: "Invoice",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }
    }
}
