using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moeen.Api.infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class tada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Teachers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TeacherExams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HalqaId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MosqueId",
                table: "SaturdayHalqes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "ProgressEntries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "ProgressEntries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "TeacherId",
                table: "Halqas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ExamTeacherHalqa",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Complaints",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuggestionStatus",
                table: "Complaints",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Complaints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Complaints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Students_HalqaId",
                table: "Students",
                column: "HalqaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Halqas_HalqaId",
                table: "Students",
                column: "HalqaId",
                principalTable: "Halqas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Halqas_HalqaId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_HalqaId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TeacherExams");

            migrationBuilder.DropColumn(
                name: "HalqaId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "MosqueId",
                table: "SaturdayHalqes");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "ProgressEntries");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "ProgressEntries");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ExamTeacherHalqa");

            migrationBuilder.DropColumn(
                name: "Response",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "SuggestionStatus",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Attendances");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeacherId",
                table: "Halqas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
