using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hyip_Payments.Context.Migrations
{
    /// <inheritdoc />
    public partial class Status_Invoice_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "PaymentTransaction",
                newName: "StatusTransaction");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Payments",
                newName: "StatusPayment");

            migrationBuilder.AlterColumn<string>(
                name: "StatusInvoice",
                table: "Invoice",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusTransaction",
                table: "PaymentTransaction",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "StatusPayment",
                table: "Payments",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "StatusInvoice",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);
        }
    }
}
