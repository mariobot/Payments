using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hyip_Payments.Context.Migrations
{
    /// <inheritdoc />
    public partial class Add_Wallet_Payments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodModelId",
                table: "Invoice",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WalletModelId",
                table: "Invoice",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wallet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PaymentMethodModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallet_PaymentMethod_PaymentMethodModelId",
                        column: x => x.PaymentMethodModelId,
                        principalTable: "PaymentMethod",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    WalletId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_Wallet_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_PaymentMethodModelId",
                table: "Invoice",
                column: "PaymentMethodModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_WalletModelId",
                table: "Invoice",
                column: "WalletModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_InvoiceId",
                table: "PaymentTransaction",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_PaymentMethodId",
                table: "PaymentTransaction",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_WalletId",
                table: "PaymentTransaction",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_PaymentMethodModelId",
                table: "Wallet",
                column: "PaymentMethodModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_PaymentMethod_PaymentMethodModelId",
                table: "Invoice",
                column: "PaymentMethodModelId",
                principalTable: "PaymentMethod",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Wallet_WalletModelId",
                table: "Invoice",
                column: "WalletModelId",
                principalTable: "Wallet",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_PaymentMethod_PaymentMethodModelId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Wallet_WalletModelId",
                table: "Invoice");

            migrationBuilder.DropTable(
                name: "PaymentTransaction");

            migrationBuilder.DropTable(
                name: "Wallet");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_PaymentMethodModelId",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_WalletModelId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "PaymentMethodModelId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "WalletModelId",
                table: "Invoice");
        }
    }
}
