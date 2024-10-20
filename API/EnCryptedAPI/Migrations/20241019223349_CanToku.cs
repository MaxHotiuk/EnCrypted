using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnCryptedAPI.Migrations
{
    /// <inheritdoc />
    public partial class CanToku : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CancelationTokenID",
                table: "CancelationTokens",
                newName: "CancellationTokenID");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CancelationTokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsCanceled",
                table: "CancelationTokens",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CancelationTokens");

            migrationBuilder.DropColumn(
                name: "IsCanceled",
                table: "CancelationTokens");

            migrationBuilder.RenameColumn(
                name: "CancellationTokenID",
                table: "CancelationTokens",
                newName: "CancelationTokenID");
        }
    }
}
