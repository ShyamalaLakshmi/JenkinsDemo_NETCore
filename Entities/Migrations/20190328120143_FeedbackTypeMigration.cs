using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations
{
    public partial class FeedbackTypeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Attended",
                table: "Feedbacks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotAttended",
                table: "Feedbacks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Unregistered",
                table: "Feedbacks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attended",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "NotAttended",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Unregistered",
                table: "Feedbacks");
        }
    }
}
