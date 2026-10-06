using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authdemo.Migrations
{
    /// <inheritdoc />
    public partial class PopulateUserRoleIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        UPDATE Users
        SET RoleId = Roles.Id
        FROM Users
        INNER JOIN Roles
            ON Users.Role = Roles.Name;
        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            UPDATE Users
            SET RoleId = NULL;
            """);
        }
    }
}
