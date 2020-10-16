using Microsoft.EntityFrameworkCore.Migrations;

namespace RcrsCore.IdentityServer.Data.Migrations.City
{
    public partial class CityDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "M_市区町村",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    市区町村CD = table.Column<string>(maxLength: 6, nullable: true),
                    都道府県CD = table.Column<string>(maxLength: 2, nullable: true),
                    都道府県名 = table.Column<string>(maxLength: 100, nullable: true),
                    市区町村名 = table.Column<string>(maxLength: 100, nullable: true),
                    郵便番号 = table.Column<string>(maxLength: 50, nullable: true),
                    住所 = table.Column<string>(nullable: true),
                    メモ = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_市区町村", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "M_都道府県",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    都道府県CD = table.Column<string>(maxLength: 2, nullable: true),
                    都道府県名 = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_都道府県", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "M_市区町村");

            migrationBuilder.DropTable(
                name: "M_都道府県");
        }
    }
}
