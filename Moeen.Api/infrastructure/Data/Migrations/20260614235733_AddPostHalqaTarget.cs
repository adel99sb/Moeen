using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moeen.Api.infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostHalqaTarget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HalqaId",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_HalqaId",
                table: "Posts",
                column: "HalqaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Halqas_HalqaId",
                table: "Posts",
                column: "HalqaId",
                principalTable: "Halqas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Halqas_HalqaId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_HalqaId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "HalqaId",
                table: "Posts");
        }
    }
}
