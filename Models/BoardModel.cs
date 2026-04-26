namespace MinesweeperWeb.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the game board for Minesweeper.
/// This class contains only data properties and constructors - no business logic.
/// </summary>
public class BoardModel
{
    /// <summary>
    /// Gets or sets the size of the square board (both length and width).
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Gets or sets the UTC time when the game starts.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the UTC time when the game ends.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Gets or sets the jagged array of cells that make up the board.
    /// </summary>
    public CellModel[][] Cells { get; set; }

    /// <summary>
    /// Gets or sets the difficulty level for bomb placement (1-3).
    /// </summary>
    public int Difficulty { get; set; }

    /// <summary>
    /// Gets or sets the number of special rewards the player has collected.
    /// Initially starts with value of zero.
    /// </summary>
    public int RewardsRemaining { get; set; } = 0;

    /// <summary>
    /// Gets or sets the current state of the game.
    /// </summary>
    public GameStateEnum GameState { get; set; } = GameStateEnum.InProgress;

    /// <summary>
    /// Parameterless constructor for JSON deserialization.
    /// </summary>
    [JsonConstructor]
    public BoardModel()
    {
        Cells = Array.Empty<CellModel[]>();
    }

    /// <summary>
    /// Initializes a new instance of the BoardModel class with the specified size.
    /// The Cells array is initialized so that a CellModel object is stored at each location.
    /// </summary>
    /// <param name="size">The size of the square board.</param>
    public BoardModel(int size)
    {
        Size = size;
        Cells = new CellModel[size][];

        // Initialize each cell in the grid
        for (int row = 0; row < size; row++)
        {
            Cells[row] = new CellModel[size];
            for (int col = 0; col < size; col++)
            {
                Cells[row][col] = new CellModel(row, col);
            }
        }
    }
}
