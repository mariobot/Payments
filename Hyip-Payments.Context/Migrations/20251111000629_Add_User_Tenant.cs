using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hyip_Payments.Context.Migrations
{
    /// <inheritdoc />
    public partial class Add_User_Tenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserTenantModelId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserApplicationModelId",
                table: "Role",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserTenant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserApplication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Password = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UserTenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserApplication_UserTenant_UserTenantId",
                        column: x => x.UserTenantId,
                        principalTable: "UserTenant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserApplication_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_UserTenantModelId",
                table: "User",
                column: "UserTenantModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_UserApplicationModelId",
                table: "Role",
                column: "UserApplicationModelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplication_UserId",
                table: "UserApplication",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplication_UserTenantId",
                table: "UserApplication",
                column: "UserTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_UserApplication_UserApplicationModelId",
                table: "Role",
                column: "UserApplicationModelId",
                principalTable: "UserApplication",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserTenant_UserTenantModelId",
                table: "User",
                column: "UserTenantModelId",
                principalTable: "UserTenant",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_UserApplication_UserApplicationModelId",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_User_UserTenant_UserTenantModelId",
                table: "User");

            migrationBuilder.DropTable(
                name: "UserApplication");

            migrationBuilder.DropTable(
                name: "UserTenant");

            migrationBuilder.DropIndex(
                name: "IX_User_UserTenantModelId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Role_UserApplicationModelId",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "UserTenantModelId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserApplicationModelId",
                table: "Role");
        }
    }
}
