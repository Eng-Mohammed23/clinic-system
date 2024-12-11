using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddHangfireDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasWhats",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasWhats",
                table: "Bookings");
        }
    }
}
