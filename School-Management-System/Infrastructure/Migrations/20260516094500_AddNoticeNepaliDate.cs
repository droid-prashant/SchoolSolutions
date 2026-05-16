using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260516094500_AddNoticeNepaliDate")]
    public partial class AddNoticeNepaliDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Notices"
                ADD COLUMN IF NOT EXISTS "NoticeDateNp" character varying(20) NOT NULL DEFAULT '';
                ALTER TABLE "Notices"
                ALTER COLUMN "NoticeDateNp" DROP DEFAULT;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Notices"
                DROP COLUMN IF EXISTS "NoticeDateNp";
                """);
        }
    }
}
