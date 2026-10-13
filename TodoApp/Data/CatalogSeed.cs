using TodoApp.Models;

namespace TodoApp.Data;

/// <summary>
/// Initial reference data. It is inserted by the first migration (HasData); after that the database is the source of truth
/// and values can be edited there (the app reloads the catalog periodically).
/// </summary>
public static class CatalogSeed
{
    // ---------- categories (the 5 leaderboards) ----------
    public static readonly CategoryEntity[] Categories =
    [
        new() { Id = ActivityCategory.Study, Slug = "study", Name = "Study", BoardName = "Scholars' Hall", Emoji = "📚", Unit = "hours", IsHours = true, MaxPerEntry = 12, Tagline = "Grind knowledge, climb the summit of wisdom", SortOrder = 1 },
        new() { Id = ActivityCategory.Gaming, Slug = "gaming", Name = "Gaming", BoardName = "Gamer Arena", Emoji = "🎮", Unit = "hours", IsHours = true, MaxPerEntry = 12, Tagline = "Fight hard, remember to rest", SortOrder = 2 },
        new() { Id = ActivityCategory.Sport, Slug = "sport", Name = "Sports", BoardName = "Fitness Colosseum", Emoji = "🏃", Unit = "hours", IsHours = true, MaxPerEntry = 6, Tagline = "Sweat drops, strength rises", SortOrder = 3 },
        new() { Id = ActivityCategory.CleanEating, Slug = "clean", Name = "No Junk Food", BoardName = "Snack Resistance", Emoji = "🥗", Unit = "times", IsHours = false, MaxPerEntry = 20, Tagline = "Every temptation resisted is a victory", SortOrder = 4 },
        new() { Id = ActivityCategory.BadHabit, Slug = "habit", Name = "Quit Bad Habits", BoardName = "Chain Breakers", Emoji = "⛓️", Unit = "times", IsHours = false, MaxPerEntry = 20, Tagline = "Every chain you break is a victory", SortOrder = 5 },
    ];

    // ---------- difficulty levels ----------
    public static readonly DifficultyLevelEntity[] Difficulties =
    [
        new() { Level = 1, Name = "Trivial", BaseGold = 8, BaseExp = 6, BaseDamage = 5, BaseHpLoss = 3 },
        new() { Level = 2, Name = "Easy", BaseGold = 14, BaseExp = 10, BaseDamage = 9, BaseHpLoss = 5 },
        new() { Level = 3, Name = "Medium", BaseGold = 20, BaseExp = 15, BaseDamage = 14, BaseHpLoss = 8 },
        new() { Level = 4, Name = "Hard", BaseGold = 30, BaseExp = 22, BaseDamage = 20, BaseHpLoss = 12 },
        new() { Level = 5, Name = "Extreme", BaseGold = 45, BaseExp = 32, BaseDamage = 30, BaseHpLoss = 18 },
    ];

    // ---------- shop structure ----------
    public static readonly ShopCategoryEntity[] ShopCategories =
    [
        new() { Key = "weapon", Name = "Weapons", Emoji = "⚔️", SortOrder = 1 },
        new() { Key = "armor", Name = "Armor", Emoji = "🛡️", SortOrder = 2 },
        new() { Key = "jewelry", Name = "Jewelry", Emoji = "💎", SortOrder = 3 },
        new() { Key = "pet", Name = "Pets & Eggs", Emoji = "🥚", SortOrder = 4 },
    ];

    public static readonly EquipmentSlotEntity[] Slots =
    [
        new() { Id = ItemSlot.Weapon, Name = "Weapon", Emoji = "🗡️", ShopCategoryKey = "weapon", IsEquippable = true },
        new() { Id = ItemSlot.Helmet, Name = "Helmet", Emoji = "🪖", ShopCategoryKey = "armor", IsEquippable = true },
        new() { Id = ItemSlot.Armor, Name = "Armor", Emoji = "🛡️", ShopCategoryKey = "armor", IsEquippable = true },
        new() { Id = ItemSlot.Boots, Name = "Boots", Emoji = "👢", ShopCategoryKey = "armor", IsEquippable = true },
        new() { Id = ItemSlot.Ring, Name = "Ring", Emoji = "💍", ShopCategoryKey = "jewelry", IsEquippable = true },
        new() { Id = ItemSlot.Amulet, Name = "Necklace", Emoji = "📿", ShopCategoryKey = "jewelry", IsEquippable = true },
        new() { Id = ItemSlot.Egg, Name = "Egg", Emoji = "🥚", ShopCategoryKey = "pet", IsEquippable = false },
    ];

