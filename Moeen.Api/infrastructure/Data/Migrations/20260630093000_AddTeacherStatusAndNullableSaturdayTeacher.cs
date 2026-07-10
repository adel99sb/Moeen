using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moeen.Api.infrastructure.Data;

#nullable disable

namespace Moeen.Api.infrastructure.Data.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260630093000_AddTeacherStatusAndNullableSaturdayTeacher")]
    public partial class AddTeacherStatusAndNullableSaturdayTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaturdayHalqes_Teachers_TeacherId",
                table: "SaturdayHalqes");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Teachers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "TeacherId",
                table: "SaturdayHalqes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_SaturdayHalqes_Teachers_TeacherId",
                table: "SaturdayHalqes",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaturdayHalqes_Teachers_TeacherId",
                table: "SaturdayHalqes");

            migrationBuilder.Sql("DELETE FROM SaturdayHalqes WHERE TeacherId IS NULL;");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeacherId",
                table: "SaturdayHalqes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "status",
                table: "Teachers");

            migrationBuilder.AddForeignKey(
                name: "FK_SaturdayHalqes_Teachers_TeacherId",
                table: "SaturdayHalqes",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
