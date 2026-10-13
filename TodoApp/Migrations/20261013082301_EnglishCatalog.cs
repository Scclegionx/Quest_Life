using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Migrations
{
    /// <inheritdoc />
    public partial class EnglishCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "clean20",
                columns: new[] { "description", "name" },
                values: new object[] { "Skip junk food 20 times", "Iron Will" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "coop3",
                columns: new[] { "description", "name" },
                values: new object[] { "Upgrade the pet coop to level 3", "Coop Architect" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "first_log",
                columns: new[] { "description", "name" },
                values: new object[] { "Complete your first task in a category", "First Steps" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "first_pet",
                columns: new[] { "description", "name" },
                values: new object[] { "Own your first pet", "Pet Keeper" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "full_gear",
                columns: new[] { "description", "name" },
                values: new object[] { "Wear all 6 equipment slots", "Fully Equipped" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "gaming20",
                columns: new[] { "description", "name" },
                values: new object[] { "Play games for 20 hours", "Dedicated Gamer" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "habit20",
                columns: new[] { "description", "name" },
                values: new object[] { "Resist a bad habit 20 times", "Chain Breaker" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "legend_pet",
                columns: new[] { "description", "name" },
                values: new object[] { "Own a Legendary pet", "Legendary Tamer" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "level10",
                columns: new[] { "description", "name" },
                values: new object[] { "Reach level 10", "Grandmaster" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "level5",
                columns: new[] { "description", "name" },
                values: new object[] { "Reach level 5", "Knight" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "pets5",
                columns: new[] { "description", "name" },
                values: new object[] { "Own 5 pets", "Pet Rancher" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "shopper",
                columns: new[] { "description", "name" },
                values: new object[] { "Own 5 different items", "Regular Customer" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "sport10",
                columns: new[] { "description", "name" },
                values: new object[] { "Exercise for 10 hours", "Athlete" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "sport50",
                columns: new[] { "description", "name" },
                values: new object[] { "Exercise for 50 hours", "Fitness Champion" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "streak3",
                columns: new[] { "description", "name" },
                values: new object[] { "Reach a 3-day streak", "Spark" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "streak30",
                columns: new[] { "description", "name" },
                values: new object[] { "Reach a 30-day streak", "Legend of Persistence" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "streak7",
                columns: new[] { "description", "name" },
                values: new object[] { "Reach a 7-day streak", "Unstoppable" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "study10",
                columns: new[] { "description", "name" },
                values: new object[] { "Study for 10 hours", "Bookworm" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "study50",
                columns: new[] { "description", "name" },
                values: new object[] { "Study for 50 hours", "Scholar Supreme" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "tasks10",
                columns: new[] { "description", "name" },
                values: new object[] { "Complete 10 tasks", "Hard Worker" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "tasks50",
                columns: new[] { "description", "name" },
                values: new object[] { "Complete 50 tasks", "Productivity Machine" });

            migrationBuilder.UpdateData(
                table: "bosses",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "lore", "name", "title" },
                values: new object[] { "It whispers: \"Tomorrow is fine...\" Let's prove it wrong together.", "Procrastination Dragon", "Lord of \"I'll do it tomorrow\"" });

            migrationBuilder.UpdateData(
                table: "bosses",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "lore", "name", "title" },
                values: new object[] { "Every snack you resist is a slash at its belly.", "Giant Snack Demon", "Destroyer of proper meals" });

            migrationBuilder.UpdateData(
                table: "bosses",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "lore", "name", "title" },
                values: new object[] { "It turns five minutes into five hours. Don't let it see your phone!", "Social Media Sorcerer", "Master of the endless scroll" });

            migrationBuilder.UpdateData(
                table: "bosses",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "lore", "name", "title" },
                values: new object[] { "Stubborn and heavy, but no match for the persistence of the whole community.", "Lazy Golem", "The rock that refuses to get up" });

            migrationBuilder.UpdateData(
                table: "bosses",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "lore", "name", "title" },
                values: new object[] { "It lives in your screen at midnight. Defeat it with a day full of energy.", "Midnight Phantom", "The entity of 3 a.m." });

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 0,
                columns: new[] { "board_name", "name", "tagline", "unit" },
                values: new object[] { "Scholars' Hall", "Study", "Grind knowledge, climb the summit of wisdom", "hours" });

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "board_name", "name", "tagline", "unit" },
                values: new object[] { "Gamer Arena", "Gaming", "Fight hard, remember to rest", "hours" });

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "board_name", "name", "tagline", "unit" },
                values: new object[] { "Fitness Colosseum", "Sports", "Sweat drops, strength rises", "hours" });

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "board_name", "name", "tagline", "unit" },
                values: new object[] { "Snack Resistance", "No Junk Food", "Every temptation resisted is a victory", "times" });

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "board_name", "name", "tagline", "unit" },
                values: new object[] { "Chain Breakers", "Quit Bad Habits", "Every chain you break is a victory", "times" });

            migrationBuilder.UpdateData(
                table: "difficulty_levels",
                keyColumn: "level",
                keyValue: 1,
                column: "name",
                value: "Trivial");

            migrationBuilder.UpdateData(
                table: "difficulty_levels",
                keyColumn: "level",
                keyValue: 2,
                column: "name",
                value: "Easy");

            migrationBuilder.UpdateData(
                table: "difficulty_levels",
                keyColumn: "level",
                keyValue: 3,
                column: "name",
                value: "Medium");

            migrationBuilder.UpdateData(
                table: "difficulty_levels",
                keyColumn: "level",
                keyValue: 4,
                column: "name",
                value: "Hard");

            migrationBuilder.UpdateData(
                table: "difficulty_levels",
                keyColumn: "level",
                keyValue: 5,
                column: "name",
                value: "Extreme");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 0,
                column: "name",
                value: "Weapon");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 1,
                column: "name",
                value: "Helmet");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 2,
                column: "name",
                value: "Armor");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 3,
                column: "name",
                value: "Boots");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 4,
                column: "name",
                value: "Ring");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 5,
                column: "name",
                value: "Necklace");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 6,
                column: "name",
                value: "Egg");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 1,
                column: "name",
                value: "Sword");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 2,
                column: "name",
                value: "Bow");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 3,
                column: "name",
                value: "Staff");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 4,
                column: "name",
                value: "Axe & Hammer");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 5,
                column: "name",
                value: "Helmet");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 6,
                column: "name",
                value: "Armor");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 7,
                column: "name",
                value: "Cloak");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 8,
                column: "name",
                value: "Boots");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 9,
                column: "name",
                value: "Ring");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 10,
                column: "name",
                value: "Necklace");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 11,
                column: "name",
                value: "Egg");

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "amulet_bone",
                columns: new[] { "description", "name" },
                values: new object[] { "Your first trophy.", "Bone Necklace" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "amulet_jade",
                columns: new[] { "description", "name" },
                values: new object[] { "Brings peace.", "Jade Pendant" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "amulet_phoenix",
                columns: new[] { "description", "name" },
                values: new object[] { "Reborn from ashes.", "Phoenix Pendant" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "amulet_star",
                columns: new[] { "description", "name" },
                values: new object[] { "Holds a piece of the sky.", "Star Orb" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "armor_chain",
                columns: new[] { "description", "name" },
                values: new object[] { "Link by link.", "Chainmail" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "armor_holy",
                columns: new[] { "description", "name" },
                values: new object[] { "Darkness cannot come near.", "Holy Light Armor" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "armor_leather",
                columns: new[] { "description", "name" },
                values: new object[] { "Light and flexible.", "Leather Vest" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "armor_plate",
                columns: new[] { "description", "name" },
                values: new object[] { "A walking fortress.", "Plate Armor" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "axe_hand",
                columns: new[] { "description", "name" },
                values: new object[] { "Great for chopping wood or monsters.", "Hand Axe" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "axe_titan",
                columns: new[] { "description", "name" },
                values: new object[] { "The handle is as tall as a person.", "Titan Axe" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "boots_cloth",
                columns: new[] { "description", "name" },
                values: new object[] { "Comfortable for walking.", "Cloth Shoes" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "boots_flash",
                columns: new[] { "description", "name" },
                values: new object[] { "Faster than thunder.", "Lightning Boots" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "boots_leather",
                columns: new[] { "description", "name" },
                values: new object[] { "Tackles rough roads.", "Leather Boots" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "boots_wind",
                columns: new[] { "description", "name" },
                values: new object[] { "Feet as light as the wind.", "Wind Boots" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "boots_wing",
                columns: new[] { "description", "name" },
                values: new object[] { "Walk on clouds.", "Angel Wing Boots" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "bow_elven",
                columns: new[] { "description", "name" },
                values: new object[] { "Strung with a fairy's hair.", "Elven Bow" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "bow_long",
                columns: new[] { "description", "name" },
                values: new object[] { "Long range.", "Longbow" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "bow_short",
                columns: new[] { "description", "name" },
                values: new object[] { "Light and fast.", "Short Bow" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "bow_star",
                columns: new[] { "description", "name" },
                values: new object[] { "Every arrow is a shooting star.", "Shooting Star Bow" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "cloak_shadow",
                columns: new[] { "description", "name" },
                values: new object[] { "Blends into the darkness.", "Shadow Cloak" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "cloak_travel",
                columns: new[] { "description", "name" },
                values: new object[] { "A companion on every journey.", "Traveler's Cloak" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "crown_dragon",
                columns: new[] { "description", "name" },
                values: new object[] { "Dragons bow to its owner.", "Dragon Crown" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "crown_gold",
                columns: new[] { "description", "name" },
                values: new object[] { "For those who lead.", "Golden Crown" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "egg_common",
                columns: new[] { "description", "name" },
                values: new object[] { "Incubates in 15 minutes. Hatches a Common pet (small chance of Rare).", "Common Egg" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "egg_epic",
                columns: new[] { "description", "name" },
                values: new object[] { "Incubates in 4 hours. Hatches an Epic pet (chance of Legendary).", "Epic Egg" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "egg_legend",
                columns: new[] { "description", "name" },
                values: new object[] { "Incubates in 12 hours. Always hatches a Legendary pet.", "Legendary Egg" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "egg_rare",
                columns: new[] { "description", "name" },
                values: new object[] { "Incubates in 1 hour. Hatches a Rare pet (chance of Epic).", "Rare Egg" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "hammer_thunder",
                columns: new[] { "description", "name" },
                values: new object[] { "Calls down lightning.", "Thunder Hammer" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "hammer_war",
                columns: new[] { "description", "name" },
                values: new object[] { "Heavy, painful, effective.", "War Hammer" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "helm_cloth",
                columns: new[] { "description", "name" },
                values: new object[] { "Mostly keeps the sun off.", "Cloth Cap" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "helm_iron",
                columns: new[] { "description", "name" },
                values: new object[] { "Sturdy, a bit hot.", "Iron Helmet" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "helm_knight",
                columns: new[] { "description", "name" },
                values: new object[] { "A symbol of honor.", "Knight's Helm" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "ring_copper",
                columns: new[] { "description", "name" },
                values: new object[] { "Plain but durable.", "Copper Ring" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "ring_infinity",
                columns: new[] { "description", "name" },
                values: new object[] { "Unlimited power.", "Infinity Ring" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "ring_ruby",
                columns: new[] { "description", "name" },
                values: new object[] { "Red as fire.", "Ruby Ring" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "ring_silver",
                columns: new[] { "description", "name" },
                values: new object[] { "Sparkling bright.", "Silver Ring" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "staff_apprentice",
                columns: new[] { "description", "name" },
                values: new object[] { "Entry-level magic.", "Apprentice Staff" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "staff_arcane",
                columns: new[] { "description", "name" },
                values: new object[] { "Whispers ancient secrets.", "Arcane Scepter" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "staff_crystal",
                columns: new[] { "description", "name" },
                values: new object[] { "Amplifies magic power.", "Crystal Staff" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "staff_sage",
                columns: new[] { "description", "name" },
                values: new object[] { "The wisdom of an entire age.", "Sage's Rod" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "sword_excal",
                columns: new[] { "description", "name" },
                values: new object[] { "Only the truly determined can draw it.", "Excalibur" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "sword_flame",
                columns: new[] { "description", "name" },
                values: new object[] { "Forged from fire-dragon scales.", "Dragonfire Blade" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "sword_iron",
                columns: new[] { "description", "name" },
                values: new object[] { "Durable, cheap and easy to find.", "Iron Sword" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "sword_steel",
                columns: new[] { "description", "name" },
                values: new object[] { "A razor-sharp steel blade.", "Steel Cleaver" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "sword_wood",
                columns: new[] { "description", "name" },
                values: new object[] { "A novice's training weapon.", "Wooden Sword" });

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "cat",
                column: "name",
                value: "Kitten");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "dog",
                column: "name",
                value: "Puppy");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "dragon",
                column: "name",
                value: "Fire Dragon");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "fox",
                column: "name",
                value: "Fire Fox");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "hamster",
                column: "name",
                value: "Round Hamster");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "lion",
                column: "name",
                value: "Golden Lion");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "owl",
                column: "name",
                value: "Wise Owl");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "panda",
                column: "name",
                value: "Panda");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "phoenix",
                column: "name",
                value: "Phoenix");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "rabbit",
                column: "name",
                value: "White Rabbit");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "unicorn",
                column: "name",
                value: "Unicorn");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "wolf",
                column: "name",
                value: "Silver Wolf");

            migrationBuilder.UpdateData(
                table: "shop_categories",
                keyColumn: "key",
                keyValue: "armor",
                column: "name",
                value: "Armor");

            migrationBuilder.UpdateData(
                table: "shop_categories",
                keyColumn: "key",
                keyValue: "jewelry",
                column: "name",
                value: "Jewelry");

            migrationBuilder.UpdateData(
                table: "shop_categories",
                keyColumn: "key",
                keyValue: "pet",
                column: "name",
                value: "Pets & Eggs");

            migrationBuilder.UpdateData(
                table: "shop_categories",
                keyColumn: "key",
                keyValue: "weapon",
                column: "name",
                value: "Weapons");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "boss1",
                column: "name",
                value: "Dragon Bane");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "boss2",
                column: "name",
                value: "Snack Slayer");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "boss3",
                column: "name",
                value: "Hex Breaker");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "boss4",
                column: "name",
                value: "Golem Crusher");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "boss5",
                column: "name",
                value: "Phantom Dispeller");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "clean20",
                column: "name",
                value: "Iron Will");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "coop3",
                column: "name",
                value: "Coop Architect");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "first_log",
                column: "name",
                value: "First Steps");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "first_pet",
                column: "name",
                value: "Pet Keeper");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "full_gear",
                column: "name",
                value: "Fully Equipped");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "gaming20",
                column: "name",
                value: "Dedicated Gamer");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "habit20",
                column: "name",
                value: "Chain Breaker");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "legend_pet",
                column: "name",
                value: "Legendary Tamer");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "level10",
                column: "name",
                value: "Grandmaster");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "level5",
                column: "name",
                value: "Knight");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "pets5",
                column: "name",
                value: "Pet Rancher");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "shopper",
                column: "name",
                value: "Regular Customer");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "sport10",
                column: "name",
                value: "Athlete");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "sport50",
                column: "name",
                value: "Fitness Champion");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "streak3",
                column: "name",
                value: "Spark");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "streak30",
                column: "name",
                value: "Legend of Persistence");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "streak7",
                column: "name",
                value: "Unstoppable");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "study10",
                column: "name",
                value: "Bookworm");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "study50",
                column: "name",
                value: "Scholar Supreme");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "tasks10",
                column: "name",
                value: "Hard Worker");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "tasks50",
                column: "name",
                value: "Productivity Machine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "clean20",
                columns: new[] { "description", "name" },
                values: new object[] { "Nhịn ăn vặt 20 lần", "Ý Chí Thép" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "coop3",
                columns: new[] { "description", "name" },
                values: new object[] { "Nâng chuồng pet lên cấp 3", "Kiến Trúc Sư Chuồng Trại" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "first_log",
                columns: new[] { "description", "name" },
                values: new object[] { "Hoàn thành nhiệm vụ đầu tiên thuộc một lĩnh vực", "Bước Chân Đầu Tiên" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "first_pet",
                columns: new[] { "description", "name" },
                values: new object[] { "Sở hữu pet đầu tiên", "Người Nuôi Thú" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "full_gear",
                columns: new[] { "description", "name" },
                values: new object[] { "Mặc đủ 6 ô trang bị", "Trang Bị Đầy Đủ" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "gaming20",
                columns: new[] { "description", "name" },
                values: new object[] { "Chơi game đủ 20 giờ", "Game Thủ Chính Hiệu" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "habit20",
                columns: new[] { "description", "name" },
                values: new object[] { "Cưỡng lại thói xấu 20 lần", "Kẻ Phá Xiềng" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "legend_pet",
                columns: new[] { "description", "name" },
                values: new object[] { "Sở hữu một pet Huyền thoại", "Thuần Hóa Huyền Thoại" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "level10",
                columns: new[] { "description", "name" },
                values: new object[] { "Đạt cấp 10", "Đại Sư" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "level5",
                columns: new[] { "description", "name" },
                values: new object[] { "Đạt cấp 5", "Hiệp Sĩ" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "pets5",
                columns: new[] { "description", "name" },
                values: new object[] { "Sở hữu 5 pet", "Chủ Trại Thú" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "shopper",
                columns: new[] { "description", "name" },
                values: new object[] { "Sở hữu 5 món đồ khác nhau", "Khách Quen" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "sport10",
                columns: new[] { "description", "name" },
                values: new object[] { "Tập thể thao đủ 10 giờ", "Lực Sĩ" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "sport50",
                columns: new[] { "description", "name" },
                values: new object[] { "Tập thể thao đủ 50 giờ", "Vô Địch Thể Lực" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "streak3",
                columns: new[] { "description", "name" },
                values: new object[] { "Chuỗi 3 ngày liên tiếp", "Lửa Khởi Đầu" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "streak30",
                columns: new[] { "description", "name" },
                values: new object[] { "Chuỗi 30 ngày liên tiếp", "Huyền Thoại Kiên Trì" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "streak7",
                columns: new[] { "description", "name" },
                values: new object[] { "Chuỗi 7 ngày liên tiếp", "Bất Diệt" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "study10",
                columns: new[] { "description", "name" },
                values: new object[] { "Học đủ 10 giờ", "Mọt Sách" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "study50",
                columns: new[] { "description", "name" },
                values: new object[] { "Học đủ 50 giờ", "Học Thần" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "tasks10",
                columns: new[] { "description", "name" },
                values: new object[] { "Hoàn thành 10 nhiệm vụ", "Chăm Chỉ" });

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "id",
                keyValue: "tasks50",
                columns: new[] { "description", "name" },
                values: new object[] { "Hoàn thành 50 nhiệm vụ", "Cỗ Máy Năng Suất" });

            migrationBuilder.UpdateData(
                table: "bosses",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "lore", "name", "title" },
                values: new object[] { "Nó thì thầm: \"Mai làm cũng được mà...\" Hãy cùng nhau chứng minh nó sai.", "Hắc Long Trì Hoãn", "Chúa tể của \"để mai làm\"" });

            migrationBuilder.UpdateData(
                table: "bosses",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "lore", "name", "title" },
                values: new object[] { "Mỗi miếng snack bạn nhịn được là một nhát chém vào bụng nó.", "Quỷ Ăn Vặt Khổng Lồ", "Kẻ hủy diệt bữa chính" });

            migrationBuilder.UpdateData(
                table: "bosses",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "lore", "name", "title" },
                values: new object[] { "Nó khiến năm phút thành năm tiếng. Đừng để nó nhìn thấy điện thoại của bạn!", "Phù Thủy Mạng Xã Hội", "Bậc thầy cuộn vô tận" });

            migrationBuilder.UpdateData(
                table: "bosses",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "lore", "name", "title" },
                values: new object[] { "Cứng đầu và nặng nề, nhưng không chịu nổi sự kiên trì của cả cộng đồng.", "Golem Lười Biếng", "Tảng đá không muốn dậy" });

            migrationBuilder.UpdateData(
                table: "bosses",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "lore", "name", "title" },
                values: new object[] { "Nó sống trong màn hình lúc nửa đêm. Hãy đánh bại nó bằng một ngày đầy năng lượng.", "Bóng Ma Thức Khuya", "Thực thể của 3 giờ sáng" });

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 0,
                columns: new[] { "board_name", "name", "tagline", "unit" },
                values: new object[] { "Học Thần Điện", "Học tập", "Cày kiến thức, leo lên đỉnh tri thức", "giờ" });

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "board_name", "name", "tagline", "unit" },
                values: new object[] { "Đấu Trường Game Thủ", "Chơi game", "Chiến hết mình, nhớ nghỉ ngơi nhé", "giờ" });

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "board_name", "name", "tagline", "unit" },
                values: new object[] { "Võ Đài Thể Lực", "Thể thao", "Mồ hôi rơi, sức mạnh lên", "giờ" });

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "board_name", "name", "tagline", "unit" },
                values: new object[] { "Thánh Nhịn Vặt", "Không ăn vặt", "Mỗi lần cưỡng lại cám dỗ là một chiến thắng", "lần" });

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "board_name", "name", "tagline", "unit" },
                values: new object[] { "Kẻ Phá Xiềng", "Bỏ thói quen xấu", "Mỗi lần bẻ gãy xiềng xích là một chiến thắng", "lần" });

            migrationBuilder.UpdateData(
                table: "difficulty_levels",
                keyColumn: "level",
                keyValue: 1,
                column: "name",
                value: "Rất dễ");

            migrationBuilder.UpdateData(
                table: "difficulty_levels",
                keyColumn: "level",
                keyValue: 2,
                column: "name",
                value: "Dễ");

            migrationBuilder.UpdateData(
                table: "difficulty_levels",
                keyColumn: "level",
                keyValue: 3,
                column: "name",
                value: "Vừa");

            migrationBuilder.UpdateData(
                table: "difficulty_levels",
                keyColumn: "level",
                keyValue: 4,
                column: "name",
                value: "Khó");

            migrationBuilder.UpdateData(
                table: "difficulty_levels",
                keyColumn: "level",
                keyValue: 5,
                column: "name",
                value: "Cực khó");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 0,
                column: "name",
                value: "Vũ khí");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 1,
                column: "name",
                value: "Mũ");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 2,
                column: "name",
                value: "Giáp");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 3,
                column: "name",
                value: "Giày");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 4,
                column: "name",
                value: "Nhẫn");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 5,
                column: "name",
                value: "Dây chuyền");

            migrationBuilder.UpdateData(
                table: "equipment_slots",
                keyColumn: "id",
                keyValue: 6,
                column: "name",
                value: "Trứng");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 1,
                column: "name",
                value: "Kiếm");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 2,
                column: "name",
                value: "Cung");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 3,
                column: "name",
                value: "Gậy phép");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 4,
                column: "name",
                value: "Rìu & Búa");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 5,
                column: "name",
                value: "Mũ");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 6,
                column: "name",
                value: "Giáp");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 7,
                column: "name",
                value: "Áo choàng");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 8,
                column: "name",
                value: "Giày");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 9,
                column: "name",
                value: "Nhẫn");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 10,
                column: "name",
                value: "Dây chuyền");

            migrationBuilder.UpdateData(
                table: "item_subtypes",
                keyColumn: "id",
                keyValue: 11,
                column: "name",
                value: "Trứng");

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "amulet_bone",
                columns: new[] { "description", "name" },
                values: new object[] { "Chiến lợi phẩm đầu tay.", "Vòng Cổ Xương" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "amulet_jade",
                columns: new[] { "description", "name" },
                values: new object[] { "Mang lại bình an.", "Ngọc Bội" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "amulet_phoenix",
                columns: new[] { "description", "name" },
                values: new object[] { "Tái sinh từ tro tàn.", "Mặt Dây Phượng Hoàng" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "amulet_star",
                columns: new[] { "description", "name" },
                values: new object[] { "Chứa một mảnh bầu trời.", "Thiên Tinh Châu" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "armor_chain",
                columns: new[] { "description", "name" },
                values: new object[] { "Từng mắt xích một.", "Giáp Xích" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "armor_holy",
                columns: new[] { "description", "name" },
                values: new object[] { "Bóng tối không thể lại gần.", "Thánh Giáp Ánh Sáng" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "armor_leather",
                columns: new[] { "description", "name" },
                values: new object[] { "Nhẹ nhàng, linh hoạt.", "Áo Da" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "armor_plate",
                columns: new[] { "description", "name" },
                values: new object[] { "Tường thành di động.", "Giáp Bản" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "axe_hand",
                columns: new[] { "description", "name" },
                values: new object[] { "Chặt củi hay chặt quái đều tốt.", "Rìu Tay" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "axe_titan",
                columns: new[] { "description", "name" },
                values: new object[] { "Cán rìu cao bằng một người.", "Rìu Titan" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "boots_cloth",
                columns: new[] { "description", "name" },
                values: new object[] { "Đi bộ thoải mái.", "Giày Vải" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "boots_flash",
                columns: new[] { "description", "name" },
                values: new object[] { "Nhanh hơn cả tiếng sấm.", "Giày Tia Chớp" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "boots_leather",
                columns: new[] { "description", "name" },
                values: new object[] { "Vượt đường gập ghềnh.", "Ủng Da" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "boots_wind",
                columns: new[] { "description", "name" },
                values: new object[] { "Chân nhẹ như gió.", "Giày Gió" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "boots_wing",
                columns: new[] { "description", "name" },
                values: new object[] { "Bước đi trên mây.", "Ủng Cánh Thiên Thần" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "bow_elven",
                columns: new[] { "description", "name" },
                values: new object[] { "Dây cung làm từ tóc tiên.", "Cung Tinh Linh" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "bow_long",
                columns: new[] { "description", "name" },
                values: new object[] { "Tầm bắn xa.", "Trường Cung" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "bow_short",
                columns: new[] { "description", "name" },
                values: new object[] { "Nhẹ và nhanh.", "Cung Ngắn" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "bow_star",
                columns: new[] { "description", "name" },
                values: new object[] { "Mỗi mũi tên là một sao băng.", "Cung Sao Băng" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "cloak_shadow",
                columns: new[] { "description", "name" },
                values: new object[] { "Hòa vào bóng tối.", "Áo Choàng Bóng Đêm" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "cloak_travel",
                columns: new[] { "description", "name" },
                values: new object[] { "Bạn đồng hành mọi chuyến đi.", "Áo Choàng Lữ Hành" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "crown_dragon",
                columns: new[] { "description", "name" },
                values: new object[] { "Rồng cúi đầu trước chủ nhân.", "Mão Rồng Thần" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "crown_gold",
                columns: new[] { "description", "name" },
                values: new object[] { "Dành cho người dẫn đầu.", "Vương Miện Vàng" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "egg_common",
                columns: new[] { "description", "name" },
                values: new object[] { "Ấp 15 phút. Nở ra pet Thường (có cơ hội ra Hiếm).", "Trứng Thường" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "egg_epic",
                columns: new[] { "description", "name" },
                values: new object[] { "Ấp 4 giờ. Nở ra pet Sử thi (có cơ hội ra Huyền thoại).", "Trứng Sử Thi" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "egg_legend",
                columns: new[] { "description", "name" },
                values: new object[] { "Ấp 12 giờ. Chắc chắn nở ra pet Huyền thoại.", "Trứng Huyền Thoại" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "egg_rare",
                columns: new[] { "description", "name" },
                values: new object[] { "Ấp 1 giờ. Nở ra pet Hiếm (có cơ hội ra Sử thi).", "Trứng Hiếm" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "hammer_thunder",
                columns: new[] { "description", "name" },
                values: new object[] { "Gọi sấm xuống trần gian.", "Thần Chùy Sấm Sét" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "hammer_war",
                columns: new[] { "description", "name" },
                values: new object[] { "Nặng, đau, hiệu quả.", "Búa Chiến" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "helm_cloth",
                columns: new[] { "description", "name" },
                values: new object[] { "Che nắng là chính.", "Mũ Vải" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "helm_iron",
                columns: new[] { "description", "name" },
                values: new object[] { "Chắc chắn, hơi nóng.", "Mũ Sắt" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "helm_knight",
                columns: new[] { "description", "name" },
                values: new object[] { "Biểu tượng của danh dự.", "Mũ Hiệp Sĩ" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "ring_copper",
                columns: new[] { "description", "name" },
                values: new object[] { "Giản dị mà bền.", "Nhẫn Đồng" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "ring_infinity",
                columns: new[] { "description", "name" },
                values: new object[] { "Sức mạnh không giới hạn.", "Nhẫn Vô Cực" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "ring_ruby",
                columns: new[] { "description", "name" },
                values: new object[] { "Đỏ như lửa cháy.", "Nhẫn Hồng Ngọc" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "ring_silver",
                columns: new[] { "description", "name" },
                values: new object[] { "Sáng lấp lánh.", "Nhẫn Bạc" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "staff_apprentice",
                columns: new[] { "description", "name" },
                values: new object[] { "Phép thuật nhập môn.", "Gậy Học Việc" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "staff_arcane",
                columns: new[] { "description", "name" },
                values: new object[] { "Thì thầm những bí mật cổ xưa.", "Quyền Trượng Thần Bí" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "staff_crystal",
                columns: new[] { "description", "name" },
                values: new object[] { "Khuếch đại ma lực.", "Gậy Pha Lê" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "staff_sage",
                columns: new[] { "description", "name" },
                values: new object[] { "Trí tuệ của một thời đại.", "Trượng Đại Hiền Giả" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "sword_excal",
                columns: new[] { "description", "name" },
                values: new object[] { "Chỉ người kiên định mới rút được.", "Thánh Kiếm Excalibur" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "sword_flame",
                columns: new[] { "description", "name" },
                values: new object[] { "Rèn từ vảy rồng lửa.", "Hỏa Long Kiếm" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "sword_iron",
                columns: new[] { "description", "name" },
                values: new object[] { "Bền, rẻ, dễ kiếm.", "Kiếm Sắt" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "sword_steel",
                columns: new[] { "description", "name" },
                values: new object[] { "Lưỡi thép sắc lẹm.", "Trảm Kiếm Thép" });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "id",
                keyValue: "sword_wood",
                columns: new[] { "description", "name" },
                values: new object[] { "Đồ tập của tân thủ.", "Kiếm Gỗ" });

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "cat",
                column: "name",
                value: "Mèo Con");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "dog",
                column: "name",
                value: "Cún Con");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "dragon",
                column: "name",
                value: "Rồng Lửa");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "fox",
                column: "name",
                value: "Cáo Lửa");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "hamster",
                column: "name",
                value: "Hamster Tròn");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "lion",
                column: "name",
                value: "Sư Tử Vàng");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "owl",
                column: "name",
                value: "Cú Thông Thái");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "panda",
                column: "name",
                value: "Gấu Trúc");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "phoenix",
                column: "name",
                value: "Phượng Hoàng");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "rabbit",
                column: "name",
                value: "Thỏ Trắng");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "unicorn",
                column: "name",
                value: "Kỳ Lân");

            migrationBuilder.UpdateData(
                table: "pet_species",
                keyColumn: "id",
                keyValue: "wolf",
                column: "name",
                value: "Sói Bạc");

            migrationBuilder.UpdateData(
                table: "shop_categories",
                keyColumn: "key",
                keyValue: "armor",
                column: "name",
                value: "Giáp trụ");

            migrationBuilder.UpdateData(
                table: "shop_categories",
                keyColumn: "key",
                keyValue: "jewelry",
                column: "name",
                value: "Trang sức");

            migrationBuilder.UpdateData(
                table: "shop_categories",
                keyColumn: "key",
                keyValue: "pet",
                column: "name",
                value: "Pet & Trứng");

            migrationBuilder.UpdateData(
                table: "shop_categories",
                keyColumn: "key",
                keyValue: "weapon",
                column: "name",
                value: "Vũ khí");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "boss1",
                column: "name",
                value: "Khắc Tinh Hắc Long");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "boss2",
                column: "name",
                value: "Sát Thủ Quỷ Vặt");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "boss3",
                column: "name",
                value: "Phá Giải Bùa Mạng");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "boss4",
                column: "name",
                value: "Đập Vỡ Golem");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "boss5",
                column: "name",
                value: "Xua Tan Bóng Ma");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "clean20",
                column: "name",
                value: "Ý Chí Thép");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "coop3",
                column: "name",
                value: "Kiến Trúc Sư Chuồng Trại");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "first_log",
                column: "name",
                value: "Bước Chân Đầu Tiên");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "first_pet",
                column: "name",
                value: "Người Nuôi Thú");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "full_gear",
                column: "name",
                value: "Trang Bị Đầy Đủ");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "gaming20",
                column: "name",
                value: "Game Thủ Chính Hiệu");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "habit20",
                column: "name",
                value: "Kẻ Phá Xiềng");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "legend_pet",
                column: "name",
                value: "Thuần Hóa Huyền Thoại");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "level10",
                column: "name",
                value: "Đại Sư");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "level5",
                column: "name",
                value: "Hiệp Sĩ");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "pets5",
                column: "name",
                value: "Chủ Trại Thú");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "shopper",
                column: "name",
                value: "Khách Quen");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "sport10",
                column: "name",
                value: "Lực Sĩ");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "sport50",
                column: "name",
                value: "Vô Địch Thể Lực");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "streak3",
                column: "name",
                value: "Lửa Khởi Đầu");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "streak30",
                column: "name",
                value: "Huyền Thoại Kiên Trì");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "streak7",
                column: "name",
                value: "Bất Diệt");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "study10",
                column: "name",
                value: "Mọt Sách");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "study50",
                column: "name",
                value: "Học Thần");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "tasks10",
                column: "name",
                value: "Chăm Chỉ");

            migrationBuilder.UpdateData(
                table: "titles",
                keyColumn: "id",
                keyValue: "tasks50",
                column: "name",
                value: "Cỗ Máy Năng Suất");
        }
    }
}