    // ---------- items ----------
    private record ItemDef(string Id, string Name, ItemSlot Slot, string Sub, Rarity Rarity, int Price, int Power, string Emoji, string Desc);

    private const Rarity C = Rarity.Common, R = Rarity.Rare, E = Rarity.Epic, L = Rarity.Legendary;
    private const ItemSlot W = ItemSlot.Weapon, H = ItemSlot.Helmet, A = ItemSlot.Armor, B = ItemSlot.Boots,
        Ri = ItemSlot.Ring, Am = ItemSlot.Amulet, Eg = ItemSlot.Egg;

    private static ItemDef I(string id, string name, ItemSlot slot, string sub, Rarity r, int price, int power, string emoji, string desc) =>
        new(id, name, slot, sub, r, price, power, emoji, desc);

    private static readonly ItemDef[] ItemDefs =
    [
        I("sword_wood", "Wooden Sword", W, "Sword", C, 60, 3, "🗡️", "A novice's training weapon."),
        I("sword_iron", "Iron Sword", W, "Sword", C, 150, 8, "🗡️", "Durable, cheap and easy to find."),
        I("sword_steel", "Steel Cleaver", W, "Sword", R, 450, 18, "⚔️", "A razor-sharp steel blade."),
        I("sword_flame", "Dragonfire Blade", W, "Sword", E, 1500, 40, "🔥", "Forged from fire-dragon scales."),
        I("sword_excal", "Excalibur", W, "Sword", L, 6000, 90, "✨", "Only the truly determined can draw it."),
        I("bow_short", "Short Bow", W, "Bow", C, 80, 4, "🏹", "Light and fast."),
        I("bow_long", "Longbow", W, "Bow", R, 400, 16, "🏹", "Long range."),
        I("bow_elven", "Elven Bow", W, "Bow", E, 1400, 38, "🏹", "Strung with a fairy's hair."),
        I("bow_star", "Shooting Star Bow", W, "Bow", L, 5500, 85, "🌠", "Every arrow is a shooting star."),
        I("staff_apprentice", "Apprentice Staff", W, "Staff", C, 90, 4, "🪄", "Entry-level magic."),
        I("staff_crystal", "Crystal Staff", W, "Staff", R, 420, 17, "🔮", "Amplifies magic power."),
        I("staff_arcane", "Arcane Scepter", W, "Staff", E, 1600, 42, "🔮", "Whispers ancient secrets."),
        I("staff_sage", "Sage's Rod", W, "Staff", L, 6500, 95, "🌟", "The wisdom of an entire age."),
        I("axe_hand", "Hand Axe", W, "Axe & Hammer", C, 100, 5, "🪓", "Great for chopping wood or monsters."),
        I("hammer_war", "War Hammer", W, "Axe & Hammer", R, 480, 20, "🔨", "Heavy, painful, effective."),
        I("axe_titan", "Titan Axe", W, "Axe & Hammer", E, 1700, 44, "🪓", "The handle is as tall as a person."),
        I("hammer_thunder", "Thunder Hammer", W, "Axe & Hammer", L, 7000, 100, "⚡", "Calls down lightning."),
        I("helm_cloth", "Cloth Cap", H, "Helmet", C, 40, 2, "🧢", "Mostly keeps the sun off."),
        I("helm_iron", "Iron Helmet", H, "Helmet", C, 120, 6, "⛑️", "Sturdy, a bit hot."),
        I("helm_knight", "Knight's Helm", H, "Helmet", R, 380, 14, "🪖", "A symbol of honor."),
        I("crown_gold", "Golden Crown", H, "Helmet", E, 1500, 35, "👑", "For those who lead."),
        I("crown_dragon", "Dragon Crown", H, "Helmet", L, 5800, 80, "🐲", "Dragons bow to its owner."),
        I("armor_leather", "Leather Vest", A, "Armor", C, 100, 5, "🦺", "Light and flexible."),
        I("armor_chain", "Chainmail", A, "Armor", R, 420, 16, "🥋", "Link by link."),
        I("armor_plate", "Plate Armor", A, "Armor", E, 1400, 36, "🛡️", "A walking fortress."),
        I("armor_holy", "Holy Light Armor", A, "Armor", L, 6200, 88, "🌞", "Darkness cannot come near."),
        I("cloak_travel", "Traveler's Cloak", A, "Cloak", C, 110, 5, "🧥", "A companion on every journey."),
        I("cloak_shadow", "Shadow Cloak", A, "Cloak", E, 1300, 34, "🦇", "Blends into the darkness."),
        I("boots_cloth", "Cloth Shoes", B, "Boots", C, 40, 2, "👟", "Comfortable for walking."),
        I("boots_leather", "Leather Boots", B, "Boots", C, 110, 5, "🥾", "Tackles rough roads."),
        I("boots_wind", "Wind Boots", B, "Boots", R, 360, 13, "💨", "Feet as light as the wind."),
        I("boots_flash", "Lightning Boots", B, "Boots", E, 1300, 32, "⚡", "Faster than thunder."),
        I("boots_wing", "Angel Wing Boots", B, "Boots", L, 5200, 78, "🪽", "Walk on clouds."),
        I("ring_copper", "Copper Ring", Ri, "Ring", C, 70, 3, "💍", "Plain but durable."),
        I("ring_silver", "Silver Ring", Ri, "Ring", R, 350, 12, "💍", "Sparkling bright."),
        I("ring_ruby", "Ruby Ring", Ri, "Ring", E, 1200, 30, "❤️", "Red as fire."),
        I("ring_infinity", "Infinity Ring", Ri, "Ring", L, 5000, 75, "♾️", "Unlimited power."),
        I("amulet_bone", "Bone Necklace", Am, "Necklace", C, 70, 3, "📿", "Your first trophy."),
        I("amulet_jade", "Jade Pendant", Am, "Necklace", R, 360, 12, "🟢", "Brings peace."),
        I("amulet_phoenix", "Phoenix Pendant", Am, "Necklace", E, 1250, 31, "🔥", "Reborn from ashes."),
        I("amulet_star", "Star Orb", Am, "Necklace", L, 5200, 76, "💫", "Holds a piece of the sky."),
        I("egg_common", "Common Egg", Eg, "Egg", C, 100, 0, "🥚", "Incubates in 15 minutes. Hatches a Common pet (small chance of Rare)."),
        I("egg_rare", "Rare Egg", Eg, "Egg", R, 450, 0, "🪺", "Incubates in 1 hour. Hatches a Rare pet (chance of Epic)."),
        I("egg_epic", "Epic Egg", Eg, "Egg", E, 1500, 0, "🔮", "Incubates in 4 hours. Hatches an Epic pet (chance of Legendary)."),
        I("egg_legend", "Legendary Egg", Eg, "Egg", L, 5000, 0, "🌟", "Incubates in 12 hours. Always hatches a Legendary pet."),
    ];

