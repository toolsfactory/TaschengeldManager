using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaschengeldManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringPaymentsMoneyRequestsInterest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedByUserId",
                table: "transactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "InterestInterval",
                table: "accounts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "money_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResponseNote = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RespondedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultingTransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_money_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_money_requests_transactions_ResultingTransactionId",
                        column: x => x.ResultingTransactionId,
                        principalTable: "transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_money_requests_users_ChildUserId",
                        column: x => x.ChildUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_money_requests_users_RespondedByUserId",
                        column: x => x.RespondedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recurring_payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Interval = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: true),
                    DayOfMonth = table.Column<int>(type: "integer", nullable: true),
                    NextExecutionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastExecutionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recurring_payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_recurring_payments_accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recurring_payments_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_money_requests_ChildUserId",
                table: "money_requests",
                column: "ChildUserId");

            migrationBuilder.CreateIndex(
                name: "IX_money_requests_ChildUserId_Status",
                table: "money_requests",
                columns: new[] { "ChildUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_money_requests_RespondedByUserId",
                table: "money_requests",
                column: "RespondedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_money_requests_ResultingTransactionId",
                table: "money_requests",
                column: "ResultingTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_money_requests_Status",
                table: "money_requests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_recurring_payments_AccountId",
                table: "recurring_payments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_recurring_payments_CreatedByUserId",
                table: "recurring_payments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_recurring_payments_IsActive_NextExecutionDate",
                table: "recurring_payments",
                columns: new[] { "IsActive", "NextExecutionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_recurring_payments_NextExecutionDate",
                table: "recurring_payments",
                column: "NextExecutionDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "money_requests");

            migrationBuilder.DropTable(
                name: "recurring_payments");

            migrationBuilder.DropColumn(
                name: "InterestInterval",
                table: "accounts");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedByUserId",
                table: "transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
