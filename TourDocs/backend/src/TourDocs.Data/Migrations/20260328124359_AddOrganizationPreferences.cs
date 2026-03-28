using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourDocs.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailNotifications",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExpiryReminders",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Organizations",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "en");

            migrationBuilder.AddColumn<string>(
                name: "Timezone",
                table: "Organizations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "UTC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailNotifications",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ExpiryReminders",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Timezone",
                table: "Organizations");
        }
    }
}