    /// <summary>Stat bonuses derived once from an item's base "power" and kind (stored explicitly in the table afterwards).</summary>
    private static (int Atk, int Def, int Int, int Luk, int MaxHp, int MaxMana) StatsOf(ItemDef i)
    {
        var p = i.Power;
        return i.Slot switch
        {
            ItemSlot.Weapon when i.Sub == "Bow" => (p * 7 / 10, 0, 0, p * 3 / 10, 0, 0),
            ItemSlot.Weapon when i.Sub == "Staff" => (0, 0, p, 0, 0, p * 2),
            ItemSlot.Weapon => (p, 0, 0, 0, 0, 0),
            ItemSlot.Helmet => (0, p, 0, 0, 0, 0),
            ItemSlot.Armor => (0, p, 0, 0, p * 2, 0),
            ItemSlot.Boots => (0, p / 2, 0, p / 2, 0, 0),
            ItemSlot.Ring => (p / 2, 0, p / 2, 0, 0, 0),
            ItemSlot.Amulet => (0, 0, 0, p / 4, p * 2, p),
            _ => (0, 0, 0, 0, 0, 0),
        };
    }

    public static readonly ItemSubtypeEntity[] Subtypes = ItemDefs
        .Select(i => (i.Slot, i.Sub)).Distinct()
        .Select((x, n) => new ItemSubtypeEntity { Id = n + 1, SlotId = x.Slot, Name = x.Sub })
        .ToArray();

