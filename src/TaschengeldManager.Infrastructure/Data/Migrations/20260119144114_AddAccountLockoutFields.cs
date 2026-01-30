using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaschengeldManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountLockoutFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_transactions_CreatedByUserId",
                table: "transactions");

            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEnd",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_AccountId_CreatedAt_Desc",
                table: "transactions",
                columns: new[] { "AccountId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_CreatedByUserId_Type",
                table: "transactions",
                columns: new[] { "CreatedByUserId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_sessions_UserId_IsRevoked_ExpiresAt",
                table: "sessions",
                columns: new[] { "UserId", "IsRevoked", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_accounts_InterestEnabled_Filtered",
                table: "accounts",
                column: "InterestEnabled",
                filter: "\"InterestEnabled\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_transactions_AccountId_CreatedAt_Desc",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_CreatedByUserId_Type",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_sessions_UserId_IsRevoked_ExpiresAt",
                table: "sessions");

            migrationBuilder.DropIndex(
                name: "IX_accounts_InterestEnabled_Filtered",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "users");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_CreatedByUserId",
                table: "transactions",
                column: "CreatedByUserId");
        }
    }
}
