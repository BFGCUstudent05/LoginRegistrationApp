namespace MinesweeperWeb.Models;

/// <summary>
/// Enumeration representing the current state of the game.
/// </summary>
public enum GameStateEnum
{
    /// <summary>
    /// Game is currently in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// Player has won the game.
    /// </summary>
    Won,

    /// <summary>
    /// Player has lost the game.
    /// </summary>
    Lost
}
