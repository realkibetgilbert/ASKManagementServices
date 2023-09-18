using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASK.API.Migrations
{
    public partial class RealPasswordResetToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RealPasswordResetToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RealPasswordResetToken",
                table: "AspNetUsers");
        }
    }
}
