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
                    Team01ID = table.Column<int>(type: "INTEGER", nullable: false),
                    Team02ID = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentUserID = table.Column<string>(type: "TEXT", nullable: false),
                    Round = table.Column<int>(type: "INTEGER", nullable: false),
                    RandomSeed = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMatchOver = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserWinnerID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_matches", x => x.MatchID);
                });

            migrationBuilder.CreateTable(
                name: "match_plays",
                schema: "match",
                columns: table => new
                {
                    PlayID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchID = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitInActionID = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillID = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetIDs = table.Column<string>(type: "TEXT", nullable: false),
                    MatchID1 = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_plays", x => x.PlayID);
                    table.ForeignKey(
                        name: "FK_match_plays_match_matches_MatchID1",
                        column: x => x.MatchID1,
                        principalSchema: "match",
                        principalTable: "match_matches",
                        principalColumn: "MatchID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_plays_MatchID1",
                schema: "match",
                table: "match_plays",
                column: "MatchID1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_plays",
                schema: "match");

            migrationBuilder.DropTable(
                name: "match_matches",
                schema: "match");
        }
    }
}
