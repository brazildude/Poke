using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poke.Server.Data.Player.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "player");

            migrationBuilder.CreateTable(
                name: "player_flag_properties",
                schema: "player",
                columns: table => new
                {
                    FlagPropertyID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsTrue = table.Column<bool>(type: "INTEGER", nullable: false),
                    PropertyName = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_flag_properties", x => x.FlagPropertyID);
                });

            migrationBuilder.CreateTable(
                name: "player_users",
                schema: "player",
                columns: table => new
                {
                    UserID = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "player_teams",
                schema: "player",
                columns: table => new
                {
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_teams", x => x.TeamID);
                    table.ForeignKey(
                        name: "FK_player_teams_player_users_UserID",
                        column: x => x.UserID,
                        principalSchema: "player",
                        principalTable: "player_users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_units",
                schema: "player",
                columns: table => new
                {
                    UnitID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_units", x => x.UnitID);
                    table.ForeignKey(
                        name: "FK_player_units_player_teams_TeamID",
                        column: x => x.TeamID,
                        principalSchema: "player",
                        principalTable: "player_teams",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_skills",
                schema: "player",
                columns: table => new
                {
                    SkillID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnitID = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_skills", x => x.SkillID);
                    table.ForeignKey(
                        name: "FK_player_skills_player_units_UnitID",
                        column: x => x.UnitID,
                        principalSchema: "player",
                        principalTable: "player_units",
                        principalColumn: "UnitID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_behaviors",
                schema: "player",
                columns: table => new
                {
                    BehaviorID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SkillID = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_behaviors", x => x.BehaviorID);
                    table.ForeignKey(
                        name: "FK_player_behaviors_player_skills_SkillID",
                        column: x => x.SkillID,
                        principalSchema: "player",
                        principalTable: "player_skills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_costs",
                schema: "player",
                columns: table => new
                {
                    CostID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BehaviorID = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    CostPropertyName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_costs", x => x.CostID);
                    table.ForeignKey(
                        name: "FK_player_costs_player_behaviors_BehaviorID",
                        column: x => x.BehaviorID,
                        principalSchema: "player",
                        principalTable: "player_behaviors",
                        principalColumn: "BehaviorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_minmax_properties",
                schema: "player",
                columns: table => new
                {
                    MinMaxPropertyID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BehaviorID = table.Column<int>(type: "INTEGER", nullable: true),
                    MinBaseValue = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxBaseValue = table.Column<int>(type: "INTEGER", nullable: false),
                    MinCurrentValue = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxCurrentValue = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_minmax_properties", x => x.MinMaxPropertyID);
                    table.ForeignKey(
                        name: "FK_player_minmax_properties_player_behaviors_BehaviorID",
                        column: x => x.BehaviorID,
                        principalSchema: "player",
                        principalTable: "player_behaviors",
                        principalColumn: "BehaviorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_targets",
                schema: "player",
                columns: table => new
                {
                    TargetID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BehaviorID = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Direction = table.Column<string>(type: "TEXT", nullable: false),
                    TargetPropertyName = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_targets", x => x.TargetID);
                    table.ForeignKey(
                        name: "FK_player_targets_player_behaviors_BehaviorID",
                        column: x => x.BehaviorID,
                        principalSchema: "player",
                        principalTable: "player_behaviors",
                        principalColumn: "BehaviorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_flat_properties",
                schema: "player",
                columns: table => new
                {
                    FlatPropertyID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnitID = table.Column<int>(type: "INTEGER", nullable: true),
                    BehaviorID = table.Column<int>(type: "INTEGER", nullable: true),
                    SkillID = table.Column<int>(type: "INTEGER", nullable: true),
                    CostID = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseValue = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentValue = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitID1 = table.Column<int>(type: "INTEGER", nullable: true),
                    SkillID1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_flat_properties", x => x.FlatPropertyID);
                    table.ForeignKey(
                        name: "FK_player_flat_properties_player_behaviors_BehaviorID",
                        column: x => x.BehaviorID,
                        principalSchema: "player",
                        principalTable: "player_behaviors",
                        principalColumn: "BehaviorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_flat_properties_player_costs_CostID",
                        column: x => x.CostID,
                        principalSchema: "player",
                        principalTable: "player_costs",
                        principalColumn: "CostID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_flat_properties_player_skills_SkillID",
                        column: x => x.SkillID,
                        principalSchema: "player",
                        principalTable: "player_skills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_flat_properties_player_skills_SkillID1",
                        column: x => x.SkillID1,
                        principalSchema: "player",
                        principalTable: "player_skills",
                        principalColumn: "SkillID");
                    table.ForeignKey(
                        name: "FK_player_flat_properties_player_units_UnitID",
                        column: x => x.UnitID,
                        principalSchema: "player",
                        principalTable: "player_units",
                        principalColumn: "UnitID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_flat_properties_player_units_UnitID1",
                        column: x => x.UnitID1,
                        principalSchema: "player",
                        principalTable: "player_units",
                        principalColumn: "UnitID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_player_behaviors_SkillID",
                schema: "player",
                table: "player_behaviors",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_player_costs_BehaviorID",
                schema: "player",
                table: "player_costs",
                column: "BehaviorID");

            migrationBuilder.CreateIndex(
                name: "IX_player_flat_properties_BehaviorID",
                schema: "player",
                table: "player_flat_properties",
                column: "BehaviorID");

            migrationBuilder.CreateIndex(
                name: "IX_player_flat_properties_CostID",
                schema: "player",
                table: "player_flat_properties",
                column: "CostID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_flat_properties_SkillID",
                schema: "player",
                table: "player_flat_properties",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_player_flat_properties_SkillID1",
                schema: "player",
                table: "player_flat_properties",
                column: "SkillID1");

            migrationBuilder.CreateIndex(
                name: "IX_player_flat_properties_UnitID",
                schema: "player",
                table: "player_flat_properties",
                column: "UnitID");

            migrationBuilder.CreateIndex(
                name: "IX_player_flat_properties_UnitID1",
                schema: "player",
                table: "player_flat_properties",
                column: "UnitID1");

            migrationBuilder.CreateIndex(
                name: "IX_player_minmax_properties_BehaviorID",
                schema: "player",
                table: "player_minmax_properties",
                column: "BehaviorID");

            migrationBuilder.CreateIndex(
                name: "IX_player_skills_UnitID",
                schema: "player",
                table: "player_skills",
                column: "UnitID");

            migrationBuilder.CreateIndex(
                name: "IX_player_targets_BehaviorID",
                schema: "player",
                table: "player_targets",
                column: "BehaviorID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_teams_UserID",
                schema: "player",
                table: "player_teams",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_player_units_TeamID",
                schema: "player",
                table: "player_units",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_player_users_UserID",
                schema: "player",
                table: "player_users",
                column: "UserID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_flag_properties",
                schema: "player");

            migrationBuilder.DropTable(
                name: "player_flat_properties",
                schema: "player");

            migrationBuilder.DropTable(
                name: "player_minmax_properties",
                schema: "player");

            migrationBuilder.DropTable(
                name: "player_targets",
                schema: "player");

            migrationBuilder.DropTable(
                name: "player_costs",
                schema: "player");

            migrationBuilder.DropTable(
                name: "player_behaviors",
                schema: "player");

            migrationBuilder.DropTable(
                name: "player_skills",
                schema: "player");

            migrationBuilder.DropTable(
                name: "player_units",
                schema: "player");

            migrationBuilder.DropTable(
                name: "player_teams",
                schema: "player");

            migrationBuilder.DropTable(
                name: "player_users",
                schema: "player");
        }
    }
}
