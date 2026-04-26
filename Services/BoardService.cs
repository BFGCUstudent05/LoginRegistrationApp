namespace MinesweeperWeb.Services;

using MinesweeperWeb.Models;

/// <summary>
/// Contains all business logic for game board operations.
/// Ported from T6 BoardLogic as a non-static service for dependency injection.
/// </summary>
public class BoardService : IBoardService
{
    private readonly Random _random = new();

    /// <summary>
    /// Randomly initializes the grid with live bombs based on difficulty level (1-3).
    /// </summary>
    /// <param name="board">The board model to set up bombs on.</param>
    /// <param name="difficulty">Difficulty level: 1 (easy), 2 (medium), 3 (hard).</param>
    public void SetupBombs(BoardModel board, int difficulty)
    {
        if (board == null || board.Cells == null)
        {
            throw new ArgumentNullException(nameof(board), "Board and Cells cannot be null.");
        }

        int totalCells = board.Size * board.Size;
        int numberOfBombs;

        // Determine number of bombs based on difficulty level
        switch (difficulty)
        {
            case 1: // Easy: ~10% of cells
                numberOfBombs = (int)(totalCells * 0.1);
                break;
            case 2: // Medium: ~15% of cells
                numberOfBombs = (int)(totalCells * 0.15);
                break;
            case 3: // Hard: ~20% of cells
                numberOfBombs = (int)(totalCells * 0.2);
                break;
            default:
                numberOfBombs = (int)(totalCells * 0.1); // Default to easy
                break;
        }

        SetupBombsByCountInternal(board, numberOfBombs);
        board.Difficulty = difficulty;
    }

    /// <summary>
    /// Helper method to place bombs randomly on the board.
    /// </summary>
    /// <param name="board">The board model to set up bombs on.</param>
    /// <param name="numberOfBombs">The number of bombs to place.</param>
    private void SetupBombsByCountInternal(BoardModel board, int numberOfBombs)
    {
        int totalCells = board.Size * board.Size;
        numberOfBombs = Math.Min(numberOfBombs, totalCells); // Ensure we don't exceed total cells

        // Create a list of all cell positions
        List<(int row, int column)> availablePositions = new List<(int, int)>();
        for (int row = 0; row < board.Size; row++)
        {
            for (int column = 0; column < board.Size; column++)
            {
                availablePositions.Add((row, column));
            }
        }

        // Randomly select positions for bombs
        for (int i = 0; i < numberOfBombs; i++)
        {
            int randomIndex = _random.Next(availablePositions.Count);
            var position = availablePositions[randomIndex];
            availablePositions.RemoveAt(randomIndex);

            board.Cells[position.row][position.column].IsBomb = true;
        }
    }

    /// <summary>
    /// Calculates the number of live bomb neighbors for every cell on the grid.
    /// A cell should have between 0 and 8 live neighbors.
    /// </summary>
    /// <param name="board">The board model to calculate neighbors for.</param>
    public void CountBombsNearby(BoardModel board)
    {
        if (board == null || board.Cells == null)
        {
            throw new ArgumentNullException(nameof(board), "Board and Cells cannot be null.");
        }

        for (int row = 0; row < board.Size; row++)
        {
            for (int column = 0; column < board.Size; column++)
            {
                board.Cells[row][column].NumberOfBombNeighbors = CountBombNeighbors(board, row, column);
            }
        }
    }

