namespace MinesweeperWeb.Services;

using MinesweeperWeb.Models;

public interface IBoardService
{
    void SetupBombs(BoardModel board, int difficulty);
    void CountBombsNearby(BoardModel board);
    GameStateEnum DetermineGameState(BoardModel board);
    void PlaceReward(BoardModel board);
    void FloodFill(BoardModel board, int row, int col);
}
