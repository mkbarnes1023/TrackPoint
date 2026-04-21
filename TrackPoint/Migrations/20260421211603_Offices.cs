using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackPoint.Migrations
{
    /// <inheritdoc />
    public partial class Offices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OfficeId",
                table: "Asset",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Office",
                columns: table => new
                {
                    OfficeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Office", x => x.OfficeId);
                    table.ForeignKey(
                        name: "FK_Office_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "LocationId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asset_OfficeId",
                table: "Asset",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Office_LocationId",
                table: "Office",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Office_OfficeId",
                table: "Asset",
                column: "OfficeId",
                principalTable: "Office",
                principalColumn: "OfficeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Office_OfficeId",
                table: "Asset");

            migrationBuilder.DropTable(
                name: "Office");

            migrationBuilder.DropIndex(
                name: "IX_Asset_OfficeId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "Asset");
        }
    }
}
