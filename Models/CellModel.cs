namespace MinesweeperWeb.Models;

/// <summary>
/// Represents a single cell on the Minesweeper game board.
/// This class contains only data properties and constructors - no business logic.
/// </summary>
public class CellModel
{
    /// <summary>
    /// Gets or sets the row index of the cell on the board.
    /// Initialized to -1.
    /// </summary>
    public int Row { get; set; } = -1;

    /// <summary>
    /// Gets or sets the column index of the cell on the board.
    /// Initialized to -1.
    /// </summary>
    public int Column { get; set; } = -1;

    /// <summary>
    /// Gets or sets a value indicating whether the cell has been visited/cleared by the player.
    /// Initially set to false.
    /// </summary>
    public bool IsVisited { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the cell contains a bomb.
    /// Initially set to false.
    /// </summary>
    public bool IsBomb { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the player has placed a flag on this cell.
    /// Initially set to false.
    /// </summary>
    public bool IsFlagged { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of neighboring cells that contain bombs.
    /// Initially set to 0.
    /// </summary>
    public int NumberOfBombNeighbors { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether the cell contains a special reward.
    /// </summary>
    public bool HasSpecialReward { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the CellModel class.
    /// </summary>
    public CellModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the CellModel class with specified row and column.
    /// </summary>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="column">The column index of the cell.</param>
    public CellModel(int row, int column)
    {
        Row = row;
        Column = column;
    }
}
