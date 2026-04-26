namespace MinesweeperWeb.Models;

public class GameBoardViewModel
{
    public BoardModel Board { get; set; } = null!;
    public string ElapsedTime { get; set; } = string.Empty;
}
