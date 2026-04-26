namespace MinesweeperWeb.Models;

public class ScoreModel
{
    public int Score { get; set; }
    public string ElapsedTime { get; set; } = string.Empty;
    public int BoardSize { get; set; }
    public string Difficulty { get; set; } = string.Empty;
}
