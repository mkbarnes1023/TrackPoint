using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackPoint.Migrations
{
    /// <inheritdoc />
    public partial class Icons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Category",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Asset",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Asset");
        }
    }
}