    public static readonly ItemEntity[] Items = ItemDefs.Select(i =>
    {
        var s = StatsOf(i);
        return new ItemEntity
        {
            Id = i.Id, Name = i.Name, SlotId = i.Slot, Rarity = i.Rarity, Price = i.Price, Emoji = i.Emoji, Description = i.Desc,
            SubtypeId = Subtypes.First(t => t.SlotId == i.Slot && t.Name == i.Sub).Id,
            Atk = s.Atk, Def = s.Def, Int = s.Int, Luk = s.Luk, MaxHp = s.MaxHp, MaxMana = s.MaxMana,
        };
    }).ToArray();

    public static readonly EggTypeEntity[] Eggs =
    [
        new() { ItemId = "egg_common", HatchMinutes = 15, BaseRarity = Rarity.Common },
        new() { ItemId = "egg_rare", HatchMinutes = 60, BaseRarity = Rarity.Rare },
        new() { ItemId = "egg_epic", HatchMinutes = 240, BaseRarity = Rarity.Epic },
        new() { ItemId = "egg_legend", HatchMinutes = 720, BaseRarity = Rarity.Legendary },
    ];

    // ---------- pet species ----------
    private record SpeciesDef(string Id, string Name, string Emoji, Rarity Rarity);

    private static readonly SpeciesDef[] SpeciesDefs =
    [
        new("cat", "Kitten", "🐱", Rarity.Common),
        new("dog", "Puppy", "🐶", Rarity.Common),
        new("rabbit", "White Rabbit", "🐰", Rarity.Common),
        new("hamster", "Round Hamster", "🐹", Rarity.Common),
        new("fox", "Fire Fox", "🦊", Rarity.Rare),
        new("panda", "Panda", "🐼", Rarity.Rare),
        new("owl", "Wise Owl", "🦉", Rarity.Rare),
        new("wolf", "Silver Wolf", "🐺", Rarity.Epic),
        new("lion", "Golden Lion", "🦁", Rarity.Epic),
        new("unicorn", "Unicorn", "🦄", Rarity.Epic),
        new("dragon", "Fire Dragon", "🐉", Rarity.Legendary),
        new("phoenix", "Phoenix", "🦅", Rarity.Legendary),
    ];

    public static readonly PetSpeciesEntity[] Species = SpeciesDefs
        .Select(s => new PetSpeciesEntity { Id = s.Id, Name = s.Name, Emoji = s.Emoji, Rarity = s.Rarity }).ToArray();

    // ---------- achievements (each grants a title with the same id) ----------
    private record AchDef(string Id, string Icon, string Name, string Desc, int Reward, string Metric, long Target);

