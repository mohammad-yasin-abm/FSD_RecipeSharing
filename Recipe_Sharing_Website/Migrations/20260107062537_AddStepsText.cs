using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipe_Sharing_Website.Migrations
{
    /// <inheritdoc />
    public partial class AddStepsText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StepsText",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StepsText",
                table: "Recipes");
        }
    }
}
