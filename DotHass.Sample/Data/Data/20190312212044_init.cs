using Microsoft.EntityFrameworkCore.Migrations;

namespace DotHass.Sample.Data.Data
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameRole",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    RoleName = table.Column<string>(nullable: true),
                    HeadId = table.Column<string>(nullable: true),
                    Money = table.Column<int>(nullable: false),
                    Gold = table.Column<int>(nullable: false),
                    Experience = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRole", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameRole");
        }
    }
}
