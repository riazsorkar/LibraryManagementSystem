using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class DonationRequestfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonationRequests_Users_UserId",
                table: "DonationRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DonationRequests",
                table: "DonationRequests");

            migrationBuilder.RenameTable(
                name: "DonationRequests",
                newName: "donationrequests");

            migrationBuilder.RenameIndex(
                name: "IX_DonationRequests_UserId",
                table: "donationrequests",
                newName: "IX_donationrequests_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDate",
                table: "donationrequests",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessedDate",
                table: "donationrequests",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BookTitle",
                table: "donationrequests",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "donationrequests",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_donationrequests",
                table: "donationrequests",
                column: "RequestId");

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "SettingId",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 8, 22, 7, 38, 29, 967, DateTimeKind.Utc).AddTicks(8145));

            migrationBuilder.AddForeignKey(
                name: "FK_donationrequests_Users_UserId",
                table: "donationrequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_donationrequests_Users_UserId",
                table: "donationrequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_donationrequests",
                table: "donationrequests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "donationrequests");

            migrationBuilder.RenameTable(
                name: "donationrequests",
                newName: "DonationRequests");

            migrationBuilder.RenameIndex(
                name: "IX_donationrequests_UserId",
                table: "DonationRequests",
                newName: "IX_DonationRequests_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDate",
                table: "DonationRequests",
                type: "datetime2(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessedDate",
                table: "DonationRequests",
                type: "datetime2(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BookTitle",
                table: "DonationRequests",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DonationRequests",
                table: "DonationRequests",
                column: "RequestId");

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "SettingId",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 8, 22, 6, 49, 31, 513, DateTimeKind.Utc).AddTicks(1214));

            migrationBuilder.AddForeignKey(
                name: "FK_DonationRequests_Users_UserId",
                table: "DonationRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
