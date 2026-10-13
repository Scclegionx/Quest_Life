namespace TodoApp.Models;

public enum RaidState { Active, Defeated, Expired }

public class Raid
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int BossId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    /// <summary>Boss HP at the time the raid was created (later edits to the boss don't change running raids).</summary>
    public long MaxHp { get; set; }
    public long DamageDealt { get; set; }
    public RaidState Status { get; set; } = RaidState.Active;
    public DateTime? DefeatedAt { get; set; }
}

public class RaidParticipant
{
    public Guid RaidId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

public class RaidHit
{
    public long Id { get; set; }
    public Guid RaidId { get; set; }
    public Guid UserId { get; set; }
    public int Damage { get; set; }
    public Guid? TaskId { get; set; }
    public string? Source { get; set; }
    public DateTime DealtAt { get; set; } = DateTime.UtcNow;
}

public class RaidRewardClaim
{
    public Guid RaidId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ClaimedAt { get; set; } = DateTime.UtcNow;
}
