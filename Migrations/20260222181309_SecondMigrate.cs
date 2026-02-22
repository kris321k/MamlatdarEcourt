using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MamlatdarEcourt.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Case_AspNetUsers_UserId",
                table: "Case");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Case",
                newName: "ApplicantId");

            migrationBuilder.RenameIndex(
                name: "IX_Case_UserId",
                table: "Case",
                newName: "IX_Case_ApplicantId");

            migrationBuilder.AlterColumn<int>(
                name: "DisputeCategory",
                table: "Case",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CaseNumber",
                table: "Case",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FiledDate",
                table: "Case",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Case",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Case_AspNetUsers_ApplicantId",
                table: "Case",
                column: "ApplicantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Case_AspNetUsers_ApplicantId",
                table: "Case");

            migrationBuilder.DropColumn(
                name: "CaseNumber",
                table: "Case");

            migrationBuilder.DropColumn(
                name: "FiledDate",
                table: "Case");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Case");

            migrationBuilder.RenameColumn(
                name: "ApplicantId",
                table: "Case",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Case_ApplicantId",
                table: "Case",
                newName: "IX_Case_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "DisputeCategory",
                table: "Case",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Case_AspNetUsers_UserId",
                table: "Case",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
