using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poke.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplyValue",
                columns: table => new
                {
                    ApplyValueID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MinValue = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxValue = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    ToProperty = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplyValue", x => x.ApplyValueID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.TeamID);
                    table.ForeignKey(
                        name: "FK_Teams_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaseUnits",
                columns: table => new
                {
                    BaseUnitID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Life = table.Column<int>(type: "INTEGER", nullable: false),
                    Mana = table.Column<int>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    TeamID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseUnits", x => x.BaseUnitID);
                    table.ForeignKey(
                        name: "FK_BaseUnits_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID");
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    MatchID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CurrentTeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    Round = table.Column<int>(type: "INTEGER", nullable: false),
                    NextTeamTeamID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.MatchID);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_CurrentTeamID",
                        column: x => x.CurrentTeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_NextTeamTeamID",
                        column: x => x.NextTeamTeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaseSkills",
                columns: table => new
                {
                    BaseSkillID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SkillCostID = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplyValueID = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseUnitID = table.Column<int>(type: "INTEGER", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseSkills", x => x.BaseSkillID);
                    table.ForeignKey(
                        name: "FK_BaseSkills_ApplyValue_ApplyValueID",
                        column: x => x.ApplyValueID,
                        principalTable: "ApplyValue",
                        principalColumn: "ApplyValueID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaseSkills_ApplyValue_SkillCostID",
                        column: x => x.SkillCostID,
                        principalTable: "ApplyValue",
                        principalColumn: "ApplyValueID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaseSkills_BaseUnits_BaseUnitID",
                        column: x => x.BaseUnitID,
                        principalTable: "BaseUnits",
                        principalColumn: "BaseUnitID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseSkills_ApplyValueID",
                table: "BaseSkills",
                column: "ApplyValueID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseSkills_BaseUnitID",
                table: "BaseSkills",
                column: "BaseUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseSkills_SkillCostID",
                table: "BaseSkills",
                column: "SkillCostID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseUnits_TeamID",
                table: "BaseUnits",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CurrentTeamID",
                table: "Matches",
                column: "CurrentTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_NextTeamTeamID",
                table: "Matches",
                column: "NextTeamTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_UserID",
                table: "Teams",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseSkills");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "ApplyValue");

            migrationBuilder.DropTable(
                name: "BaseUnits");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
