using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hyip_Payments.Context.Migrations
{
    /// <inheritdoc />
    public partial class Status_Invoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Invoice",
                newName: "StatusInvoice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusInvoice",
                table: "Invoice",
                newName: "Status");
        }
    }
}
