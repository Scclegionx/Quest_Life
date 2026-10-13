namespace TodoApp.Data;

/// <summary>
/// Typed access to the game_settings table. Every number that balances the game lives there; the defaults below are only
/// used if a row is missing, so deleting a setting can never break the app.
/// </summary>
public static class GameSettings
{
    private static decimal V(string key, decimal fallback) =>
        GameCatalog.Current.Settings.TryGetValue(key, out var v) ? v : fallback;

    // ----- progression -----
    /// <summary>EXP needed for level N = base x (N-1)^exponent.</summary>
    public static decimal LevelExpBase => V("level_exp_base", 100);
    public static double LevelExpExponent => (double)V("level_exp_exponent", 2);
    public static int StartingGold => (int)V("starting_gold", 100);
    public static int HealPerDifficulty => (int)V("heal_per_difficulty", 1);

    // ----- rewards and penalties -----
    public static double StreakStep => (double)V("streak_bonus_step", 0.05m);
    public static int StreakCapDays => (int)V("streak_bonus_cap_days", 10);
    public static double LuckGoldDivisor => (double)V("luck_gold_divisor", 200);
    public static double PowerDamageDivisor => (double)V("power_damage_divisor", 100);
    public static double DefReductionDivisor => (double)V("def_reduction_divisor", 100);
    public static double DefReductionCap => (double)V("def_reduction_cap", 0.6m);

    // ----- base stats by level -----
    public static decimal HpBase => V("hp_base", 100);
    public static decimal HpPerLevel => V("hp_per_level", 10);
    public static decimal ManaBase => V("mana_base", 20);
    public static decimal ManaPerLevel => V("mana_per_level", 5);
    public static decimal AtkBase => V("atk_base", 5);
    public static decimal AtkPerLevel => V("atk_per_level", 1);
    public static decimal IntBase => V("int_base", 5);
    public static decimal IntPerLevel => V("int_per_level", 1);
    public static int DefBase => (int)V("def_base", 2);
    public static int DefLevelsPerPoint => Math.Max(1, (int)V("def_levels_per_point", 2));
    public static int LukBase => (int)V("luk_base", 1);
    public static int LukLevelsPerPoint => Math.Max(1, (int)V("luk_levels_per_point", 3));

    // ----- pets -----
    public static int MaxCoopLevel => (int)V("max_coop_level", 5);
    public static int MaxIncubatorLevel => (int)V("max_incubator_level", 3);
    public static int MaxPetLevel => (int)V("max_pet_level", 20);
    public static int PetFeedCost => (int)V("pet_feed_cost", 10);
    public static int PetFeedXp => (int)V("pet_feed_xp", 15);
    public static int PetXpPerLevel => (int)V("pet_xp_per_level", 50);
    public static int CoopCapacityBase => (int)V("coop_capacity_base", 1);
    public static int CoopCapacityPerLevel => (int)V("coop_capacity_per_level", 2);
    public static int CoopUpgradeCostPerLevel => (int)V("coop_upgrade_cost_per_level", 200);
    public static int IncubatorUpgradeCostPerLevel => (int)V("incubator_upgrade_cost_per_level", 500);
    public static double HatchUpgradeChance => (double)V("hatch_upgrade_chance", 0.10m);
}
