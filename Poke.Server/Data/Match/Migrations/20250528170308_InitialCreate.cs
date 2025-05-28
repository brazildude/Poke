using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poke.Server.Data.Match.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "match");

            migrationBuilder.CreateTable(
                name: "match_matches",
                schema: "match",
                columns: table => new
                {
                    MatchID = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserID01 = table.Column<string>(type: "TEXT", nullable: false),
                    UserID02 = table.Column<string>(type: "TEXT", nullable: false),
                    Team01ID = table.Column<int>(type: "INTEGER", nullable: false),
                    Team02ID = table.Column<int>(type: "INTEGER", nullable: false),
                    UserWinnerID = table.Column<string>(type: "TEXT", nullable: true),
                    IsMatchOver = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_matches", x => x.MatchID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_matches",
                schema: "match");
        }
    }
}
