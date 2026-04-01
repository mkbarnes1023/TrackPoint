using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackPoint.Migrations
{
    /// <inheritdoc />
    public partial class AuditTrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditTrail_ApprovalReason_ApprovalReasonReasonId",
                table: "AuditTrail");

            migrationBuilder.AlterColumn<int>(
                name: "RelatedApprovalId",
                table: "AuditTrail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ApprovalReasonReasonId",
                table: "AuditTrail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditTrail_ApprovalReason_ApprovalReasonReasonId",
                table: "AuditTrail",
                column: "ApprovalReasonReasonId",
                principalTable: "ApprovalReason",
                principalColumn: "ReasonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditTrail_ApprovalReason_ApprovalReasonReasonId",
                table: "AuditTrail");

            migrationBuilder.AlterColumn<int>(
                name: "RelatedApprovalId",
                table: "AuditTrail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ApprovalReasonReasonId",
                table: "AuditTrail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditTrail_ApprovalReason_ApprovalReasonReasonId",
                table: "AuditTrail",
                column: "ApprovalReasonReasonId",
                principalTable: "ApprovalReason",
                principalColumn: "ReasonId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
