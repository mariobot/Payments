using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hyip_Payments.Context.Migrations
{
    /// <inheritdoc />
    public partial class Add_Presition_Wallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserTenant",
                columns: new[] { "Id", "Name", "IsActive", "CreatedAt" },
                values: new object[] { 1, "HomeCompany", true, DateTime.UtcNow }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserTenant",
                keyColumn: "Id",
                keyValue: 1
            );
        }
    }
}
