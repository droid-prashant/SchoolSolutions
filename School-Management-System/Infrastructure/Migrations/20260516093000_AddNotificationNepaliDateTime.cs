using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260516093000_AddNotificationNepaliDateTime")]
    public partial class AddNotificationNepaliDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Notifications"
                ADD COLUMN IF NOT EXISTS "NotificationDateTimeNp" character varying(40) NOT NULL DEFAULT '';
                ALTER TABLE "Notifications"
                ALTER COLUMN "NotificationDateTimeNp" DROP DEFAULT;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Notifications"
                DROP COLUMN IF EXISTS "NotificationDateTimeNp";
                """);
        }
    }
}
