using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // accounts
    public DbSet<User> Users => Set<User>();

    // reference data
    public DbSet<RarityEntity> Rarities => Set<RarityEntity>();
    public DbSet<LevelTitleEntity> LevelTitles => Set<LevelTitleEntity>();
    public DbSet<BadgeEntity> Badges => Set<BadgeEntity>();
    public DbSet<TimeZoneEntity> TimeZones => Set<TimeZoneEntity>();
    public DbSet<GameSettingEntity> GameSettings => Set<GameSettingEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<DifficultyLevelEntity> DifficultyLevels => Set<DifficultyLevelEntity>();
    public DbSet<ShopCategoryEntity> ShopCategories => Set<ShopCategoryEntity>();
    public DbSet<EquipmentSlotEntity> EquipmentSlots => Set<EquipmentSlotEntity>();
    public DbSet<ItemSubtypeEntity> ItemSubtypes => Set<ItemSubtypeEntity>();
    public DbSet<ItemEntity> Items => Set<ItemEntity>();
    public DbSet<EggTypeEntity> EggTypes => Set<EggTypeEntity>();
    public DbSet<PetSpeciesEntity> PetSpecies => Set<PetSpeciesEntity>();
    public DbSet<TitleEntity> Titles => Set<TitleEntity>();
    public DbSet<AchievementEntity> Achievements => Set<AchievementEntity>();
    public DbSet<BossEntity> Bosses => Set<BossEntity>();
    public DbSet<BossRewardEntity> BossRewards => Set<BossRewardEntity>();
    public DbSet<SkillEntity> Skills => Set<SkillEntity>();
    public DbSet<ItemSkillEntity> ItemSkills => Set<ItemSkillEntity>();
    public DbSet<SkillNodeEntity> SkillNodes => Set<SkillNodeEntity>();

    // character
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterItem> CharacterItems => Set<CharacterItem>();
    public DbSet<CharacterEquipment> CharacterEquipment => Set<CharacterEquipment>();
    public DbSet<OwnedPet> Pets => Set<OwnedPet>();
    public DbSet<IncubatingEgg> IncubatingEggs => Set<IncubatingEgg>();
    public DbSet<CharacterTitle> CharacterTitles => Set<CharacterTitle>();
    public DbSet<CharacterAchievement> CharacterAchievements => Set<CharacterAchievement>();
    public DbSet<CharacterSkill> CharacterSkills => Set<CharacterSkill>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<Notification> Notifications => Set<Notification>();

    // tasks and activity
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<DailyCompletion> DailyCompletions => Set<DailyCompletion>();
    public DbSet<HabitLog> HabitLogs => Set<HabitLog>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    // raids
    public DbSet<Raid> Raids => Set<Raid>();
    public DbSet<RaidParticipant> RaidParticipants => Set<RaidParticipant>();
    public DbSet<RaidHit> RaidHits => Set<RaidHit>();
    public DbSet<RaidRewardClaim> RaidRewardClaims => Set<RaidRewardClaim>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // ---------- accounts ----------
        b.Entity<User>(e =>
        {
            e.ToTable("users");
            e.Property(u => u.Username).HasMaxLength(50).IsRequired();
            e.Property(u => u.Email).HasMaxLength(256).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.TimeZone).HasMaxLength(64).IsRequired();
            e.HasIndex(u => u.Username).IsUnique();
            e.HasIndex(u => u.Email).IsUnique();
        });

        // ---------- reference data ----------
        b.Entity<RarityEntity>(e =>
        {
            e.ToTable("rarities");
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).HasConversion<string>().HasMaxLength(12).ValueGeneratedNever();
            e.Property(r => r.Label).HasMaxLength(30).IsRequired();
            e.HasData(CatalogSeed.Rarities);
        });

        b.Entity<LevelTitleEntity>(e =>
        {
            e.ToTable("level_titles");
            e.HasKey(t => t.MinLevel);
            e.Property(t => t.MinLevel).ValueGeneratedNever();
            e.Property(t => t.Name).HasMaxLength(60).IsRequired();
            e.Property(t => t.Icon).HasMaxLength(16);
            e.Property(t => t.TitleId).HasMaxLength(40);
            e.HasOne<TitleEntity>().WithMany().HasForeignKey(t => t.TitleId).OnDelete(DeleteBehavior.SetNull);
            e.ToTable(t => t.HasCheckConstraint("ck_level_titles_min_level", "min_level >= 1"));
            e.HasData(CatalogSeed.LevelTitles);
        });

        b.Entity<BadgeEntity>(e =>
        {
            e.ToTable("badges");
            e.Property(x => x.Id).HasMaxLength(40);
            e.Property(x => x.Icon).HasMaxLength(16);
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.Metric).HasMaxLength(40).IsRequired();
            e.HasData(CatalogSeed.Badges);
        });

        b.Entity<TimeZoneEntity>(e =>
        {
            e.ToTable("time_zones");
            e.Property(x => x.Id).HasMaxLength(64);
            e.Property(x => x.Label).HasMaxLength(100).IsRequired();
            e.HasData(CatalogSeed.TimeZones);
        });

        b.Entity<GameSettingEntity>(e =>
        {
            e.ToTable("game_settings");
            e.HasKey(x => x.Key);
            e.Property(x => x.Key).HasMaxLength(60);
            e.Property(x => x.Value).HasPrecision(14, 4);
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.GroupName).HasMaxLength(30);
            e.HasData(CatalogSeed.Settings);
        });

        b.Entity<CategoryEntity>(e =>
        {
            e.ToTable("categories");
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).ValueGeneratedNever();
            e.Property(c => c.Slug).HasMaxLength(20).IsRequired();
            e.Property(c => c.Name).HasMaxLength(60).IsRequired();
            e.Property(c => c.BoardName).HasMaxLength(60).IsRequired();
            e.Property(c => c.Emoji).HasMaxLength(16);
            e.Property(c => c.Unit).HasMaxLength(16);
            e.Property(c => c.Tagline).HasMaxLength(200);
            e.Property(c => c.MaxPerEntry).HasPrecision(6, 2);
            e.HasIndex(c => c.Slug).IsUnique();
            e.HasData(CatalogSeed.Categories);
        });

        b.Entity<DifficultyLevelEntity>(e =>
        {
            e.ToTable("difficulty_levels");
            e.HasKey(d => d.Level);
            e.Property(d => d.Level).ValueGeneratedNever();
            e.Property(d => d.Name).HasMaxLength(30).IsRequired();
            e.ToTable(t => t.HasCheckConstraint("ck_difficulty_level", "level BETWEEN 1 AND 5"));
            e.HasData(CatalogSeed.Difficulties);
        });

        b.Entity<ShopCategoryEntity>(e =>
        {
            e.ToTable("shop_categories");
            e.HasKey(c => c.Key);
            e.Property(c => c.Key).HasMaxLength(20);
            e.Property(c => c.Name).HasMaxLength(60).IsRequired();
            e.Property(c => c.Emoji).HasMaxLength(16);
            e.HasData(CatalogSeed.ShopCategories);
        });

        b.Entity<EquipmentSlotEntity>(e =>
        {
            e.ToTable("equipment_slots");
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).ValueGeneratedNever();
            e.Property(s => s.Name).HasMaxLength(40).IsRequired();
            e.Property(s => s.Emoji).HasMaxLength(16);
            e.Property(s => s.ShopCategoryKey).HasMaxLength(20).IsRequired();
            e.HasOne<ShopCategoryEntity>().WithMany().HasForeignKey(s => s.ShopCategoryKey);
            e.HasData(CatalogSeed.Slots);
        });

        b.Entity<ItemSubtypeEntity>(e =>
        {
            e.ToTable("item_subtypes");
            e.Property(s => s.Name).HasMaxLength(40).IsRequired();
            e.HasOne<EquipmentSlotEntity>().WithMany().HasForeignKey(s => s.SlotId);
            e.HasIndex(s => new { s.SlotId, s.Name }).IsUnique();
            e.Property(s => s.Id).ValueGeneratedNever();
            e.HasData(CatalogSeed.Subtypes);
        });

        b.Entity<ItemEntity>(e =>
        {
            e.ToTable("items");
            e.Property(i => i.Id).HasMaxLength(40);
            e.Property(i => i.Name).HasMaxLength(80).IsRequired();
            e.Property(i => i.Rarity).HasConversion<string>().HasMaxLength(12);
            e.Property(i => i.Emoji).HasMaxLength(16);
            e.Property(i => i.Description).HasMaxLength(300);
            e.HasOne<EquipmentSlotEntity>().WithMany().HasForeignKey(i => i.SlotId);
            e.HasOne<RarityEntity>().WithMany().HasForeignKey(i => i.Rarity);
            e.HasOne<ItemSubtypeEntity>().WithMany().HasForeignKey(i => i.SubtypeId);
            e.HasAlternateKey(i => new { i.Id, i.SlotId }); // lets character_equipment check item/slot agree
            e.HasIndex(i => i.SlotId);
            e.HasData(CatalogSeed.Items);
        });

        b.Entity<EggTypeEntity>(e =>
        {
            e.ToTable("egg_types");
            e.HasKey(x => x.ItemId);
            e.Property(x => x.ItemId).HasMaxLength(40);
            e.Property(x => x.BaseRarity).HasConversion<string>().HasMaxLength(12);
            e.HasOne<ItemEntity>().WithOne().HasForeignKey<EggTypeEntity>(x => x.ItemId);
            e.HasOne<RarityEntity>().WithMany().HasForeignKey(x => x.BaseRarity);
            e.HasData(CatalogSeed.Eggs);
        });

        b.Entity<PetSpeciesEntity>(e =>
        {
            e.ToTable("pet_species");
            e.Property(s => s.Id).HasMaxLength(30);
            e.Property(s => s.Name).HasMaxLength(60).IsRequired();
            e.Property(s => s.Emoji).HasMaxLength(16);
            e.Property(s => s.Rarity).HasConversion<string>().HasMaxLength(12);
            e.HasOne<RarityEntity>().WithMany().HasForeignKey(s => s.Rarity);
            e.HasData(CatalogSeed.Species);
        });

        b.Entity<TitleEntity>(e =>
        {
            e.ToTable("titles");
            e.Property(t => t.Id).HasMaxLength(40);
            e.Property(t => t.Name).HasMaxLength(80).IsRequired();
            e.Property(t => t.Icon).HasMaxLength(16);
            e.HasData(CatalogSeed.Titles);
        });

        b.Entity<AchievementEntity>(e =>
        {
            e.ToTable("achievements");
            e.Property(a => a.Id).HasMaxLength(40);
            e.Property(a => a.Name).HasMaxLength(80).IsRequired();
            e.Property(a => a.Icon).HasMaxLength(16);
            e.Property(a => a.Description).HasMaxLength(300);
            e.Property(a => a.Metric).HasMaxLength(40).IsRequired();
            e.Property(a => a.TitleId).HasMaxLength(40).IsRequired();
            e.HasOne<TitleEntity>().WithMany().HasForeignKey(a => a.TitleId);
            e.HasData(CatalogSeed.Achievements);
        });

        b.Entity<BossEntity>(e =>
        {
            e.ToTable("bosses");
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.Property(x => x.Emoji).HasMaxLength(16);
            e.Property(x => x.Title).HasMaxLength(120);
            e.Property(x => x.Lore).HasMaxLength(400);
            e.HasData(CatalogSeed.Bosses);
        });

        b.Entity<BossRewardEntity>(e =>
        {
            e.ToTable("boss_rewards");
            e.Property(r => r.Id).ValueGeneratedNever();
            e.Property(r => r.Kind).HasConversion<string>().HasMaxLength(12);
            e.Property(r => r.ItemId).HasMaxLength(40);
            e.Property(r => r.PetSpeciesId).HasMaxLength(30);
            e.Property(r => r.TitleId).HasMaxLength(40);
            e.HasOne<BossEntity>().WithMany().HasForeignKey(r => r.BossId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<ItemEntity>().WithMany().HasForeignKey(r => r.ItemId);
            e.HasOne<PetSpeciesEntity>().WithMany().HasForeignKey(r => r.PetSpeciesId);
            e.HasOne<TitleEntity>().WithMany().HasForeignKey(r => r.TitleId);
            e.ToTable(t => t.HasCheckConstraint("ck_boss_reward_kind",
                "(kind = 'Gold' AND gold > 0) OR (kind = 'Item' AND item_id IS NOT NULL) " +
                "OR (kind = 'Pet' AND pet_species_id IS NOT NULL) OR (kind = 'Title' AND title_id IS NOT NULL)"));
            e.HasData(CatalogSeed.BossRewards);
        });

        // skills: tables only for now (no gameplay yet)
        b.Entity<SkillEntity>(e =>
        {
            e.ToTable("skills");
            e.Property(s => s.Id).HasMaxLength(40);
            e.Property(s => s.Name).HasMaxLength(80).IsRequired();
            e.Property(s => s.Description).HasMaxLength(400);
        });
        b.Entity<ItemSkillEntity>(e =>
        {
            e.ToTable("item_skills");
            e.HasKey(x => new { x.ItemId, x.SkillId });
            e.HasOne<ItemEntity>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<SkillEntity>().WithMany().HasForeignKey(x => x.SkillId).OnDelete(DeleteBehavior.Cascade);
        });
        b.Entity<SkillNodeEntity>(e =>
        {
            e.ToTable("skill_nodes");
            e.HasOne<SkillEntity>().WithMany().HasForeignKey(x => x.SkillId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<SkillNodeEntity>().WithMany().HasForeignKey(x => x.ParentNodeId);
        });

        // ---------- character ----------
        b.Entity<Character>(e =>
        {
            e.ToTable("characters");
            e.HasKey(c => c.UserId);
            e.Property(c => c.ActiveTitleId).HasMaxLength(40);
            e.HasOne<User>().WithOne().HasForeignKey<Character>(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<TitleEntity>().WithMany().HasForeignKey(c => c.ActiveTitleId).OnDelete(DeleteBehavior.SetNull);
            e.ToTable(t =>
            {
                t.HasCheckConstraint("ck_characters_gold", "gold >= 0");
                t.HasCheckConstraint("ck_characters_hp", "hp >= 0");
            });
        });

        b.Entity<CharacterItem>(e =>
        {
            e.ToTable("character_items");
            e.HasKey(x => new { x.UserId, x.ItemId });
            e.Property(x => x.ItemId).HasMaxLength(40);
            e.HasOne<Character>().WithMany(c => c.Items).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<ItemEntity>().WithMany().HasForeignKey(x => x.ItemId);
            e.ToTable(t => t.HasCheckConstraint("ck_character_items_qty", "quantity > 0"));
        });

        b.Entity<CharacterEquipment>(e =>
        {
            e.ToTable("character_equipment");
            e.HasKey(x => new { x.UserId, x.SlotId });
            e.Property(x => x.ItemId).HasMaxLength(40);
            e.HasOne<Character>().WithMany(c => c.Equipment).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            // must own the item, and the item must belong to that slot
            e.HasOne<CharacterItem>().WithMany().HasForeignKey(x => new { x.UserId, x.ItemId }).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ItemEntity>().WithMany().HasForeignKey(x => new { x.ItemId, x.SlotId }).HasPrincipalKey(i => new { i.Id, i.SlotId });
        });

        b.Entity<OwnedPet>(e =>
        {
            e.ToTable("pets");
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(p => p.SpeciesId).HasMaxLength(30);
            e.Property(p => p.Name).HasMaxLength(60).IsRequired();
            e.HasOne<Character>().WithMany(c => c.Pets).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<PetSpeciesEntity>().WithMany().HasForeignKey(p => p.SpeciesId);
            e.HasIndex(p => p.UserId);
        });

        b.Entity<Character>().HasOne<OwnedPet>().WithMany().HasForeignKey(c => c.CompanionPetId).OnDelete(DeleteBehavior.SetNull);

        b.Entity<IncubatingEgg>(e =>
        {
            e.ToTable("incubating_eggs");
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.EggItemId).HasMaxLength(40);
            e.HasOne<Character>().WithMany(c => c.IncubatingEggs).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<EggTypeEntity>().WithMany().HasForeignKey(x => x.EggItemId);
        });

        b.Entity<CharacterTitle>(e =>
        {
            e.ToTable("character_titles");
            e.HasKey(x => new { x.UserId, x.TitleId });
            e.Property(x => x.TitleId).HasMaxLength(40);
            e.Property(x => x.Source).HasMaxLength(60);
            e.HasOne<Character>().WithMany(c => c.Titles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<TitleEntity>().WithMany().HasForeignKey(x => x.TitleId);
        });

        b.Entity<CharacterAchievement>(e =>
        {
            e.ToTable("character_achievements");
            e.HasKey(x => new { x.UserId, x.AchievementId });
            e.Property(x => x.AchievementId).HasMaxLength(40);
            e.HasOne<Character>().WithMany(c => c.Achievements).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<AchievementEntity>().WithMany().HasForeignKey(x => x.AchievementId);
        });

        b.Entity<CharacterSkill>(e =>
        {
            e.ToTable("character_skills");
            e.HasKey(x => new { x.UserId, x.SkillId });
            e.Property(x => x.SkillId).HasMaxLength(40);
            e.Property(x => x.Source).HasMaxLength(30);
            e.HasOne<Character>().WithMany(c => c.Skills).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<SkillEntity>().WithMany().HasForeignKey(x => x.SkillId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<LedgerEntry>(e =>
        {
            e.ToTable("ledger_entries");
            e.Property(x => x.Kind).HasMaxLength(30).IsRequired();
            e.Property(x => x.ItemId).HasMaxLength(40);
            e.Property(x => x.Note).HasMaxLength(300);
            e.HasOne<Character>().WithMany(c => c.Ledger).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.UserId, x.CreatedAt });
        });

        b.Entity<Notification>(e =>
        {
            e.ToTable("notifications");
            e.Property(x => x.Kind).HasMaxLength(30).IsRequired();
            e.Property(x => x.Message).HasMaxLength(1000).IsRequired();
            e.HasOne<Character>().WithMany(c => c.Notifications).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.UserId, x.ReadAt });
        });

        // ---------- tasks and activity ----------
        b.Entity<TaskItem>(e =>
        {
            e.ToTable("tasks");
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Ignore(t => t.IsDone);
            e.Property(t => t.Type).HasConversion<string>().HasMaxLength(10);
            e.Property(t => t.Repeat).HasConversion<string>().HasMaxLength(10);
            e.Property(t => t.Priority).HasConversion<int>();
            e.Property(t => t.Title).HasMaxLength(200).IsRequired();
            e.Property(t => t.Description).HasMaxLength(2000);
            e.Property(t => t.Effort).HasPrecision(6, 2);
            e.HasOne<User>().WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<DifficultyLevelEntity>().WithMany().HasForeignKey(t => t.Difficulty);
            e.HasOne<CategoryEntity>().WithMany().HasForeignKey(t => t.Category);
            e.HasIndex(t => new { t.UserId, t.Type });
            e.HasQueryFilter(t => t.ArchivedAt == null);
            e.ToTable(t =>
            {
                t.HasCheckConstraint("ck_tasks_priority", "priority BETWEEN 1 AND 3");
                t.HasCheckConstraint("ck_tasks_todo_only",
                    "type = 'Todo' OR (deadline IS NULL AND category IS NULL AND effort IS NULL AND completed_at IS NULL)");
                t.HasCheckConstraint("ck_tasks_daily_only", "(type = 'Daily') = (repeat IS NOT NULL)");
                t.HasCheckConstraint("ck_tasks_effort_category", "(category IS NULL) = (effort IS NULL)");
                t.HasCheckConstraint("ck_tasks_effort_positive", "effort IS NULL OR effort > 0");
            });
        });

        b.Entity<DailyCompletion>(e =>
        {
            e.ToTable("daily_completions");
            e.HasKey(x => new { x.TaskId, x.PeriodStart });
            e.HasOne<TaskItem>().WithMany().HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<HabitLog>(e =>
        {
            e.ToTable("habit_logs");
            e.HasOne<TaskItem>().WithMany().HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.TaskId);
        });

        b.Entity<ActivityLog>(e =>
        {
            e.ToTable("activity_logs");
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(a => a.Amount).HasPrecision(6, 2);
            e.Property(a => a.Note).HasMaxLength(200);
            e.HasOne<User>().WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<CategoryEntity>().WithMany().HasForeignKey(a => a.Category);
            e.HasOne<TaskItem>().WithMany().HasForeignKey(a => a.SourceTaskId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(a => new { a.Category, a.LoggedAt });
            e.HasIndex(a => new { a.UserId, a.LoggedAt });
        });

        // ---------- raids ----------
        b.Entity<Raid>(e =>
        {
            e.ToTable("raids");
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(r => r.Status).HasConversion<string>().HasMaxLength(12);
            e.HasOne<BossEntity>().WithMany().HasForeignKey(r => r.BossId);
            e.HasIndex(r => r.StartsAt).IsUnique();
        });

        b.Entity<RaidParticipant>(e =>
        {
            e.ToTable("raid_participants");
            e.HasKey(x => new { x.RaidId, x.UserId });
            e.HasOne<Raid>().WithMany().HasForeignKey(x => x.RaidId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<RaidHit>(e =>
        {
            e.ToTable("raid_hits");
            e.Property(h => h.Source).HasMaxLength(200);
            e.HasOne<Raid>().WithMany().HasForeignKey(h => h.RaidId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<User>().WithMany().HasForeignKey(h => h.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<TaskItem>().WithMany().HasForeignKey(h => h.TaskId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(h => new { h.RaidId, h.UserId });
        });

        b.Entity<RaidRewardClaim>(e =>
        {
            e.ToTable("raid_reward_claims");
            e.HasKey(x => new { x.RaidId, x.UserId });
            e.HasOne<Raid>().WithMany().HasForeignKey(x => x.RaidId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // snake_case column names (tables are named explicitly above)
        foreach (var entity in b.Model.GetEntityTypes())
            foreach (var property in entity.GetProperties())
                property.SetColumnName(ToSnakeCase(property.Name));
    }

    private static string ToSnakeCase(string name) =>
        Regex.Replace(name, "(?<=[a-z0-9])([A-Z])", "_$1").ToLowerInvariant();
}
