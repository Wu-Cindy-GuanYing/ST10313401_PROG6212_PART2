using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractMonthlyClaimSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitClaims_Oracle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    LecturerId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    LecturerName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Month = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TotalHours = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClaimItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ClaimId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Hours = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false),
                    Rate = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimItems_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ClaimId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FileName = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ContentType = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    SizeBytes = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimItems_ClaimId",
                table: "ClaimItems",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ClaimId",
                table: "Documents",
                column: "ClaimId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimItems");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Claims");
        }
    }
}
