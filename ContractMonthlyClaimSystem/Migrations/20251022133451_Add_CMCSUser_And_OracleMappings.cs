using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractMonthlyClaimSystem.Migrations
{
    /// <inheritdoc />
    public partial class Add_CMCSUser_And_OracleMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClaimItems_Claims_ClaimId",
                table: "ClaimItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Claims_ClaimId",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Claims",
                table: "Claims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClaimItems",
                table: "ClaimItems");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "DOCUMENTS");

            migrationBuilder.RenameTable(
                name: "Claims",
                newName: "CLAIMS");

            migrationBuilder.RenameTable(
                name: "ClaimItems",
                newName: "CLAIMITEMS");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_ClaimId",
                table: "DOCUMENTS",
                newName: "IX_DOCUMENTS_ClaimId");

            migrationBuilder.RenameColumn(
                name: "Month",
                table: "CLAIMS",
                newName: "CLAIM_MONTH");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "CLAIMITEMS",
                newName: "WORK_DATE");

            migrationBuilder.RenameIndex(
                name: "IX_ClaimItems_ClaimId",
                table: "CLAIMITEMS",
                newName: "IX_CLAIMITEMS_ClaimId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalHours",
                table: "CLAIMS",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "LecturerName",
                table: "CLAIMS",
                type: "NVARCHAR2(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "LecturerId",
                table: "CLAIMS",
                type: "NVARCHAR2(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Hours",
                table: "CLAIMITEMS",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18,2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DOCUMENTS",
                table: "DOCUMENTS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CLAIMS",
                table: "CLAIMS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CLAIMITEMS",
                table: "CLAIMITEMS",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CMCS_USERS",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMCS_USERS", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CMCS_USERS_Email",
                table: "CMCS_USERS",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CLAIMITEMS_CLAIMS_ClaimId",
                table: "CLAIMITEMS",
                column: "ClaimId",
                principalTable: "CLAIMS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DOCUMENTS_CLAIMS_ClaimId",
                table: "DOCUMENTS",
                column: "ClaimId",
                principalTable: "CLAIMS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CLAIMITEMS_CLAIMS_ClaimId",
                table: "CLAIMITEMS");

            migrationBuilder.DropForeignKey(
                name: "FK_DOCUMENTS_CLAIMS_ClaimId",
                table: "DOCUMENTS");

            migrationBuilder.DropTable(
                name: "CMCS_USERS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DOCUMENTS",
                table: "DOCUMENTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CLAIMS",
                table: "CLAIMS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CLAIMITEMS",
                table: "CLAIMITEMS");

            migrationBuilder.RenameTable(
                name: "DOCUMENTS",
                newName: "Documents");

            migrationBuilder.RenameTable(
                name: "CLAIMS",
                newName: "Claims");

            migrationBuilder.RenameTable(
                name: "CLAIMITEMS",
                newName: "ClaimItems");

            migrationBuilder.RenameIndex(
                name: "IX_DOCUMENTS_ClaimId",
                table: "Documents",
                newName: "IX_Documents_ClaimId");

            migrationBuilder.RenameColumn(
                name: "CLAIM_MONTH",
                table: "Claims",
                newName: "Month");

            migrationBuilder.RenameColumn(
                name: "WORK_DATE",
                table: "ClaimItems",
                newName: "Date");

            migrationBuilder.RenameIndex(
                name: "IX_CLAIMITEMS_ClaimId",
                table: "ClaimItems",
                newName: "IX_ClaimItems_ClaimId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalHours",
                table: "Claims",
                type: "DECIMAL(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(9,2)",
                oldPrecision: 9,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "LecturerName",
                table: "Claims",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "LecturerId",
                table: "Claims",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<decimal>(
                name: "Hours",
                table: "ClaimItems",
                type: "DECIMAL(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(9,2)",
                oldPrecision: 9,
                oldScale: 2);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Claims",
                table: "Claims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClaimItems",
                table: "ClaimItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClaimItems_Claims_ClaimId",
                table: "ClaimItems",
                column: "ClaimId",
                principalTable: "Claims",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Claims_ClaimId",
                table: "Documents",
                column: "ClaimId",
                principalTable: "Claims",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
