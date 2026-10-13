using TodoApp.Models;

namespace TodoApp.ViewModels;

public class BoardRow
{
    public int Rank { get; set; }
    public string Username { get; set; } = "";
    public decimal Total { get; set; }
    public bool IsMe { get; set; }
}

public class Board
{
    public CategoryInfo Info { get; set; } = null!;
    public List<BoardRow> Rows { get; set; } = [];
}

public class HomeViewModel
{
    public string Period { get; set; } = "week";
    public List<Board> Boards { get; set; } = [];
}

public record BadgeViewModel(string Icon, string Name, string Description, bool Unlocked);

public class CategoryCardViewModel
{
    public CategoryInfo Info { get; set; } = null!;
    public decimal WeekTotal { get; set; }
    public decimal AllTotal { get; set; }
    public int? WeekRank { get; set; }
    public int WeekPlayers { get; set; }
    public int? AllRank { get; set; }
    public int AllPlayers { get; set; }
}