    private static readonly AchDef[] AchDefs =
    [
        new("first_log", "🌱", "First Steps", "Complete your first task in a category", 50, "log_count", 1),
        new("streak3", "🔥", "Spark", "Reach a 3-day streak", 100, "streak", 3),
        new("streak7", "🌋", "Unstoppable", "Reach a 7-day streak", 300, "streak", 7),
        new("streak30", "☄️", "Legend of Persistence", "Reach a 30-day streak", 2000, "streak", 30),
        new("study10", "📚", "Bookworm", "Study for 10 hours", 150, "study_hours", 10),
        new("study50", "🎓", "Scholar Supreme", "Study for 50 hours", 800, "study_hours", 50),
        new("sport10", "🏅", "Athlete", "Exercise for 10 hours", 150, "sport_hours", 10),
        new("sport50", "🏆", "Fitness Champion", "Exercise for 50 hours", 800, "sport_hours", 50),
        new("gaming20", "🎮", "Dedicated Gamer", "Play games for 20 hours", 100, "gaming_hours", 20),
        new("clean20", "🥗", "Iron Will", "Skip junk food 20 times", 200, "clean_times", 20),
        new("habit20", "⛓️", "Chain Breaker", "Resist a bad habit 20 times", 200, "habit_times", 20),
        new("level5", "🎖️", "Knight", "Reach level 5", 250, "level", 5),
        new("level10", "👑", "Grandmaster", "Reach level 10", 1000, "level", 10),
        new("tasks10", "✅", "Hard Worker", "Complete 10 tasks", 150, "tasks_done", 10),
        new("tasks50", "🏗️", "Productivity Machine", "Complete 50 tasks", 700, "tasks_done", 50),
        new("first_pet", "🐣", "Pet Keeper", "Own your first pet", 100, "pets_owned", 1),
        new("pets5", "🐾", "Pet Rancher", "Own 5 pets", 500, "pets_owned", 5),
        new("legend_pet", "🐉", "Legendary Tamer", "Own a Legendary pet", 1500, "legendary_pets", 1),
        new("shopper", "🛍️", "Regular Customer", "Own 5 different items", 200, "distinct_items", 5),
        new("full_gear", "🧙", "Fully Equipped", "Wear all 6 equipment slots", 500, "equipped_count", 6),
        new("coop3", "🏠", "Coop Architect", "Upgrade the pet coop to level 3", 300, "coop_level", 3),
    ];

    public static readonly AchievementEntity[] Achievements = AchDefs.Select(a => new AchievementEntity
    {
        Id = a.Id, Name = a.Name, Icon = a.Icon, Description = a.Desc, RewardGold = a.Reward, Metric = a.Metric, Target = a.Target, TitleId = a.Id,
    }).ToArray();

    // ---------- bosses and their sample rewards ----------
    public static readonly BossEntity[] Bosses =
    [
        new() { Id = 1, Name = "Procrastination Dragon", Emoji = "🐲", Title = "Lord of \"I'll do it tomorrow\"", MaxHp = 6000, SortOrder = 1, Lore = "It whispers: \"Tomorrow is fine...\" Let's prove it wrong together." },
        new() { Id = 2, Name = "Giant Snack Demon", Emoji = "👹", Title = "Destroyer of proper meals", MaxHp = 5000, SortOrder = 2, Lore = "Every snack you resist is a slash at its belly." },
        new() { Id = 3, Name = "Social Media Sorcerer", Emoji = "🧙", Title = "Master of the endless scroll", MaxHp = 7000, SortOrder = 3, Lore = "It turns five minutes into five hours. Don't let it see your phone!" },
        new() { Id = 4, Name = "Lazy Golem", Emoji = "🗿", Title = "The rock that refuses to get up", MaxHp = 5500, SortOrder = 4, Lore = "Stubborn and heavy, but no match for the persistence of the whole community." },
        new() { Id = 5, Name = "Midnight Phantom", Emoji = "👻", Title = "The entity of 3 a.m.", MaxHp = 6500, SortOrder = 5, Lore = "It lives in your screen at midnight. Defeat it with a day full of energy." },
    ];

    /// <summary>Titles: one per achievement plus one per boss.</summary>
    public static readonly TitleEntity[] Titles = Achievements
        .Select(a => new TitleEntity { Id = a.TitleId, Name = a.Name, Icon = a.Icon })
        .Concat(
        [
            new TitleEntity { Id = "boss1", Name = "Dragon Bane", Icon = "🐲" },
            new TitleEntity { Id = "boss2", Name = "Snack Slayer", Icon = "👹" },
            new TitleEntity { Id = "boss3", Name = "Hex Breaker", Icon = "🧙" },
            new TitleEntity { Id = "boss4", Name = "Golem Crusher", Icon = "🗿" },
            new TitleEntity { Id = "boss5", Name = "Phantom Dispeller", Icon = "👻" },
        ]).ToArray();

