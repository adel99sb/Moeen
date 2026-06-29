using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moeen.Api.infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddParentStudentLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParentStudentLinks",
                columns: table => new
                {
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentStudentLinks", x => new { x.ParentId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_ParentStudentLinks_Students_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParentStudentLinks_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParentStudentLinks_StudentId",
                table: "ParentStudentLinks",
                column: "StudentId");

            // Backfill legacy one-parent relation into the new parent-student link table.
            migrationBuilder.Sql(@"
                INSERT INTO ParentStudentLinks (ParentId, StudentId, CreatedAt)
                SELECT s.ParentId, s.Id, GETUTCDATE()
                FROM Students s
                WHERE s.ParentId IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM ParentStudentLinks link
                      WHERE link.ParentId = s.ParentId AND link.StudentId = s.Id
                  );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParentStudentLinks");
        }
    }
}
