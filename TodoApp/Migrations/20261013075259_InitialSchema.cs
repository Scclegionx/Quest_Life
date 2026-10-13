using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TodoApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bosses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    emoji = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    lore = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    max_hp = table.Column<long>(type: "bigint", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bosses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    slug = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    board_name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    emoji = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    unit = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    is_hours = table.Column<bool>(type: "boolean", nullable: false),
                    max_per_entry = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    tagline = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "difficulty_levels",
                columns: table => new
                {
                    level = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    base_gold = table.Column<int>(type: "integer", nullable: false),
                    base_exp = table.Column<int>(type: "integer", nullable: false),
                    base_damage = table.Column<int>(type: "integer", nullable: false),
                    base_hp_loss = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_difficulty_levels", x => x.level);
                    table.CheckConstraint("ck_difficulty_level", "level BETWEEN 1 AND 5");
                });

            migrationBuilder.CreateTable(
                name: "pet_species",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    emoji = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    rarity = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pet_species", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shop_categories",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    emoji = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop_categories", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    description = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    mana_cost = table.Column<int>(type: "integer", nullable: false),
                    cooldown_seconds = table.Column<int>(type: "integer", nullable: false),
                    effect_json = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "titles",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    icon = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_titles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    time_zone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "raids",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    boss_id = table.Column<int>(type: "integer", nullable: false),
                    starts_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ends_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    max_hp = table.Column<long>(type: "bigint", nullable: false),
                    damage_dealt = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    defeated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raids", x => x.id);
                    table.ForeignKey(
                        name: "FK_raids_bosses_boss_id",
                        column: x => x.boss_id,
                        principalTable: "bosses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "equipment_slots",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    shop_category_key = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_equippable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment_slots", x => x.id);
                    table.ForeignKey(
                        name: "FK_equipment_slots_shop_categories_shop_category_key",
                        column: x => x.shop_category_key,
                        principalTable: "shop_categories",
                        principalColumn: "key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "skill_nodes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    skill_id = table.Column<string>(type: "character varying(40)", nullable: false),
                    parent_node_id = table.Column<int>(type: "integer", nullable: true),
                    required_level = table.Column<int>(type: "integer", nullable: false),
                    point_cost = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skill_nodes", x => x.id);
                    table.ForeignKey(
                        name: "FK_skill_nodes_skill_nodes_parent_node_id",
                        column: x => x.parent_node_id,
                        principalTable: "skill_nodes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_skill_nodes_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "achievements",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    icon = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    reward_gold = table.Column<int>(type: "integer", nullable: false),
                    metric = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    target = table.Column<long>(type: "bigint", nullable: false),
                    title_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievements", x => x.id);
                    table.ForeignKey(
                        name: "FK_achievements_titles_title_id",
                        column: x => x.title_id,
                        principalTable: "titles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    difficulty = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    archived_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deadline = table.Column<DateOnly>(type: "date", nullable: true),
                    category = table.Column<int>(type: "integer", nullable: true),
                    effort = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    overdue_penalized_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    repeat = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    last_checked_period = table.Column<DateOnly>(type: "date", nullable: true),
                    streak = table.Column<int>(type: "integer", nullable: false),
                    best_streak = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.id);
                    table.CheckConstraint("ck_tasks_daily_only", "(type = 'Daily') = (repeat IS NOT NULL)");
                    table.CheckConstraint("ck_tasks_effort_category", "(category IS NULL) = (effort IS NULL)");
                    table.CheckConstraint("ck_tasks_effort_positive", "effort IS NULL OR effort > 0");
                    table.CheckConstraint("ck_tasks_priority", "priority BETWEEN 1 AND 3");
                    table.CheckConstraint("ck_tasks_todo_only", "type = 'Todo' OR (deadline IS NULL AND category IS NULL AND effort IS NULL AND completed_at IS NULL)");
                    table.ForeignKey(
                        name: "FK_tasks_categories_category",
                        column: x => x.category,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tasks_difficulty_levels_difficulty",
                        column: x => x.difficulty,
                        principalTable: "difficulty_levels",
                        principalColumn: "level",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tasks_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "raid_participants",
                columns: table => new
                {
                    raid_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raid_participants", x => new { x.raid_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_raid_participants_raids_raid_id",
                        column: x => x.raid_id,
                        principalTable: "raids",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_raid_participants_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "raid_reward_claims",
                columns: table => new
                {
                    raid_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claimed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raid_reward_claims", x => new { x.raid_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_raid_reward_claims_raids_raid_id",
                        column: x => x.raid_id,
                        principalTable: "raids",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_raid_reward_claims_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_subtypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    slot_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_subtypes", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_subtypes_equipment_slots_slot_id",
                        column: x => x.slot_id,
                        principalTable: "equipment_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "activity_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    note = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    source_task_id = table.Column<Guid>(type: "uuid", nullable: true),
                    logged_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_activity_logs_categories_category",
                        column: x => x.category,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_activity_logs_tasks_source_task_id",
                        column: x => x.source_task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_activity_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "daily_completions",
                columns: table => new
                {
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_start = table.Column<DateOnly>(type: "date", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_completions", x => new { x.task_id, x.period_start });
                    table.ForeignKey(
                        name: "FK_daily_completions_tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "habit_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    positive = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habit_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_habit_logs_tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "raid_hits",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    raid_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    damage = table.Column<int>(type: "integer", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    dealt_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raid_hits", x => x.id);
                    table.ForeignKey(
                        name: "FK_raid_hits_raids_raid_id",
                        column: x => x.raid_id,
                        principalTable: "raids",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_raid_hits_tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_raid_hits_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    slot_id = table.Column<int>(type: "integer", nullable: false),
                    subtype_id = table.Column<int>(type: "integer", nullable: false),
                    rarity = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    price = table.Column<int>(type: "integer", nullable: false),
                    emoji = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    atk = table.Column<int>(type: "integer", nullable: false),
                    def = table.Column<int>(type: "integer", nullable: false),
                    @int = table.Column<int>(name: "int", type: "integer", nullable: false),
                    luk = table.Column<int>(type: "integer", nullable: false),
                    max_hp = table.Column<int>(type: "integer", nullable: false),
                    max_mana = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.id);
                    table.UniqueConstraint("AK_items_id_slot_id", x => new { x.id, x.slot_id });
                    table.ForeignKey(
                        name: "FK_items_equipment_slots_slot_id",
                        column: x => x.slot_id,
                        principalTable: "equipment_slots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_items_item_subtypes_subtype_id",
                        column: x => x.subtype_id,
                        principalTable: "item_subtypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "boss_rewards",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    boss_id = table.Column<int>(type: "integer", nullable: false),
                    kind = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    gold = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    pet_species_id = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    title_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boss_rewards", x => x.id);
                    table.CheckConstraint("ck_boss_reward_kind", "(kind = 'Gold' AND gold > 0) OR (kind = 'Item' AND item_id IS NOT NULL) OR (kind = 'Pet' AND pet_species_id IS NOT NULL) OR (kind = 'Title' AND title_id IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_boss_rewards_bosses_boss_id",
                        column: x => x.boss_id,
                        principalTable: "bosses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_boss_rewards_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_boss_rewards_pet_species_pet_species_id",
                        column: x => x.pet_species_id,
                        principalTable: "pet_species",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_boss_rewards_titles_title_id",
                        column: x => x.title_id,
                        principalTable: "titles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "egg_types",
                columns: table => new
                {
                    item_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    hatch_minutes = table.Column<int>(type: "integer", nullable: false),
                    base_rarity = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_egg_types", x => x.item_id);
                    table.ForeignKey(
                        name: "FK_egg_types_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_skills",
                columns: table => new
                {
                    item_id = table.Column<string>(type: "character varying(40)", nullable: false),
                    skill_id = table.Column<string>(type: "character varying(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_skills", x => new { x.item_id, x.skill_id });
                    table.ForeignKey(
                        name: "FK_item_skills_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_skills_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_achievements",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    achievement_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    claimed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_achievements", x => new { x.user_id, x.achievement_id });
                    table.ForeignKey(
                        name: "FK_character_achievements_achievements_achievement_id",
                        column: x => x.achievement_id,
                        principalTable: "achievements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_equipment",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    slot_id = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_equipment", x => new { x.user_id, x.slot_id });
                    table.ForeignKey(
                        name: "FK_character_equipment_items_item_id_slot_id",
                        columns: x => new { x.item_id, x.slot_id },
                        principalTable: "items",
                        principalColumns: new[] { "id", "slot_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_items",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_items", x => new { x.user_id, x.item_id });
                    table.CheckConstraint("ck_character_items_qty", "quantity > 0");
                    table.ForeignKey(
                        name: "FK_character_items_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_skills",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    unlocked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_skills", x => new { x.user_id, x.skill_id });
                    table.ForeignKey(
                        name: "FK_character_skills_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_titles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    acquired_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    source = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_titles", x => new { x.user_id, x.title_id });
                    table.ForeignKey(
                        name: "FK_character_titles_titles_title_id",
                        column: x => x.title_id,
                        principalTable: "titles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exp = table.Column<int>(type: "integer", nullable: false),
                    gold = table.Column<int>(type: "integer", nullable: false),
                    hp = table.Column<int>(type: "integer", nullable: false),
                    mana = table.Column<int>(type: "integer", nullable: false),
                    mana_refreshed_on = table.Column<DateOnly>(type: "date", nullable: false),
                    deaths = table.Column<int>(type: "integer", nullable: false),
                    tasks_completed = table.Column<int>(type: "integer", nullable: false),
                    coop_level = table.Column<int>(type: "integer", nullable: false),
                    incubator_level = table.Column<int>(type: "integer", nullable: false),
                    companion_pet_id = table.Column<Guid>(type: "uuid", nullable: true),
                    active_title_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    skill_points = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.user_id);
                    table.CheckConstraint("ck_characters_gold", "gold >= 0");
                    table.CheckConstraint("ck_characters_hp", "hp >= 0");
                    table.ForeignKey(
                        name: "FK_characters_titles_active_title_id",
                        column: x => x.active_title_id,
                        principalTable: "titles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_characters_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "incubating_eggs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    egg_item_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ready_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_incubating_eggs", x => x.id);
                    table.ForeignKey(
                        name: "FK_incubating_eggs_characters_user_id",
                        column: x => x.user_id,
                        principalTable: "characters",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_incubating_eggs_egg_types_egg_item_id",
                        column: x => x.egg_item_id,
                        principalTable: "egg_types",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ledger_entries",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    gold_delta = table.Column<int>(type: "integer", nullable: false),
                    exp_delta = table.Column<int>(type: "integer", nullable: false),
                    hp_delta = table.Column<int>(type: "integer", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: true),
                    item_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ledger_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_ledger_entries_characters_user_id",
                        column: x => x.user_id,
                        principalTable: "characters",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    discord_sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifications_characters_user_id",
                        column: x => x.user_id,
                        principalTable: "characters",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    species_id = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    xp = table.Column<int>(type: "integer", nullable: false),
                    adopted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pets", x => x.id);
                    table.ForeignKey(
                        name: "FK_pets_characters_user_id",
                        column: x => x.user_id,
                        principalTable: "characters",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pets_pet_species_species_id",
                        column: x => x.species_id,
                        principalTable: "pet_species",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "bosses",
                columns: new[] { "id", "emoji", "lore", "max_hp", "name", "sort_order", "title" },
                values: new object[,]
                {
                    { 1, "🐲", "Nó thì thầm: \"Mai làm cũng được mà...\" Hãy cùng nhau chứng minh nó sai.", 6000L, "Hắc Long Trì Hoãn", 1, "Chúa tể của \"để mai làm\"" },
                    { 2, "👹", "Mỗi miếng snack bạn nhịn được là một nhát chém vào bụng nó.", 5000L, "Quỷ Ăn Vặt Khổng Lồ", 2, "Kẻ hủy diệt bữa chính" },
                    { 3, "🧙", "Nó khiến năm phút thành năm tiếng. Đừng để nó nhìn thấy điện thoại của bạn!", 7000L, "Phù Thủy Mạng Xã Hội", 3, "Bậc thầy cuộn vô tận" },
                    { 4, "🗿", "Cứng đầu và nặng nề, nhưng không chịu nổi sự kiên trì của cả cộng đồng.", 5500L, "Golem Lười Biếng", 4, "Tảng đá không muốn dậy" },
                    { 5, "👻", "Nó sống trong màn hình lúc nửa đêm. Hãy đánh bại nó bằng một ngày đầy năng lượng.", 6500L, "Bóng Ma Thức Khuya", 5, "Thực thể của 3 giờ sáng" }
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "id", "board_name", "emoji", "is_hours", "max_per_entry", "name", "slug", "sort_order", "tagline", "unit" },
                values: new object[,]
                {
                    { 0, "Học Thần Điện", "📚", true, 12m, "Học tập", "study", 1, "Cày kiến thức, leo lên đỉnh tri thức", "giờ" },
                    { 1, "Đấu Trường Game Thủ", "🎮", true, 12m, "Chơi game", "gaming", 2, "Chiến hết mình, nhớ nghỉ ngơi nhé", "giờ" },
                    { 2, "Võ Đài Thể Lực", "🏃", true, 6m, "Thể thao", "sport", 3, "Mồ hôi rơi, sức mạnh lên", "giờ" },
                    { 3, "Thánh Nhịn Vặt", "🥗", false, 20m, "Không ăn vặt", "clean", 4, "Mỗi lần cưỡng lại cám dỗ là một chiến thắng", "lần" },
                    { 4, "Kẻ Phá Xiềng", "⛓️", false, 20m, "Bỏ thói quen xấu", "habit", 5, "Mỗi lần bẻ gãy xiềng xích là một chiến thắng", "lần" }
                });

            migrationBuilder.InsertData(
                table: "difficulty_levels",
                columns: new[] { "level", "base_damage", "base_exp", "base_gold", "base_hp_loss", "name" },
                values: new object[,]
                {
                    { 1, 5, 6, 8, 3, "Rất dễ" },
                    { 2, 9, 10, 14, 5, "Dễ" },
                    { 3, 14, 15, 20, 8, "Vừa" },
                    { 4, 20, 22, 30, 12, "Khó" },
                    { 5, 30, 32, 45, 18, "Cực khó" }
                });

            migrationBuilder.InsertData(
                table: "pet_species",
                columns: new[] { "id", "emoji", "name", "rarity" },
                values: new object[,]
                {
                    { "cat", "🐱", "Mèo Con", "Common" },
                    { "dog", "🐶", "Cún Con", "Common" },
                    { "dragon", "🐉", "Rồng Lửa", "Legendary" },
                    { "fox", "🦊", "Cáo Lửa", "Rare" },
                    { "hamster", "🐹", "Hamster Tròn", "Common" },
                    { "lion", "🦁", "Sư Tử Vàng", "Epic" },
                    { "owl", "🦉", "Cú Thông Thái", "Rare" },
                    { "panda", "🐼", "Gấu Trúc", "Rare" },
                    { "phoenix", "🦅", "Phượng Hoàng", "Legendary" },
                    { "rabbit", "🐰", "Thỏ Trắng", "Common" },
                    { "unicorn", "🦄", "Kỳ Lân", "Epic" },
                    { "wolf", "🐺", "Sói Bạc", "Epic" }
                });

            migrationBuilder.InsertData(
                table: "shop_categories",
                columns: new[] { "key", "emoji", "name", "sort_order" },
                values: new object[,]
                {
                    { "armor", "🛡️", "Giáp trụ", 2 },
                    { "jewelry", "💎", "Trang sức", 3 },
                    { "pet", "🥚", "Pet & Trứng", 4 },
                    { "weapon", "⚔️", "Vũ khí", 1 }
                });

            migrationBuilder.InsertData(
                table: "titles",
                columns: new[] { "id", "icon", "name" },
                values: new object[,]
                {
                    { "boss1", "🐲", "Khắc Tinh Hắc Long" },
                    { "boss2", "👹", "Sát Thủ Quỷ Vặt" },
                    { "boss3", "🧙", "Phá Giải Bùa Mạng" },
                    { "boss4", "🗿", "Đập Vỡ Golem" },
                    { "boss5", "👻", "Xua Tan Bóng Ma" },
                    { "clean20", "🥗", "Ý Chí Thép" },
                    { "coop3", "🏠", "Kiến Trúc Sư Chuồng Trại" },
                    { "first_log", "🌱", "Bước Chân Đầu Tiên" },
                    { "first_pet", "🐣", "Người Nuôi Thú" },
                    { "full_gear", "🧙", "Trang Bị Đầy Đủ" },
                    { "gaming20", "🎮", "Game Thủ Chính Hiệu" },
                    { "habit20", "⛓️", "Kẻ Phá Xiềng" },
                    { "legend_pet", "🐉", "Thuần Hóa Huyền Thoại" },
                    { "level10", "👑", "Đại Sư" },
                    { "level5", "🎖️", "Hiệp Sĩ" },
                    { "pets5", "🐾", "Chủ Trại Thú" },
                    { "shopper", "🛍️", "Khách Quen" },
                    { "sport10", "🏅", "Lực Sĩ" },
                    { "sport50", "🏆", "Vô Địch Thể Lực" },
                    { "streak3", "🔥", "Lửa Khởi Đầu" },
                    { "streak30", "☄️", "Huyền Thoại Kiên Trì" },
                    { "streak7", "🌋", "Bất Diệt" },
                    { "study10", "📚", "Mọt Sách" },
                    { "study50", "🎓", "Học Thần" },
                    { "tasks10", "✅", "Chăm Chỉ" },
                    { "tasks50", "🏗️", "Cỗ Máy Năng Suất" }
                });

            migrationBuilder.InsertData(
                table: "achievements",
                columns: new[] { "id", "description", "icon", "metric", "name", "reward_gold", "target", "title_id" },
                values: new object[,]
                {
                    { "clean20", "Nhịn ăn vặt 20 lần", "🥗", "clean_times", "Ý Chí Thép", 200, 20L, "clean20" },
                    { "coop3", "Nâng chuồng pet lên cấp 3", "🏠", "coop_level", "Kiến Trúc Sư Chuồng Trại", 300, 3L, "coop3" },
                    { "first_log", "Hoàn thành nhiệm vụ đầu tiên thuộc một lĩnh vực", "🌱", "log_count", "Bước Chân Đầu Tiên", 50, 1L, "first_log" },
                    { "first_pet", "Sở hữu pet đầu tiên", "🐣", "pets_owned", "Người Nuôi Thú", 100, 1L, "first_pet" },
                    { "full_gear", "Mặc đủ 6 ô trang bị", "🧙", "equipped_count", "Trang Bị Đầy Đủ", 500, 6L, "full_gear" },
                    { "gaming20", "Chơi game đủ 20 giờ", "🎮", "gaming_hours", "Game Thủ Chính Hiệu", 100, 20L, "gaming20" },
                    { "habit20", "Cưỡng lại thói xấu 20 lần", "⛓️", "habit_times", "Kẻ Phá Xiềng", 200, 20L, "habit20" },
                    { "legend_pet", "Sở hữu một pet Huyền thoại", "🐉", "legendary_pets", "Thuần Hóa Huyền Thoại", 1500, 1L, "legend_pet" },
                    { "level10", "Đạt cấp 10", "👑", "level", "Đại Sư", 1000, 10L, "level10" },
                    { "level5", "Đạt cấp 5", "🎖️", "level", "Hiệp Sĩ", 250, 5L, "level5" },
                    { "pets5", "Sở hữu 5 pet", "🐾", "pets_owned", "Chủ Trại Thú", 500, 5L, "pets5" },
                    { "shopper", "Sở hữu 5 món đồ khác nhau", "🛍️", "distinct_items", "Khách Quen", 200, 5L, "shopper" },
                    { "sport10", "Tập thể thao đủ 10 giờ", "🏅", "sport_hours", "Lực Sĩ", 150, 10L, "sport10" },
                    { "sport50", "Tập thể thao đủ 50 giờ", "🏆", "sport_hours", "Vô Địch Thể Lực", 800, 50L, "sport50" },
                    { "streak3", "Chuỗi 3 ngày liên tiếp", "🔥", "streak", "Lửa Khởi Đầu", 100, 3L, "streak3" },
                    { "streak30", "Chuỗi 30 ngày liên tiếp", "☄️", "streak", "Huyền Thoại Kiên Trì", 2000, 30L, "streak30" },
                    { "streak7", "Chuỗi 7 ngày liên tiếp", "🌋", "streak", "Bất Diệt", 300, 7L, "streak7" },
                    { "study10", "Học đủ 10 giờ", "📚", "study_hours", "Mọt Sách", 150, 10L, "study10" },
                    { "study50", "Học đủ 50 giờ", "🎓", "study_hours", "Học Thần", 800, 50L, "study50" },
                    { "tasks10", "Hoàn thành 10 nhiệm vụ", "✅", "tasks_done", "Chăm Chỉ", 150, 10L, "tasks10" },
                    { "tasks50", "Hoàn thành 50 nhiệm vụ", "🏗️", "tasks_done", "Cỗ Máy Năng Suất", 700, 50L, "tasks50" }
                });

            migrationBuilder.InsertData(
                table: "boss_rewards",
                columns: new[] { "id", "boss_id", "gold", "item_id", "kind", "pet_species_id", "quantity", "title_id" },
                values: new object[,]
                {
                    { 1, 1, 300, null, "Gold", null, 1, null },
                    { 3, 1, 0, null, "Pet", "fox", 1, null },
                    { 4, 1, 0, null, "Title", null, 1, "boss1" },
                    { 5, 2, 400, null, "Gold", null, 1, null },
                    { 7, 2, 0, null, "Pet", "owl", 1, null },
                    { 8, 2, 0, null, "Title", null, 1, "boss2" },
                    { 9, 3, 500, null, "Gold", null, 1, null },
                    { 11, 3, 0, null, "Pet", "fox", 1, null },
                    { 12, 3, 0, null, "Title", null, 1, "boss3" },
                    { 13, 4, 600, null, "Gold", null, 1, null },
                    { 15, 4, 0, null, "Pet", "owl", 1, null },
                    { 16, 4, 0, null, "Title", null, 1, "boss4" },
                    { 17, 5, 700, null, "Gold", null, 1, null },
                    { 19, 5, 0, null, "Pet", "fox", 1, null },
                    { 20, 5, 0, null, "Title", null, 1, "boss5" }
                });

            migrationBuilder.InsertData(
                table: "equipment_slots",
                columns: new[] { "id", "is_equippable", "name", "shop_category_key" },
                values: new object[,]
                {
                    { 0, true, "Vũ khí", "weapon" },
                    { 1, true, "Mũ", "armor" },
                    { 2, true, "Giáp", "armor" },
                    { 3, true, "Giày", "armor" },
                    { 4, true, "Nhẫn", "jewelry" },
                    { 5, true, "Dây chuyền", "jewelry" },
                    { 6, false, "Trứng", "pet" }
                });

            migrationBuilder.InsertData(
                table: "item_subtypes",
                columns: new[] { "id", "name", "slot_id" },
                values: new object[,]
                {
                    { 1, "Kiếm", 0 },
                    { 2, "Cung", 0 },
                    { 3, "Gậy phép", 0 },
                    { 4, "Rìu & Búa", 0 },
                    { 5, "Mũ", 1 },
                    { 6, "Giáp", 2 },
                    { 7, "Áo choàng", 2 },
                    { 8, "Giày", 3 },
                    { 9, "Nhẫn", 4 },
                    { 10, "Dây chuyền", 5 },
                    { 11, "Trứng", 6 }
                });

            migrationBuilder.InsertData(
                table: "items",
                columns: new[] { "id", "atk", "def", "description", "emoji", "int", "is_active", "luk", "max_hp", "max_mana", "name", "price", "rarity", "slot_id", "subtype_id" },
                values: new object[,]
                {
                    { "amulet_bone", 0, 0, "Chiến lợi phẩm đầu tay.", "📿", 0, true, 0, 6, 3, "Vòng Cổ Xương", 70, "Common", 5, 10 },
                    { "amulet_jade", 0, 0, "Mang lại bình an.", "🟢", 0, true, 3, 24, 12, "Ngọc Bội", 360, "Rare", 5, 10 },
                    { "amulet_phoenix", 0, 0, "Tái sinh từ tro tàn.", "🔥", 0, true, 7, 62, 31, "Mặt Dây Phượng Hoàng", 1250, "Epic", 5, 10 },
                    { "amulet_star", 0, 0, "Chứa một mảnh bầu trời.", "💫", 0, true, 19, 152, 76, "Thiên Tinh Châu", 5200, "Legendary", 5, 10 },
                    { "armor_chain", 0, 16, "Từng mắt xích một.", "🥋", 0, true, 0, 32, 0, "Giáp Xích", 420, "Rare", 2, 6 },
                    { "armor_holy", 0, 88, "Bóng tối không thể lại gần.", "🌞", 0, true, 0, 176, 0, "Thánh Giáp Ánh Sáng", 6200, "Legendary", 2, 6 },
                    { "armor_leather", 0, 5, "Nhẹ nhàng, linh hoạt.", "🦺", 0, true, 0, 10, 0, "Áo Da", 100, "Common", 2, 6 },
                    { "armor_plate", 0, 36, "Tường thành di động.", "🛡️", 0, true, 0, 72, 0, "Giáp Bản", 1400, "Epic", 2, 6 },
                    { "axe_hand", 5, 0, "Chặt củi hay chặt quái đều tốt.", "🪓", 0, true, 0, 0, 0, "Rìu Tay", 100, "Common", 0, 4 },
                    { "axe_titan", 44, 0, "Cán rìu cao bằng một người.", "🪓", 0, true, 0, 0, 0, "Rìu Titan", 1700, "Epic", 0, 4 },
                    { "boots_cloth", 0, 1, "Đi bộ thoải mái.", "👟", 0, true, 1, 0, 0, "Giày Vải", 40, "Common", 3, 8 },
                    { "boots_flash", 0, 16, "Nhanh hơn cả tiếng sấm.", "⚡", 0, true, 16, 0, 0, "Giày Tia Chớp", 1300, "Epic", 3, 8 },
                    { "boots_leather", 0, 2, "Vượt đường gập ghềnh.", "🥾", 0, true, 2, 0, 0, "Ủng Da", 110, "Common", 3, 8 },
                    { "boots_wind", 0, 6, "Chân nhẹ như gió.", "💨", 0, true, 6, 0, 0, "Giày Gió", 360, "Rare", 3, 8 },
                    { "boots_wing", 0, 39, "Bước đi trên mây.", "🪽", 0, true, 39, 0, 0, "Ủng Cánh Thiên Thần", 5200, "Legendary", 3, 8 },
                    { "bow_elven", 26, 0, "Dây cung làm từ tóc tiên.", "🏹", 0, true, 11, 0, 0, "Cung Tinh Linh", 1400, "Epic", 0, 2 },
                    { "bow_long", 11, 0, "Tầm bắn xa.", "🏹", 0, true, 4, 0, 0, "Trường Cung", 400, "Rare", 0, 2 },
                    { "bow_short", 2, 0, "Nhẹ và nhanh.", "🏹", 0, true, 1, 0, 0, "Cung Ngắn", 80, "Common", 0, 2 },
                    { "bow_star", 59, 0, "Mỗi mũi tên là một sao băng.", "🌠", 0, true, 25, 0, 0, "Cung Sao Băng", 5500, "Legendary", 0, 2 },
                    { "cloak_shadow", 0, 34, "Hòa vào bóng tối.", "🦇", 0, true, 0, 68, 0, "Áo Choàng Bóng Đêm", 1300, "Epic", 2, 7 },
                    { "cloak_travel", 0, 5, "Bạn đồng hành mọi chuyến đi.", "🧥", 0, true, 0, 10, 0, "Áo Choàng Lữ Hành", 110, "Common", 2, 7 },
                    { "crown_dragon", 0, 80, "Rồng cúi đầu trước chủ nhân.", "🐲", 0, true, 0, 0, 0, "Mão Rồng Thần", 5800, "Legendary", 1, 5 },
                    { "crown_gold", 0, 35, "Dành cho người dẫn đầu.", "👑", 0, true, 0, 0, 0, "Vương Miện Vàng", 1500, "Epic", 1, 5 },
                    { "egg_common", 0, 0, "Ấp 15 phút. Nở ra pet Thường (có cơ hội ra Hiếm).", "🥚", 0, true, 0, 0, 0, "Trứng Thường", 100, "Common", 6, 11 },
                    { "egg_epic", 0, 0, "Ấp 4 giờ. Nở ra pet Sử thi (có cơ hội ra Huyền thoại).", "🔮", 0, true, 0, 0, 0, "Trứng Sử Thi", 1500, "Epic", 6, 11 },
                    { "egg_legend", 0, 0, "Ấp 12 giờ. Chắc chắn nở ra pet Huyền thoại.", "🌟", 0, true, 0, 0, 0, "Trứng Huyền Thoại", 5000, "Legendary", 6, 11 },
                    { "egg_rare", 0, 0, "Ấp 1 giờ. Nở ra pet Hiếm (có cơ hội ra Sử thi).", "🪺", 0, true, 0, 0, 0, "Trứng Hiếm", 450, "Rare", 6, 11 },
                    { "hammer_thunder", 100, 0, "Gọi sấm xuống trần gian.", "⚡", 0, true, 0, 0, 0, "Thần Chùy Sấm Sét", 7000, "Legendary", 0, 4 },
                    { "hammer_war", 20, 0, "Nặng, đau, hiệu quả.", "🔨", 0, true, 0, 0, 0, "Búa Chiến", 480, "Rare", 0, 4 },
                    { "helm_cloth", 0, 2, "Che nắng là chính.", "🧢", 0, true, 0, 0, 0, "Mũ Vải", 40, "Common", 1, 5 },
                    { "helm_iron", 0, 6, "Chắc chắn, hơi nóng.", "⛑️", 0, true, 0, 0, 0, "Mũ Sắt", 120, "Common", 1, 5 },
                    { "helm_knight", 0, 14, "Biểu tượng của danh dự.", "🪖", 0, true, 0, 0, 0, "Mũ Hiệp Sĩ", 380, "Rare", 1, 5 },
                    { "ring_copper", 1, 0, "Giản dị mà bền.", "💍", 1, true, 0, 0, 0, "Nhẫn Đồng", 70, "Common", 4, 9 },
                    { "ring_infinity", 37, 0, "Sức mạnh không giới hạn.", "♾️", 37, true, 0, 0, 0, "Nhẫn Vô Cực", 5000, "Legendary", 4, 9 },
                    { "ring_ruby", 15, 0, "Đỏ như lửa cháy.", "❤️", 15, true, 0, 0, 0, "Nhẫn Hồng Ngọc", 1200, "Epic", 4, 9 },
                    { "ring_silver", 6, 0, "Sáng lấp lánh.", "💍", 6, true, 0, 0, 0, "Nhẫn Bạc", 350, "Rare", 4, 9 },
                    { "staff_apprentice", 0, 0, "Phép thuật nhập môn.", "🪄", 4, true, 0, 0, 8, "Gậy Học Việc", 90, "Common", 0, 3 },
                    { "staff_arcane", 0, 0, "Thì thầm những bí mật cổ xưa.", "🔮", 42, true, 0, 0, 84, "Quyền Trượng Thần Bí", 1600, "Epic", 0, 3 },
                    { "staff_crystal", 0, 0, "Khuếch đại ma lực.", "🔮", 17, true, 0, 0, 34, "Gậy Pha Lê", 420, "Rare", 0, 3 },
                    { "staff_sage", 0, 0, "Trí tuệ của một thời đại.", "🌟", 95, true, 0, 0, 190, "Trượng Đại Hiền Giả", 6500, "Legendary", 0, 3 },
                    { "sword_excal", 90, 0, "Chỉ người kiên định mới rút được.", "✨", 0, true, 0, 0, 0, "Thánh Kiếm Excalibur", 6000, "Legendary", 0, 1 },
                    { "sword_flame", 40, 0, "Rèn từ vảy rồng lửa.", "🔥", 0, true, 0, 0, 0, "Hỏa Long Kiếm", 1500, "Epic", 0, 1 },
                    { "sword_iron", 8, 0, "Bền, rẻ, dễ kiếm.", "🗡️", 0, true, 0, 0, 0, "Kiếm Sắt", 150, "Common", 0, 1 },
                    { "sword_steel", 18, 0, "Lưỡi thép sắc lẹm.", "⚔️", 0, true, 0, 0, 0, "Trảm Kiếm Thép", 450, "Rare", 0, 1 },
                    { "sword_wood", 3, 0, "Đồ tập của tân thủ.", "🗡️", 0, true, 0, 0, 0, "Kiếm Gỗ", 60, "Common", 0, 1 }
                });

            migrationBuilder.InsertData(
                table: "boss_rewards",
                columns: new[] { "id", "boss_id", "gold", "item_id", "kind", "pet_species_id", "quantity", "title_id" },
                values: new object[,]
                {
                    { 2, 1, 0, "egg_rare", "Item", null, 1, null },
                    { 6, 2, 0, "egg_rare", "Item", null, 1, null },
                    { 10, 3, 0, "egg_rare", "Item", null, 1, null },
                    { 14, 4, 0, "egg_rare", "Item", null, 1, null },
                    { 18, 5, 0, "egg_rare", "Item", null, 1, null }
                });

            migrationBuilder.InsertData(
                table: "egg_types",
                columns: new[] { "item_id", "base_rarity", "hatch_minutes" },
                values: new object[,]
                {
                    { "egg_common", "Common", 15 },
                    { "egg_epic", "Epic", 240 },
                    { "egg_legend", "Legendary", 720 },
                    { "egg_rare", "Rare", 60 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_achievements_title_id",
                table: "achievements",
                column: "title_id");

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_category_logged_at",
                table: "activity_logs",
                columns: new[] { "category", "logged_at" });

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_source_task_id",
                table: "activity_logs",
                column: "source_task_id");

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_user_id_logged_at",
                table: "activity_logs",
                columns: new[] { "user_id", "logged_at" });

            migrationBuilder.CreateIndex(
                name: "IX_boss_rewards_boss_id",
                table: "boss_rewards",
                column: "boss_id");

            migrationBuilder.CreateIndex(
                name: "IX_boss_rewards_item_id",
                table: "boss_rewards",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_boss_rewards_pet_species_id",
                table: "boss_rewards",
                column: "pet_species_id");

            migrationBuilder.CreateIndex(
                name: "IX_boss_rewards_title_id",
                table: "boss_rewards",
                column: "title_id");

            migrationBuilder.CreateIndex(
                name: "IX_categories_slug",
                table: "categories",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_character_achievements_achievement_id",
                table: "character_achievements",
                column: "achievement_id");

            migrationBuilder.CreateIndex(
                name: "IX_character_equipment_item_id_slot_id",
                table: "character_equipment",
                columns: new[] { "item_id", "slot_id" });

            migrationBuilder.CreateIndex(
                name: "IX_character_equipment_user_id_item_id",
                table: "character_equipment",
                columns: new[] { "user_id", "item_id" });

            migrationBuilder.CreateIndex(
                name: "IX_character_items_item_id",
                table: "character_items",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_character_skills_skill_id",
                table: "character_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_character_titles_title_id",
                table: "character_titles",
                column: "title_id");

            migrationBuilder.CreateIndex(
                name: "IX_characters_active_title_id",
                table: "characters",
                column: "active_title_id");

            migrationBuilder.CreateIndex(
                name: "IX_characters_companion_pet_id",
                table: "characters",
                column: "companion_pet_id");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_slots_shop_category_key",
                table: "equipment_slots",
                column: "shop_category_key");

            migrationBuilder.CreateIndex(
                name: "IX_habit_logs_task_id",
                table: "habit_logs",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_incubating_eggs_egg_item_id",
                table: "incubating_eggs",
                column: "egg_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_incubating_eggs_user_id",
                table: "incubating_eggs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_skills_skill_id",
                table: "item_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_subtypes_slot_id_name",
                table: "item_subtypes",
                columns: new[] { "slot_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_items_slot_id",
                table: "items",
                column: "slot_id");

            migrationBuilder.CreateIndex(
                name: "IX_items_subtype_id",
                table: "items",
                column: "subtype_id");

            migrationBuilder.CreateIndex(
                name: "IX_ledger_entries_user_id_created_at",
                table: "ledger_entries",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user_id_read_at",
                table: "notifications",
                columns: new[] { "user_id", "read_at" });

            migrationBuilder.CreateIndex(
                name: "IX_pets_species_id",
                table: "pets",
                column: "species_id");

            migrationBuilder.CreateIndex(
                name: "IX_pets_user_id",
                table: "pets",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_raid_hits_raid_id_user_id",
                table: "raid_hits",
                columns: new[] { "raid_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "IX_raid_hits_task_id",
                table: "raid_hits",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_raid_hits_user_id",
                table: "raid_hits",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_raid_participants_user_id",
                table: "raid_participants",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_raid_reward_claims_user_id",
                table: "raid_reward_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_raids_boss_id",
                table: "raids",
                column: "boss_id");

            migrationBuilder.CreateIndex(
                name: "IX_raids_starts_at",
                table: "raids",
                column: "starts_at",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_skill_nodes_parent_node_id",
                table: "skill_nodes",
                column: "parent_node_id");

            migrationBuilder.CreateIndex(
                name: "IX_skill_nodes_skill_id",
                table: "skill_nodes",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_category",
                table: "tasks",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_difficulty",
                table: "tasks",
                column: "difficulty");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_user_id_type",
                table: "tasks",
                columns: new[] { "user_id", "type" });

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_character_achievements_characters_user_id",
                table: "character_achievements",
                column: "user_id",
                principalTable: "characters",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_equipment_character_items_user_id_item_id",
                table: "character_equipment",
                columns: new[] { "user_id", "item_id" },
                principalTable: "character_items",
                principalColumns: new[] { "user_id", "item_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_character_equipment_characters_user_id",
                table: "character_equipment",
                column: "user_id",
                principalTable: "characters",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_items_characters_user_id",
                table: "character_items",
                column: "user_id",
                principalTable: "characters",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_skills_characters_user_id",
                table: "character_skills",
                column: "user_id",
                principalTable: "characters",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_titles_characters_user_id",
                table: "character_titles",
                column: "user_id",
                principalTable: "characters",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_characters_pets_companion_pet_id",
                table: "characters",
                column: "companion_pet_id",
                principalTable: "pets",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_characters_titles_active_title_id",
                table: "characters");

            migrationBuilder.DropForeignKey(
                name: "FK_characters_users_user_id",
                table: "characters");

            migrationBuilder.DropForeignKey(
                name: "FK_pets_pet_species_species_id",
                table: "pets");

            migrationBuilder.DropForeignKey(
                name: "FK_pets_characters_user_id",
                table: "pets");

            migrationBuilder.DropTable(
                name: "activity_logs");

            migrationBuilder.DropTable(
                name: "boss_rewards");

            migrationBuilder.DropTable(
                name: "character_achievements");

            migrationBuilder.DropTable(
                name: "character_equipment");

            migrationBuilder.DropTable(
                name: "character_skills");

            migrationBuilder.DropTable(
                name: "character_titles");

            migrationBuilder.DropTable(
                name: "daily_completions");

            migrationBuilder.DropTable(
                name: "habit_logs");

            migrationBuilder.DropTable(
                name: "incubating_eggs");

            migrationBuilder.DropTable(
                name: "item_skills");

            migrationBuilder.DropTable(
                name: "ledger_entries");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "raid_hits");

            migrationBuilder.DropTable(
                name: "raid_participants");

            migrationBuilder.DropTable(
                name: "raid_reward_claims");

            migrationBuilder.DropTable(
                name: "skill_nodes");

            migrationBuilder.DropTable(
                name: "achievements");

            migrationBuilder.DropTable(
                name: "character_items");

            migrationBuilder.DropTable(
                name: "egg_types");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "raids");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "difficulty_levels");

            migrationBuilder.DropTable(
                name: "bosses");

            migrationBuilder.DropTable(
                name: "item_subtypes");

            migrationBuilder.DropTable(
                name: "equipment_slots");

            migrationBuilder.DropTable(
                name: "shop_categories");

            migrationBuilder.DropTable(
                name: "titles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "pet_species");

            migrationBuilder.DropTable(
                name: "characters");

            migrationBuilder.DropTable(
                name: "pets");
        }
    }
}