    /// <summary>Sample rewards (gold, item, pet, title). Edit freely in the boss_rewards table.</summary>
    public static readonly BossRewardEntity[] BossRewards = Bosses.SelectMany((b, i) => new[]
    {
        new BossRewardEntity { Id = i * 4 + 1, BossId = b.Id, Kind = RewardKind.Gold, Gold = 300 + 100 * i },
        new BossRewardEntity { Id = i * 4 + 2, BossId = b.Id, Kind = RewardKind.Item, ItemId = "egg_rare", Quantity = 1 },
        new BossRewardEntity { Id = i * 4 + 3, BossId = b.Id, Kind = RewardKind.Pet, PetSpeciesId = i % 2 == 0 ? "fox" : "owl" },
        new BossRewardEntity { Id = i * 4 + 4, BossId = b.Id, Kind = RewardKind.Title, TitleId = "boss" + b.Id },
    }).ToArray();

    // ---------- rarities ----------
    public static readonly RarityEntity[] Rarities =
    [
        new() { Id = Rarity.Common, Label = "Common", SortOrder = 1 },
        new() { Id = Rarity.Rare, Label = "Rare", SortOrder = 2 },
        new() { Id = Rarity.Epic, Label = "Epic", SortOrder = 3 },
        new() { Id = Rarity.Legendary, Label = "Legendary", SortOrder = 4 },
    ];

    // ---------- level rank names (add a row to add a rank; set TitleId to also grant a selectable title) ----------
    public static readonly LevelTitleEntity[] LevelTitles =
    [
        new() { MinLevel = 1, Name = "Novice", Icon = "🌱" },
        new() { MinLevel = 3, Name = "Apprentice", Icon = "📘" },
        new() { MinLevel = 5, Name = "Warrior", Icon = "⚔️" },
        new() { MinLevel = 8, Name = "Knight", Icon = "🛡️" },
        new() { MinLevel = 12, Name = "Grandmaster", Icon = "🎓" },
        new() { MinLevel = 16, Name = "Legend", Icon = "👑" },
    ];

    // ---------- dashboard badges ----------
    public static readonly BadgeEntity[] Badges =
    [
        new() { Id = "spark", Icon = "🔥", Name = "Spark", Description = "3-day streak", Metric = "streak", Target = 3, SortOrder = 1 },
        new() { Id = "unstoppable", Icon = "🌋", Name = "Unstoppable", Description = "7-day streak", Metric = "streak", Target = 7, SortOrder = 2 },
        new() { Id = "bookworm", Icon = "📚", Name = "Bookworm", Description = "Study 10 hours", Metric = "study_hours", Target = 10, SortOrder = 3 },
        new() { Id = "athlete", Icon = "🏅", Name = "Athlete", Description = "Exercise 10 hours", Metric = "sport_hours", Target = 10, SortOrder = 4 },
        new() { Id = "iron_will", Icon = "🥗", Name = "Iron Will", Description = "Skip junk food 10 times", Metric = "clean_times", Target = 10, SortOrder = 5 },
        new() { Id = "chain_breaker", Icon = "⛓️", Name = "Chain Breaker", Description = "Resist a bad habit 10 times", Metric = "habit_times", Target = 10, SortOrder = 6 },
        new() { Id = "knight", Icon = "🎖️", Name = "Knight", Description = "Reach level 5", Metric = "level", Target = 5, SortOrder = 7 },
        new() { Id = "achiever", Icon = "✅", Name = "Achiever", Description = "Complete 5 tasks", Metric = "tasks_done", Target = 5, SortOrder = 8 },
    ];

