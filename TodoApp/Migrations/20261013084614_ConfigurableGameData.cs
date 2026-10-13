using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TodoApp.Migrations
{
    /// <inheritdoc />
    public partial class ConfigurableGameData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "emoji",
                table: "equipment_slots",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "badges",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    icon = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    metric = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    target = table.Column<long>(type: "bigint", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badges", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "game_settings",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    value = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    group_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_settings", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "level_titles",
                columns: table => new
                {
                    min_level = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    icon = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    title_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_level_titles", x => x.min_level);
                    table.CheckConstraint("ck_level_titles_min_level", "min_level >= 1");
                    table.ForeignKey(
                        name: "FK_level_titles_titles_title_id",
                        column: x => x.title_id,
                        principalTable: "titles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "rarities",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    label = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rarities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "time_zones",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_zones", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "badges",
                columns: new[] { "id", "description", "icon", "metric", "name", "sort_order", "target" },
                values: new object[,]
                {
                    { "achiever", "Complete 5 tasks", "✅", "tasks_done", "Achiever", 8, 5L },
                    { "athlete", "Exercise 10 hours", "🏅", "sport_hours", "Athlete", 4, 10L },
                    { "bookworm", "Study 10 hours", "📚", "study_hours", "Bookworm", 3, 10L },
                    { "chain_breaker", "Resist a bad habit 10 times", "⛓️", "habit_times", "Chain Breaker", 6, 10L },
                    { "iron_will", "Skip junk food 10 times", "🥗", "clean_times", "Iron Will", 5, 10L },
                    { "knight", "Reach level 5", "🎖️", "level", "Knight", 7, 5L },
                    { "spark", "3-day streak", "🔥", "streak", "Spark", 1, 3L },
                    { "unstoppable", "7-day streak", "🌋", "streak", "Unstoppable", 2, 7L }
                });

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 0,
                column: "emoji",
                value: "🗡️");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 1,
                column: "emoji",
                value: "🪖");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 2,
                column: "emoji",
                value: "🛡️");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 3,
                column: "emoji",
                value: "👢");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 4,
                column: "emoji",
                value: "💍");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 5,
                column: "emoji",
                value: "📿");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 6,
                column: "emoji",
                value: "🥚");

            migrationBuilder.InsertData(
                table: "game_settings",
                columns: new[] { "key", "description", "group_name", "value" },
                values: new object[,]
                {
                    { "atk_base", "ATK at level 0", "stats", 5m },
                    { "atk_per_level", "ATK gained per level", "stats", 1m },
                    { "coop_capacity_base", "Coop capacity at level 0", "pets", 1m },
                    { "coop_capacity_per_level", "Extra pets the coop holds per level", "pets", 2m },
                    { "coop_upgrade_cost_per_level", "Coop upgrade cost = this x current level", "pets", 200m },
                    { "def_base", "DEF at level 0", "stats", 2m },
                    { "def_levels_per_point", "Levels needed to gain 1 DEF", "stats", 2m },
                    { "def_reduction_cap", "Maximum HP loss reduction from DEF (0.6 = 60%)", "rewards", 0.6m },
                    { "def_reduction_divisor", "HP loss reduction = DEF / this value", "rewards", 100m },
                    { "hatch_upgrade_chance", "Chance an egg hatches a pet one rarity tier higher", "pets", 0.10m },
                    { "heal_per_difficulty", "HP restored per difficulty point when a task is completed", "progression", 1m },
                    { "hp_base", "Max HP at level 0", "stats", 100m },
                    { "hp_per_level", "Max HP gained per level", "stats", 10m },
                    { "incubator_upgrade_cost_per_level", "Incubator upgrade cost = this x current level", "pets", 500m },
                    { "int_base", "INT at level 0", "stats", 5m },
                    { "int_per_level", "INT gained per level", "stats", 1m },
                    { "level_exp_base", "EXP needed for level N = base x (N-1)^exponent", "progression", 100m },
                    { "level_exp_exponent", "Exponent of the level curve (2 = quadratic)", "progression", 2m },
                    { "luck_gold_divisor", "Gold multiplier = 1 + LUK / this value", "rewards", 200m },
                    { "luk_base", "LUK at level 0", "stats", 1m },
                    { "luk_levels_per_point", "Levels needed to gain 1 LUK", "stats", 3m },
                    { "mana_base", "Max mana at level 0", "stats", 20m },
                    { "mana_per_level", "Max mana gained per level", "stats", 5m },
                    { "max_coop_level", "Highest pet coop level", "pets", 5m },
                    { "max_incubator_level", "Highest incubator level", "pets", 3m },
                    { "max_pet_level", "Highest pet level", "pets", 20m },
                    { "pet_feed_cost", "Gold cost of one feeding", "pets", 10m },
                    { "pet_feed_xp", "Pet XP gained per feeding", "pets", 15m },
                    { "pet_xp_per_level", "Pet XP needed to leave level N = this x N", "pets", 50m },
                    { "power_damage_divisor", "Boss damage multiplier = 1 + (ATK + INT/2) / this value", "rewards", 100m },
                    { "starting_gold", "Gold a brand-new character starts with", "progression", 100m },
                    { "streak_bonus_cap_days", "Streak days after which the bonus stops growing", "rewards", 10m },
                    { "streak_bonus_step", "Gold/EXP bonus per streak day (0.05 = +5%)", "rewards", 0.05m }
                });

            migrationBuilder.InsertData(
                table: "level_titles",
                columns: new[] { "min_level", "icon", "name", "title_id" },
                values: new object[,]
                {
                    { 1, "🌱", "Novice", null },
                    { 3, "📘", "Apprentice", null },
                    { 5, "⚔️", "Warrior", null },
                    { 8, "🛡️", "Knight", null },
                    { 12, "🎓", "Grandmaster", null },
                    { 16, "👑", "Legend", null }
                });

            migrationBuilder.InsertData(
                table: "rarities",
                columns: new[] { "id", "label", "sort_order" },
                values: new object[,]
                {
                    { "Common", "Common", 1 },
                    { "Epic", "Epic", 3 },
                    { "Legendary", "Legendary", 4 },
                    { "Rare", "Rare", 2 }
                });

            migrationBuilder.InsertData(
                table: "time_zones",
                columns: new[] { "id", "label", "sort_order" },
                values: new object[,]
                {
                    { "America/Los_Angeles", "US West Coast (Los Angeles)", 12 },
                    { "America/New_York", "US East Coast (New York)", 11 },
                    { "Asia/Bangkok", "Thailand (Bangkok)", 7 },
                    { "Asia/Ho_Chi_Minh", "Vietnam (UTC+7)", 1 },
                    { "Asia/Seoul", "South Korea (Seoul)", 9 },
                    { "Asia/Singapore", "Singapore", 6 },
                    { "Asia/Tokyo", "Japan (Tokyo)", 8 },
                    { "Australia/Sydney", "Australia (Sydney)", 10 },
                    { "Europe/Berlin", "Germany / Central Europe (Berlin)", 2 },
                    { "Europe/London", "United Kingdom (London)", 3 },
                    { "Europe/Moscow", "Russia (Moscow)", 5 },
                    { "Europe/Paris", "France (Paris)", 4 },
                    { "UTC", "UTC", 13 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_pet_species_rarity",
                table: "pet_species",
                column: "rarity");

            migrationBuilder.CreateIndex(
                name: "IX_items_rarity",
                table: "items",
                column: "rarity");

            migrationBuilder.CreateIndex(
                name: "IX_egg_types_base_rarity",
                table: "egg_types",
                column: "base_rarity");

            migrationBuilder.CreateIndex(
                name: "IX_level_titles_title_id",
                table: "level_titles",
                column: "title_id");

            migrationBuilder.AddForeignKey(
                name: "FK_egg_types_rarities_base_rarity",
                table: "egg_types",
                column: "base_rarity",
                principalTable: "rarities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_items_rarities_rarity",
                table: "items",
                column: "rarity",
                principalTable: "rarities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_pet_species_rarities_rarity",
                table: "pet_species",
                column: "rarity",
                principalTable: "rarities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_egg_types_rarities_base_rarity",
                table: "egg_types");

            migrationBuilder.DropForeignKey(
                name: "FK_items_rarities_rarity",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_pet_species_rarities_rarity",
                table: "pet_species");

            migrationBuilder.DropTable(
                name: "badges");

            migrationBuilder.DropTable(
                name: "game_settings");

            migrationBuilder.DropTable(
                name: "level_titles");

            migrationBuilder.DropTable(
                name: "rarities");

            migrationBuilder.DropTable(
                name: "time_zones");

            migrationBuilder.DropIndex(
                name: "IX_pet_species_rarity",
                table: "pet_species");

            migrationBuilder.DropIndex(
                name: "IX_items_rarity",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_egg_types_base_rarity",
                table: "egg_types");

            migrationBuilder.DropColumn(
                name: "emoji",
                table: "equipment_slots");
        }
    }
}