    /// <summary>
    /// Counts the number of bomb neighbors for a specific cell.
    /// Checks all 8 adjacent cells (including diagonals).
    /// </summary>
    /// <param name="board">The board model.</param>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="column">The column index of the cell.</param>
    /// <returns>The number of neighboring cells that contain bombs (0-8).</returns>
    private int CountBombNeighbors(BoardModel board, int row, int column)
    {
        int count = 0;

        // Check all 8 neighboring cells
        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (int columnOffset = -1; columnOffset <= 1; columnOffset++)
            {
                // Skip the cell itself
                if (rowOffset == 0 && columnOffset == 0)
                {
                    continue;
                }

                int neighborRow = row + rowOffset;
                int neighborColumn = column + columnOffset;

                // Check if neighbor is within board bounds
                if (neighborRow >= 0 && neighborRow < board.Size &&
                    neighborColumn >= 0 && neighborColumn < board.Size)
                {
                    if (board.Cells[neighborRow][neighborColumn].IsBomb)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    /// <summary>
    /// Determines the current game state by inspecting the board.
    /// </summary>
    /// <param name="board">The board model to inspect.</param>
    /// <returns>Won when all non-bomb cells have been visited; Lost if a bomb was visited; InProgress otherwise.</returns>
    public GameStateEnum DetermineGameState(BoardModel board)
    {
        if (board == null || board.Cells == null)
        {
            throw new ArgumentNullException(nameof(board), "Board and Cells cannot be null.");
        }

        bool bombVisited = false;
        bool allNonBombsVisited = true;

        for (int row = 0; row < board.Size; row++)
        {
            for (int column = 0; column < board.Size; column++)
            {
                CellModel cell = board.Cells[row][column];
                if (cell.IsBomb && cell.IsVisited)
                {
                    bombVisited = true;
                }
                else if (!cell.IsBomb && !cell.IsVisited)
                {
                    allNonBombsVisited = false;
                }
            }
        }

        if (bombVisited)
        {
            return GameStateEnum.Lost;
        }

        if (allNonBombsVisited)
        {
            return GameStateEnum.Won;
        }

        return GameStateEnum.InProgress;
    }

    /// <summary>
    /// Places a single reward on a randomly chosen non-bomb cell.
    /// </summary>
    /// <param name="board">The board model to place the reward on.</param>
    public void PlaceReward(BoardModel board)
    {
        if (board == null || board.Cells == null)
        {
            throw new ArgumentNullException(nameof(board), "Board and Cells cannot be null.");
        }

        List<(int row, int column)> availablePositions = new List<(int, int)>();
        for (int row = 0; row < board.Size; row++)
        {
            for (int column = 0; column < board.Size; column++)
            {
                if (!board.Cells[row][column].IsBomb)
                {
                    availablePositions.Add((row, column));
                }
            }
        }

        if (availablePositions.Count == 0)
        {
            return;
        }

        int randomIndex = _random.Next(availablePositions.Count);
        var position = availablePositions[randomIndex];
        board.Cells[position.row][position.column].HasSpecialReward = true;
    }

    /// <summary>
    /// Recursively reveals the block of cells with no live (bomb) neighbors starting at (row, col).
    /// Marks each affected cell as visited and recurses into surrounding cells when the cell has zero bomb neighbors.
    /// </summary>
    /// <param name="board">The board model.</param>
    /// <param name="row">Row index of the starting cell.</param>
    /// <param name="col">Column index of the starting cell.</param>
    public void FloodFill(BoardModel board, int row, int col)
    {
        if (board == null || board.Cells == null)
        {
            throw new ArgumentNullException(nameof(board), "Board and Cells cannot be null.");
        }

        FloodFillRecursive(board, row, col);
    }

    private void FloodFillRecursive(BoardModel board, int row, int col)
    {
        if (row < 0 || row >= board.Size || col < 0 || col >= board.Size)
        {
            return;
        }

        CellModel cell = board.Cells[row][col];
        if (cell.IsBomb || cell.IsVisited)
        {
            return;
        }

        cell.IsVisited = true;

        if (cell.NumberOfBombNeighbors > 0)
        {
            return;
        }

        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (int colOffset = -1; colOffset <= 1; colOffset++)
            {
                if (rowOffset == 0 && colOffset == 0)
                {
                    continue;
                }

                FloodFillRecursive(board, row + rowOffset, col + colOffset);
            }
        }
    }
}