    // ---------- time zones offered in the UI ----------
    public static readonly TimeZoneEntity[] TimeZones =
    [
        new() { Id = "Asia/Ho_Chi_Minh", Label = "Vietnam (UTC+7)", SortOrder = 1 },
        new() { Id = "Europe/Berlin", Label = "Germany / Central Europe (Berlin)", SortOrder = 2 },
        new() { Id = "Europe/London", Label = "United Kingdom (London)", SortOrder = 3 },
        new() { Id = "Europe/Paris", Label = "France (Paris)", SortOrder = 4 },
        new() { Id = "Europe/Moscow", Label = "Russia (Moscow)", SortOrder = 5 },
        new() { Id = "Asia/Singapore", Label = "Singapore", SortOrder = 6 },
        new() { Id = "Asia/Bangkok", Label = "Thailand (Bangkok)", SortOrder = 7 },
        new() { Id = "Asia/Tokyo", Label = "Japan (Tokyo)", SortOrder = 8 },
        new() { Id = "Asia/Seoul", Label = "South Korea (Seoul)", SortOrder = 9 },
        new() { Id = "Australia/Sydney", Label = "Australia (Sydney)", SortOrder = 10 },
        new() { Id = "America/New_York", Label = "US East Coast (New York)", SortOrder = 11 },
        new() { Id = "America/Los_Angeles", Label = "US West Coast (Los Angeles)", SortOrder = 12 },
        new() { Id = "UTC", Label = "UTC", SortOrder = 13 },
    ];

    // ---------- tunable game numbers ----------
    private static GameSettingEntity S(string key, decimal value, string group, string description) =>
        new() { Key = key, Value = value, GroupName = group, Description = description };

    public static readonly GameSettingEntity[] Settings =
    [
        S("level_exp_base", 100, "progression", "EXP needed for level N = base x (N-1)^exponent"),
        S("level_exp_exponent", 2, "progression", "Exponent of the level curve (2 = quadratic)"),
        S("starting_gold", 100, "progression", "Gold a brand-new character starts with"),
        S("heal_per_difficulty", 1, "progression", "HP restored per difficulty point when a task is completed"),

        S("streak_bonus_step", 0.05m, "rewards", "Gold/EXP bonus per streak day (0.05 = +5%)"),
        S("streak_bonus_cap_days", 10, "rewards", "Streak days after which the bonus stops growing"),
        S("luck_gold_divisor", 200, "rewards", "Gold multiplier = 1 + LUK / this value"),
        S("power_damage_divisor", 100, "rewards", "Boss damage multiplier = 1 + (ATK + INT/2) / this value"),
        S("def_reduction_divisor", 100, "rewards", "HP loss reduction = DEF / this value"),
        S("def_reduction_cap", 0.6m, "rewards", "Maximum HP loss reduction from DEF (0.6 = 60%)"),

        S("hp_base", 100, "stats", "Max HP at level 0"),
        S("hp_per_level", 10, "stats", "Max HP gained per level"),
        S("mana_base", 20, "stats", "Max mana at level 0"),
        S("mana_per_level", 5, "stats", "Max mana gained per level"),
        S("atk_base", 5, "stats", "ATK at level 0"),
        S("atk_per_level", 1, "stats", "ATK gained per level"),
        S("int_base", 5, "stats", "INT at level 0"),
        S("int_per_level", 1, "stats", "INT gained per level"),
        S("def_base", 2, "stats", "DEF at level 0"),
        S("def_levels_per_point", 2, "stats", "Levels needed to gain 1 DEF"),
        S("luk_base", 1, "stats", "LUK at level 0"),
        S("luk_levels_per_point", 3, "stats", "Levels needed to gain 1 LUK"),

        S("max_coop_level", 5, "pets", "Highest pet coop level"),
        S("max_incubator_level", 3, "pets", "Highest incubator level"),
        S("max_pet_level", 20, "pets", "Highest pet level"),
        S("pet_feed_cost", 10, "pets", "Gold cost of one feeding"),
        S("pet_feed_xp", 15, "pets", "Pet XP gained per feeding"),
        S("pet_xp_per_level", 50, "pets", "Pet XP needed to leave level N = this x N"),
        S("coop_capacity_base", 1, "pets", "Coop capacity at level 0"),
        S("coop_capacity_per_level", 2, "pets", "Extra pets the coop holds per level"),
        S("coop_upgrade_cost_per_level", 200, "pets", "Coop upgrade cost = this x current level"),
        S("incubator_upgrade_cost_per_level", 500, "pets", "Incubator upgrade cost = this x current level"),
        S("hatch_upgrade_chance", 0.10m, "pets", "Chance an egg hatches a pet one rarity tier higher"),
    ];
}
