using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireConnect.InterviewService.Migrations
{
    /// <inheritdoc />
    public partial class AddJobAndCandidateToInterview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CandidateId",
                schema: "interview",
                table: "interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                schema: "interview",
                table: "interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandidateId",
                schema: "interview",
                table: "interviews");

            migrationBuilder.DropColumn(
                name: "JobId",
                schema: "interview",
                table: "interviews");
        }
    }
}
